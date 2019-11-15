using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Device;

namespace SilverHorn.Cloo.Platform
{
    /// <summary>
    /// Represents an OpenCL platform.
    /// </summary>
    /// <remarks> The host plus a collection of devices managed by the OpenCL framework that allow an application to share resources and execute kernels on devices in the platform. </remarks>
    /// <seealso cref="ComputeResource"/>
    public sealed class ComputePlatform : ComputeObject, IComputePlatform
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// The handle of the platform.
        /// </summary>
        public CLPlatformHandle Handle { get; private set; }

        /// <summary>
        /// Gets a read-only collection of devices available on the platform.
        /// </summary>
        /// <value> A read-only collection of devices available on the platform. </value>
        public ReadOnlyCollection<IComputeDevice> Devices { get; private set; }

        /// <summary>
        /// Gets a read-only collection of extension names supported by the platform.
        /// </summary>
        /// <value> A read-only collection of extension names supported by the platform. </value>
        public ReadOnlyCollection<string> Extensions { get; private set; }

        /// <summary>
        /// Gets the platform name.
        /// </summary>
        /// <value> The platform name. </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a read-only collection of available platforms.
        /// </summary>
        /// <value> A read-only collection of available platforms. </value>
        /// <remarks> The collection will contain no items, if no OpenCL platforms are found on the system. </remarks>
        public static ReadOnlyCollection<ComputePlatform> Platforms { get; private set; }

        /// <summary>
        /// Gets the name of the profile supported by the platform.
        /// </summary>
        /// <value> The name of the profile supported by the platform. </value>
        public string Profile { get; private set; }

        /// <summary>
        /// Gets the platform vendor.
        /// </summary>
        /// <value> The platform vendor. </value>
        public string Vendor { get; private set; }

        /// <summary>
        /// Gets the OpenCL version string supported by the platform.
        /// </summary>
        /// <value> The OpenCL version string supported by the platform. It has the following format: <c>OpenCL[space][major_version].[minor_version][space][vendor-specific information]</c>. </value>
        public string Version { get; private set; }

        private static readonly object lockObj = new object();
        #endregion

        #region Constructors
        static ComputePlatform()
        {
            if (Platforms != null)
            {
                return;
            }
            lock (lockObj)
            {
                try
                {
                    CL10.GetPlatformIDsWrapper(0, null, out int handlesLength);
                    CLPlatformHandle[] handles = new CLPlatformHandle[handlesLength];
                    CL10.GetPlatformIDsWrapper(handlesLength, handles, out handlesLength);
                    
                    var platformList = new List<ComputePlatform>(handlesLength);
                    foreach (CLPlatformHandle handle in handles)
                    {
                        platformList.Add(new ComputePlatform(handle));
                    }
                    Platforms = platformList.AsReadOnly();
                }
                catch (DllNotFoundException)
                {
                    Platforms = new List<ComputePlatform>().AsReadOnly();
                }
            }
        }

        private ComputePlatform(CLPlatformHandle handle)
        {
            Handle = handle;
            SetID(Handle.Value);

            string extensionString = GetStringInfo(Handle, ComputePlatformInfo.Extensions, CL10.GetPlatformInfoWrapper);
            Extensions = new ReadOnlyCollection<string>(extensionString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            Name = GetStringInfo(Handle, ComputePlatformInfo.Name, CL10.GetPlatformInfoWrapper);
            Profile = GetStringInfo(Handle, ComputePlatformInfo.Profile, CL10.GetPlatformInfoWrapper);
            Vendor = GetStringInfo(Handle, ComputePlatformInfo.Vendor, CL10.GetPlatformInfoWrapper);
            Version = GetStringInfo(Handle, ComputePlatformInfo.Version, CL10.GetPlatformInfoWrapper);
            QueryDevices();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets a platform of a specified handle.
        /// </summary>
        /// <param name="handle"> The handle of the queried platform. </param>
        /// <returns> The platform of the matching handle or <c>null</c> if none matches. </returns>
        public static ComputePlatform GetByHandle(IntPtr handle)
        {
            foreach (ComputePlatform platform in Platforms)
            {
                if (platform.Handle.Value == handle)
                {
                    return platform;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first matching platform of a specified name.
        /// </summary>
        /// <param name="platformName"> The name of the queried platform. </param>
        /// <returns> The first platform of the specified name or <c>null</c> if none matches. </returns>
        public static ComputePlatform GetByName(string platformName)
        {
            foreach (ComputePlatform platform in Platforms)
            {
                if (platform.Name.Equals(platformName))
                {
                    return platform;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first matching platform of a specified vendor.
        /// </summary>
        /// <param name="platformVendor"> The vendor of the queried platform. </param>
        /// <returns> The first platform of the specified vendor or <c>null</c> if none matches. </returns>
        public static ComputePlatform GetByVendor(string platformVendor)
        {
            foreach (ComputePlatform platform in Platforms)
            {
                if (platform.Vendor.Equals(platformVendor))
                {
                    return platform;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a read-only collection of available devices on the platform.
        /// </summary>
        /// <returns> A read-only collection of the available devices on the platform. </returns>
        /// <remarks> This method resets the <c>ComputePlatform.Devices</c>. This is useful if one or more of them become unavailable (<c>ComputeDevice.Available</c> is <c>false</c>) after a device and command queues that use the device have been created and commands have been queued to them. Further calls will trigger an <c>OutOfResourcesComputeException</c> until this method is executed. You will also need to recreate any <see cref="ComputeResource"/> that was created on the no longer available device. </remarks>
        public ReadOnlyCollection<IComputeDevice> QueryDevices()
        {
            CL10.GetDeviceIDsWrapper(Handle, ComputeDeviceTypes.All, 0, null, out int handlesLength);
            var handles = new CLDeviceHandle[handlesLength];
            CL10.GetDeviceIDsWrapper(Handle, ComputeDeviceTypes.All, handlesLength, handles, out handlesLength);
            var devices = new ComputeDevice[handlesLength];
            for (int i = 0; i < handlesLength; i++)
            {
                devices[i] = new ComputeDevice(this, handles[i]);
            }
            Devices = new ReadOnlyCollection<IComputeDevice>(devices);
            return Devices;
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
                    //CL10.ReleaseProgram(Handle);
                    Handle.Invalidate();
                }
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~ComputePlatform()
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
