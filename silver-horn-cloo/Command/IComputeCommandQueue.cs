using Cloo;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverHorn.Cloo.Command
{
    public interface IComputeCommandQueue : IDisposable
    {
        #region Properties

        /// <summary>
        /// The handle of the command queue.
        /// </summary>
        CLCommandQueueHandle Handle { get; }

        /// <summary>
        /// Gets the context of the command queue.
        /// </summary>
        /// <value> The context of the command queue. </value>
        IComputeContext Context { get; }

        /// <summary>
        /// Gets the device of the command queue.
        /// </summary>
        /// <value> The device of the command queue. </value>
        IComputeDevice Device { get; }

        /// <summary>
        /// Gets the out-of-order execution mode of the commands in the command queue.
        /// </summary>
        /// <value> Is <c>true</c> if command queue has out-of-order execution mode enabled and <c>false</c> otherwise. </value>
        bool OutOfOrderExecution { get; }

        /// <summary>
        /// Gets the profiling mode of the commands in the command queue.
        /// </summary>
        /// <value> Is <c>true</c> if command queue has profiling enabled and <c>false</c> otherwise. </value>
        bool Profiling { get; }
        #endregion

    }
}
