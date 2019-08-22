using NLog;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Cloo.Bindings
{
    public class OpenCL210
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
        [DllImport(libName, EntryPoint = "clSetDefaultDeviceCommandQueue")]
        public extern static ComputeErrorCode SetDefaultDeviceCommandQueue(
            CLContextHandle context,
            CLDeviceHandle device,
            CLCommandQueueHandle command_queue);

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




    }
}
