using Cloo;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Program;
using SilverHorn.Cloo.Sampler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverHorn.Cloo.Factories
{
    public interface IOpenCLFactory
    {
        #region Compute Program Constructors
        /// <summary>
        /// Creates a new program from a source code string.
        /// </summary>
        /// <param name="context"> A program. </param>
        /// <param name="source"> The source code for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        IComputeProgram BuildComputeProgram(IComputeContext context, string source);

        /// <summary>
        /// Creates a new program from an array of source code strings.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="source"> The source code lines for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        IComputeProgram BuildComputeProgram(IComputeContext context, string[] source);

        /// <summary>
        /// Creates a new program from a specified list of binaries.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="binaries"> A list of binaries, one for each item in <paramref name="devices"/>. </param>
        /// <param name="devices"> A subset of the context devices. If <paramref name="devices"/> is <c>null</c>, OpenCL will associate every binary from binaries with a corresponding device from devices. </param>
        IComputeProgram BuildComputeProgram(IComputeContext context, IList<byte[]> binaries, IList<IComputeDevice> devices);
        #endregion

        #region Kernel Constructors
        /// <summary>
        /// Creates a kernel for every <c>kernel</c> function in program.
        /// </summary>
        /// <returns> The collection of created kernels. </returns>
        /// <remarks> kernels are not created for any <c>kernel</c> functions in program that do not have the same function definition across all devices for which a program executable has been successfully built. </remarks>
        ICollection<IComputeKernel> CreateAllKernels(IComputeProgram program);

        /// <summary>
        /// Creates a kernel for a kernel function of a specified name.
        /// </summary>
        /// <returns> The created kernel. </returns>
        IComputeKernel CreateKernel(IComputeProgram program, string functionName);
        #endregion

        #region Sampler Constructors
        /// <summary>
        /// Creates a new sampler.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="normalizedCoords"> The usage state of normalized coordinates when accessing a image in a kernel. </param>
        /// <param name="addressing"> The <see cref="ComputeImageAddressing"/> mode of the sampler. Specifies how out-of-range image coordinates are handled while reading. </param>
        /// <param name="filtering"> The <see cref="ComputeImageFiltering"/> mode of the sampler. Specifies the type of filter that must be applied when reading data from an image. </param>
        IComputeSampler CreateSampler(IComputeContext context, bool normalizedCoords,
            ComputeImageAddressing addressing, ComputeImageFiltering filtering);
        #endregion




    }
}
