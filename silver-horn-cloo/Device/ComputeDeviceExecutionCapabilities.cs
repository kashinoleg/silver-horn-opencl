namespace Cloo
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComputeDeviceExecutionCapabilities : int
    {
        /// <summary> </summary>
        OpenCLKernel = 1 << 0,
        /// <summary> </summary>
        NativeKernel = 1 << 1
    }
}
