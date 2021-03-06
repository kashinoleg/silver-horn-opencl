﻿namespace Cloo
{
    /// <summary>
    /// The OpenCL error codes.
    /// </summary>
    public enum ComputeErrorCode : int
    {
        /// <summary> </summary>
        Success = 0,
        /// <summary> </summary>
        DeviceNotFound = -1,
        /// <summary> </summary>
        DeviceNotAvailable = -2,
        /// <summary> </summary>
        CompilerNotAvailable = -3,
        /// <summary> </summary>
        MemoryObjectAllocationFailure = -4,
        /// <summary> </summary>
        OutOfResources = -5,
        /// <summary> </summary>
        OutOfHostMemory = -6,
        /// <summary> </summary>
        ProfilingInfoNotAvailable = -7,
        /// <summary> </summary>
        MemoryCopyOverlap = -8,
        /// <summary> </summary>
        ImageFormatMismatch = -9,
        /// <summary> </summary>
        ImageFormatNotSupported = -10,
        /// <summary> </summary>
        BuildProgramFailure = -11,
        /// <summary> </summary>
        MapFailure = -12,
        /// <summary> </summary>
        MisalignedSubBufferOffset = -13,
        /// <summary> </summary>
        ExecutionStatusErrorForEventsInWaitList = -14,
        /// <summary> </summary>
        InvalidValue = -30,
        /// <summary> </summary>
        InvalidDeviceType = -31,
        /// <summary> </summary>
        InvalidPlatform = -32,
        /// <summary> </summary>
        InvalidDevice = -33,
        /// <summary> </summary>
        InvalidContext = -34,
        /// <summary> </summary>
        InvalidCommandQueueFlags = -35,
        /// <summary> </summary>
        InvalidCommandQueue = -36,
        /// <summary> </summary>
        InvalidHostPointer = -37,
        /// <summary> </summary>
        InvalidMemoryObject = -38,
        /// <summary> </summary>
        InvalidImageFormatDescriptor = -39,
        /// <summary> </summary>
        InvalidImageSize = -40,
        /// <summary> </summary>
        InvalidSampler = -41,
        /// <summary> </summary>
        InvalidBinary = -42,
        /// <summary> </summary>
        InvalidBuildOptions = -43,
        /// <summary> </summary>
        InvalidProgram = -44,
        /// <summary> </summary>
        InvalidProgramExecutable = -45,
        /// <summary> </summary>
        InvalidKernelName = -46,
        /// <summary> </summary>
        InvalidKernelDefinition = -47,
        /// <summary> </summary>
        InvalidKernel = -48,
        /// <summary> </summary>
        InvalidArgumentIndex = -49,
        /// <summary> </summary>
        InvalidArgumentValue = -50,
        /// <summary> </summary>
        InvalidArgumentSize = -51,
        /// <summary> </summary>
        InvalidKernelArguments = -52,
        /// <summary> </summary>
        InvalidWorkDimension = -53,
        /// <summary> </summary>
        InvalidWorkGroupSize = -54,
        /// <summary> </summary>
        InvalidWorkItemSize = -55,
        /// <summary> </summary>
        InvalidGlobalOffset = -56,
        /// <summary> </summary>
        InvalidEventWaitList = -57,
        /// <summary> </summary>
        InvalidEvent = -58,
        /// <summary> </summary>
        InvalidOperation = -59,
        /// <summary> </summary>
        InvalidGLObject = -60,
        /// <summary> </summary>
        InvalidBufferSize = -61,
        /// <summary> </summary>
        InvalidMipLevel = -62,
        /// <summary> </summary>
        InvalidGlobalWorkSize = -63,
        /// <summary> </summary>
        CL_INVALID_GL_SHAREGROUP_REFERENCE_KHR = -1000,
        /// <summary> </summary>
        CL_PLATFORM_NOT_FOUND_KHR = -1001,
        /// <summary> </summary>
        CL_DEVICE_PARTITION_FAILED_EXT = -1057,
        /// <summary> </summary>
        CL_INVALID_PARTITION_COUNT_EXT = -1058,
        /// <summary> </summary>
        CL_INVALID_PARTITION_NAME_EXT = -1059,
    }
}
