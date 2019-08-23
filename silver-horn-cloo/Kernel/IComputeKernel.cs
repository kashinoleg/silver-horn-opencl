using Cloo;
using Cloo.Bindings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilverHorn.Cloo.Kernel
{
    public interface IComputeKernel : IDisposable
    {
        #region Properties
        /// <summary>
        /// The handle of the kernel.
        /// </summary>
        CLKernelHandle Handle { get; }

        /// <summary>
        /// Gets the function name of the kernel.
        /// </summary>
        /// <value> The function name of the kernel. </value>
        string FunctionName { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the amount of local memory in bytes used by the kernel.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The amount of local memory in bytes used by the kernel. </returns>
        long GetLocalMemorySize(ComputeDevice device);

        /// <summary>
        /// Gets the compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The compile work-group size specified by the <c>__attribute__((reqd_work_group_size(X, Y, Z)))</c> qualifier. If no such qualifier is specified, (0, 0, 0) is returned. </returns>
        long[] GetCompileWorkGroupSize(ComputeDevice device);

        /// <summary>
        /// Gets the preferred multiple of workgroup size for launch. 
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The preferred multiple of workgroup size for launch. </returns>
        /// <remarks> The returned value is a performance hint. Specifying a workgroup size that is not a multiple of the value returned by this query as the value of the local work size argument to ComputeCommandQueue.Execute will not fail to enqueue the kernel for execution unless the work-group size specified is larger than the device maximum. </remarks>
        /// <remarks> Requires OpenCL 1.1. </remarks>
        long GetPreferredWorkGroupSizeMultiple(ComputeDevice device);

        /// <summary>
        /// Gets the minimum amount of memory, in bytes, used by each work-item in the kernel.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The minimum amount of memory, in bytes, used by each work-item in the kernel. </returns>
        /// <remarks> The returned value may include any private memory needed by an implementation to execute the kernel, including that used by the language built-ins and variable declared inside the kernel with the <c>__private</c> or <c>private</c> qualifier. </remarks>
        long GetPrivateMemorySize(ComputeDevice device);

        /// <summary>
        /// Gets the maximum work-group size that can be used to execute the kernel on a <see cref="ComputeDevice"/>.
        /// </summary>
        /// <param name="device"> One of the device. </param>
        /// <returns> The maximum work-group size that can be used to execute the kernel on device. </returns>
        long GetWorkGroupSize(ComputeDevice device);

        /// <summary>
        /// Sets an argument of the kernel (no argument tracking).
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="dataSize"> The size of the argument data in bytes. </param>
        /// <param name="dataAddr"> A pointer to the data that should be used as the argument value. </param>
        /// <remarks> 
        /// Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel.
        /// <br/>
        /// Note that this method does not provide argument tracking. It is up to the user to reference the kernel arguments (i.e. prevent them from being garbage collected) until the kernel has finished execution.
        /// </remarks>
        void SetArgument(int index, IntPtr dataSize, IntPtr dataAddr);

        /// <summary>
        /// Sets the size in bytes of an argument specfied with the <c>local</c> or <c>__local</c> address space qualifier.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="dataSize"> The size of the argument data in bytes. </param>
        /// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        void SetLocalArgument(int index, long dataSize);

        /// <summary>
        /// Sets a <c>T*</c>, <c>image2d_t</c> or <c>image3d_t</c> argument of the kernel.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="memObj"> The <see cref="ComputeMemory"/> that is passed as the argument. </param>
        /// <remarks> This method will automatically track <paramref name="memObj"/> to prevent it from being collected by the GC.<br/> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        void SetMemoryArgument(int index, ComputeMemory memObj);

        /// <summary>
        /// Sets a <c>sampler_t</c> argument of the kernel.
        /// </summary>
        /// <param name="index"> The argument index. </param>
        /// <param name="sampler"> The <see cref="ComputeSampler"/> that is passed as the argument. </param>
        /// <remarks> This method will automatically track <paramref name="sampler"/> to prevent it from being collected by the GC.<br/> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        void SetSamplerArgument(int index, ComputeSampler sampler);

        /// <summary>
        /// Sets a value argument of the kernel.
        /// </summary>
        /// <typeparam name="T"> The type of the argument. </typeparam>
        /// <param name="index"> The argument index. </param>
        /// <param name="data"> The data that is passed as the argument value. </param>
        /// <remarks> Arguments to the kernel are referred by indices that go from 0 for the leftmost argument to n-1, where n is the total number of arguments declared by the kernel. </remarks>
        void SetValueArgument<T>(int index, T data) where T : struct;
        #endregion
    }
}
