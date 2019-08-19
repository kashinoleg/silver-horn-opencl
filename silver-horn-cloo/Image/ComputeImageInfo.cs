namespace Cloo
{
    /// <summary>
    /// The image info query symbols.
    /// </summary>
    public enum ComputeImageInfo : int
    {
        /// <summary> </summary>
        Format = 0x1110,
        /// <summary> </summary>
        ElementSize = 0x1111,
        /// <summary> </summary>
        RowPitch = 0x1112,
        /// <summary> </summary>
        SlicePitch = 0x1113,
        /// <summary> </summary>
        Width = 0x1114,
        /// <summary> </summary>
        Height = 0x1115,
        /// <summary> </summary>
        Depth = 0x1116
    }
}
