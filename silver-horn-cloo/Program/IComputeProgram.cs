using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using System;
using System.Collections.Generic;

namespace SilverHorn.Cloo.Program
{
    public interface IComputeProgram : IDisposable
    {
        #region Properties
        /// <summary>
        /// The handle of the program.
        /// </summary>
        CLProgramHandle Handle { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Builds (compiles and links) a program executable from the program source or binary for all or some of the devices.
        /// </summary>
        /// <param name="devices"> A subset or all of devices. If devices is <c>null</c>, the executable is built for every item of devices for which a source or a binary has been loaded. </param>
        /// <param name="options"> A set of options for the OpenCL compiler. </param>
        /// <param name="notify"> A delegate instance that represents a reference to a notification routine. This routine is a callback function that an application can register and which will be called when the program executable has been built (successfully or unsuccessfully). If <paramref name="notify"/> is not <c>null</c>, build does not need to wait for the build to complete and can return immediately. If <paramref name="notify"/> is <c>null</c>, build does not return until the build has completed. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until the build operation triggers the callback. </param>
        /// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify"/>. </param>
        void Build(ICollection<IComputeDevice> devices, string options,
            ComputeProgramBuildNotifier notify, IntPtr notifyDataPtr);

        /// <summary>
        /// Creates a kernel for every <c>kernel</c> function in program.
        /// </summary>
        /// <returns> The collection of created kernels. </returns>
        /// <remarks> kernels are not created for any <c>kernel</c> functions in program that do not have the same function definition across all devices for which a program executable has been successfully built. </remarks>
        ICollection<IComputeKernel> CreateAllKernels();

        /// <summary>
        /// Creates a kernel for a kernel function of a specified name.
        /// </summary>
        /// <returns> The created kernel. </returns>
        IComputeKernel CreateKernel(string functionName);

        /// <summary>
        /// Gets the build log of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The build log of the program for device. </returns>
        string GetBuildLog(IComputeDevice device);

        /// <summary>
        /// Gets the <see cref="ComputeProgramBuildStatus"/> of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The <see cref="ComputeProgramBuildStatus"/> of the program for device. </returns>
        ComputeProgramBuildStatus GetBuildStatus(IComputeDevice device);

        List<byte[]> GetBinaries();
        #endregion
    }
}
