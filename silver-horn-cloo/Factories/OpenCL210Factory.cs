using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Platform;
using SilverHorn.Cloo.Program;
using SilverHorn.Cloo.Sampler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SilverHorn.Cloo.Factories
{
    public sealed class OpenCL210Factory : IOpenCLFactory
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Compute Program Constructors
        /// <summary>
        /// Creates a new program from a source code string.
        /// </summary>
        /// <param name="context"> A program. </param>
        /// <param name="source"> The source code for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public IComputeProgram BuildComputeProgram(IComputeContext context, string source)
        {
            var program = new ComputeProgram210();
            program.Handle = OpenCL210.CreateProgramWithSource(
                context.Handle,
                1,
                new string[] { source },
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            program.SetID(program.Handle.Value);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }

        /// <summary>
        /// Creates a new program from an array of source code strings.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="source"> The source code lines for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public IComputeProgram BuildComputeProgram(IComputeContext context, string[] source)
        {
            var program = new ComputeProgram210();
            program.Handle = OpenCL210.CreateProgramWithSource(
                context.Handle,
                source.Length,
                source,
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }

        /// <summary>
        /// Creates a new program from a specified list of binaries.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="binaries"> A list of binaries, one for each item in <paramref name="devices"/>. </param>
        /// <param name="devices"> A subset of the context devices. If <paramref name="devices"/> is <c>null</c>, OpenCL will associate every binary from binaries with a corresponding device from devices. </param>
        public IComputeProgram BuildComputeProgram(IComputeContext context, IList<byte[]> binaries, IList<IComputeDevice> devices)
        {
            var program = new ComputeProgram210();
            int count;
            CLDeviceHandle[] deviceHandles;
            if (devices != null)
            {
                deviceHandles = ComputeTools.ExtractHandles(devices, out count);
            }
            else
            {
                deviceHandles = ComputeTools.ExtractHandles(context.Devices, out count);
            }

            IntPtr[] binariesPtrs = new IntPtr[count];
            IntPtr[] binariesLengths = new IntPtr[count];
            int[] binariesStats = new int[count];
            GCHandle[] binariesGCHandles = new GCHandle[count];

            try
            {
                for (int i = 0; i < count; i++)
                {
                    binariesGCHandles[i] = GCHandle.Alloc(binaries[i], GCHandleType.Pinned);
                    binariesPtrs[i] = binariesGCHandles[i].AddrOfPinnedObject();
                    binariesLengths[i] = new IntPtr(binaries[i].Length);
                }

                program.Handle = OpenCL210.CreateProgramWithBinary(
                    context.Handle,
                    count,
                    deviceHandles,
                    binariesLengths,
                    binariesPtrs,
                    binariesStats,
                    out ComputeErrorCode error);
                ComputeException.ThrowOnError(error);
            }
            finally
            {
                for (int i = 0; i < count; i++)
                {
                    binariesGCHandles[i].Free();
                }
            }
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }
        #endregion

        #region Kernel Constructors
        /// <summary>
        /// Creates a kernel for every <c>kernel</c> function in program.
        /// </summary>
        /// <returns> The collection of created kernels. </returns>
        /// <remarks> kernels are not created for any <c>kernel</c> functions in program that do not have the same function definition across all devices for which a program executable has been successfully built. </remarks>
        public ICollection<IComputeKernel> CreateAllKernels(IComputeProgram program)
        {
            var kernels = new Collection<IComputeKernel>();
            var error = OpenCL110.CreateKernelsInProgram(
                program.Handle,
                0,
                null,
                out int kernelsCount);
            ComputeException.ThrowOnError(error);

            var kernelHandles = new CLKernelHandle[kernelsCount];
            error = OpenCL110.CreateKernelsInProgram(
                program.Handle,
                kernelsCount,
                kernelHandles,
                out kernelsCount);
            ComputeException.ThrowOnError(error);

            for (int i = 0; i < kernelsCount; i++)
            {
                kernels.Add(CreateKernel(kernelHandles[i]));
            }
            return kernels;
        }

        internal IComputeKernel CreateKernel(CLKernelHandle handle)
        {
            var kernel = new ComputeKernel210();
            kernel.Handle = handle;
            kernel.SetID(kernel.Handle.Value);

            kernel.FunctionName = kernel.GetStringInfo<CLKernelHandle, ComputeKernelInfo>(kernel.Handle,
                ComputeKernelInfo.FunctionName, OpenCL210.GetKernelInfo);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return kernel;
        }

        /// <summary>
        /// Creates a kernel for a kernel function of a specified name.
        /// </summary>
        /// <returns> The created kernel. </returns>
        public IComputeKernel CreateKernel(IComputeProgram program, string functionName)
        {
            var kernel = new ComputeKernel210();
            kernel.Handle = OpenCL210.CreateKernel(
                program.Handle,
                functionName,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            kernel.SetID(kernel.Handle.Value);
            kernel.FunctionName = functionName;
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return kernel;
        }
        #endregion

        #region Sampler Constructors
        /// <summary>
        /// Creates a new sampler.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="normalizedCoords"> The usage state of normalized coordinates when accessing a image in a kernel. </param>
        /// <param name="addressing"> The <see cref="ComputeImageAddressing"/> mode of the sampler. Specifies how out-of-range image coordinates are handled while reading. </param>
        /// <param name="filtering"> The <see cref="ComputeImageFiltering"/> mode of the sampler. Specifies the type of filter that must be applied when reading data from an image. </param>
        public IComputeSampler CreateSampler(IComputeContext context, bool normalizedCoords,
            ComputeImageAddressing addressing, ComputeImageFiltering filtering)
        {
            var sampler = new ComputeSampler210();
            sampler.Handle = OpenCL210.CreateSampler(context.Handle, normalizedCoords, addressing, filtering, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            sampler.SetID(sampler.Handle.Value);

            sampler.Addressing = addressing;
            sampler.Filtering = filtering;
            sampler.NormalizedCoords = normalizedCoords;

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return sampler;
        }
        #endregion

        #region Context Constructors
        /// <summary>
        /// Creates a new context on a collection of devices.
        /// </summary>
        /// <param name="devices"> A collection of devices to associate with the context. </param>
        /// <param name="properties"> A list of context properties of the context. </param>
        /// <param name="notify"> A delegate instance that refers to a notification routine. This routine is a callback function that will be used by the OpenCL implementation to report information on errors that occur in the context. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until context is disposed. If <paramref name="notify"/> is <c>null</c>, no callback function is registered. </param>
        /// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify"/>. </param>
        public IComputeContext CreateContext(ICollection<IComputeDevice> devices, List<ComputeContextProperty> properties,
            ComputeContextNotifier notify, IntPtr notifyDataPtr)
        {
            var context = new ComputeContext210();
            var deviceHandles = ComputeTools.ExtractHandles(devices, out int handleCount);
            var propertyArray = context.ToIntPtrArray(properties);
            context.Handle = OpenCL210.CreateContext(
                propertyArray,
                handleCount,
                deviceHandles,
                notify,
                notifyDataPtr,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            context.SetID(context.Handle.Value);
            var platformProperty = context.GetByName(properties, ComputeContextPropertyName.Platform);
            context.Platform = ComputePlatform.GetByHandle(platformProperty.Value);
            context.Devices = context.GetDevices();

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return context;
        }

        /// <summary>
        /// Creates a new context on all the devices that match the specified <see cref="ComputeDeviceTypes"/>.
        /// </summary>
        /// <param name="deviceType"> A bit-field that identifies the type of device to associate with the context. </param>
        /// <param name="properties"> A list of context properties of the context. </param>
        /// <param name="notify"> A delegate instance that refers to a notification routine. This routine is a callback function that will be used by the OpenCL implementation to report information on errors that occur in the context. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until context is disposed. If <paramref name="notify"/> is <c>null</c>, no callback function is registered. </param>
        /// <param name="userDataPtr"> Optional user data that will be passed to <paramref name="notify"/>. </param>
        public IComputeContext CreateContext(ComputeDeviceTypes deviceType, List<ComputeContextProperty> properties,
            ComputeContextNotifier notify, IntPtr userDataPtr)
        {
            var context = new ComputeContext210();
            var propertyArray = context.ToIntPtrArray(properties);
            context.Handle = OpenCL210.CreateContextFromType(
                propertyArray,
                deviceType,
                notify,
                userDataPtr,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            context.SetID(context.Handle.Value);
            var platformProperty = context.GetByName(properties, ComputeContextPropertyName.Platform);
            context.Platform = ComputePlatform.GetByHandle(platformProperty.Value);
            context.Devices = context.GetDevices();
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return context;
        }
        #endregion


    }
}
