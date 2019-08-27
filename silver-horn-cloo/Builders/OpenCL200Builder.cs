using Cloo;
using Cloo.Bindings;
using NLog;
using SilverHorn.Cloo.Context;
using SilverHorn.Cloo.Device;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SilverHorn.Cloo.Builders
{
    public sealed class OpenCL200Builder : IOpenCLBuilder
    {
        #region Services
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Compute Program Constructors
        /// <summary>
        /// Creates a new program from a source code string.
        /// </summary>
        /// <param name="context"> A program. </param>
        /// <param name="source"> The source code for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public ComputeProgram BuildComputeProgram(IComputeContext context, string source)
        {
            var program = new ComputeProgram();
            program.Handle = OpenCL200.CreateProgramWithSource(
                context.Handle,
                1,
                new string[] { source },
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);

            program.SetID(program.Handle.Value);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }

        /// <summary>
        /// Creates a new program from an array of source code strings.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="source"> The source code lines for the program. </param>
        /// <remarks> The created program is associated with the devices. </remarks>
        public ComputeProgram BuildComputeProgram(IComputeContext context, string[] source)
        {
            var program = new ComputeProgram();
            program.Handle = OpenCL200.CreateProgramWithSource(
                context.Handle,
                source.Length,
                source,
                null,
                out ComputeErrorCode error);
            ComputeException.ThrowOnError(error);
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }

        /// <summary>
        /// Creates a new program from a specified list of binaries.
        /// </summary>
        /// <param name="context"> A context. </param>
        /// <param name="binaries"> A list of binaries, one for each item in <paramref name="devices"/>. </param>
        /// <param name="devices"> A subset of the context devices. If <paramref name="devices"/> is <c>null</c>, OpenCL will associate every binary from binaries with a corresponding device from devices. </param>
        public ComputeProgram BuildComputeProgram(IComputeContext context, IList<byte[]> binaries, IList<IComputeDevice> devices)
        {
            var program = new ComputeProgram();
            int count;
            CLDeviceHandle[] deviceHandles;
            if (devices != null)
            {
                deviceHandles = ComputeTools.ExtractHandles(devices, out count);
            }
            else
            {
                deviceHandles = ComputeTools.ExtractHandles(context.Devices, out count);
            }

            IntPtr[] binariesPtrs = new IntPtr[count];
            IntPtr[] binariesLengths = new IntPtr[count];
            int[] binariesStats = new int[count];
            GCHandle[] binariesGCHandles = new GCHandle[count];

            try
            {
                for (int i = 0; i < count; i++)
                {
                    binariesGCHandles[i] = GCHandle.Alloc(binaries[i], GCHandleType.Pinned);
                    binariesPtrs[i] = binariesGCHandles[i].AddrOfPinnedObject();
                    binariesLengths[i] = new IntPtr(binaries[i].Length);
                }

                program.Handle = OpenCL200.CreateProgramWithBinary(
                    context.Handle,
                    count,
                    deviceHandles,
                    binariesLengths,
                    binariesPtrs,
                    binariesStats,
                    out ComputeErrorCode error);
                ComputeException.ThrowOnError(error);
            }
            finally
            {
                for (int i = 0; i < count; i++)
                {
                    binariesGCHandles[i].Free();
                }
            }
            logger.Info("Create " + this + " in Thread(" + Thread.CurrentThread.ManagedThreadId + ").", "Information");
            return program;
        }
        #endregion

    }
}
