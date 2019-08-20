using System;
using System.Diagnostics;
using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL sampler.
    /// </summary>
    /// <remarks> An object that describes how to sample an image when the image is read in the kernel. The image read functions take a sampler as an argument. The sampler specifies the image addressing-mode i.e. how out-of-range image coordinates are handled, the filtering mode, and whether the input image coordinate is a normalized or unnormalized value. </remarks>
    /// <seealso cref="ComputeImage"/>
    public class ComputeSampler : ComputeResource
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeContext context;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeImageAddressing addressing;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeImageFiltering filtering;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool normalizedCoords;

        #endregion

        #region Properties

        /// <summary>
        /// The handle of the <see cref="ComputeSampler"/>.
        /// </summary>
        public CLSamplerHandle Handle
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <see cref="ComputeContext"/> of the <see cref="ComputeSampler"/>.
        /// </summary>
        /// <value> The <see cref="ComputeContext"/> of the <see cref="ComputeSampler"/>. </value>
        public ComputeContext Context { get { return context; } }

        /// <summary>
        /// Gets the <see cref="ComputeImageAddressing"/> mode of the <see cref="ComputeSampler"/>.
        /// </summary>
        /// <value> The <see cref="ComputeImageAddressing"/> mode of the <see cref="ComputeSampler"/>. </value>
        public ComputeImageAddressing Addressing { get { return addressing; } }

        /// <summary>
        /// Gets the <see cref="ComputeImageFiltering"/> mode of the <see cref="ComputeSampler"/>.
        /// </summary>
        /// <value> The <see cref="ComputeImageFiltering"/> mode of the <see cref="ComputeSampler"/>. </value>
        public ComputeImageFiltering Filtering { get { return filtering; } }

        /// <summary>
        /// Gets the state of usage of normalized x, y and z coordinates when accessing a <see cref="ComputeImage"/> in a <see cref="ComputeKernel"/> through the <see cref="ComputeSampler"/>.
        /// </summary>
        /// <value> The state of usage of normalized x, y and z coordinates when accessing a <see cref="ComputeImage"/> in a <see cref="ComputeKernel"/> through the <see cref="ComputeSampler"/>. </value>
        public bool NormalizedCoords { get { return normalizedCoords; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ComputeSampler"/>.
        /// </summary>
        /// <param name="context"> A <see cref="ComputeContext"/>. </param>
        /// <param name="normalizedCoords"> The usage state of normalized coordinates when accessing a <see cref="ComputeImage"/> in a <see cref="ComputeKernel"/>. </param>
        /// <param name="addressing"> The <see cref="ComputeImageAddressing"/> mode of the <see cref="ComputeSampler"/>. Specifies how out-of-range image coordinates are handled while reading. </param>
        /// <param name="filtering"> The <see cref="ComputeImageFiltering"/> mode of the <see cref="ComputeSampler"/>. Specifies the type of filter that must be applied when reading data from an image. </param>
        public ComputeSampler(ComputeContext context, bool normalizedCoords, ComputeImageAddressing addressing, ComputeImageFiltering filtering)
        {
            ComputeErrorCode error = ComputeErrorCode.Success;
            Handle = CL10.CreateSampler(context.Handle, normalizedCoords, addressing, filtering, out error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            this.addressing = addressing;
            this.context = context;
            this.filtering = filtering;
            this.normalizedCoords = normalizedCoords;

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
