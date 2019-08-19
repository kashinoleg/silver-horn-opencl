using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL image.
    /// </summary>
    /// <remarks> A memory object that stores a two- or three- dimensional structured array. Image data can only be accessed with read and write functions. The read functions use a sampler. </remarks>
    /// <seealso cref="ComputeMemory"/>
    /// <seealso cref="ComputeSampler"/>
    public abstract class ComputeImage : ComputeMemory
    {
        #region Properties

        /// <summary>
        /// Gets or sets (protected) the depth in pixels of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The depth in pixels of the <see cref="ComputeImage"/>. </value>
        public int Depth { get; protected set; }

        /// <summary>
        /// Gets or sets (protected) the size of the elements (pixels) of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The size of the elements (pixels) of the <see cref="ComputeImage"/>. </value>
        public int ElementSize { get; protected set; }

        /// <summary>
        /// Gets or sets (protected) the height in pixels of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The height in pixels of the <see cref="ComputeImage"/>. </value>
        public int Height { get; protected set; }

        /// <summary>
        /// Gets or sets (protected) the size in bytes of a row of elements of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The size in bytes of a row of elements of the <see cref="ComputeImage"/>. </value>
        public long RowPitch { get; protected set; }

        /// <summary>
        /// Gets or sets (protected) the size in bytes of a 2D slice of a <see cref="ComputeImage3D"/>.
        /// </summary>
        /// <value> The size in bytes of a 2D slice of a <see cref="ComputeImage3D"/>. For a <see cref="ComputeImage2D"/> this value is 0. </value>
        public long SlicePitch { get; protected set; }

        /// <summary>
        /// Gets or sets (protected) the width in pixels of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The width in pixels of the <see cref="ComputeImage"/>. </value>
        public int Width { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        protected ComputeImage(ComputeContext context, ComputeMemoryFlags flags)
            : base(context, flags)
        { }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flags"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected static ICollection<ComputeImageFormat> GetSupportedFormats(ComputeContext context, ComputeMemoryFlags flags, ComputeMemoryType type)
        {
            int formatCountRet = 0;
            ComputeErrorCode error = CL10.GetSupportedImageFormats(context.Handle, flags, type, 0, null, out formatCountRet);
            ComputeException.ThrowOnError(error);

            ComputeImageFormat[] formats = new ComputeImageFormat[formatCountRet];
            error = CL10.GetSupportedImageFormats(context.Handle, flags, type, formatCountRet, formats, out formatCountRet);
            ComputeException.ThrowOnError(error);

            return new Collection<ComputeImageFormat>(formats);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Init()
        {
            SetID(Handle.Value);

            Depth = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.Depth, CL10.GetImageInfo);
            ElementSize = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.ElementSize, CL10.GetImageInfo);
            Height = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.Height, CL10.GetImageInfo);
            RowPitch = (long)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.RowPitch, CL10.GetImageInfo);
            Size = (long)GetInfo<CLMemoryHandle, ComputeMemoryInfo, IntPtr>(Handle, ComputeMemoryInfo.Size, CL10.GetMemObjectInfo);
            SlicePitch = (long)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.SlicePitch, CL10.GetImageInfo);
            Width = (int)GetInfo<CLMemoryHandle, ComputeImageInfo, IntPtr>(Handle, ComputeImageInfo.Width, CL10.GetImageInfo);

            Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion
    }
}
