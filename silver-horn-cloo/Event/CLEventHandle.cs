﻿using System;
using System.Diagnostics;

namespace SilverHorn.Cloo.Event
{
    /// <summary>
    /// Represents the event ID.
    /// </summary>
    public struct CLEventHandle
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IntPtr value;

        /// <summary>
        /// Gets a logic value indicating whether the handle is valid.
        /// </summary>
        public bool IsValid => value != IntPtr.Zero;

        /// <summary>
        /// Gets the value of the handle.
        /// </summary>
        public IntPtr Value => value;

        /// <summary>
        /// Invalidates the handle.
        /// </summary>
        public void Invalidate()
        {
            value = IntPtr.Zero;
        }
    }
}
