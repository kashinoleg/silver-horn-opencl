using System;
using System.Diagnostics;
using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL memory object.
    /// </summary>
    /// <remarks> A memory object is a handle to a region of global memory. </remarks>
    /// <seealso cref="ComputeBuffer{T}"/>
    /// <seealso cref="ComputeImage"/>
    public abstract class ComputeMemory : ComputeResource
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeContext context;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeMemoryFlags flags;

        #endregion

        #region Properties

        /// <summary>
        /// The handle of the <see cref="ComputeMemory"/>.
        /// </summary>
        public CLMemoryHandle Handle
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeContext"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeContext Context { get { return context; } }

        /// <summary>
        /// Gets the <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>.
        /// </summary>
        /// <value> The <see cref="ComputeMemoryFlags"/> of the <see cref="ComputeMemory"/>. </value>
        public ComputeMemoryFlags Flags { get { return flags; } }

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
            this.context = context;
            this.flags = flags;
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
