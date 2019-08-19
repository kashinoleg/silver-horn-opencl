using System;
using System.Runtime.InteropServices;

namespace Cloo
{
    /// <summary>
    /// A structure of three integers of platform specific size.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SysIntX3
    {
        #region Fields

        /// <summary>
        /// The first coordinate.
        /// </summary>
        public IntPtr X;

        /// <summary>
        /// The second coordinate.
        /// </summary>
        public IntPtr Y;

        /// <summary>
        /// The third coordinate.
        /// </summary>
        public IntPtr Z;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x2"></param>
        /// <param name="z"></param>
        public SysIntX3(SysIntX2 x2, long z)
            : this(x2.X, x2.Y, new IntPtr(z))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SysIntX3(int x, int y, int z)
            : this(new IntPtr(x), new IntPtr(y), new IntPtr(z))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SysIntX3(long x, long y, long z)
            : this(new IntPtr(x), new IntPtr(y), new IntPtr(z))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SysIntX3(IntPtr x, IntPtr y, IntPtr z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the string representation of the SysIntX2.
        /// </summary>
        /// <returns> The string representation of the SysIntX2. </returns>
        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        #endregion
    }
}
