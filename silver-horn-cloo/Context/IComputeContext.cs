using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Platform;
using System.Collections.ObjectModel;

namespace Cloo
{
    public interface IComputeContext
    {
        /// <summary>
        /// Gets a read-only collection of the devices of the <see cref="ComputeContext"/>.
        /// </summary>
        /// <value> A read-only collection of the devices of the <see cref="ComputeContext"/>. </value>
        ReadOnlyCollection<IComputeDevice> Devices { get; }

        /// <summary>
        /// Gets the platform of the <see cref="ComputeContext"/>.
        /// </summary>
        /// <value> The platform of the <see cref="ComputeContext"/>. </value>
        ComputePlatform Platform { get; }
    }
}
