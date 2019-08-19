namespace Cloo
{
    /// <summary>
    /// The memory info query symbols.
    /// </summary>
    public enum ComputeMemoryInfo : int
    {
        /// <summary> </summary>
        Type = 0x1100,
        /// <summary> </summary>
        Flags = 0x1101,
        /// <summary> </summary>
        Size = 0x1102,
        /// <summary> </summary>
        HostPointer = 0x1103,
        /// <summary> </summary>
        MapppingCount = 0x1104,
        /// <summary> </summary>
        ReferenceCount = 0x1105,
        /// <summary> </summary>
        Context = 0x1106,
        /// <summary> </summary>
        AssociatedMemoryObject = 0x1107,
        /// <summary> </summary>
        Offset = 0x1108
    }
}
