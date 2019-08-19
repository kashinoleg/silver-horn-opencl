using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents the parent type to any Cloo buffer types.
    /// </summary>
    /// <typeparam name="T"> The type of the elements of the buffer. </typeparam>
    public abstract class ComputeBufferBase<T> : ComputeMemory where T : struct
    {
        #region Properties

        /// <summary>
        /// Gets the number of elements in the <see cref="ComputeBufferBase{T}"/>.
        /// </summary>
        /// <value> The number of elements in the <see cref="ComputeBufferBase{T}"/>. </value>
        public long Count { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeBufferBase(ComputeContext context, ComputeMemoryFlags flags)
            : base(context, flags)
        { }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected void Init()
        {
            SetID(Handle.Value);

            Size = (long)GetInfo<CLMemoryHandle, ComputeMemoryInfo, IntPtr>(Handle, ComputeMemoryInfo.Size, CL10.GetMemObjectInfo);
            Count = Size / Marshal.SizeOf(typeof(T));

            Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion
    }
}
