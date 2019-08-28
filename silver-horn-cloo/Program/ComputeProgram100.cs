using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Kernel;
using SilverHorn.Cloo.Program;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL program.
    /// </summary>
    /// <remarks> An OpenCL program consists of a set of kernels. Programs may also contain auxiliary functions called by the kernel functions and constant data. </remarks>
    public sealed class ComputeProgram100 : ComputeObject, IComputeProgram
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// The handle of the program.
        /// </summary>
        public CLProgramHandle Handle { get; internal set; }
        #endregion

        #region Constructors
        internal ComputeProgram100() { }
        #endregion

        #region Public methods
        /// <summary>
        /// Builds (compiles and links) a program executable from the program source or binary for all or some of the devices.
        /// </summary>
        /// <param name="devices"> A subset or all of devices. If devices is <c>null</c>, the executable is built for every item of devices for which a source or a binary has been loaded. </param>
        /// <param name="options"> A set of options for the OpenCL compiler. </param>
        /// <param name="notify"> A delegate instance that represents a reference to a notification routine. This routine is a callback function that an application can register and which will be called when the program executable has been built (successfully or unsuccessfully). If <paramref name="notify"/> is not <c>null</c>, build does not need to wait for the build to complete and can return immediately. If <paramref name="notify"/> is <c>null</c>, build does not return until the build has completed. The callback function may be called asynchronously by the OpenCL implementation. It is the application's responsibility to ensure that the callback function is thread-safe and that the delegate instance doesn't get collected by the Garbage Collector until the build operation triggers the callback. </param>
        /// <param name="notifyDataPtr"> Optional user data that will be passed to <paramref name="notify"/>. </param>
        public void Build(ICollection<IComputeDevice> devices, string options,
            ComputeProgramBuildNotifier notify, IntPtr notifyDataPtr)
        {
            var deviceHandles = ComputeTools.ExtractHandles(devices, out int handleCount);
            var BuildOptions = options ?? "";
            var error = OpenCL100.BuildProgram(
                Handle,
                handleCount,
                deviceHandles,
                options,
                notify,
                notifyDataPtr);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Gets the build log of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The build log of the program for device. </returns>
        public string GetBuildLog(IComputeDevice device)
        {
            return GetStringInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo>(Handle, device.Handle,
                ComputeProgramBuildInfo.BuildLog, OpenCL100.GetProgramBuildInfo);
        }

        /// <summary>
        /// Gets the <see cref="ComputeProgramBuildStatus"/> of the program for a specified device.
        /// </summary>
        /// <param name="device"> The device building the program. Must be one of devices. </param>
        /// <returns> The <see cref="ComputeProgramBuildStatus"/> of the program for device. </returns>
        public ComputeProgramBuildStatus GetBuildStatus(IComputeDevice device)
        {
            return (ComputeProgramBuildStatus)GetInfo<CLProgramHandle, CLDeviceHandle, ComputeProgramBuildInfo, uint>(Handle,
                device.Handle, ComputeProgramBuildInfo.Status, OpenCL100.GetProgramBuildInfo);
        }

        public List<byte[]> GetBinaries()
        {
            var binaryLengths = GetArrayInfo<CLProgramHandle, ComputeProgramInfo, IntPtr>(
                Handle,
                ComputeProgramInfo.BinarySizes,
                OpenCL100.GetProgramInfo);

            var binariesGCHandles = new GCHandle[binaryLengths.Length];
            var binariesPtrs = new IntPtr[binaryLengths.Length];
            var binaries = new List<byte[]>();
            GCHandle binariesPtrsGCHandle = GCHandle.Alloc(binariesPtrs, GCHandleType.Pinned);

            try
            {
                for (int i = 0; i < binaryLengths.Length; i++)
                {
                    byte[] binary = new byte[binaryLengths[i].ToInt64()];
                    binariesGCHandles[i] = GCHandle.Alloc(binary, GCHandleType.Pinned);
                    binariesPtrs[i] = binariesGCHandles[i].AddrOfPinnedObject();
                    binaries.Add(binary);
                }
                ComputeErrorCode error = OpenCL100.GetProgramInfo(
                    Handle,
                    ComputeProgramInfo.Binaries,
                    new IntPtr(binariesPtrs.Length * IntPtr.Size),
                    binariesPtrsGCHandle.AddrOfPinnedObject(),
                    out IntPtr sizeRet);
                ComputeException.ThrowOnError(error);
            }
            finally
            {
                for (int i = 0; i < binaryLengths.Length; i++)
                    binariesGCHandles[i].Free();
                binariesPtrsGCHandle.Free();
            }
            return binaries;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }
                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.
                if (Handle.IsValid)
                {
                    logger.Info("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
                    OpenCL100.ReleaseProgram(Handle);
                    Handle.Invalidate();
                }
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~ComputeProgram100()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
