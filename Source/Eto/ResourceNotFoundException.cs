using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Globalization;

namespace Eto
{
	/// <summary>
	/// Exception for when a resource is not found
	/// </summary>
	/// <remarks>
	/// Used typically when using FromResource methods
	/// </remarks>
	[Serializable]
	public class ResourceNotFoundException : EtoException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceNotFoundException"/> class
		/// </summary>
		public ResourceNotFoundException () { }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public ResourceNotFoundException (string message) : base (message) { }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceNotFoundException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public ResourceNotFoundException (string message, Exception inner) : base (message, inner) { }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceNotFoundException"/> class
		/// </summary>
		/// <param name="assembly">The assembly the resource was attempted to be retrieved from</param>
		/// <param name="resourceName">Name of the resource</param>
		public ResourceNotFoundException (Assembly assembly, string resourceName)
			: this (string.Format (CultureInfo.CurrentCulture, "Resource '{0}' not found in assembly '{1}'", resourceName, assembly.FullName))
		{ }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ResourceNotFoundException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected ResourceNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) { }
	}
}
