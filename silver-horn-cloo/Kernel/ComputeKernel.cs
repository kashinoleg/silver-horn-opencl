using System;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Program;
using SilverHorn.Cloo.Sampler;

namespace SilverHorn.Cloo.Kernel
{
    /// <summary>
    /// Represents an OpenCL kernel.
    /// </summary>
    /// <remarks> A kernel object encapsulates a specific kernel function declared in a program and the argument values to be used when executing this kernel function. </remarks>
    public sealed class ComputeKernel : ComputeObject, IComputeKernel
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// The handle of the kernel.
        /// </summary>
        public CLKernelHandle Handle { get; private set; }

        /// <summary>
        /// Gets the function name of the kernel.
        /// </summary>
        /// <value> The function name of the kernel. </value>
        public string FunctionName { get; private set; }
        #endregion

        #region Constructors
        internal ComputeKernel(CLKernelHandle handle)
        {
            Handle = handle;
            SetID(Handle.Value);

            FunctionName = GetStringInfo<CLKernelHandle, ComputeKernelInfo>(Handle, ComputeKernelInfo.FunctionName, CL10.GetKernelInfo);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        internal ComputeKernel(string functionName, IComputeProgram program)
        {
            Handle = CL10.CreateKernel(
                program.Handle,
                functionName,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);
            FunctionName = functionName;
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Gets the amount of local memory in bytes used by the kernel.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The amount of local memory in bytes used by the kernel. </returns>
        public long GetLocalMemorySize(IComputeDevice device)
        {
            return GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, long>(
                Handle, device.Handle, ComputeKernelWorkGroupInfo.LocalMemorySize, CL10.GetKernelWorkGroupInfo);
        }

        /// <summary>
        /// Gets the compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier. If no such qualifier is specified, (0, 0, 0) is returned. </returns>
        public long[] GetCompileWorkGroupSize(IComputeDevice device)
        {
            return ComputeTools.ConvertArray(
                GetArrayInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(
                    Handle, device.Handle, ComputeKernelWorkGroupInfo.CompileWorkGroupSize, CL10.GetKernelWorkGroupInfo));
        }

        /// <summary>
        /// Gets the preferred multiple of workgroup size for launch. 
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The preferred multiple of workgroup size for launch. </returns>
        /// <remarks> The returned value is a performance hint. Specifying a workgroup size that is not a multiple of the value returned by this query as the value of the local work size argument to ComputeCommandQueue.Execute will not fail to enqueue the kernel for execution unless the work-group size specified is larger than the device maximum. </remarks>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public long GetPreferredWorkGroupSizeMultiple(IComputeDevice device)
        {
            return (long)GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(
                Handle, device.Handle, ComputeKernelWorkGroupInfo.PreferredWorkGroupSizeMultiple, CL10.GetKernelWorkGroupInfo);
        }

        /// <summary>
        /// Gets the minimum amount of memory, in bytes, used by each work-item in the kernel.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The minimum amount of memory, in bytes, used by each work-item in the kernel. </returns>
        /// <remarks> The returned value may include any private memory needed by an implementation to execute the kernel, including that used by the language built-ins and variable declared inside the kernel with the <c>__private</c> or <c>private</c> qualifier. </remarks>
        public long GetPrivateMemorySize(IComputeDevice device)
        {
            return GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, long>(
                Handle, device.Handle, ComputeKernelWorkGroupInfo.PrivateMemorySize, CL10.GetKernelWorkGroupInfo);
        }

        /// <summary>
        /// Gets the maximum work-group size that can be used to execute the kernel on a device.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The maximum work-group size that can be used to execute the kernel on device. </returns>
        public long GetWorkGroupSize(IComputeDevice device)
        {
            return (long)GetInfo<CLKernelHandle, CLDeviceHandle, ComputeKernelWorkGroupInfo, IntPtr>(
                    Handle, device.Handle, ComputeKernelWorkGroupInfo.WorkGroupSize, CL10.GetKernelWorkGroupInfo);
        }

        /// <summary>
        /// Sets an argument of the kernel (no argument tracking).
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="dataSize"> The size of the argument data in bytes. </param>
        /// <param name="dataAddr"> A pointer to the data that should be used as the argument value. </param>
        /// <remarks> 
        /// Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel.
        /// <br/>
        /// Note that this method does not provide argument tracking. It is up to the user to reference the kernel arguments (i.e. prevent them from being garbage collected) until the kernel has finished execution.
        /// </remarks>
        public void SetArgument(int index, IntPtr dataSize, IntPtr dataAddr)
        {
            var error = CL10.SetKernelArg(
                Handle,
                index,
                dataSize,
                dataAddr);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Sets the size in bytes of an argument specfied with the <c>local</c> or <c>__local</c> address space qualifier.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="dataSize"> The size of the argument data in bytes. </param>
        /// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        public void SetLocalArgument(int index, long dataSize)
        {
            SetArgument(index, new IntPtr(dataSize), IntPtr.Zero);
        }

        /// <summary>
        /// Sets a <c>T*</c>, <c>image2d_t</c> or <c>image3d_t</c> argument of the kernel.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="memObj"> The memory that is passed as the argument. </param>
        /// <remarks> This method will automatically track <paramref name="memObj"/> to prevent it from being collected by the GC.<br/> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        public void SetMemoryArgument(int index, ComputeMemory memObj)
        {
            SetValueArgument<CLMemoryHandle>(index, memObj.Handle);
        }

        /// <summary>
        /// Sets a <c>sampler_t</c> argument of the kernel.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="sampler"> The sampler that is passed as the argument. </param>
        /// <remarks> This method will automatically track sampler to prevent it from being collected by the GC.<br/> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        public void SetSamplerArgument(int index, IComputeSampler sampler)
        {
            SetValueArgument<CLSamplerHandle>(index, sampler.Handle);
        }

        /// <summary>
        /// Sets a value argument of the kernel.
        /// </summary>
        /// <typeparam name="T"> The type of the argument. </typeparam>
        /// <param name="index"> The argument index. </param>
        /// <param name="data"> The data that is passed as the argument value. </param>
        /// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        public void SetValueArgument<T>(int index, T data) where T : struct
        {
            GCHandle gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                SetArgument(
                    index,
                    new IntPtr(Marshal.SizeOf(typeof(T))),
                    gcHandle.AddrOfPinnedObject());
            }
            finally
            {
                gcHandle.Free();
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }
                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.
                if (Handle.IsValid)
                {
                    logger.Info("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
                    CL10.ReleaseKernel(Handle);
                    Handle.Invalidate();
                }
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~ComputeKernel()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
