using NLog;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Cloo.Bindings
{
    public class OpenCL200
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

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
        [DllImport(libName, EntryPoint = "clCreateCommandQueueWithPropeties")]
        public extern static CLCommandQueueHandle CreateCommandQueueWithProperties(
            CLContextHandle context,
            CLDeviceHandle device,
            [MarshalAs(UnmanagedType.LPArray)] ComputeCommandQueueFlags[] properties,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainCommandQueue")]
        public extern static ComputeErrorCode RetainCommandQueue(
            CLCommandQueueHandle command_queue);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseCommandQueue")]
        public extern static ComputeErrorCode
        ReleaseCommandQueue(
            CLCommandQueueHandle command_queue);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetCommandQueueInfo")]
        public extern static ComputeErrorCode GetCommandQueueInfo(
            CLCommandQueueHandle command_queue,
            ComputeCommandQueueInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);
        #endregion

        #region The OpenCL Platform Layer - Contexts
        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clCreateContext")]
        public extern static CLContextHandle CreateContext(
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
        public extern static CLContextHandle CreateContextFromType(
            [MarshalAs(UnmanagedType.LPArray)] IntPtr[] properties,
            ComputeDeviceTypes device_type,
            ComputeContextNotifier pfn_notify,
            IntPtr user_data,
            out ComputeErrorCode errcode_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clRetainContext")]
        public extern static ComputeErrorCode RetainContext(
            CLContextHandle context);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clReleaseContext")]
        public extern static ComputeErrorCode ReleaseContext(
            CLContextHandle context);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clGetContextInfo")]
        public extern static ComputeErrorCode GetContextInfo(
            CLContextHandle context,
            ComputeContextInfo param_name,
            IntPtr param_value_size,
            IntPtr param_value,
            out IntPtr param_value_size_ret);

        /// <summary>
        /// See the OpenCL specification.
        /// </summary>
        [DllImport(libName, EntryPoint = "clTerminateContextKHR")]
        public extern static ComputeErrorCode TerminateContextKHR(
            CLContextHandle context);
        #endregion



    }
}
