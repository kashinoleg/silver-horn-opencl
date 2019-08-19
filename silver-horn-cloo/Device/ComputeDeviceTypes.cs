using System;

namespace Cloo
{
    /// <summary>
    /// The types of devices.
    /// </summary>
    [Flags]
    public enum ComputeDeviceTypes : long
    {
        /// <summary> </summary>
        Default = 1 << 0,
        /// <summary> </summary>
        Cpu = 1 << 1,
        /// <summary> </summary>
        Gpu = 1 << 2,
        /// <summary> </summary>
        Accelerator = 1 << 3,
        /// <summary> </summary>
        All = 0xFFFFFFFF
    }
}
