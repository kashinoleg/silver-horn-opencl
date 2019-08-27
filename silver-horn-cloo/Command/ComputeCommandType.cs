namespace SilverHorn.Cloo.Command
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComputeCommandType : int
    {
        /// <summary> </summary>
        NDRangeKernel = 0x11F0,
        /// <summary> </summary>
        Task = 0x11F1,
        /// <summary> </summary>
        NativeKernel = 0x11F2,
        /// <summary> </summary>
        ReadBuffer = 0x11F3,
        /// <summary> </summary>
        WriteBuffer = 0x11F4,
        /// <summary> </summary>
        CopyBuffer = 0x11F5,
        /// <summary> </summary>
        ReadImage = 0x11F6,
        /// <summary> </summary>
        WriteImage = 0x11F7,
        /// <summary> </summary>
        CopyImage = 0x11F8,
        /// <summary> </summary>
        CopyImageToBuffer = 0x11F9,
        /// <summary> </summary>
        CopyBufferToImage = 0x11FA,
        /// <summary> </summary>
        MapBuffer = 0x11FB,
        /// <summary> </summary>
        MapImage = 0x11FC,
        /// <summary> </summary>
        UnmapMemory = 0x11FD,
        /// <summary> </summary>
        Marker = 0x11FE,
        /// <summary> </summary>
        AcquireGLObjects = 0x11FF,
        /// <summary> </summary>
        ReleaseGLObjects = 0x1200,
        /// <summary> </summary>
        ReadBufferRectangle = 0x1201,
        /// <summary> </summary>
        WriteBufferRectangle = 0x1202,
        /// <summary> </summary>
        CopyBufferRectangle = 0x1203,
        /// <summary> </summary>
        User = 0x1204,
        /// <summary> </summary>
        CL_COMMAND_MIGRATE_MEM_OBJECT_EXT = 0x4040
    }
}
