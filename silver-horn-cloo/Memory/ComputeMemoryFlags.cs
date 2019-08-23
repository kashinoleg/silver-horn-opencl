using System;

namespace Cloo
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ComputeMemoryFlags : long
    {
        /// <summary> Let the OpenCL choose the default flags. </summary>
        None = 0,
        /// <summary> The <see cref="ComputeMemory"/> will be accessible from the kernel for read and write operations. </summary>
        ReadWrite = 1 << 0,
        /// <summary> The <see cref="ComputeMemory"/> will be accessible from the kernel for write operations only. </summary>
        WriteOnly = 1 << 1,
        /// <summary> The <see cref="ComputeMemory"/> will be accessible from the kernel for read operations only. </summary>
        ReadOnly = 1 << 2,
        /// <summary> </summary>
        UseHostPointer = 1 << 3,
        /// <summary> </summary>
        AllocateHostPointer = 1 << 4,
        /// <summary> </summary>
        CopyHostPointer = 1 << 5
    }
}
