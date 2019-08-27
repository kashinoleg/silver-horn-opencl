using System;
using System.Collections.ObjectModel;
using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Command;
using SilverHorn.Cloo.Platform;

namespace SilverHorn.Cloo.Device
{
    /// <summary>
    /// Represents an OpenCL device.
    /// </summary>
    /// <value> A device is a collection of compute units. A command queue is used to queue commands to a device. Examples of commands include executing kernels, or reading and writing memory objects. OpenCL devices typically correspond to a GPU, a multi-core CPU, and other processors such as DSPs and the Cell/B.E. processor. </value>
    public class ComputeDevice : ComputeObject, IComputeDevice
    {
        #region Properties
        /// <summary>
        /// The handle of the device.
        /// </summary>
        public CLDeviceHandle Handle { get; protected set; }

        /// <summary>
        /// Gets the default device address space size in bits.
        /// </summary>
        /// <value> Currently supported values are 32 or 64 bits. </value>
        public long AddressBits { get; private set; }

        /// <summary>
        /// Gets the availability state of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device is available and <c>false</c> otherwise. </value>
        public bool Available { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeCommandQueueFlags"/> supported by the device.
        /// </summary>
        /// <value> The <see cref="ComputeCommandQueueFlags"/> supported by the device. </value>
        public ComputeCommandQueueFlags CommandQueueFlags { get; private set; }

        /// <summary>
        /// Gets the availability state of the OpenCL compiler of the platform.
        /// </summary>
        /// <value> Is <c>true</c> if the implementation has a compiler available to compile the program source and <c>false</c> otherwise. This can be <c>false</c> for the embededed platform profile only. </value>
        public bool CompilerAvailable { get; private set; }

        /// <summary>
        /// Gets the OpenCL software driver version string of the device.
        /// </summary>
        /// <value> The version string in the form <c>major_number.minor_number</c>. </value>
        public string DriverVersion { get; private set; }

        /// <summary>
        /// Gets the endianness of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device is a little endian device and <c>false</c> otherwise. </value>
        public bool EndianLittle { get; private set; }

        /// <summary>
        /// Gets the error correction support state of the device.
        /// </summary>
        /// <value> Is <c>true</c> if the device implements error correction for the memories, caches, registers etc. Is <c>false</c> if the device does not implement error correction. This can be a requirement for certain clients of OpenCL. </value>
        public bool ErrorCorrectionSupport { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceExecutionCapabilities"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceExecutionCapabilities"/> of the device. </value>
        public ComputeDeviceExecutionCapabilities ExecutionCapabilities { get; private set; }

        /// <summary>
        /// Gets a read-only collection of names of extensions that the device supports.
        /// </summary>
        /// <value> A read-only collection of names of extensions that the device supports. </value>
        public ReadOnlyCollection<string> Extensions { get; private set; }

        /// <summary>
        /// Gets the size of the global device memory cache line in bytes.
        /// </summary>
        /// <value> The size of the global device memory cache line in bytes. </value>
        public long GlobalMemoryCacheLineSize { get; private set; }

        /// <summary>
        /// Gets the size of the global device memory cache in bytes.
        /// </summary>
        /// <value> The size of the global device memory cache in bytes. </value>
        public long GlobalMemoryCacheSize { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceMemoryCacheType"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceMemoryCacheType"/> of the device. </value>
        public ComputeDeviceMemoryCacheType GlobalMemoryCacheType { get; private set; }

        /// <summary>
        /// Gets the size of the global device memory in bytes.
        /// </summary>
        /// <value> The size of the global device memory in bytes. </value>
        public long GlobalMemorySize { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage2D.Height"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 8192 if device image support is <c>true</c>. </value>
        public long Image2DMaxHeight { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage2D.Width"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 8192 if device image support is <c>true</c>. </value>
        public long Image2DMaxWidth { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Depth"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        public long Image3DMaxDepth { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Height"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        public long Image3DMaxHeight { get; private set; }

        /// <summary>
        /// Gets the maximum <see cref="ComputeImage3D.Width"/> value that the device supports in pixels.
        /// </summary>
        /// <value> The minimum value is 2048 if device image support is <c>true</c>. </value>
        public long Image3DMaxWidth { get; private set; }

        /// <summary>
        /// Gets the state of image support of the device.
        /// </summary>
        /// <value> Is <c>true</c> if images are supported by the device and <c>false</c> otherwise. </value>
        public bool ImageSupport { get; private set; }

        /// <summary>
        /// Gets the size of local memory are of the device in bytes.
        /// </summary>
        /// <value> The minimum value is 16 KB (OpenCL 1.0) or 32 KB (OpenCL 1.1). </value>
        public long LocalMemorySize { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceLocalMemoryType"/> that is supported on the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceLocalMemoryType"/> that is supported on the device. </value>
        public ComputeDeviceLocalMemoryType LocalMemoryType { get; private set; }

        /// <summary>
        /// Gets the maximum configured clock frequency of the device in MHz.
        /// </summary>
        /// <value> The maximum configured clock frequency of the device in MHz. </value>
        public long MaxClockFrequency { get; private set; }

        /// <summary>
        /// Gets the number of parallel compute cores on the device.
        /// </summary>
        /// <value> The minimum value is 1. </value>
        public long MaxComputeUnits { get; private set; }

        /// <summary>
        /// Gets the maximum number of arguments declared with the <c>__constant</c> or <c>constant</c> qualifier in a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 8. </value>
        public long MaxConstantArguments { get; private set; }

        /// <summary>
        /// Gets the maximum size in bytes of a constant buffer allocation in the device memory.
        /// </summary>
        /// <value> The minimum value is 64 KB. </value>
        public long MaxConstantBufferSize { get; private set; }

        /// <summary>
        /// Gets the maximum size of memory object allocation in the device memory in bytes.
        /// </summary>
        /// <value> The minimum value is <c>max device global memory size /4, 128*1024*1024)</c>. </value>
        public long MaxMemoryAllocationSize { get; private set; }

        /// <summary>
        /// Gets the maximum size in bytes of the arguments that can be passed to a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 256 (OpenCL 1.0) or 1024 (OpenCL 1.1). </value>
        public long MaxParameterSize { get; private set; }

        /// <summary>
        /// Gets the maximum number of simultaneous images that can be read by a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 128 if device image support is <c>true</c>. </value>
        public long MaxReadImageArguments { get; private set; }

        /// <summary>
        /// Gets the maximum number of samplers that can be used in a kernel.
        /// </summary>
        /// <value> The minimum value is 16 if device image support is <c>true</c>. </value>
        public long MaxSamplers { get; private set; }

        /// <summary>
        /// Gets the maximum number of work-items in a work-group executing a kernel in a device using the data parallel execution model.
        /// </summary>
        /// <value> The minimum value is 1. </value>
        public long MaxWorkGroupSize { get; private set; }

        /// <summary>
        /// Gets the maximum number of dimensions that specify the global and local work-item IDs used by the data parallel execution model.
        /// </summary>
        /// <value> The minimum value is 3. </value>
        public long MaxWorkItemDimensions { get; private set; }

        /// <summary>
        /// Gets the maximum number of work-items that can be specified in each dimension of the <paramref name="globalWorkSize"/> argument of execute.
        /// </summary>
        /// <value> The maximum number of work-items that can be specified in each dimension of the <paramref name="globalWorkSize"/> argument of execute. </value>
        public ReadOnlyCollection<long> MaxWorkItemSizes { get; private set; }

        /// <summary>
        /// Gets the maximum number of simultaneous devices that can be written to by a kernel executing in the device.
        /// </summary>
        /// <value> The minimum value is 8 if device image support is <c>true</c>. </value>
        public long MaxWriteImageArguments { get; private set; }

        /// <summary>
        /// Gets the alignment in bits of the base address of any memory allocated in the device memory.
        /// </summary>
        /// <value> The alignment in bits of the base address of any memory allocated in the device memory. </value>
        public long MemoryBaseAddressAlignment { get; private set; }

        /// <summary>
        /// Gets the smallest alignment in bytes which can be used for any data type allocated in the device memory.
        /// </summary>
        /// <value> The smallest alignment in bytes which can be used for any data type allocated in the device memory. </value>
        public long MinDataTypeAlignmentSize { get; private set; }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        /// <value> The name of the device. </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the platform associated with the device.
        /// </summary>
        /// <value> The platform associated with the device. </value>
        public ComputePlatform Platform { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>char</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>char</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthChar { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthDouble { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>float</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>float</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthFloat { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthHalf { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>int</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>int</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthInt { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>long</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>long</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthLong { get; private set; }

        /// <summary>
        /// Gets the device's preferred native vector width size for vector of <c>short</c>s.
        /// </summary>
        /// <value> The device's preferred native vector width size for vector of <c>short</c>s. </value>
        /// <remarks> The vector width is defined as the number of scalar elements that can be stored in the vector. </remarks>
        public long PreferredVectorWidthShort { get; private set; }

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
        public string Profile { get; private set; }

        /// <summary>
        /// Gets the resolution of the device timer in nanoseconds.
        /// </summary>
        /// <value> The resolution of the device timer in nanoseconds. </value>
        public long ProfilingTimerResolution { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceSingleCapabilities"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceSingleCapabilities"/> of the device. </value>
        public ComputeDeviceSingleCapabilities SingleCapabilities { get; private set; }

        /// <summary>
        /// Gets the <see cref="ComputeDeviceTypes"/> of the device.
        /// </summary>
        /// <value> The <see cref="ComputeDeviceTypes"/> of the device. </value>
        public ComputeDeviceTypes Type { get; private set; }

        /// <summary>
        /// Gets the device vendor name string.
        /// </summary>
        /// <value> The device vendor name string. </value>
        public string Vendor { get; private set; }

        /// <summary>
        /// Gets a unique device vendor identifier.
        /// </summary>
        /// <value> A unique device vendor identifier. </value>
        /// <remarks> An example of a unique device identifier could be the PCIe ID. </remarks>
        public long VendorId { get; private set; }

        /// <summary>
        /// Gets the OpenCL version supported by the device.
        /// </summary>
        /// <value> The OpenCL version supported by the device. </value>
        public Version Version { get; private set; }

        /// <summary>
        /// Gets the OpenCL version string supported by the device.
        /// </summary>
        /// <value> The version string has the following format: <c>OpenCL[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
        public string VersionString { get; private set; }

        //////////////////////////////////
        // OpenCL 1.1 device properties //
        //////////////////////////////////

        /// <summary>
        /// Gets information about the presence of the unified memory subsystem.
        /// </summary>
        /// <value> Is <c>true</c> if the device and the host have a unified memory subsystem and <c>false</c> otherwise. </value>
        /// <remarks> Requires OpenCL 1.1 </remarks>
        public bool HostUnifiedMemory { get { return GetBoolInfo(ComputeDeviceInfo.HostUnifiedMemory); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>char</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>char</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthChar { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthChar); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>double</c>s or 0 if the <c>cl_khr_fp64</c> format is not supported. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthDouble { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthDouble); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>float</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>float</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthFloat { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthFloat); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>half</c>s or 0 if the <c>cl_khr_fp16</c> format is not supported. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthHalf { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthHalf); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>int</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>int</c>s. </value>
        /// <remarks>
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthInt { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthInt); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>long</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>long</c>s. </value>
        /// <remarks>
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthLong { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthLong); } }

        /// <summary>
        /// Gets the native ISA vector width size for vector of <c>short</c>s.
        /// </summary>
        /// <value> The native ISA vector width size for vector of <c>short</c>s. </value>
        /// <remarks> 
        ///     <para> The vector width is defined as the number of scalar elements that can be stored in the vector. </para>
        ///     <para> Requires OpenCL 1.1 </para>
        /// </remarks>
        public long NativeVectorWidthShort { get { return GetInfo<long>(ComputeDeviceInfo.NativeVectorWidthShort); } }

        /// <summary>
        /// Gets the OpenCL C version supported by the device.
        /// </summary>
        /// <value> Is <c>1.1</c> if device version is <c>1.1</c>. Is <c>1.0</c> or <c>1.1</c> if device version is <c>1.0</c>. </value>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public Version OpenCLCVersion { get { return ComputeTools.ParseVersionString(OpenCLCVersionString, 2); } }

        /// <summary>
        /// Gets the OpenCL C version string supported by the device.
        /// </summary>
        /// <value> The OpenCL C version string supported by the device. The version string has the following format: <c>OpenCL[space]C[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public string OpenCLCVersionString { get { return GetStringInfo(ComputeDeviceInfo.OpenCLCVersion); } }

        #endregion

        #region Constructors

        internal ComputeDevice(ComputePlatform platform, CLDeviceHandle handle)
        {
            Handle = handle;
            SetID(Handle.Value);

            AddressBits = GetInfo<uint>(ComputeDeviceInfo.AddressBits);
            Available = GetBoolInfo(ComputeDeviceInfo.Available);
            CompilerAvailable = GetBoolInfo(ComputeDeviceInfo.CompilerAvailable);
            DriverVersion = GetStringInfo(ComputeDeviceInfo.DriverVersion);
            EndianLittle = GetBoolInfo(ComputeDeviceInfo.EndianLittle);
            ErrorCorrectionSupport = GetBoolInfo(ComputeDeviceInfo.ErrorCorrectionSupport);
            ExecutionCapabilities = (ComputeDeviceExecutionCapabilities)GetInfo<long>(ComputeDeviceInfo.ExecutionCapabilities);

            string extensionString = GetStringInfo(ComputeDeviceInfo.Extensions);
            Extensions = new ReadOnlyCollection<string>(extensionString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            GlobalMemoryCacheLineSize = GetInfo<uint>(ComputeDeviceInfo.GlobalMemoryCachelineSize);
            GlobalMemoryCacheSize = (long)GetInfo<ulong>(ComputeDeviceInfo.GlobalMemoryCacheSize);
            GlobalMemoryCacheType = (ComputeDeviceMemoryCacheType)GetInfo<long>(ComputeDeviceInfo.GlobalMemoryCacheType);
            GlobalMemorySize = (long)GetInfo<ulong>(ComputeDeviceInfo.GlobalMemorySize);
            Image2DMaxHeight = (long)GetInfo<IntPtr>(ComputeDeviceInfo.Image2DMaxHeight);
            Image2DMaxWidth = (long)GetInfo<IntPtr>(ComputeDeviceInfo.Image2DMaxWidth);
            Image3DMaxDepth = (long)GetInfo<IntPtr>(ComputeDeviceInfo.Image3DMaxDepth);
            Image3DMaxHeight = (long)GetInfo<IntPtr>(ComputeDeviceInfo.Image3DMaxHeight);
            Image3DMaxWidth = (long)GetInfo<IntPtr>(ComputeDeviceInfo.Image3DMaxWidth);
            ImageSupport = GetBoolInfo(ComputeDeviceInfo.ImageSupport);
            LocalMemorySize = (long)GetInfo<ulong>(ComputeDeviceInfo.LocalMemorySize);
            LocalMemoryType = (ComputeDeviceLocalMemoryType)GetInfo<long>(ComputeDeviceInfo.LocalMemoryType);
            MaxClockFrequency = GetInfo<uint>(ComputeDeviceInfo.MaxClockFrequency);
            MaxComputeUnits = GetInfo<uint>(ComputeDeviceInfo.MaxComputeUnits);
            MaxConstantArguments = GetInfo<uint>(ComputeDeviceInfo.MaxConstantArguments);
            MaxConstantBufferSize = (long)GetInfo<ulong>(ComputeDeviceInfo.MaxConstantBufferSize);
            MaxMemoryAllocationSize = (long)GetInfo<ulong>(ComputeDeviceInfo.MaxMemoryAllocationSize);
            MaxParameterSize = (long)GetInfo<IntPtr>(ComputeDeviceInfo.MaxParameterSize);
            MaxReadImageArguments = GetInfo<uint>(ComputeDeviceInfo.MaxReadImageArguments);
            MaxSamplers = GetInfo<uint>(ComputeDeviceInfo.MaxSamplers);
            MaxWorkGroupSize = (long)GetInfo<IntPtr>(ComputeDeviceInfo.MaxWorkGroupSize);
            MaxWorkItemDimensions = GetInfo<uint>(ComputeDeviceInfo.MaxWorkItemDimensions);
            MaxWorkItemSizes = new ReadOnlyCollection<long>(ComputeTools.ConvertArray(GetArrayInfo<CLDeviceHandle, ComputeDeviceInfo, IntPtr>(Handle, ComputeDeviceInfo.MaxWorkItemSizes, CL10.GetDeviceInfo)));
            MaxWriteImageArguments = GetInfo<uint>(ComputeDeviceInfo.MaxWriteImageArguments);
            MemoryBaseAddressAlignment = GetInfo<uint>(ComputeDeviceInfo.MemoryBaseAddressAlignment);
            MinDataTypeAlignmentSize = GetInfo<uint>(ComputeDeviceInfo.MinDataTypeAlignmentSize);
            Name = GetStringInfo(ComputeDeviceInfo.Name);
            Platform = platform;
            PreferredVectorWidthChar = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthChar);
            PreferredVectorWidthDouble = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthDouble);
            PreferredVectorWidthFloat = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthFloat);
            PreferredVectorWidthHalf = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthHalf);
            PreferredVectorWidthInt = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthInt);
            PreferredVectorWidthLong = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthLong);
            PreferredVectorWidthShort = GetInfo<uint>(ComputeDeviceInfo.PreferredVectorWidthShort);
            Profile = GetStringInfo(ComputeDeviceInfo.Profile);
            ProfilingTimerResolution = (long)GetInfo<IntPtr>(ComputeDeviceInfo.ProfilingTimerResolution);
            CommandQueueFlags = (ComputeCommandQueueFlags)GetInfo<long>(ComputeDeviceInfo.CommandQueueProperties);
            SingleCapabilities = (ComputeDeviceSingleCapabilities)GetInfo<long>(ComputeDeviceInfo.SingleFPConfig);
            Type = (ComputeDeviceTypes)GetInfo<long>(ComputeDeviceInfo.Type);
            Vendor = GetStringInfo(ComputeDeviceInfo.Vendor);
            VendorId = GetInfo<uint>(ComputeDeviceInfo.VendorId);
            VersionString = GetStringInfo(ComputeDeviceInfo.Version);
            Version = ComputeTools.ParseVersionString(VersionString, 1);
        }

        #endregion

        #region Private methods

        private bool GetBoolInfo(ComputeDeviceInfo paramName)
        {
            return GetBoolInfo<CLDeviceHandle, ComputeDeviceInfo>(Handle, paramName, CL10.GetDeviceInfo);
        }

        private NativeType GetInfo<NativeType>(ComputeDeviceInfo paramName) where NativeType : struct
        {
            return GetInfo<CLDeviceHandle, ComputeDeviceInfo, NativeType>(Handle, paramName, CL10.GetDeviceInfo);
        }

        private string GetStringInfo(ComputeDeviceInfo paramName)
        {
            return GetStringInfo<CLDeviceHandle, ComputeDeviceInfo>(Handle, paramName, CL10.GetDeviceInfo);
        }
        #endregion
    }
}
