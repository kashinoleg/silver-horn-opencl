using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL memory object.
    /// </summary>
    /// <remarks> A memory object is a handle to a region of global memory. </remarks>
    /// <seealso cref="ComputeImage"/>
    public abstract class ComputeMemory : ComputeResource
    {
        #region Properties
        /// <summary>
        /// The handle of the <see cref="ComputeMemory"/>.
        /// </summary>
        public CLMemoryHandle Handle { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeContext Context { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeMemoryFlags Flags { get; private set; }

        /// <summary>
        /// Gets or sets (protected) the size in bytes of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The size in bytes of the <see cref="ComputeMemory"/>. </value>
        public long Size { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeMemory(ComputeContext context, ComputeMemoryFlags flags)
        {
            Context = context;
            Flags = flags;
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
                CL10.ReleaseMemObject(Handle);
                Handle.Invalidate();
            }
        }
        #endregion
    }
}
