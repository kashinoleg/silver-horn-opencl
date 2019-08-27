using System.Threading;
using Cloo.Bindings;
using SilverHorn.Cloo.Context;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL memory object.
    /// </summary>
    /// <remarks> A memory object is a handle to a region of global memory. </remarks>
    public abstract class ComputeMemory : ComputeResource
    {
        #region Properties
        /// <summary>
        /// The handle of the memory.
        /// </summary>
        public CLMemoryHandle Handle { get; protected set; }

        /// <summary>
        /// Gets the context of the memory.
        /// </summary>
        /// <value> The context of the memory. </value>
        public IComputeContext Context { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeMemoryFlags"/> of the memory.
        /// </summary>
        /// <value> The <see cref="ComputeMemoryFlags"/> of the memory. </value>
        public ComputeMemoryFlags Flags { get; private set; }

        /// <summary>
        /// Gets or sets (protected) the size in bytes of the memory.
        /// </summary>
        /// <value> The size in bytes of the memory. </value>
        public long Size { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeMemory(IComputeContext context, ComputeMemoryFlags flags)
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
