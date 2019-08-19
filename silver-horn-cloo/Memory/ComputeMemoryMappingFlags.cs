using System;

namespace Cloo
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ComputeMemoryMappingFlags : long
    {
        /// <summary> </summary>
        Read = 1 << 0,
        /// <summary> </summary>
        Write = 1 << 1
    }
}
