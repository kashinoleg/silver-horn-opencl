using System.Threading;
using Cloo;
using Cloo.Bindings;
using SilverHorn.Cloo.Command;
using SilverHorn.Cloo.Context;

namespace SilverHorn.Cloo.Event
{
    /// <summary>
    /// Represents an user created event.
    /// </summary>
    /// <remarks> Requires OpenCL 1.1. </remarks>
    public class ComputeUserEvent : ComputeEventBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new user created event.
        /// </summary>
        /// <param name="context"> The context in which the user created event is created. </param>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        public ComputeUserEvent(IComputeContext context)
        {
            Handle = CL11.CreateUserEvent(context.Handle, out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            SetID(Handle.Value);

            Type = (ComputeCommandType)GetInfo<CLEventHandle, ComputeEventInfo, uint>(Handle, ComputeEventInfo.CommandType,
                CL10.GetEventInfo);
            Context = context;
            HookNotifier();

            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the new status of the user created event.
        /// </summary>
        /// <param name="status"> The new status of the user created event. Allowed value is <see cref="ComputeCommandExecutionStatus.Complete"/>. </param>
        public void SetStatus(ComputeCommandExecutionStatus status)
        {
            SetStatus((int)status);
        }

        /// <summary>
        /// Sets the new status of the user created event to an error value.
        /// </summary>
        /// <param name="status"> The error status of the user created event. This should be a negative value. </param>
        public void SetStatus(int status)
        {
            ComputeErrorCode error = CL11.SetUserEventStatus(Handle, status);
            ComputeException.ThrowOnError(error);
        }

        #endregion
    }
}
