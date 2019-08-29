using System;
using System.Collections.Generic;
using System.Globalization;
using Cloo.Bindings;
using SilverHorn.Cloo.Device;
using SilverHorn.Cloo.Event;

namespace Cloo
{
    /// <summary>
    /// Contains various helper methods.
    /// </summary>
    public class ComputeTools
    {
        #region Public methods

        /*
        /// <summary>
        /// Attempts to convert a PixelFormat to a <see cref="ComputeImageFormat"/>.
        /// </summary>
        /// <param name="format"> The format to convert. </param>
        /// <returns> A <see cref="ComputeImageFormat"/> that matches the specified argument. </returns>
        /// <remarks> Note that only <c>Alpha</c>, <c>Format16bppRgb555</c>, <c>Format16bppRgb565</c> and <c>Format32bppArgb</c> input values are currently supported. </remarks>        
        public static ComputeImageFormat ConvertImageFormat(PixelFormat format)
        {
            switch(format)
            {
                case PixelFormat.Alpha:
                    return new ComputeImageFormat(ComputeImageChannelOrder.A, ComputeImageChannelType.UnsignedInt8);
                case PixelFormat.Format16bppRgb555:
                    return new ComputeImageFormat(ComputeImageChannelOrder.Rgb, ComputeImageChannelType.UNormShort555);
                case PixelFormat.Format16bppRgb565:
                    return new ComputeImageFormat(ComputeImageChannelOrder.Rgb, ComputeImageChannelType.UNormShort565);
                case PixelFormat.Format32bppArgb:
                    return new ComputeImageFormat(ComputeImageChannelOrder.Argb, ComputeImageChannelType.UnsignedInt8);
                default: throw new ArgumentException("Pixel format not supported.");
            }
        }
        */

        /// <summary>
        /// Parses an OpenCL version string.
        /// </summary>
        /// <param name="versionString"> The version string to parse. Must be in the format: <c>Additional substrings[space][major_version].[minor_version][space]Additional substrings</c>. </param>
        /// <param name="substringIndex"> The index of the substring that specifies the OpenCL version. </param>
        /// <returns> A <c>Version</c> instance containing the major and minor version from <paramref name="versionString"/>. </returns>
        public static Version ParseVersionString(String versionString, int substringIndex)
        {
            string[] verstring = versionString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new Version(verstring[substringIndex]);
        }

        #endregion

        #region Internal methods

        internal static IntPtr[] ConvertArray(long[] array)
        {
            if (array == null) return null;

            NumberFormatInfo nfi = new NumberFormatInfo();

            IntPtr[] result = new IntPtr[array.Length];
            for (long i = 0; i < array.Length; i++)
            {
                result[i] = new IntPtr(array[i]);
            }
            return result;
        }

        internal static long[] ConvertArray(IntPtr[] array)
        {
            if (array == null)
            {
                return null;
            }

            NumberFormatInfo nfi = new NumberFormatInfo();

            long[] result = new long[array.Length];
            for (long i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToInt64();
            }
            return result;
        }

        internal static CLDeviceHandle[] ExtractHandles(ICollection<IComputeDevice> computeObjects, out int handleCount)
        {
            if (computeObjects == null || computeObjects.Count == 0)
            {
                handleCount = 0;
                return null;
            }

            var result = new CLDeviceHandle[computeObjects.Count];
            int i = 0;
            foreach (var computeObj in computeObjects)
            {
                result[i] = computeObj.Handle;
                i++;
            }
            handleCount = computeObjects.Count;
            return result;
        }

        internal static CLEventHandle[] ExtractHandles(ICollection<IComputeEvent> computeObjects, out int handleCount)
        {
            if (computeObjects == null || computeObjects.Count == 0)
            {
                handleCount = 0;
                return null;
            }

            var result = new CLEventHandle[computeObjects.Count];
            int i = 0;
            foreach (var computeObj in computeObjects)
            {
                result[i] = computeObj.Handle;
                i++;
            }
            handleCount = computeObjects.Count;
            return result;
        }

        internal static CLMemoryHandle[] ExtractHandles(ICollection<ComputeMemory> computeObjects, out int handleCount)
        {
            if (computeObjects == null || computeObjects.Count == 0)
            {
                handleCount = 0;
                return null;
            }
            var result = new CLMemoryHandle[computeObjects.Count];
            int i = 0;
            foreach (var computeObj in computeObjects)
            {
                result[i] = computeObj.Handle;
                i++;
            }
            handleCount = computeObjects.Count;
            return result;
        }
        #endregion
    }
}
