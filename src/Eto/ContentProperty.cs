using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto
{
	/// <summary>
	/// Attribute to indicate which property of a type is the content property
	/// </summary>
	/// <remarks>
	/// Used for compatibility with XAML.
	/// </remarks>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ContentPropertyAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the ContentPropertyAttribute class
		/// </summary>
		public ContentPropertyAttribute ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ContentPropertyAttribute class with the specified name
		/// </summary>
		/// <param name="name">Name of the property that should be used as the content property</param>
		public ContentPropertyAttribute (string name)
		{
			Name = name;
		}

		/// <summary>
		/// Gets the name of the property to use as the content property
		/// </summary>
		public string Name { get; private set; }
	}
}