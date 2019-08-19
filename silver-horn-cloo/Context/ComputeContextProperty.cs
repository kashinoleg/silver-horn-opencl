using System;
using System.Diagnostics;

namespace Cloo
{
    /// <summary>
    /// Represents an OpenCL context property.
    /// </summary>
    /// <remarks> An OpenCL context property is a (name, value) data pair. </remarks>
    public class ComputeContextProperty
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComputeContextPropertyName name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IntPtr value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ComputeContextPropertyName"/> of the <see cref="ComputeContextProperty"/>.
        /// </summary>
        /// <value> The <see cref="ComputeContextPropertyName"/> of the <see cref="ComputeContextProperty"/>. </value>
        public ComputeContextPropertyName Name { get { return name; } }

        /// <summary>
        /// Gets the value of the <see cref="ComputeContextProperty"/>.
        /// </summary>
        /// <value> The value of the <see cref="ComputeContextProperty"/>. </value>
        public IntPtr Value { get { return value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ComputeContextProperty"/>.
        /// </summary>
        /// <param name="name"> The name of the <see cref="ComputeContextProperty"/>. </param>
        /// <param name="value"> The value of the created <see cref="ComputeContextProperty"/>. </param>
        public ComputeContextProperty(ComputeContextPropertyName name, IntPtr value)
        {
            this.name = name;
            this.value = value;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the string representation of the <see cref="ComputeContextProperty"/>.
        /// </summary>
        /// <returns> The string representation of the <see cref="ComputeContextProperty"/>. </returns>
        public override string ToString()
        {
            return GetType().Name + "(" + name + ", " + value + ")";
        }

        #endregion
    }
}
