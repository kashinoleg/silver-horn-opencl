using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Command;

namespace SilverHorn.Cloo.Event
{
    /// <summary>
    /// Represents an OpenCL event.
    /// </summary>
    /// <remarks> An event encapsulates the status of an operation such as a command. It can be used to synchronize operations in a context. </remarks>
    public class ComputeEvent : ComputeResource, IComputeEvent
    {
        #region Fields

        private event ComputeCommandStatusChanged aborted;
        private event ComputeCommandStatusChanged completed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ComputeCommandStatusArgs status;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ComputeEventCallback statusNotify;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GCHandle gcHandle;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the command associated with the event is abnormally terminated.
        /// </summary>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public event ComputeCommandStatusChanged Aborted
        {
            add
            {
                aborted += value;
                if (status != null && status.Status != ComputeCommandExecutionStatus.Complete)
                    value.Invoke(this, status);
            }
            remove
            {
                aborted -= value;
            }
        }

        /// <summary>
        /// Occurs when <c>ComputeEventBase.Status</c> changes to <c>ComputeCommandExecutionStatus.Complete</c>.
        /// </summary>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public event ComputeCommandStatusChanged Completed
        {
            add
            {
                completed += value;
                if (status != null && status.Status == ComputeCommandExecutionStatus.Complete)
                    value.Invoke(this, status);
            }
            remove
            {
                completed -= value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The handle of the event types.
        /// </summary>
        public CLEventHandle Handle { get; protected set; }

        /// <summary>
        /// Gets the device time counter in nanoseconds when the associated command has finished execution.
        /// </summary>
        /// <value> The device time counter in nanoseconds when the associated command has finished execution. </value>
        public long FinishTime
        {
            get { return GetInfo<CLEventHandle, ComputeCommandProfilingInfo, long>(Handle, ComputeCommandProfilingInfo.Ended, CL10.GetEventProfilingInfo); }
        }

        /// <summary>
        /// Gets the device time counter in nanoseconds when the associated command is enqueued in the command queue by the host.
        /// </summary>
        /// <value> The device time counter in nanoseconds when the associated command is enqueued in the command queue by the host. </value>
        public long EnqueueTime
        {
            get { return (long)GetInfo<CLEventHandle, ComputeCommandProfilingInfo, long>(Handle, ComputeCommandProfilingInfo.Queued, CL10.GetEventProfilingInfo); }
        }

        /// <summary>
        /// Gets the execution status of the associated command.
        /// </summary>
        /// <value> The execution status of the associated command or a negative value if the execution was abnormally terminated. </value>
        public ComputeCommandExecutionStatus Status
        {
            get { return (ComputeCommandExecutionStatus)GetInfo<CLEventHandle, ComputeEventInfo, int>(Handle, ComputeEventInfo.ExecutionStatus, CL10.GetEventInfo); }
        }

        /// <summary>
        /// Gets the device time counter in nanoseconds when the associated command starts execution.
        /// </summary>
        /// <value> The device time counter in nanoseconds when the associated command starts execution. </value>
        public long StartTime
        {
            get { return (long)GetInfo<CLEventHandle, ComputeCommandProfilingInfo, ulong>(Handle, ComputeCommandProfilingInfo.Started, CL10.GetEventProfilingInfo); }
        }

        /// <summary>
        /// Gets the device time counter in nanoseconds when the associated command that has been enqueued is submitted by the host to the device.
        /// </summary>
        /// <value> The device time counter in nanoseconds when the associated command that has been enqueued is submitted by the host to the device. </value>
        public long SubmitTime
        {
            get { return (long)GetInfo<CLEventHandle, ComputeCommandProfilingInfo, ulong>(Handle, ComputeCommandProfilingInfo.Submitted, CL10.GetEventProfilingInfo); }
        }

        /// <summary>
        /// Gets the <see cref="ComputeCommandType"/> associated with the event.
        /// </summary>
        /// <value> The <see cref="ComputeCommandType"/> associated with the event. </value>
        public ComputeCommandType Type { get; protected set; }

        /// <summary>
        /// Gets the command queue associated with the event.
        /// </summary>
        /// <value> The command queue associated with the event. </value>
        public ComputeCommandQueue CommandQueue { get; private set; }
        #endregion

        #region Constructors

        internal ComputeEvent(CLEventHandle handle, ComputeCommandQueue queue)
        {
            Handle = handle;
            SetID(Handle.Value);

            CommandQueue = queue;
            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, int>(Handle,
                ComputeEventInfo.CommandType, CL10.GetEventInfo);

            if (ComputeTools.ParseVersionString(CommandQueue.Device.Platform.Version, 1) > new Version(1, 0))
            {
                HookNotifier();
            }
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
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
        /// 
        /// </summary>
        protected void HookNotifier()
        {
            statusNotify = new ComputeEventCallback(StatusNotify);
            ComputeErrorCode error = CL11.SetEventCallback(Handle, (int)ComputeCommandExecutionStatus.Complete, statusNotify, IntPtr.Zero);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evArgs"></param>
        protected virtual void OnCompleted(object sender, ComputeCommandStatusArgs evArgs)
        {
            logger.Info("Complete " + Type + " operation of " + this + ".", "Information");
            if (completed != null)
                completed(sender, evArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evArgs"></param>
        protected virtual void OnAborted(object sender, ComputeCommandStatusArgs evArgs)
        {
            logger.Info("Abort " + Type + " operation of " + this + ".", "Information");
            if (aborted != null)
                aborted(sender, evArgs);
        }

        /// <summary>
        /// Releases the associated OpenCL object.
        /// </summary>
        /// <param name="manual"> Specifies the operation mode of this method. </param>
        /// <remarks> <paramref name="manual"/> must be <c>true</c> if this method is invoked directly by the application. </remarks>
        protected override void Dispose(bool manual)
        {
            FreeGCHandle();
            if (Handle.IsValid)
            {
                logger.Info("Dispose " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
                CL10.ReleaseEvent(Handle);
                Handle.Invalidate();
            }
        }

        #endregion

        #region Private methods
        private void StatusNotify(CLEventHandle eventHandle, int cmdExecStatusOrErr, IntPtr userData)
        {
            status = new ComputeCommandStatusArgs(this, (ComputeCommandExecutionStatus)cmdExecStatusOrErr);
            switch (cmdExecStatusOrErr)
            {
                case (int)ComputeCommandExecutionStatus.Complete: OnCompleted(this, status); break;
                default: OnAborted(this, status); break;
            }
        }
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
                {
                    FreeGCHandle();
                }
            }
        }

        private void FreeGCHandle()
        {
            if (gcHandle.IsAllocated && gcHandle.Target != null)
            {
                gcHandle.Free();
            }
        }
        #endregion
    }
}
