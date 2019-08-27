using System;

namespace SilverHorn.Cloo.Device
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ComputeDeviceSingleCapabilities : long
    {
        /// <summary> </summary>
        Denorm = 1 << 0,
        /// <summary> </summary>
        InfNan = 1 << 1,
        /// <summary> </summary>
        RoundToNearest = 1 << 2,
        /// <summary> </summary>
        RoundToZero = 1 << 3,
        /// <summary> </summary>
        RoundToInf = 1 << 4,
        /// <summary> </summary>
        Fma = 1 << 5,
        /// <summary> </summary>
        SoftFloat = 1 << 6
    }
}
