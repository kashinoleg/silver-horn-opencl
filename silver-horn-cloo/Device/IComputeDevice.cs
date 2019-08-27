using Cloo;
using SilverHorn.Cloo.Platform;
using System;
using System.Collections.ObjectModel;

namespace SilverHorn.Cloo.Device
{
    public interface IComputeDevice
    {
        #region Properties
        /// <summary>
        /// The handle of the device.
        /// </summary>
        CLDeviceHandle Handle { get; }

        /// <summary>
        /// Gets the default device address space size in bits.
        /// </summary>
        /// <value> Currently supported values are 32 or 64 bits. </value>
        long AddressBits { get; }

        /// <summary>
        /// Gets the availability state of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device is available and <c>false</c> otherwise. </value>
        bool Available { get; }

        /// <summary>
        /// Gets the <see cref="ComputeCommandQueueFlags"/> supported by the device.
        /// </summary>
        /// <value> The <see cref="ComputeCommandQueueFlags"/> supported by the device. </value>
        ComputeCommandQueueFlags CommandQueueFlags { get; }

        /// <summary>
        /// Gets the availability state of the OpenCL compiler of the platform.
        /// </summary>
        /// <value> Is <c>true</c> if the implementation has a compiler available to compile the program source and <c>false</c> otherwise. This can be <c>false</c> for the embededed platform profile only. </value>
        bool CompilerAvailable { get; }

        /// <summary>
        /// Gets the OpenCL software driver version string of the device.
        /// </summary>
        /// <value> The version string in the form <c>major_number.minor_number</c>. </value>
        string DriverVersion { get; }

        /// <summary>
        /// Gets the endianness of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device is a little endian device and <c>false</c> otherwise. </value>
        bool EndianLittle { get; }

        /// <summary>
        /// Gets the error correction support state of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device implements error correction for the memories, caches, registers etc. Is <c>false</c> if the device does not implement error correction. This can be a requirement for certain clients of OpenCL. </value>
        bool ErrorCorrectionSupport { get; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceExecutionCapabilities"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceExecutionCapabilities"/> of the device. </value>
        ComputeDeviceExecutionCapabilities ExecutionCapabilities { get; }

        /// <summary>
        /// Gets a read-only collection of names of extensions that the device supports.
        /// </summary>
        /// <value> A read-only collection of names of extensions that the device supports. </value>
        ReadOnlyCollection<string> Extensions { get; }

        /// <summary>
        /// Gets the size of the global device memory cache line in bytes.
        /// </summary>
        /// <value> The size of the global device memory cache line in bytes. </value>
        long GlobalMemoryCacheLineSize { get; }

        /// <summary>
        /// Gets the size of the global device memory cache in bytes.
        /// </summary>
        /// <value> The size of the global device memory cache in bytes. </value>
        long GlobalMemoryCacheSize { get; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceMemoryCacheType"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceMemoryCacheType"/> of the device. </value>
        ComputeDeviceMemoryCacheType GlobalMemoryCacheType { get; }

        /// <summary>
        /// Gets the size of the global device memory in bytes.
        /// </summary>
        /// <value> The size of the global device memory in bytes. </value>
        long GlobalMemorySize { get; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage2D.Height"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 8192 if device image support is <c>true</c>. </value>
        long Image2DMaxHeight { get; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage2D.Width"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 8192 if device image support is <c>true</c>. </value>
        long Image2DMaxWidth { get; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Depth"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        long Image3DMaxDepth { get; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Height"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        long Image3DMaxHeight { get; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Width"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        long Image3DMaxWidth { get; }

        /// <summary>
        /// Gets the state of image support of the device.
        /// </summary>
        /// <value> Is <c>true</c> if <see cref="ComputeImage"/>s are supported by the device and <c>false</c> otherwise. </value>
        bool ImageSupport { get; }

        /// <summary>
        /// Gets the size of local memory are of the device in bytes.
        /// </summary>
        /// <value> The minimum value is 16 KB (OpenCL 1.0) or 32 KB (OpenCL 1.1). </value>
        long LocalMemorySize { get; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceLocalMemoryType"/> that is supported on the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceLocalMemoryType"/> that is supported on the device. </value>
        ComputeDeviceLocalMemoryType LocalMemoryType { get; }

        /// <summary>
        /// Gets the maximum configured clock frequency of the device in MHz.
        /// </summary>
        /// <value> The maximum configured clock frequency of the device in MHz. </value>
        long MaxClockFrequency { get; }

        /// <summary>
        /// Gets the number of parallel compute cores on the device.
        /// </summary>
        /// <value> The minimum value is 1. </value>
        long MaxComputeUnits { get; }

        /// <summary>
        /// Gets the maximum number of arguments declared with the <c>__constant</c> or <c>constant</c> qualifier in a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 8. </value>
        long MaxConstantArguments { get; }

        /// <summary>
        /// Gets the maximum size in bytes of a constant buffer allocation in the device memory.
        /// </summary>
        /// <value> The minimum value is 64 KB. </value>
        long MaxConstantBufferSize { get; }

        /// <summary>
        /// Gets the maximum size of memory object allocation in the device memory in bytes.
        /// </summary>
        /// <value> The minimum value is <c>max device global memory size /4, 128*1024*1024)</c>. </value>
        long MaxMemoryAllocationSize { get; }

        /// <summary>
        /// Gets the maximum size in bytes of the arguments that can be passed to a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 256 (OpenCL 1.0) or 1024 (OpenCL 1.1). </value>
        long MaxParameterSize { get; }

        /// <summary>
        /// Gets the maximum number of simultaneous <see cref="ComputeImage"/>s that can be read by a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 128 if device image support is <c>true</c>. </value>
        long MaxReadImageArguments { get; }

        /// <summary>
        /// Gets the maximum number of samplers that can be used in a kernel.
        /// </summary>
        /// <value> The minimum value is 16 if device image support is <c>true</c>. </value>
        long MaxSamplers { get; }

        /// <summary>
        /// Gets the maximum number of work-items in a work-group executing a kernel in a device using the data parallel execution model.
        /// </summary>
        /// <value> The minimum value is 1. </value>
        long MaxWorkGroupSize { get; }

        /// <summary>
        /// Gets the maximum number of dimensions that specify the global and local work-item IDs used by the data parallel execution model.
        /// </summary>
        /// <value> The minimum value is 3. </value>
        long MaxWorkItemDimensions { get; }

        /// <summary>
        /// Gets the maximum number of work-items that can be specified in each dimension of the <paramref name="globalWorkSize"/> argument of execute.
        /// </summary>
        /// <value> The maximum number of work-items that can be specified in each dimension of the <paramref name="globalWorkSize"/> argument of execute. </value>
        ReadOnlyCollection<long> MaxWorkItemSizes { get; }

        /// <summary>
        /// Gets the maximum number of simultaneous devices that can be written to by a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 8 if device image support is <c>true</c>. </value>
        long MaxWriteImageArguments { get; }

        /// <summary>
        /// Gets the alignment in bits of the base address of any <see cref="ComputeMemory"/> allocated in the device memory.
        /// </summary>
        /// <value> The alignment in bits of the base address of any <see cref="ComputeMemory"/> allocated in the device memory. </value>
        long MemoryBaseAddressAlignment { get; }

        /// <summary>
        /// Gets the smallest alignment in bytes which can be used for any data type allocated in the device memory.
        /// </summary>
        /// <value> The smallest alignment in bytes which can be used for any data type allocated in the device memory. </value>
        long MinDataTypeAlignmentSize { get; }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        /// <value> The name of the device. </value>
        string Name { get; }

        /// <summary>
        /// Gets the platform associated with the device.
        /// </summary>
        /// <value> The platform associated with the device. </value>
        ComputePlatform Platform { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>char</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>char</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthChar { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthDouble { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>float</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>float</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthFloat { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthHalf { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>int</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>int</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthInt { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>long</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>long</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthLong { get; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>short</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>short</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        long PreferredVectorWidthShort { get; }

        /// <summary>
        /// Gets the OpenCL profile name supported by the device.
        /// </summary>
        /// <value> 
        /// The profile name returned can be one of the following strings:
        /// <list type="bullets">
        /// <item>
        ///     <term> FULL_PROFILE </term>
        ///     <description> The device supports the OpenCL specification (functionality defined as part of the core specification and does not require any extensions to be supported). </description>
        /// </item>
        /// <item>
        ///     <term> EMBEDDED_PROFILE </term>
        ///     <description> The device supports the OpenCL embedded profile. </description>
        /// </item>
        /// </list>
        /// </value>
        string Profile { get; }

        /// <summary>
        /// Gets the resolution of the device timer in nanoseconds.
        /// </summary>
        /// <value> The resolution of the device timer in nanoseconds. </value>
        long ProfilingTimerResolution { get; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceSingleCapabilities"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceSingleCapabilities"/> of the device. </value>
        ComputeDeviceSingleCapabilities SingleCapabilities { get; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceTypes"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceTypes"/> of the device. </value>
        ComputeDeviceTypes Type { get; }

        /// <summary>
        /// Gets the device vendor name string.
        /// </summary>
        /// <value> The device vendor name string. </value>
        string Vendor { get; }

        /// <summary>
        /// Gets a unique device vendor identifier.
        /// </summary>
        /// <value> A unique device vendor identifier. </value>
        /// <remarks> An example of a unique device identifier could be the PCIe ID. </remarks>
        long VendorId { get; }

        /// <summary>
        /// Gets the OpenCL version supported by the device.
        /// </summary>
        /// <value> The OpenCL version supported by the device. </value>
        Version Version { get; }

        /// <summary>
        /// Gets the OpenCL version string supported by the device.
        /// </summary>
        /// <value> The version string has the following format: <c>OpenCL[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
        string VersionString { get; }

        //////////////////////////////////
        // OpenCL 1.1 device properties //
        //////////////////////////////////

        /// <summary>
        /// Gets information about the presence of the unified memory subsystem.
        /// </summary>
        /// <value> Is <c>true</c> if the device and the host have a unified memory subsystem and <c>false</c> otherwise. </value>
        /// <remarks> Requires OpenCL 1.1 </remarks>
        bool HostUnifiedMemory { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>char</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>char</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthChar { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthDouble { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>float</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>float</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthFloat { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthHalf { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>int</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>int</c>s. </value>
        /// <remarks>
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthInt { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>long</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>long</c>s. </value>
        /// <remarks>
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthLong { get; }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>short</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>short</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        long NativeVectorWidthShort { get; }

        /// <summary>
        /// Gets the OpenCL C version supported by the device.
        /// </summary>
        /// <value> Is <c>1.1</c> if device version is <c>1.1</c>. Is <c>1.0</c> or <c>1.1</c> if device version is <c>1.0</c>. </value>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        Version OpenCLCVersion { get; }

        /// <summary>
        /// Gets the OpenCL C version string supported by the device.
        /// </summary>
        /// <value> The OpenCL C version string supported by the device. The version string has the following format: <c>OpenCL[space]C[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        string OpenCLCVersionString { get; }

        #endregion
    }
}
