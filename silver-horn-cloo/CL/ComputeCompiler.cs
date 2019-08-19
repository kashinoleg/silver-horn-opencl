using Cloo.Bindings;

namespace Cloo
{
    /// <summary>
    /// Represents the OpenCL compiler.
    /// </summary>
    public class ComputeCompiler
    {
        #region Public methods

        /// <summary>
        /// Unloads the OpenCL compiler.
        /// </summary>
        public static void Unload()
        {
            ComputeErrorCode error = CL10.UnloadCompiler();
            ComputeException.ThrowOnError(error);
        }

        #endregion
    }
}
