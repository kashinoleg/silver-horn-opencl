using NLog;
using SilverHorn.Cloo.CL;
using SilverHorn.Cloo.Command;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Sampler;
using System;
using System.Runtime.InteropServices;

namespace Cloo.Bindings
{
    public sealed class OpenCL210 : OpenCLBase
    {
        #region The OpenCL Runtime - Command Queues
        public static CLCommandQueueHandle CreateCommandQueueWithPropertiesWrapper(CLContextHandle context, CLDeviceHandle device, ComputeCommandQueueFlags[] properties)
        {
            var handle = CreateCommandQueueWithProperties(context, device, properties, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static void SetDefaultDeviceCommandQueueWrapper(CLContextHandle context, CLDeviceHandle device, CLCommandQueueHandle command_queue)
        {
            ComputeException.ThrowOnError(SetDefaultDeviceCommandQueue(context, device, command_queue));
        }

        public static void RetainCommandQueueWrapper(CLCommandQueueHandle command_queue)
        {
            ComputeException.ThrowOnError(RetainCommandQueue(command_queue));
        }

        public static void ReleaseCommandQueueWrapper(CLCommandQueueHandle command_queue)
        {
            ComputeException.ThrowOnError(ReleaseCommandQueue(command_queue));
        }

        public static void GetCommandQueueInfoWrapper(CLCommandQueueHandle command_queue, ComputeCommandQueueInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetCommandQueueInfo(command_queue, param_name, param_value_size, param_value, out param_value_size_ret));
        }
        #endregion

        #region The OpenCL Platform Layer - Contexts
        public static CLContextHandle CreateContextWrapper(IntPtr[] properties, int num_devices, CLDeviceHandle[] devices, ComputeContextNotifier pfn_notify, IntPtr user_data)
        {
            var context = CreateContext(properties, num_devices, devices, pfn_notify, user_data, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return context;
        }

        public static CLContextHandle CreateContextFromTypeWrapper(IntPtr[] properties, ComputeDeviceTypes device_type, ComputeContextNotifier pfn_notify, IntPtr user_data)
        {
            var context = CreateContextFromType(properties, device_type, pfn_notify, user_data, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return context;
        }

        public static void RetainContextWrapper(CLContextHandle context)
        {
            ComputeException.ThrowOnError(RetainContext(context));
        }

        public static void ReleaseContextWrapper(CLContextHandle context)
        {
            ComputeException.ThrowOnError(ReleaseContext(context));
        }

        public static void GetContextInfoWrapper(CLContextHandle context, ComputeContextInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetContextInfo(context, param_name, param_value_size, param_value, out param_value_size_ret));
        }

        public static void TerminateContextKHRWrapper(CLContextHandle context)
        {
            ComputeException.ThrowOnError(TerminateContextKHR(context));
        }
        #endregion

        #region Program Objects - Create Program Objects
        public static CLProgramHandle CreateProgramWithSourceWrapper(CLContextHandle context, int count, string[] strings, IntPtr[] lengths)
        {
            var handle = CreateProgramWithSource(context, count, strings, lengths, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static CLProgramHandle CreateProgramWithILWrapper(CLContextHandle context, IntPtr il, IntPtr length)
        {
            var handle = CreateProgramWithIL(context, il, length, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static CLProgramHandle CreateProgramWithBinaryWrapper(CLContextHandle context,
            int num_devices, CLDeviceHandle[] device_list, IntPtr[] lengths, IntPtr[] binaries, int[] binary_status)
        {
            var handle = CreateProgramWithBinary(context, num_devices, device_list, lengths, binaries, binary_status, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static CLProgramHandle CreateProgramWithBinaryWrapper(CLContextHandle context, int num_devices, CLDeviceHandle[] device_list, string kernel_names, IntPtr[] lengths)
        {
            var handle = CreateProgramWithBuiltInKernels(context, num_devices, device_list, kernel_names, lengths, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static void RetainProgramWrapper(CLProgramHandle program)
        {
            ComputeException.ThrowOnError(RetainProgram(program));
        }

        public static void ReleaseProgramWrapper(CLProgramHandle program)
        {
            ComputeException.ThrowOnError(ReleaseProgram(program));
        }
        #endregion

        #region Program
        public static void BuildProgramWrapper(CLProgramHandle program, int num_devices, CLDeviceHandle[] device_list, string options, ComputeProgramBuildNotifier pfn_notify, IntPtr user_data)
        {
            ComputeException.ThrowOnError(BuildProgram(program, num_devices, device_list, options, pfn_notify, user_data));
        }

        public static void CreateKernelsInProgramWrapper(CLProgramHandle program, Int32 num_kernels, CLKernelHandle[] kernels, out Int32 num_kernels_ret)
        {
            ComputeException.ThrowOnError(CreateKernelsInProgram(program, num_kernels, kernels, out num_kernels_ret));
        }

        public static void GetProgramBuildInfoWrapper(CLProgramHandle program, CLDeviceHandle device, ComputeProgramBuildInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetProgramBuildInfo(program, device, param_name, param_value_size, param_value, out param_value_size_ret));
        }

        public static void GetProgramInfoWrapper(CLProgramHandle program, ComputeProgramInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetProgramInfo(program, param_name, param_value_size, param_value, out param_value_size_ret));
        }
        #endregion

        #region Kernel
        public static void GetKernelWorkGroupInfoWrapper(CLKernelHandle kernel, CLDeviceHandle device, ComputeKernelWorkGroupInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetKernelWorkGroupInfo(kernel, device, param_name, param_value_size, param_value, out param_value_size_ret));
        }

        public static void SetKernelArgWrapper(CLKernelHandle kernel, int arg_index, IntPtr arg_size, IntPtr arg_value)
        {
            ComputeException.ThrowOnError(SetKernelArg(kernel, arg_index, arg_size, arg_value));
        }

        public static void RetainKernelWrapper(CLKernelHandle kernel)
        {
            ComputeException.ThrowOnError(RetainKernel(kernel));
        }

        public static void ReleaseKernelWrapper(CLKernelHandle kernel)
        {
            ComputeException.ThrowOnError(ReleaseKernel(kernel));
        }

        public static void GetKernelInfoWrapper(CLKernelHandle kernel, ComputeKernelInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetKernelInfo(kernel, param_name, param_value_size, param_value, out param_value_size_ret));
        }

        public static CLKernelHandle CreateKernelWrapper(CLProgramHandle program, String kernel_nam)
        {
            var handle = CreateKernel(program, kernel_nam, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }
        #endregion

        #region Sampler
        public static CLSamplerHandle CreateSamplerWrapper(CLContextHandle context, bool normalized_coords, ComputeImageAddressing addressing_mode, ComputeImageFiltering filter_mode)
        {
            var handle = CreateSampler(context, normalized_coords, addressing_mode, filter_mode, out ComputeErrorCode errcode_ret);
            ComputeException.ThrowOnError(errcode_ret);
            return handle;
        }

        public static void RetainSamplerWrapper(CLSamplerHandle sample)
        {
            ComputeException.ThrowOnError(RetainSampler(sample));
        }

        public static void ReleaseSamplerWrapper(CLSamplerHandle sample)
        {
            ComputeException.ThrowOnError(ReleaseSampler(sample));
        }

        public static void GetSamplerInfoWrapper(CLSamplerHandle sample, ComputeSamplerInfo param_name, IntPtr param_value_size, IntPtr param_value, out IntPtr param_value_size_ret)
        {
            ComputeException.ThrowOnError(GetSamplerInfo(sample, param_name, param_value_size, param_value, out param_value_size_ret));
        }
        #endregion



    }
}
