using System;
using System.Runtime.InteropServices;
using Cloo.Bindings;
using SilverHorn.Cloo.Context;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL buffer.
    /// </summary>
    /// <typeparam name="T"> The type of the elements of the buffer. T is restricted to value types and <c>struct</c>s containing such types. </typeparam>
    /// <remarks> A memory object that stores a linear collection of bytes. Buffer objects are accessible using a pointer in a kernel executing on a device. </remarks>
    public class ComputeBuffer<T> : ComputeBufferBase<T> where T : struct
    {
        #region Constructors

        /// <summary>
        /// Creates a new buffer.
        /// </summary>
        /// <param name="context"> A context used to create the buffer. </param>
        /// <param name="flags"> A bit-field that is used to specify allocation and usage information about the buffer. </param>
        /// <param name="count"> The number of elements of the buffer. </param>
        public ComputeBuffer(IComputeContext context, ComputeMemoryFlags flags, long count)
            : this(context, flags, count, IntPtr.Zero)
        { }

		/// <summary>
		/// Creates a new buffer from the given pointer.
		/// </summary>
		/// <param name="context"> A context used to create the buffer. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the buffer. </param>
		/// <param name="count"> The number of elements of the buffer. </param>
		/// <param name="dataPtr"> A pointer to the data for the buffer. </param>
		/// <remarks> Note, that if <paramref name="dataPtr"/> does not persist for the life of this buffer, <c>ComputeMemoryFlags.CopyHostPointer</c> should be set in flags to ensure that the underlying buffer remains available. </remarks>
		public ComputeBuffer(IComputeContext context, ComputeMemoryFlags flags, long count, IntPtr dataPtr)
            : base(context, flags)
        {
			InternalCreateBuffer(context, flags, count, dataPtr);
        }

        /// <summary>
        /// Creates a new buffer from the given array.
        /// </summary>
        /// <param name="context"> A context used to create the buffer. </param>
        /// <param name="flags"> A bit-field that is used to specify allocation and usage information about the buffer.  Any xxxHostPointer flags are ignored. </param>
        /// <param name="data"> The data for the buffer. </param>
        public ComputeBuffer(IComputeContext context, ComputeMemoryFlags flags, T[] data)
            : base(context, flags)
        {
            GCHandle dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
				InternalCreateBuffer(context, (flags | ComputeMemoryFlags.CopyHostPointer) & ~(ComputeMemoryFlags.UseHostPointer | ComputeMemoryFlags.AllocateHostPointer), data.Length, dataPtr.AddrOfPinnedObject());
			}
			finally
            {
                dataPtr.Free();
            }
        }

		/// <summary>
		/// Creates a new buffer from the given 2-dimensional array.
		/// </summary>
		/// <param name="context"> A context used to create the buffer. </param>
		/// <param name="flags"> A bit-field that is used to specify allocation and usage information about the buffer.  Any xxxHostPointer flags are ignored. </param>
		/// <param name="data"> The 2-dimensional data for the buffer. </param>
		public ComputeBuffer(IComputeContext context, ComputeMemoryFlags flags, T[,] data)
			: base(context, flags)
		{
			GCHandle dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				InternalCreateBuffer(context, (flags | ComputeMemoryFlags.CopyHostPointer) & ~(ComputeMemoryFlags.UseHostPointer | ComputeMemoryFlags.AllocateHostPointer), data.Length, dataPtr.AddrOfPinnedObject());
			}
			finally
			{
				dataPtr.Free();
			}
		}

		private void InternalCreateBuffer(IComputeContext context, ComputeMemoryFlags flags, long count, IntPtr dataPtr)
		{
            ComputeErrorCode error = ComputeErrorCode.Success;
            Handle = CL10.CreateBuffer(context.Handle, flags, new IntPtr(Marshal.SizeOf(typeof(T)) * count), dataPtr, out error);
            ComputeException.ThrowOnError(error);
            Init();
		}

		private ComputeBuffer(CLMemoryHandle handle, IComputeContext context, ComputeMemoryFlags flags)
            : base(context, flags)
        {
            Handle = handle;
            Init();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a new buffer from an existing OpenGL buffer object.
        /// </summary>
        /// <typeparam name="DataType"> The type of the elements of the buffer. <typeparamref name="T"/> should match the type of the elements in the OpenGL buffer. </typeparam>
        /// <param name="context"> A context with enabled CL/GL sharing. </param>
        /// <param name="flags"> A bit-field that is used to specify usage information about the buffer. Only <see cref="ComputeMemoryFlags.ReadOnly"/>, <see cref="ComputeMemoryFlags.WriteOnly"/> and <see cref="ComputeMemoryFlags.ReadWrite"/> are allowed. </param>
        /// <param name="bufferId"> The OpenGL buffer object id to use for the creation of the buffer. </param>
        /// <returns> The created buffer. </returns>
        public static ComputeBuffer<DataType> CreateFromGLBuffer<DataType>(IComputeContext context, ComputeMemoryFlags flags, int bufferId) where DataType : struct
        {
            ComputeErrorCode error = ComputeErrorCode.Success;
            CLMemoryHandle handle = CL10.CreateFromGLBuffer(context.Handle, flags, bufferId, out error);
            ComputeException.ThrowOnError(error);
            return new ComputeBuffer<DataType>(handle, context, flags);
        }

        #endregion
    }
}
