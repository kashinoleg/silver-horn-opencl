using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL image format.
    /// </summary>
    /// <remarks> This structure defines the type, count and size of the image channels. </remarks>
    /// <seealso cref="ComputeImage"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct ComputeImageFormat
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeImageChannelOrder channelOrder;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeImageChannelType channelType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ComputeImageChannelOrder"/> of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The <see cref="ComputeImageChannelOrder"/> of the <see cref="ComputeImage"/>. </value>
        public ComputeImageChannelOrder ChannelOrder { get { return channelOrder; } }

        /// <summary>
        /// Gets the <see cref="ComputeImageChannelType"/> of the <see cref="ComputeImage"/>.
        /// </summary>
        /// <value> The <see cref="ComputeImageChannelType"/> of the <see cref="ComputeImage"/>. </value>
        public ComputeImageChannelType ChannelType { get { return channelType; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ComputeImageFormat"/>.
        /// </summary>
        /// <param name="channelOrder"> The number of channels and the channel layout i.e. the memory layout in which channels are stored in the <see cref="ComputeImage"/>. </param>
        /// <param name="channelType"> The type of the channel data. The number of bits per element determined by the <paramref name="channelType"/> and <paramref name="channelOrder"/> must be a power of two. </param>
        public ComputeImageFormat(ComputeImageChannelOrder channelOrder, ComputeImageChannelType channelType)
        {
            this.channelOrder = channelOrder;
            this.channelType = channelType;
        }

        #endregion
    }
}
