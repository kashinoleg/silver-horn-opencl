namespace SilverHorn.Cloo.Command
{
    /// <summary>
    /// The command profiling info query symbols.
    /// </summary>
    public enum ComputeCommandProfilingInfo : int
    {
        /// <summary> </summary>
        Queued = 0x1280,
        /// <summary> </summary>
        Submitted = 0x1281,
        /// <summary> </summary>
        Started = 0x1282,
        /// <summary> </summary>
        Ended = 0x1283
    }
}
