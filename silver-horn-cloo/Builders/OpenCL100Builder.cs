using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Program;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SilverHorn.Cloo.Builders
{
    public sealed class OpenCL100Builder : IOpenCLBuilder
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
        public ComputeProgram BuildComputeProgram(IComputeContext context, string source)
        {
            var program = new ComputeProgram();
            program.Handle = OpenCL100.CreateProgramWithSource(
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
        public ComputeProgram BuildComputeProgram(IComputeContext context, string[] source)
        {
            var program = new ComputeProgram();
            program.Handle = OpenCL100.CreateProgramWithSource(
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
        public ComputeProgram BuildComputeProgram(IComputeContext context, IList<byte[]> binaries, IList<IComputeDevice> devices)
        {
            var program = new ComputeProgram();
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

                program.Handle = OpenCL100.CreateProgramWithBinary(
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

        #region Public methods
        /// <summary>
        /// Builds (compiles and links) a program executable from the program source or binary for all or some of the devices.
        /// </summary>
        /// <param name="devices"> A subset or all of devices. If devices is <c>null</c>, the executable is built for every item of devices for which a source or a binary has been loaded. </param>
        /// <param name="options"> A set of options for the OpenCL compiler. </param>
        /// <param name="notify"> A delegate instance that represents a reference to a notification routine. This routine is a callback function that an application can register and which will be called when the program executable has been built (successfully or unsuccessfully). If <paramref name="notify"/> is not <c>null</c>, build does not need to wait for the build to complete and can return immediately. If <paramref name="notify"/> is <c>null</c>, build does not return until the build has completed. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until the build operation triggers the callback. </param>
        /// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify"/>. </param>
        public void PogramBuild(ComputeProgram program, ICollection<IComputeDevice> devices, string options,
            ComputeProgramBuildNotifier notify, IntPtr notifyDataPtr)
        {
            var deviceHandles = ComputeTools.ExtractHandles(devices, out int handleCount);
            var BuildOptions = options ?? "";
            var error = CL10.BuildProgram(
                program.Handle,
                handleCount,
                deviceHandles,
                options,
                notify,
                notifyDataPtr);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Creates a kernel for every <c>kernel</c> function in program.
        /// </summary>
        /// <returns> The collection of created kernels. </returns>
        /// <remarks> kernels are not created for any <c>kernel</c> functions in program that do not have the same function definition across all devices for which a program executable has been successfully built. </remarks>
        public ICollection<IComputeKernel> ProgramCreateAllKernels(ComputeProgram program)
        {
            var kernels = new Collection<IComputeKernel>();
            var error = CL10.CreateKernelsInProgram(
                program.Handle,
                0,
                null,
                out int kernelsCount);
            ComputeException.ThrowOnError(error);

            var kernelHandles = new CLKernelHandle[kernelsCount];
            error = CL10.CreateKernelsInProgram(
                program.Handle,
                kernelsCount,
                kernelHandles,
                out kernelsCount);
            ComputeException.ThrowOnError(error);

            for (int i = 0; i < kernelsCount; i++)
            {
                kernels.Add(new ComputeKernel(kernelHandles[i]));
            }
            return kernels;
        }

        /// <summary>
        /// Creates a kernel for a kernel function of a specified name.
        /// </summary>
        /// <returns> The created kernel. </returns>
        public IComputeKernel ProgramCreateKernel(IComputeProgram program, string functionName)
        {
            return new ComputeKernel(functionName, program);
        }

        /// <summary>
        /// Gets the build log of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The build log of the program for device. </returns>
        /*public string GetBuildLog(ComputeProgram program, IComputeDevice device)
        {
            return GetStringInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo>(program.Handle, device.Handle,
                ComputeProgramBuildInfo.BuildLog, CL10.GetProgramBuildInfo);
        }//*/

        /// <summary>
        /// Gets the <see cref="ComputeProgramBuildStatus"/> of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The <see cref="ComputeProgramBuildStatus"/> of the program for device. </returns>
        /*public ComputeProgramBuildStatus GetBuildStatus(ComputeProgram program, IComputeDevice device)
        {
            return (ComputeProgramBuildStatus)GetInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo, uint>(program.Handle,
                device.Handle, ComputeProgramBuildInfo.Status, CL10.GetProgramBuildInfo);
        }//*/

        /*public List<byte[]> GetBinaries(ComputeProgram program)
        {
            var binaryLengths = GetArrayInfo<CLProgramHandle, ComputeProgramInfo, IntPtr>(
                program.Handle,
                ComputeProgramInfo.BinarySizes,
                CL10.GetProgramInfo);

            var binariesGCHandles = new GCHandle[binaryLengths.Length];
            var binariesPtrs = new IntPtr[binaryLengths.Length];
            var binaries = new List<byte[]>();
            GCHandle binariesPtrsGCHandle = GCHandle.Alloc(binariesPtrs, GCHandleType.Pinned);

            try
            {
                for (int i = 0; i < binaryLengths.Length; i++)
                {
                    byte[] binary = new byte[binaryLengths[i].ToInt64()];
                    binariesGCHandles[i] = GCHandle.Alloc(binary, GCHandleType.Pinned);
                    binariesPtrs[i] = binariesGCHandles[i].AddrOfPinnedObject();
                    binaries.Add(binary);
                }
                ComputeErrorCode error = CL10.GetProgramInfo(
                    program.Handle,
                    ComputeProgramInfo.Binaries,
                    new IntPtr(binariesPtrs.Length * IntPtr.Size),
                    binariesPtrsGCHandle.AddrOfPinnedObject(),
                    out IntPtr sizeRet);
                ComputeException.ThrowOnError(error);
            }
            finally
            {
                for (int i = 0; i < binaryLengths.Length; i++)
                    binariesGCHandles[i].Free();
                binariesPtrsGCHandle.Free();
            }
            return binaries;
        }//*/
        #endregion


    }
}
