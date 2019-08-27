namespace SilverHorn.Cloo.Kernel
{
    /// <summary>
    /// The kernel work-group info query symbols.
    /// </summary>
    public enum ComputeKernelWorkGroupInfo : int
    {
        /// <summary> </summary>
        WorkGroupSize = 0x11B0,
        /// <summary> </summary>
        CompileWorkGroupSize = 0x11B1,
        /// <summary> </summary>
        LocalMemorySize = 0x11B2,
        /// <summary> </summary>
        PreferredWorkGroupSizeMultiple = 0x11B3,
        /// <summary> </summary>
        PrivateMemorySize = 0x11B4
    }
}
