namespace Cloo
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComputeImageChannelType : int
    {
        /// <summary> </summary>
        SNormInt8 = 0x10D0,
        /// <summary> </summary>
        SNormInt16 = 0x10D1,
        /// <summary> </summary>
        UNormInt8 = 0x10D2,
        /// <summary> </summary>
        UNormInt16 = 0x10D3,
        /// <summary> </summary>
        UNormShort565 = 0x10D4,
        /// <summary> </summary>
        UNormShort555 = 0x10D5,
        /// <summary> </summary>
        UNormInt101010 = 0x10D6,
        /// <summary> </summary>
        SignedInt8 = 0x10D7,
        /// <summary> </summary>
        SignedInt16 = 0x10D8,
        /// <summary> </summary>
        SignedInt32 = 0x10D9,
        /// <summary> </summary>
        UnsignedInt8 = 0x10DA,
        /// <summary> </summary>
        UnsignedInt16 = 0x10DB,
        /// <summary> </summary>
        UnsignedInt32 = 0x10DC,
        /// <summary> </summary>
        HalfFloat = 0x10DD,
        /// <summary> </summary>
        Float = 0x10DE,
    }
}
