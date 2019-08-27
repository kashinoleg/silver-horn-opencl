using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Platform;
using System;
using System.Collections.ObjectModel;

namespace SilverHorn.Cloo.Context
{
    public interface IComputeContext : IDisposable
    {
        #region Properties
        /// <summary>
        /// The handle of the context.
        /// </summary>
        CLContextHandle Handle { get; }

        /// <summary>
        /// Gets a read-only collection of the devices of the context.
        /// </summary>
        /// <value> A read-only collection of the devices of the context. </value>
        ReadOnlyCollection<IComputeDevice> Devices { get; }

        /// <summary>
        /// Gets the platform of the context.
        /// </summary>
        /// <value> The platform of the context. </value>
        ComputePlatform Platform { get; }
        #endregion
    }
}
