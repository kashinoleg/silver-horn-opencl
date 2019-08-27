using Cloo;
using System;

namespace SilverHorn.Cloo.Sampler
{
    public interface IComputeSampler : IDisposable
    {
        #region Properties
        /// <summary>
        /// The handle of the sampler.
        /// </summary>
        CLSamplerHandle Handle { get; }

        /// <summary>
        /// Gets the <see cref="ComputeImageAddressing"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageAddressing"/> mode of the sampler. </value>
        ComputeImageAddressing Addressing { get; }

        /// <summary>
        /// Gets the <see cref="ComputeImageFiltering"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageFiltering"/> mode of the sampler. </value>
        ComputeImageFiltering Filtering { get; }

        /// <summary>
        /// Gets the state of usage of normalized x, y and z coordinates when accessing a image in a kernel through the sampler.
        /// </summary>
        /// <value> The state of usage of normalized x, y and z coordinates when accessing a image in a kernel through the sampler. </value>
        bool NormalizedCoords { get; }
        #endregion
    }
}
