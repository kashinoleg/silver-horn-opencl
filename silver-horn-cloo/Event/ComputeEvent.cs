using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL event.
    /// </summary>
    /// <remarks> An event encapsulates the status of an operation such as a command. It can be used to synchronize operations in a context. </remarks>
    /// <seealso cref="ComputeUserEvent"/>
    /// <seealso cref="ComputeCommandQueue"/>
    /// <seealso cref="ComputeContext"/>
    public class ComputeEvent : ComputeEventBase
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GCHandle gcHandle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ComputeCommandQueue"/> associated with the <see cref="ComputeEvent"/>.
        /// </summary>
        /// <value> The <see cref="ComputeCommandQueue"/> associated with the <see cref="ComputeEvent"/>. </value>
        public ComputeCommandQueue CommandQueue { get; private set; }

        #endregion

        #region Constructors

        internal ComputeEvent(CLEventHandle handle, ComputeCommandQueue queue)
        {
            Handle = handle;
            SetID(Handle.Value);

            CommandQueue = queue;
            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, int>(Handle, ComputeEventInfo.CommandType, CL10.GetEventInfo);
            Context = queue.Context;

            if (ComputeTools.ParseVersionString(CommandQueue.Device.Platform.Version, 1) > new Version(1, 0))
                HookNotifier();

            Trace.WriteLine("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        #region Internal methods

        internal void TrackGCHandle(GCHandle handle)
        {
            gcHandle = handle;

            Completed += new ComputeCommandStatusChanged(Cleanup);
            Aborted += new ComputeCommandStatusChanged(Cleanup);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Releases the associated OpenCL object.
        /// </summary>
        /// <param name="manual"> Specifies the operation mode of this method. </param>
        /// <remarks> <paramref name="manual"/> must be <c>true</c> if this method is invoked directly by the application. </remarks>
        protected override void Dispose(bool manual)
        {
            FreeGCHandle();
            base.Dispose(manual);
        }

        #endregion

        #region Private methods

        private void Cleanup(object sender, ComputeCommandStatusArgs e)
        {
            lock (CommandQueue.Events)
            {
                if (CommandQueue.Events.Contains(this))
                {
                    CommandQueue.Events.Remove(this);
                    Dispose();
                }
                else
                    FreeGCHandle();
            }
        }

        private void FreeGCHandle()
        {
            if (gcHandle.IsAllocated && gcHandle.Target != null)
                gcHandle.Free();
        }

        #endregion
    }
}
