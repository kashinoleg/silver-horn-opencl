using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo.Bindings;
using SilverHorn.Cloo.Kernel;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL program.
    /// </summary>
    /// <remarks> An OpenCL program consists of a set of kernels. Programs may also contain auxiliary functions called by the kernel functions and constant data. </remarks>
    public class ComputeProgram : ComputeResource
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeContext context;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyCollection<ComputeDevice> devices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyCollection<string> source;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ReadOnlyCollection<byte[]> binaries;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string buildOptions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ComputeProgramBuildNotifier buildNotify;

        #endregion

        #region Properties

        /// <summary>
        /// The handle of the program.
        /// </summary>
        public CLProgramHandle Handle { get; protected set; }

        /// <summary>
        /// Gets a read-only collection of program binaries associated with the devices.
        /// </summary>
        /// <value> A read-only collection of program binaries associated with the devices. </value>
        /// <remarks> The bits returned can be an implementation-specific intermediate representation (a.k.a. IR) or device specific executable bits or both. The decision on which information is returned in the binary is up to the OpenCL implementation. </remarks>
        public ReadOnlyCollection<byte[]> Binaries
        {
            get
            {
                if (binaries == null)
                    binaries = GetBinaries();
                return binaries;
            }
        }

        /// <summary>
        /// Gets the program build options as specified in options argument of build.
        /// </summary>
        /// <value> The program build options as specified in options argument of build. </value>
        public string BuildOptions => buildOptions;

        /// <summary>
        /// Gets the <see cref="ComputeContext"/> of the program.
        /// </summary>
        /// <value> The <see cref="ComputeContext"/> of the program. </value>
        public ComputeContext Context => context;

        /// <summary>
        /// Gets a read-only collection of <see cref="ComputeDevice"/>s associated with the program.
        /// </summary>
        /// <value> A read-only collection of <see cref="ComputeDevice"/>s associated with the program. </value>
        /// <remarks> This collection is a subset of devices. </remarks>
        public ReadOnlyCollection<ComputeDevice> Devices => devices;

        /// <summary>
        /// Gets a read-only collection of program source code strings specified when creating the program or <c>null</c> if program was created using program binaries.
        /// </summary>
        /// <value> A read-only collection of program source code strings specified when creating the program or <c>null</c> if program was created using program binaries. </value>
        public ReadOnlyCollection<string> Source => source;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new program from a source code string.
        /// </summary>
        /// <param name="context"> A program. </param>
        /// <param name="source"> The source code for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public ComputeProgram(ComputeContext context, string source)
        {
            Handle = CL10.CreateProgramWithSource(
                context.Handle,
                1,
                new string[] { source },
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            this.context = context;
            this.devices = context.Devices;
            this.source = new ReadOnlyCollection<string>(new string[] { source });

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        /// <summary>
        /// Creates a new program from an array of source code strings.
        /// </summary>
        /// <param name="context"> A <see cref="ComputeContext"/>. </param>
        /// <param name="source"> The source code lines for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public ComputeProgram(ComputeContext context, string[] source)
        {
            Handle = CL10.CreateProgramWithSource(
                context.Handle,
                source.Length,
                source,
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            this.context = context;
            this.devices = context.Devices;
            this.source = new ReadOnlyCollection<string>(source);

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        /// <summary>
        /// Creates a new program from a specified list of binaries.
        /// </summary>
        /// <param name="context"> A <see cref="ComputeContext"/>. </param>
        /// <param name="binaries"> A list of binaries, one for each item in <paramref name="devices"/>. </param>
        /// <param name="devices"> A subset of the <see cref="ComputeContext.Devices"/>. If <paramref name="devices"/> is <c>null</c>, OpenCL will associate every binary from binaries with a corresponding <see cref="ComputeDevice"/> from <see cref="ComputeContext.Devices"/>. </param>
        public ComputeProgram(ComputeContext context, IList<byte[]> binaries, IList<ComputeDevice> devices)
        {
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

                Handle = CL10.CreateProgramWithBinary(
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


            this.binaries = new ReadOnlyCollection<byte[]>(binaries);
            this.context = context;
            if (devices != null)
            {
                this.devices = new ReadOnlyCollection<ComputeDevice>(devices);
            }
            else
            {
                this.devices = new ReadOnlyCollection<ComputeDevice>(context.Devices);
            }
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
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
        public void Build(ICollection<ComputeDevice> devices, string options,
            ComputeProgramBuildNotifier notify, IntPtr notifyDataPtr)
        {
            var deviceHandles = ComputeTools.ExtractHandles(devices, out int handleCount);
            buildOptions = options ?? "";
            buildNotify = notify;
            var error = CL10.BuildProgram(
                Handle,
                handleCount,
                deviceHandles,
                options,
                buildNotify,
                notifyDataPtr);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Creates a kernel for every <c>kernel</c> function in program.
        /// </summary>
        /// <returns> The collection of created kernels. </returns>
        /// <remarks> kernels are not created for any <c>kernel</c> functions in program that do not have the same function definition across all devices for which a program executable has been successfully built. </remarks>
        public ICollection<IComputeKernel> CreateAllKernels()
        {
            var kernels = new Collection<IComputeKernel>();
            var error = CL10.CreateKernelsInProgram(
                Handle,
                0,
                null,
                out int kernelsCount);
            ComputeException.ThrowOnError(error);

            var kernelHandles = new CLKernelHandle[kernelsCount];
            error = CL10.CreateKernelsInProgram(
                Handle,
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
        public IComputeKernel CreateKernel(string functionName)
        {
            return new ComputeKernel(functionName, this);
        }

        /// <summary>
        /// Gets the build log of the program for a specified <see cref="ComputeDevice"/>.
        /// </summary>
        /// <param name="device"> The <see cref="ComputeDevice"/> building the program. Must be one of devices. </param>
        /// <returns> The build log of the program for device. </returns>
        public string GetBuildLog(ComputeDevice device)
        {
            return GetStringInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo>(Handle, device.Handle,
                ComputeProgramBuildInfo.BuildLog, CL10.GetProgramBuildInfo);
        }

        /// <summary>
        /// Gets the <see cref="ComputeProgramBuildStatus"/> of the program for a specified <see cref="ComputeDevice"/>.
        /// </summary>
        /// <param name="device"> The <see cref="ComputeDevice"/> building the program. Must be one of devices. </param>
        /// <returns> The <see cref="ComputeProgramBuildStatus"/> of the program for device. </returns>
        public ComputeProgramBuildStatus GetBuildStatus(ComputeDevice device)
        {
            return (ComputeProgramBuildStatus)GetInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo, uint>(Handle,
                device.Handle, ComputeProgramBuildInfo.Status, CL10.GetProgramBuildInfo);
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
                CL10.ReleaseProgram(Handle);
                Handle.Invalidate();
            }
        }

        #endregion

        #region Private methods

        private ReadOnlyCollection<byte[]> GetBinaries()
        {
            IntPtr[] binaryLengths = GetArrayInfo<CLProgramHandle, ComputeProgramInfo, IntPtr>(Handle, ComputeProgramInfo.BinarySizes, CL10.GetProgramInfo);

            GCHandle[] binariesGCHandles = new GCHandle[binaryLengths.Length];
            IntPtr[] binariesPtrs = new IntPtr[binaryLengths.Length];
            IList<byte[]> binaries = new List<byte[]>();
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
                    Handle,
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

            return new ReadOnlyCollection<byte[]>(binaries);
        }

        #endregion
    }
}
