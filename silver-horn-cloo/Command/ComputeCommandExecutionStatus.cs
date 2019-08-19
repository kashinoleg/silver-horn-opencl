namespace Cloo
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComputeCommandExecutionStatus : int
    {
        /// <summary> </summary>
        Complete = 0x0,
        /// <summary> </summary>
        Running = 0x1,
        /// <summary> </summary>
        Submitted = 0x2,
        /// <summary> </summary>
        Queued = 0x3
    }
}
