using System;

namespace Eto
{
	/// <summary>
	/// Attribute to specify the name property of a control for serialization
	/// </summary>
	/// <remarks>
	/// Used by Eto.Serialization.Xaml, for example, to specify which property is used for the name property.
	/// 
	/// E.g. when specifying the ID, it also specifies which name of the backing field.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RuntimeNamePropertyAttribute : Attribute
	{
		/// <summary>
		/// Gets the name of the property to use as the runtime name
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.RuntimeNamePropertyAttribute"/> class.
		/// </summary>
		/// <param name="name">Name of the property</param>
		public RuntimeNamePropertyAttribute(string name)
		{
			Name = name;
		}
	}
}