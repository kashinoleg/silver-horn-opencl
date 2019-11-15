using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Platform;

namespace SilverHorn.Cloo.Context
{
    /// <summary>
    /// Represents an OpenCL context.
    /// </summary>
    /// <remarks> The environment within which the kernels execute and the domain in which synchronization and memory management is defined. </remarks>
    /// <br/>
    /// <example> 
    /// This example shows how to create a context that is able to share data with an OpenGL context in a Microsoft Windows OS:
    /// <code>
    /// <![CDATA[
    /// 
    /// // NOTE: If you see some non C# bits surrounding this code section, ignore them. They're not part of the code.
    /// 
    /// // We will need the device context, which is obtained through an OS specific function.
    /// [DllImport("opengl32.dll")]
    /// extern static IntPtr wglGetCurrentDC();
    /// 
    /// // Query the device context.
    /// IntPtr deviceContextHandle = wglGetCurrentDC();
    /// 
    /// // Select a platform which is capable of OpenCL/OpenGL interop.
    /// ComputePlatform platform = ComputePlatform.GetByName(name);
    /// 
    /// // Create the context property list and populate it.
    /// ComputeContextProperty p1 = new ComputeContextProperty(ComputeContextPropertyName.Platform, platform.Handle.Value);
    /// ComputeContextProperty p2 = new ComputeContextProperty(ComputeContextPropertyName.CL_GL_CONTEXT_KHR, openGLContextHandle);
    /// ComputeContextProperty p3 = new ComputeContextProperty(ComputeContextPropertyName.CL_WGL_HDC_KHR, deviceContextHandle);
    /// ComputeContextPropertyList cpl = new ComputeContextPropertyList(new ComputeContextProperty[] { p1, p2, p3 });
    /// 
    /// // Create the context. Usually, you'll want this on a GPU but other options might be available as well.
    /// ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, cpl, null, IntPtr.Zero);
    /// 
    /// // Create a shared OpenCL/OpenGL buffer.
    /// // The generic type should match the type of data that the buffer contains.
    /// // glBufferId is an existing OpenGL buffer identifier.
    /// ComputeBuffer<float> clglBuffer = ComputeBuffer.CreateFromGLBuffer<float>(context, ComputeMemoryFlags.ReadWrite, glBufferId);
    /// 
    /// ]]>
    /// </code>
    /// Before working with the <c>clglBuffer</c> you should make sure of two things:<br/>
    /// 1) OpenGL isn't using <c>glBufferId</c>. You can achieve this by calling <c>glFinish</c>.<br/>
    /// 2) Make it available to OpenCL through the Queue Acquire GLObjects method.<br/>
    /// When finished, you should wait until <c>clglBuffer</c> isn't used any longer by OpenCL.
    /// After that, call Queue Release GLObjects to make the buffer available to OpenGL again.
    /// </example>
    public sealed class ComputeContext110 : ComputeObject, IComputeContext
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// The handle of the context.
        /// </summary>
        public CLContextHandle Handle { get; internal set; }

        /// <summary>
        /// Gets a read-only collection of the devices of the context.
        /// </summary>
        /// <value> A read-only collection of the devices of the context. </value>
        public ReadOnlyCollection<IComputeDevice> Devices { get; internal set; }

        /// <summary>
        /// Gets the platform of the context.
        /// </summary>
        /// <value> The platform of the context. </value>
        public ComputePlatform Platform { get; internal set; }
        #endregion

        #region Constructors
        internal ComputeContext110() { }
        #endregion

        #region Internal methods
        /// <summary>
        /// Gets a context property of a specified <c>ComputeContextPropertyName</c>.
        /// </summary>
        /// <param name="name"> The <see cref="ComputeContextPropertyName"/> of the context property. </param>
        /// <returns> The requested context property or <c>null</c> if no such context property exists in the list of context properties. </returns>
        internal ComputeContextProperty GetByName(List<ComputeContextProperty> properties, ComputeContextPropertyName name)
        {
            foreach (var property in properties)
            {
                if (property.Name == name)
                {
                    return property;
                }
            }
            return null;
        }

        internal IntPtr[] ToIntPtrArray(List<ComputeContextProperty> properties)
        {
            if (properties == null)
            {
                return null;
            }
            var result = new IntPtr[2 * properties.Count + 1];
            for (int i = 0; i < properties.Count; i++)
            {
                result[2 * i] = new IntPtr((int)properties[i].Name);
                result[2 * i + 1] = properties[i].Value;
            }
            result[result.Length - 1] = IntPtr.Zero;
            return result;
        }

        internal ReadOnlyCollection<IComputeDevice> GetDevices()
        {
            var arrayDevices = GetArrayInfo<CLContextHandle, ComputeContextInfo, CLDeviceHandle>(Handle,
                ComputeContextInfo.Devices, OpenCL110.GetContextInfoWrapper);
            var deviceHandles = new List<CLDeviceHandle>(arrayDevices);
            var devices = new List<IComputeDevice>();
            foreach (var platform in ComputePlatform.Platforms)
            {
                foreach (var device in platform.Devices)
                {
                    if (deviceHandles.Contains(device.Handle))
                    {
                        devices.Add(device);
                    }
                }
            }
            return new ReadOnlyCollection<IComputeDevice>(devices);
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
                    OpenCL110.ReleaseContext(Handle);
                    Handle.Invalidate();
                }
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~ComputeContext110()
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
