using System.Diagnostics;
using System.Threading;
using Cloo.Bindings;
using SilverHorn.Cloo.Context;

namespace Cloo
{
    /// <summary>
    /// Represents an user created event.
    /// </summary>
    /// <remarks> Requires OpenCL 1.1. </remarks>
    public class ComputeUserEvent : ComputeEventBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ComputeUserEvent"/>.
        /// </summary>
        /// <param name="context"> The context in which the <see cref="ComputeUserEvent"/> is created. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public ComputeUserEvent(IComputeContext context)
        {
            Handle = CL11.CreateUserEvent(context.Handle, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, uint>(Handle, ComputeEventInfo.CommandType, CL10.GetEventInfo);
            Context = context;
            HookNotifier();

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the new status of the <see cref="ComputeUserEvent"/>.
        /// </summary>
        /// <param name="status"> The new status of the <see cref="ComputeUserEvent"/>. Allowed value is <see cref="ComputeCommandExecutionStatus.Complete"/>. </param>
        public void SetStatus(ComputeCommandExecutionStatus status)
        {
            SetStatus((int)status);
        }

        /// <summary>
        /// Sets the new status of the <see cref="ComputeUserEvent"/> to an error value.
        /// </summary>
        /// <param name="status"> The error status of the <see cref="ComputeUserEvent"/>. This should be a negative value. </param>
        public void SetStatus(int status)
        {
            ComputeErrorCode error = CL11.SetUserEventStatus(Handle, status);
            ComputeException.ThrowOnError(error);
        }

        #endregion
    }
}
