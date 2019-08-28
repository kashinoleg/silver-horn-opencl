using System;

namespace SilverHorn.Cloo.Context
{
    /// <summary>
    /// Represents an OpenCL context property.
    /// </summary>
    /// <remarks> An OpenCL context property is a (name, value) data pair. </remarks>
    public sealed class ComputeContextProperty
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="ComputeContextPropertyName"/> of the context property.
        /// </summary>
        /// <value> The <see cref="ComputeContextPropertyName"/> of the context property. </value>
        public ComputeContextPropertyName Name { get; private set; }

        /// <summary>
        /// Gets the value of the context property.
        /// </summary>
        /// <value> The value of the context property. </value>
        public IntPtr Value { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new context property.
        /// </summary>
        /// <param name="name"> The name of the context property. </param>
        /// <param name="value"> The value of the created context property. </param>
        public ComputeContextProperty(ComputeContextPropertyName name, IntPtr value)
        {
            Name = name;
            Value = value;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the string representation of the context property.
        /// </summary>
        /// <returns> The string representation of the context property. </returns>
        public override string ToString()
        {
            return GetType().Name + "(" + Name + ", " + Value + ")";
        }
        #endregion
    }
}
