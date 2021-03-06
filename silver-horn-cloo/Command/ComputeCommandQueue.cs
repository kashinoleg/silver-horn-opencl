﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Event;
using SilverHorn.Cloo.Kernel;

namespace SilverHorn.Cloo.Command
{
    /// <summary>
    /// Represents an OpenCL command queue.
    /// </summary>
    /// <remarks> A command queue is an object that holds commands that will be executed on a specific device. The command queue is created on a specific device in a context. Commands to a command queue are queued in-order but may be executed in-order or out-of-order. </remarks>
    public sealed class ComputeCommandQueue : ComputeResource
    {
        #region Properties

        /// <summary>
        /// The handle of the command queue.
        /// </summary>
        public CLCommandQueueHandle Handle { get; private set; }

        /// <summary>
        /// Gets the device of the command queue.
        /// </summary>
        /// <value> The device of the command queue. </value>
        public IComputeDevice Device { get; private set; }

        /// <summary>
        /// Gets the out-of-order execution mode of the commands in the command queue.
        /// </summary>
        /// <value> Is <c>true</c> if command queue has out-of-order execution mode enabled and <c>false</c> otherwise. </value>
        public bool OutOfOrderExecution { get; private set; }

        /// <summary>
        /// Gets the profiling mode of the commands in the command queue.
        /// </summary>
        /// <value> Is <c>true</c> if command queue has profiling enabled and <c>false</c> otherwise. </value>
        public bool Profiling { get; private set; }

        public IList<IComputeEvent> Events { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command queue.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="device"> A device associated with the <paramref name="context"/>. It can either be one of context devices or have the same <see cref="ComputeDeviceTypes"/> as the <paramref name="device"/> specified when the <paramref name="context"/> is created. </param>
        /// <param name="properties"> The properties for the command queue. </param>
        public ComputeCommandQueue(IComputeContext context, IComputeDevice device, ComputeCommandQueueFlags properties)
        {
            Handle = OpenCL100.CreateCommandQueue(context.Handle, device.Handle, properties, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            Device = device;

            OutOfOrderExecution = ((properties & ComputeCommandQueueFlags.OutOfOrderExecution) == ComputeCommandQueueFlags.OutOfOrderExecution);
            Profiling = ((properties & ComputeCommandQueueFlags.Profiling) == ComputeCommandQueueFlags.Profiling);

            Events = new List<IComputeEvent>();

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Enqueues a command to acquire a collection of memorys that have been previously created from OpenGL objects.
        /// </summary>
        /// <param name="memObjs"> A collection of OpenCL memory objects that correspond to OpenGL objects. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void AcquireGLObjects(ICollection<ComputeMemory> memObjs, ICollection<IComputeEvent> events)
        {
            var memObjHandles = ComputeTools.ExtractHandles(memObjs, out int memObjCount);
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;
            var error = CL10.EnqueueAcquireGLObjects(
                Handle,
                memObjCount,
                memObjHandles,
                eventWaitListSize,
                eventHandles,
                newEventHandle);
            ComputeException.ThrowOnError(error);
            if (eventsWritable)
            {
                events.Add(new ComputeEvent(newEventHandle[0], this));
            }
        }

        /// <summary>
        /// Enqueues a barrier.
        /// </summary>
        /// <remarks> A barrier ensures that all queued commands have finished execution before the next batch of commands can begin execution. </remarks>
        public void AddBarrier()
        {
            var error = CL10.EnqueueBarrier(Handle);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Enqueues a marker.
        /// </summary>
        public ComputeEvent AddMarker()
        {
            var error = CL10.EnqueueMarker(
                Handle,
                out CLEventHandle newEventHandle);
            ComputeException.ThrowOnError(error);
            return new ComputeEvent(newEventHandle, this);
        }

        /// <summary>
        /// Enqueues a command to copy data between buffers.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void Copy<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination, long sourceOffset,
            long destinationOffset, long region, ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueCopyBuffer(
                Handle,
                source.Handle,
                destination.Handle,
                new IntPtr(sourceOffset * sizeofT),
                new IntPtr(destinationOffset * sizeofT),
                new IntPtr(region * sizeofT),
                eventWaitListSize,
                eventHandles,
                newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
            {
                events.Add(new ComputeEvent(newEventHandle[0], this));
            }
        }

        /// <summary>
        /// Enqueues a command to copy a 2D or 3D region of elements between two buffers.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="sourceRowPitch"> The size of the source buffer row in bytes. If set to zero then <paramref name="sourceRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="sourceSlicePitch"> The size of the source buffer 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch"/> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
        /// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch"/> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public void Copy<T>(ComputeBufferBase<T> source, ComputeBufferBase<T> destination,
            SysIntX3 sourceOffset, SysIntX3 destinationOffset, SysIntX3 region,
            long sourceRowPitch, long sourceSlicePitch, long destinationRowPitch, long destinationSlicePitch,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            sourceOffset.X = new IntPtr(sizeofT * sourceOffset.X.ToInt64());
            destinationOffset.X = new IntPtr(sizeofT * destinationOffset.X.ToInt64());
            region.X = new IntPtr(sizeofT * region.X.ToInt64());

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL11.EnqueueCopyBufferRect(this.Handle, source.Handle, destination.Handle,
                ref sourceOffset, ref destinationOffset, ref region, new IntPtr(sourceRowPitch),
                new IntPtr(sourceSlicePitch), new IntPtr(destinationRowPitch), new IntPtr(destinationSlicePitch),
                eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
            {
                events.Add(new ComputeEvent(newEventHandle[0], this));
            }
        }

        /// <summary>
        /// Enqueues a command to copy data from buffer to image.
        /// </summary>
        /// <typeparam name="T"> The type of data in source. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void Copy<T>(ComputeBufferBase<T> source, ComputeImage destination,
            long sourceOffset, SysIntX3 destinationOffset, SysIntX3 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueCopyBufferToImage(Handle, source.Handle, destination.Handle,
                new IntPtr(sourceOffset * sizeofT), ref destinationOffset, ref region,
                eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to copy data from image to buffer.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void Copy<T>(ComputeImage source,
            ComputeBufferBase<T> destination,
            SysIntX3 sourceOffset,
            long destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;
            var error = CL10.EnqueueCopyImageToBuffer(Handle,
                source.Handle,
                destination.Handle,
                ref sourceOffset,
                ref region,
                new IntPtr(destinationOffset * sizeofT),
                eventWaitListSize,
                eventHandles,
                newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to copy data between images.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void Copy(ComputeImage source,
            ComputeImage destination,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueCopyImage(Handle, source.Handle, destination.Handle, ref sourceOffset,
                ref destinationOffset, ref region, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to execute a kernel single.
        /// </summary>
        /// <param name="kernel"> The kernel to execute. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void ExecuteTask(IComputeKernel kernel, ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueTask(Handle, kernel.Handle, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to execute a range of kernels in parallel.
        /// </summary>
        /// <param name="kernel"> The kernel to execute. </param>
        /// <param name="globalWorkOffset"> An array of values that describe the offset used to calculate the global ID of a work-item instead of having the global IDs always start at offset (0, 0,... 0). </param>
        /// <param name="globalWorkSize"> An array of values that describe the number of global work-items in dimensions that will execute the kernel function. The total number of global work-items is computed as global_work_size[0] *...* global_work_size[work_dim - 1]. </param>
        /// <param name="localWorkSize"> An array of values that describe the number of work-items that make up a work-group (also referred to as the size of the work-group) that will execute the <paramref name="kernel"/>. The total number of work-items in a work-group is computed as local_work_size[0] *... * local_work_size[work_dim - 1]. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void Execute(IComputeKernel kernel,
            long[] globalWorkOffset,
            long[] globalWorkSize,
            long[] localWorkSize,
            ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueNDRangeKernel(
                Handle,
                kernel.Handle,
                globalWorkSize.Length,
                ComputeTools.ConvertArray(globalWorkOffset),
                ComputeTools.ConvertArray(globalWorkSize),
                ComputeTools.ConvertArray(localWorkSize),
                eventWaitListSize,
                eventHandles,
                newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
            {
                events.Add(new ComputeEvent(newEventHandle[0], this));
            }
        }

        /// <summary>
        /// Blocks until all previously enqueued commands are issued to the device and have completed.
        /// </summary>
        public void Finish()
        {
            var error = CL10.Finish(Handle);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Issues all previously enqueued commands to the device.
        /// </summary>
        /// <remarks> This method only guarantees that all previously enqueued commands get issued to the OpenCL device. There is no guarantee that they will be complete after this method returns. </remarks>
        public void Flush()
        {
            var error = CL10.Flush(Handle);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Enqueues a command to map a part of a buffer into the host address space.
        /// </summary>
        /// <param name="buffer"> The buffer to map. </param>
        /// <param name="blocking">  The mode of operation of this call. </param>
        /// <param name="flags"> A list of properties for the mapping mode. </param>
        /// <param name="offset"> The <paramref name="buffer"/> element position where mapping starts. </param>
        /// <param name="region"> The region of elements to map. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public IntPtr Map<T>(
            ComputeBufferBase<T> buffer,
            bool blocking,
            ComputeMemoryMappingFlags flags,
            long offset,
            long region,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;
            var mappedPtr = CL10.EnqueueMapBuffer(Handle, buffer.Handle, blocking, flags,
                new IntPtr(offset * sizeofT), new IntPtr(region * sizeofT), eventWaitListSize,
                eventHandles, newEventHandle, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));

            return mappedPtr;
        }

        /// <summary>
        /// Enqueues a command to map a part of a image into the host address space.
        /// </summary>
        /// <param name="image"> The image to map. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="flags"> A list of properties for the mapping mode. </param>
        /// <param name="offset"> The <paramref name="image"/> element position where mapping starts. </param>
        /// <param name="region"> The region of elements to map. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public IntPtr Map(ComputeImage image,
            bool blocking,
            ComputeMemoryMappingFlags flags,
            SysIntX3 offset,
            SysIntX3 region,
            ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;
            var mappedPtr = CL10.EnqueueMapImage(Handle, image.Handle, blocking, flags, ref offset, ref region,
                out _, out _, eventWaitListSize, eventHandles,
                newEventHandle, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));

            return mappedPtr;
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="offset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public T[] Read<T>(
            ComputeBufferBase<T> source,
            bool blocking,
            long offset,
            long region,
            ICollection<IComputeEvent> events) where T : struct
        {
            var value = new T[region - offset];
            GCHandle gch = GCHandle.Alloc(value, GCHandleType.Pinned);
            Read(source, blocking, offset, region, gch.AddrOfPinnedObject(), events);
            gch.Free();
            return value;
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="offset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public void Read<T>(
            ComputeBufferBase<T> source,
            bool blocking,
            long offset,
            long region,
            IntPtr destination,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueReadBuffer(
                Handle,
                source.Handle,
                blocking,
                new IntPtr(offset * sizeofT),
                new IntPtr(region * sizeofT),
                destination,
                eventWaitListSize,
                eventHandles,
                newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
            {
                events.Add(new ComputeEvent(newEventHandle[0], this));
            }
        }

        /// <summary>
        /// Enqueues a command to read a 2D or 3D region of elements from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="sourceRowPitch"> The size of the source buffer row in bytes. If set to zero then <paramref name="sourceRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="sourceSlicePitch"> The size of the source buffer 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch"/> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
        /// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch"/> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
        /// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        private void Read<T>(
            ComputeBufferBase<T> source,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long sourceRowPitch,
            long sourceSlicePitch,
            long destinationRowPitch,
            long destinationSlicePitch,
            IntPtr destination,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            sourceOffset.X = new IntPtr(sizeofT * sourceOffset.X.ToInt64());
            destinationOffset.X = new IntPtr(sizeofT * destinationOffset.X.ToInt64());
            region.X = new IntPtr(sizeofT * region.X.ToInt64());

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL11.EnqueueReadBufferRect(this.Handle, source.Handle, blocking, ref sourceOffset,
                ref destinationOffset, ref region, new IntPtr(sourceRowPitch), new IntPtr(sourceSlicePitch),
                new IntPtr(destinationRowPitch), new IntPtr(destinationSlicePitch), destination, eventWaitListSize,
                eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to read data from a image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="offset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="rowPitch"> The image row pitch of source or 0. </param>
        /// <param name="slicePitch"> The image slice pitch of source or 0. </param>
        /// <param name="destination"> A pointer to a preallocated memory area to read the data into. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public void Read(
            ComputeImage source,
            bool blocking,
            SysIntX3 offset,
            SysIntX3 region,
            long rowPitch,
            long slicePitch,
            IntPtr destination,
            ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueReadImage(Handle, source.Handle, blocking, ref offset, ref region,
                new IntPtr(rowPitch), new IntPtr(slicePitch), destination,
                eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to release memorys that have been created from OpenGL objects.
        /// </summary>
        /// <param name="memObjs"> A collection of memorys that correspond to OpenGL memory objects. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void ReleaseGLObjects(ICollection<ComputeMemory> memObjs, ICollection<IComputeEvent> events)
        {
            var memObjHandles = ComputeTools.ExtractHandles(memObjs, out int memObjCount);

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueReleaseGLObjects(Handle, memObjCount, memObjHandles, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to unmap a buffer or a image from the host address space.
        /// </summary>
        /// <param name="memory"> The memory. </param>
        /// <param name="mappedPtr"> The host address returned by a previous call to map. This pointer is <c>IntPtr.Zero</c> after this method returns. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        public void Unmap(ComputeMemory memory, ref IntPtr mappedPtr, ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueUnmapMemObject(Handle, memory.Handle, mappedPtr,
                eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            mappedPtr = IntPtr.Zero;

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a wait command for a collection of events to complete before any future commands queued in the command queue are executed.
        /// </summary>
        /// <param name="events"> The events that this command will wait for. </param>
        public void Wait(ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var error = CL10.EnqueueWaitForEvents(Handle, eventWaitListSize, eventHandles);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="source"> The data written to the buffer. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public void Write<T>(
            ComputeBufferBase<T> destination,
            bool blocking,
            long destinationOffset,
            long region,
            IntPtr source,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueWriteBuffer(Handle, destination.Handle, blocking, new IntPtr(destinationOffset * sizeofT),
                new IntPtr(region * sizeofT), source, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to write a 2D or 3D region of elements to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="destinationRowPitch"> The size of the destination buffer row in bytes. If set to zero then <paramref name="destinationRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="destinationSlicePitch"> The size of the destination buffer 2D slice in bytes. If set to zero then <paramref name="destinationSlicePitch"/> equals <c>region.Y * sizeof(T) * destinationRowPitch</c>. </param>
        /// <param name="sourceRowPitch"> The size of the memory area row in bytes. If set to zero then <paramref name="sourceRowPitch"/> equals <c>region.X * sizeof(T)</c>. </param>
        /// <param name="sourceSlicePitch"> The size of the memory area 2D slice in bytes. If set to zero then <paramref name="sourceSlicePitch"/> equals <c>region.Y * sizeof(T) * sourceRowPitch</c>. </param>
        /// <param name="source"> The data written to the buffer. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        private void Write<T>(
            ComputeBufferBase<T> destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long destinationRowPitch,
            long destinationSlicePitch,
            long sourceRowPitch,
            long sourceSlicePitch,
            IntPtr source,
            ICollection<IComputeEvent> events) where T : struct
        {
            int sizeofT = Marshal.SizeOf(typeof(T));

            sourceOffset.X = new IntPtr(sizeofT * sourceOffset.X.ToInt64());
            destinationOffset.X = new IntPtr(sizeofT * destinationOffset.X.ToInt64());
            region.X = new IntPtr(sizeofT * region.X.ToInt64());

            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL11.EnqueueWriteBufferRect(this.Handle, destination.Handle, blocking,
                ref destinationOffset, ref sourceOffset, ref region, new IntPtr(destinationRowPitch),
                new IntPtr(destinationSlicePitch), new IntPtr(sourceRowPitch), new IntPtr(sourceSlicePitch),
                source, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        /// <summary>
        /// Enqueues a command to write data to a image.
        /// </summary>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="rowPitch"> The image row pitch of <paramref name="destination"/> or 0. </param>
        /// <param name="slicePitch"> The image slice pitch of <paramref name="destination"/> or 0. </param>
        /// <param name="source"> The content written to the image. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If events is not <c>null</c> or read-only a new event identifying this command is created and attached to the end of the collection. </param>
        /// <remarks> If <paramref name="blocking"/> is <c>true</c> this method will not return until the command completes. If <paramref name="blocking"/> is <c>false</c> this method will return immediately after the command is enqueued. </remarks>
        public void Write(
            ComputeImage destination,
            bool blocking,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long rowPitch,
            long slicePitch,
            IntPtr source,
            ICollection<IComputeEvent> events)
        {
            var eventHandles = ComputeTools.ExtractHandles(events, out int eventWaitListSize);
            var eventsWritable = (events != null && !events.IsReadOnly);
            var newEventHandle = (eventsWritable) ? new CLEventHandle[1] : null;

            var error = CL10.EnqueueWriteImage(Handle, destination.Handle, blocking, ref destinationOffset,
                ref region, new IntPtr(rowPitch), new IntPtr(slicePitch),
                source, eventWaitListSize, eventHandles, newEventHandle);
            ComputeException.ThrowOnError(error);

            if (eventsWritable)
                events.Add(new ComputeEvent(newEventHandle[0], this));
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Releases the associated OpenCL object.
        /// </summary>
        /// <param name="manual"> Specifies the operation mode of this method. </param>
        /// <remarks> <paramref name="manual"/> must be <c>true</c> if this method is invoked directly by the application. </remarks>
        protected override void Dispose(bool manual)
        {
            if (Handle.IsValid)
            {
                logger.Info("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
                OpenCL100.ReleaseCommandQueue(Handle);
                Handle.Invalidate();
            }
        }

        #endregion

        #region CopyBuffer

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, 0, 0, source.Count, events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            long sourceOffset,
            long destinationOffset,
            long region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, new SysIntX3(sourceOffset, 0), new SysIntX3(destinationOffset, 0),
                new SysIntX3(region, 1), 0, 0, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, 0, 0, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            long sourceRowPitch,
            long destinationRowPitch,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, new SysIntX3(sourceOffset, 0), new SysIntX3(destinationOffset, 0),
                new SysIntX3(region, 1), sourceRowPitch, 0, destinationRowPitch, 0, events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source buffer to a destination buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffers. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBuffer<T>(
            ComputeBufferBase<T> source,
            ComputeBufferBase<T> destination,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long sourceRowPitch,
            long destinationRowPitch,
            long sourceSlicePitch,
            long destinationSlicePitch,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, sourceRowPitch,
                sourceSlicePitch, destinationRowPitch, destinationSlicePitch, events);
        }

        #endregion

        #region CopyBufferToImage

        /// <summary>
        /// Enqueues a command to copy data from a buffer to an image.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="source"/>. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBufferToImage<T>(
            ComputeBufferBase<T> source,
            ComputeImage destination,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, 0, new SysIntX3(),
                new SysIntX3(destination.Width, destination.Height, (destination.Depth == 0) ? 1 : destination.Depth), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a buffer to an image.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="source"/>. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBufferToImage<T>(
            ComputeBufferBase<T> source,
            ComputeImage2D destination,
            long sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a buffer to an image.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="source"/>. </typeparam>
        /// <param name="source"> The buffer to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyBufferToImage<T>(
            ComputeBufferBase<T> source,
            ComputeImage3D destination,
            long sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, events);
        }

        #endregion

        #region CopyImage

        /// <summary>
        /// Enqueues a command to copy data from a source image to a destination image.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImage(
            ComputeImage source,
            ComputeImage destination,
            ICollection<IComputeEvent> events)
        {
            Copy(source, destination, new SysIntX3(), new SysIntX3(),
                new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source image to a destination image.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImage(
            ComputeImage2D source,
            ComputeImage2D destination,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events)
        {
            Copy(source, destination, new SysIntX3(sourceOffset, 0),
                new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source image to a destination image.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImage(
            ComputeImage2D source,
            ComputeImage3D destination,
            SysIntX2 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events)
        {
            Copy(source, destination, new SysIntX3(sourceOffset, 0),
                destinationOffset, new SysIntX3(region, 1), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source image to a destination image.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImage(
            ComputeImage3D source,
            ComputeImage2D destination,
            SysIntX3 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events)
        {
            Copy(source, destination, sourceOffset, new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), null);
        }

        /// <summary>
        /// Enqueues a command to copy data from a source image to a destination image.
        /// </summary>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The image to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImage(
            ComputeImage3D source,
            ComputeImage3D destination,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events)
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, events);
        }

        #endregion

        #region CopyImageToBuffer

        /// <summary>
        /// Enqueues a command to copy data from an image to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="destination"/>. </typeparam>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImageToBuffer<T>(
            ComputeImage source,
            ComputeBufferBase<T> destination,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, new SysIntX3(), 0,
                new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from an image to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="destination"/>. </typeparam>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImageToBuffer<T>(
            ComputeImage2D source,
            ComputeBufferBase<T> destination,
            SysIntX2 sourceOffset,
            long destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, new SysIntX3(sourceOffset, 0), destinationOffset, new SysIntX3(region, 1), events);
        }

        /// <summary>
        /// Enqueues a command to copy data from a 3D image to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in <paramref name="destination"/>. </typeparam>
        /// <param name="source"> The image to copy from. </param>
        /// <param name="destination"> The buffer to copy to. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to copy. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void CopyImageToBuffer<T>(
            ComputeImage3D source,
            ComputeBufferBase<T> destination,
            SysIntX3 sourceOffset,
            long destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events) where T : struct
        {
            Copy(source, destination, sourceOffset, destinationOffset, region, events);
        }

        #endregion

        #region ReadFromBuffer

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[] destination,
            bool blocking,
            IList<IComputeEvent> events) where T : struct
        {
            ReadFromBuffer(source, ref destination, blocking, 0, 0, source.Count, events);
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[] destination,
            bool blocking,
            long sourceOffset,
            long destinationOffset,
            long region,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle destinationGCHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            IntPtr destinationOffsetPtr = Marshal.UnsafeAddrOfPinnedArrayElement(destination, (int)destinationOffset);

            if (blocking)
            {
                Read(source, blocking, sourceOffset, region, destinationOffsetPtr, events);
                destinationGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Read(source, blocking, sourceOffset, region, destinationOffsetPtr, eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(destinationGCHandle);
            }
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[,] destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            IList<IComputeEvent> events) where T : struct
        {
            ReadFromBuffer(source, ref destination, blocking, sourceOffset, destinationOffset, region, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[,,] destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            IList<IComputeEvent> events) where T : struct
        {
            ReadFromBuffer(source, ref destination, blocking, sourceOffset, destinationOffset, region, 0, 0, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[,] destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            long sourceRowPitch,
            long destinationRowPitch,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle destinationGCHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);

            if (blocking)
            {
                Read(source, blocking, new SysIntX3(sourceOffset, 0),
                    new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1),
                    sourceRowPitch, 0, destinationRowPitch, 0, destinationGCHandle.AddrOfPinnedObject(), events);
                destinationGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Read(source, blocking, new SysIntX3(sourceOffset, 0), new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), sourceRowPitch, 0, destinationRowPitch, 0, destinationGCHandle.AddrOfPinnedObject(), eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(destinationGCHandle);
            }
        }

        /// <summary>
        /// Enqueues a command to read data from a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The buffer to read from. </param>
        /// <param name="destination"> The array to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromBuffer<T>(
            ComputeBufferBase<T> source,
            ref T[,,] destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long sourceRowPitch,
            long destinationRowPitch,
            long sourceSlicePitch,
            long destinationSlicePitch,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle destinationGCHandle = GCHandle.Alloc(destination, GCHandleType.Pinned);

            if (blocking)
            {
                Read(source, blocking, sourceOffset, destinationOffset, region,
                    sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch,
                    destinationGCHandle.AddrOfPinnedObject(), events);
                destinationGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Read(source, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, destinationGCHandle.AddrOfPinnedObject(), eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(destinationGCHandle);
            }
        }

        #endregion

        #region ReadFromImage

        /// <summary>
        /// Enqueues a command to read data from an image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromImage(
            ComputeImage source,
            IntPtr destination,
            bool blocking,
            ICollection<IComputeEvent> events)
        {
            Read(source, blocking, new SysIntX3(),
                new SysIntX3(source.Width, source.Height, (source.Depth == 0) ? 1 : source.Depth),
                0, 0, destination, events);
        }

        /// <summary>
        /// Enqueues a command to read data from an image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromImage(
            ComputeImage2D source,
            IntPtr destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events)
        {
            Read(source, blocking, new SysIntX3(sourceOffset, 0), new SysIntX3(region, 1), 0, 0, destination, events);
        }

        /// <summary>
        /// Enqueues a command to read data from an image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromImage(
            ComputeImage3D source,
            IntPtr destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events)
        {
            Read(source, blocking, sourceOffset, region, 0, 0, destination, events);
        }

        /// <summary>
        /// Enqueues a command to read data from an image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="sourceRowPitch"> The size of a row of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromImage(
            ComputeImage2D source,
            IntPtr destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 region,
            long sourceRowPitch,
            ICollection<IComputeEvent> events)
        {
            Read(source, blocking, new SysIntX3(sourceOffset, 0),
                new SysIntX3(region, 1), sourceRowPitch, 0, destination, events);
        }

        /// <summary>
        /// Enqueues a command to read data from an image.
        /// </summary>
        /// <param name="source"> The image to read from. </param>
        /// <param name="destination"> A valid pointer to a preallocated memory area to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="region"> The region of elements to read. </param>
        /// <param name="sourceRowPitch"> The size of a row of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="sourceSlicePitch"> The size of a 2D slice of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void ReadFromImage(
            ComputeImage3D source,
            IntPtr destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 region,
            long sourceRowPitch,
            long sourceSlicePitch,
            ICollection<IComputeEvent> events)
        {
            Read(source, blocking, sourceOffset, region, sourceRowPitch,
                sourceSlicePitch, destination, events);
        }

        #endregion

        #region WriteToBuffer

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            IList<IComputeEvent> events) where T : struct
        {
            WriteToBuffer(source, destination, blocking, 0, 0, destination.Count, events);
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            long sourceOffset,
            long destinationOffset,
            long region,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle sourceGCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
            IntPtr sourceOffsetPtr = Marshal.UnsafeAddrOfPinnedArrayElement(source, (int)sourceOffset);

            if (blocking)
            {
                Write(destination, blocking, destinationOffset, region, sourceOffsetPtr, events);
                sourceGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Write(destination, blocking, destinationOffset, region, sourceOffsetPtr, eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(sourceGCHandle);
            }
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[,] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            IList<IComputeEvent> events) where T : struct
        {
            WriteToBuffer(source, destination, blocking, sourceOffset, destinationOffset, region, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[,,] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            IList<IComputeEvent> events) where T : struct
        {
            WriteToBuffer(source, destination, blocking, sourceOffset, destinationOffset, region, 0, 0, 0, 0, events);
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[,] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            SysIntX2 sourceOffset,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            long sourceRowPitch,
            long destinationRowPitch,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle sourceGCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);

            if (blocking)
            {
                Write(destination, blocking, new SysIntX3(sourceOffset, 0),
                    new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1),
                    sourceRowPitch, 0, destinationRowPitch, 0,
                    sourceGCHandle.AddrOfPinnedObject(), events);
                sourceGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Write(destination, blocking, new SysIntX3(sourceOffset, 0), new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), sourceRowPitch, 0, destinationRowPitch, 0, sourceGCHandle.AddrOfPinnedObject(), eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(sourceGCHandle);
            }
        }

        /// <summary>
        /// Enqueues a command to write data to a buffer.
        /// </summary>
        /// <typeparam name="T"> The type of data in the buffer. </typeparam>
        /// <param name="source"> The array to read from. </param>
        /// <param name="destination"> The buffer to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="sourceOffset"> The <paramref name="source"/> element position where reading starts. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="sourceRowPitch"> The size of a row of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationRowPitch"> The size of a row of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="sourceSlicePitch"> The size of a 2D slice of elements of <paramref name="source"/> in bytes. </param>
        /// <param name="destinationSlicePitch"> The size of a 2D slice of elements of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToBuffer<T>(
            T[,,] source,
            ComputeBufferBase<T> destination,
            bool blocking,
            SysIntX3 sourceOffset,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long sourceRowPitch,
            long destinationRowPitch,
            long sourceSlicePitch,
            long destinationSlicePitch,
            IList<IComputeEvent> events) where T : struct
        {
            GCHandle sourceGCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);

            if (blocking)
            {
                Write(destination, blocking, sourceOffset,
                    destinationOffset, region, sourceRowPitch,
                    sourceSlicePitch, destinationRowPitch, destinationSlicePitch,
                    sourceGCHandle.AddrOfPinnedObject(), events);
                sourceGCHandle.Free();
            }
            else
            {
                bool userEventsWritable = (events != null && !events.IsReadOnly);
                var eventList = (userEventsWritable) ? events : Events;
                Write(destination, blocking, sourceOffset, destinationOffset, region, sourceRowPitch, sourceSlicePitch, destinationRowPitch, destinationSlicePitch, sourceGCHandle.AddrOfPinnedObject(), eventList);
                var newEvent = (ComputeEvent)eventList[eventList.Count - 1];
                newEvent.TrackGCHandle(sourceGCHandle);
            }
        }

        #endregion

        #region WriteToImage

        /// <summary>
        /// Enqueues a command to write data to an image.
        /// </summary>
        /// <param name="source"> A pointer to a memory area to read from. </param>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToImage(
            IntPtr source,
            ComputeImage destination,
            bool blocking,
            ICollection<IComputeEvent> events)
        {
            Write(destination, blocking, new SysIntX3(),
                new SysIntX3(destination.Width, destination.Height, (destination.Depth == 0) ? 1 : destination.Depth),
                0, 0, source, events);
        }

        /// <summary>
        /// Enqueues a command to write data to an image.
        /// </summary>
        /// <param name="source"> A pointer to a memory area to read from. </param>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToImage(
            IntPtr source,
            ComputeImage2D destination,
            bool blocking,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            ICollection<IComputeEvent> events)
        {
            Write(destination, blocking, new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), 0, 0, source, events);
        }

        /// <summary>
        /// Enqueues a command to write data to an image.
        /// </summary>
        /// <param name="source"> A pointer to a memory area to read from. </param>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToImage(
            IntPtr source,
            ComputeImage3D destination,
            bool blocking,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            ICollection<IComputeEvent> events)
        {
            Write(destination, blocking, destinationOffset, region, 0, 0, source, events);
        }

        /// <summary>
        /// Enqueues a command to write data to an image.
        /// </summary>
        /// <param name="source"> A pointer to a memory area to read from. </param>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="destinationRowPitch"> The size of a row of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToImage(
            IntPtr source,
            ComputeImage2D destination,
            bool blocking,
            SysIntX2 destinationOffset,
            SysIntX2 region,
            long destinationRowPitch,
            ICollection<IComputeEvent> events)
        {
            Write(destination, blocking, new SysIntX3(destinationOffset, 0), new SysIntX3(region, 1), destinationRowPitch, 0, source, events);
        }

        /// <summary>
        /// Enqueues a command to write data to an image.
        /// </summary>
        /// <param name="source"> A pointer to a memory area to read from. </param>
        /// <param name="destination"> The image to write to. </param>
        /// <param name="blocking"> The mode of operation of this command. If <c>true</c> this call will not return until the command has finished execution. </param>
        /// <param name="destinationOffset"> The <paramref name="destination"/> element position where writing starts. </param>
        /// <param name="region"> The region of elements to write. </param>
        /// <param name="destinationRowPitch"> The size of a row of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="destinationSlicePitch"> The size of a 2D slice of pixels of <paramref name="destination"/> in bytes. </param>
        /// <param name="events"> A collection of events that need to complete before this particular command can be executed. If <paramref name="events"/> is not <c>null</c> a new event identifying this command is attached to the end of the collection. </param>
        public void WriteToImage(
            IntPtr source,
            ComputeImage3D destination,
            bool blocking,
            SysIntX3 destinationOffset,
            SysIntX3 region,
            long destinationRowPitch,
            long destinationSlicePitch,
            ICollection<IComputeEvent> events)
        {
            Write(destination, blocking, destinationOffset, region,
                destinationRowPitch, destinationSlicePitch, source, events);
        }

        #endregion
    }
}
