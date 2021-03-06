﻿using System;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using NLog;

namespace SilverHorn.Cloo.Sampler
{
    /// <summary>
    /// Represents an OpenCL sampler.
    /// </summary>
    /// <remarks> An object that describes how to sample an image when the image is read in the kernel. The image read functions take a sampler as an argument. The sampler specifies the image addressing-mode i.e. how out-of-range image coordinates are handled, the filtering mode, and whether the input image coordinate is a normalized or unnormalized value. </remarks>
    public sealed class ComputeSampler120 : ComputeObject, IComputeSampler
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// The handle of the sampler.
        /// </summary>
        public CLSamplerHandle Handle { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ComputeImageAddressing"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageAddressing"/> mode of the sampler. </value>
        public ComputeImageAddressing Addressing { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ComputeImageFiltering"/> mode of the sampler.
        /// </summary>
        /// <value> The <see cref="ComputeImageFiltering"/> mode of the sampler. </value>
        public ComputeImageFiltering Filtering { get; internal set; }

        /// <summary>
        /// Gets the state of usage of normalized x, y and z coordinates when accessing a image in a kernel through the sampler.
        /// </summary>
        /// <value> The state of usage of normalized x, y and z coordinates when accessing a image in a kernel through the sampler. </value>
        public bool NormalizedCoords { get; internal set; }
        #endregion

        #region Constructors
        internal ComputeSampler120() { }
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
                    OpenCL120.ReleaseSampler(Handle);
                    Handle.Invalidate();
                }
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~ComputeSampler120()
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
