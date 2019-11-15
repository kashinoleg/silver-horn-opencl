using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Command;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Sampler;
using System;
using System.Runtime.InteropServices;

namespace SilverHorn.Cloo.CL
{
    public abstract class OpenCLBase
    {
        #region Properties
        /// <summary>
        /// The name of the library that contains the available OpenCL function points.
        /// </summary>
        protected const string libName = "OpenCL.dll";
        #endregion

        #region The OpenCL Runtime - Command Queues
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateCommandQueue")]
        extern internal static CLCommandQueueHandle CreateCommandQueue(
            CLContextHandle context,
            CLDeviceHandle device,
            ComputeCommandQueueFlags properties,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateCommandQueueWithPropeties")]
        extern internal static CLCommandQueueHandle CreateCommandQueueWithProperties(
            CLContextHandle context,
            CLDeviceHandle device,
            [MarshalAs(UnmanagedType.LPArray)] ComputeCommandQueueFlags[] properties,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clSetDefaultDeviceCommandQueue")]
        extern internal static ComputeErrorCode SetDefaultDeviceCommandQueue(
            CLContextHandle context,
            CLDeviceHandle device,
            CLCommandQueueHandle command_queue);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainCommandQueue")]
        extern internal static ComputeErrorCode RetainCommandQueue(CLCommandQueueHandle command_queue);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseCommandQueue")]
        extern internal static ComputeErrorCode ReleaseCommandQueue(CLCommandQueueHandle command_queue);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetCommandQueueInfo")]
        extern internal static ComputeErrorCode GetCommandQueueInfo(
            CLCommandQueueHandle command_queue,
            ComputeCommandQueueInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clSetCommandQueueProperty")]
        extern internal static ComputeErrorCode SetCommandQueueProperty(
            CLCommandQueueHandle command_queue,
            ComputeCommandQueueFlags properties,
            [MarshalAs(UnmanagedType.Bool)] bool enable,
            out ComputeCommandQueueFlags old_properties);
        #endregion

        #region The OpenCL Platform Layer - Contexts
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateContext")]
        extern internal static CLContextHandle CreateContext(
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            Int32 num_devices,
            [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] devices,
            ComputeContextNotifier pfn_notify,
            IntPtr user_data,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateContextFromType")]
        extern internal static CLContextHandle CreateContextFromType(
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            ComputeDeviceTypes device_type,
            ComputeContextNotifier pfn_notify,
            IntPtr user_data,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainContext")]
        extern internal static ComputeErrorCode RetainContext(CLContextHandle context);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseContext")]
        extern internal static ComputeErrorCode ReleaseContext(CLContextHandle context);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetContextInfo")]
        extern internal static ComputeErrorCode GetContextInfo(
            CLContextHandle context,
            ComputeContextInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clTerminateContextKHR")]
        extern internal static ComputeErrorCode TerminateContextKHR(CLContextHandle context);
        #endregion

        #region Program Objects - Create Program Objects
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateProgramWithSource")]
        extern internal static CLProgramHandle CreateProgramWithSource(
            CLContextHandle context,
            Int32 count,
            String[] strings,
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateProgramWithIL")]
        extern internal static CLProgramHandle CreateProgramWithIL(
            CLContextHandle context,
            IntPtr il,
            IntPtr length,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateProgramWithBinary")]
        extern internal static CLProgramHandle CreateProgramWithBinary(
            CLContextHandle context,
            Int32 num_devices,
            [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] device_list,
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths,
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] binaries,
            [MarshalAs(UnmanagedType.LPArray)] Int32[] binary_status,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateProgramWithBuiltInKernels")]
        extern internal static CLProgramHandle CreateProgramWithBuiltInKernels(
            CLContextHandle context,
            Int32 num_devices,
            [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] device_list,
            String kernel_names,
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] lengths,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainProgram")]
        extern internal static ComputeErrorCode RetainProgram(CLProgramHandle program);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseProgram")]
        extern internal static ComputeErrorCode ReleaseProgram(CLProgramHandle program);
        #endregion

        #region Program
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clBuildProgram")]
        extern internal static ComputeErrorCode BuildProgram(
            CLProgramHandle program,
            Int32 num_devices,
            [MarshalAs(UnmanagedType.LPArray)] CLDeviceHandle[] device_list,
            String options,
            ComputeProgramBuildNotifier pfn_notify,
            IntPtr user_data);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateKernelsInProgram")]
        extern internal static ComputeErrorCode CreateKernelsInProgram(
            CLProgramHandle program,
            Int32 num_kernels,
            [Out, MarshalAs(UnmanagedType.LPArray)] CLKernelHandle[] kernels,
            out Int32 num_kernels_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetProgramBuildInfo")]
        extern internal static ComputeErrorCode GetProgramBuildInfo(
            CLProgramHandle program,
            CLDeviceHandle device,
            ComputeProgramBuildInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetProgramInfo")]
        extern internal static ComputeErrorCode GetProgramInfo(
            CLProgramHandle program,
            ComputeProgramInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);
        #endregion

        #region Kernel
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetKernelWorkGroupInfo")]
        extern internal static ComputeErrorCode GetKernelWorkGroupInfo(
            CLKernelHandle kernel,
            CLDeviceHandle device,
            ComputeKernelWorkGroupInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clSetKernelArg")]
        extern internal static ComputeErrorCode SetKernelArg(
            CLKernelHandle kernel,
            Int32 arg_index,
            IntPtr arg_size,
            IntPtr arg_value);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainKernel")]
        extern internal static ComputeErrorCode RetainKernel(CLKernelHandle kernel);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseKernel")]
        extern internal static ComputeErrorCode ReleaseKernel(CLKernelHandle kernel);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetKernelInfo")]
        extern internal static ComputeErrorCode GetKernelInfo(
            CLKernelHandle kernel,
            ComputeKernelInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateKernel")]
        extern internal static CLKernelHandle CreateKernel(CLProgramHandle program, string kernel_name, out ComputeErrorCode errcode_ret);
        #endregion

        #region Sampler
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateSampler")]
        extern internal static CLSamplerHandle CreateSampler(
            CLContextHandle context,
            [MarshalAs(UnmanagedType.Bool)] bool normalized_coords,
            ComputeImageAddressing addressing_mode,
            ComputeImageFiltering filter_mode,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainSampler")]
        extern internal static ComputeErrorCode RetainSampler(CLSamplerHandle sample);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseSampler")]
        extern internal static ComputeErrorCode ReleaseSampler(CLSamplerHandle sample);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetSamplerInfo")]
        extern internal static ComputeErrorCode GetSamplerInfo(
            CLSamplerHandle sample,
            ComputeSamplerInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        #endregion


    }
}
