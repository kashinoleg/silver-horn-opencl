using System.Threading;
using Cloo;
using Cloo.Bindings;

namespace SilverHorn.Cloo.Sampler
{
    /// <summary>
    /// Represents an OpenCL sampler.
    /// </summary>
    /// <remarks> An object that describes how to sample an image when the image is read in the kernel. The image read functions take a sampler as an argument. The sampler specifies the image addressing-mode i.e. how out-of-range image coordinates are handled, the filtering mode, and whether the input image coordinate is a normalized or unnormalized value. </remarks>
    /// <seealso cref="ComputeImage"/>
    public sealed class ComputeSampler : ComputeResource
    {
        #region Properties
        /// <summary>
        /// The handle of the sampler.
        /// </summary>
        public CLSamplerHandle Handle { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeImageAddressing"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageAddressing"/> mode of the sampler. </value>
        public ComputeImageAddressing Addressing { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeImageFiltering"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageFiltering"/> mode of the sampler. </value>
        public ComputeImageFiltering Filtering { get; private set; }

        /// <summary>
        /// Gets the state of usage of normalized x, y and z coordinates when accessing a <see cref="ComputeImage"/> in a kernel through the sampler.
        /// </summary>
        /// <value> The state of usage of normalized x, y and z coordinates when accessing a <see cref="ComputeImage"/> in a kernel through the sampler. </value>
        public bool NormalizedCoords { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new sampler.
        /// </summary>
        /// <param name="context"> A <see cref="ComputeContext"/>. </param>
        /// <param name="normalizedCoords"> The usage state of normalized coordinates when accessing a <see cref="ComputeImage"/> in a kernel. </param>
        /// <param name="addressing"> The <see cref="ComputeImageAddressing"/> mode of the sampler. Specifies how out-of-range image coordinates are handled while reading. </param>
        /// <param name="filtering"> The <see cref="ComputeImageFiltering"/> mode of the sampler. Specifies the type of filter that must be applied when reading data from an image. </param>
        public ComputeSampler(ComputeContext context, bool normalizedCoords, ComputeImageAddressing addressing, ComputeImageFiltering filtering)
        {
            Handle = CL10.CreateSampler(context.Handle, normalizedCoords, addressing, filtering, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            Addressing = addressing;
            Filtering = filtering;
            NormalizedCoords = normalizedCoords;

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
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
                CL10.ReleaseSampler(Handle);
                Handle.Invalidate();
            }
        }
        #endregion
    }
}
