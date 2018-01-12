using System;
using System.ComponentModel;

namespace Eto
{
	/// <summary>
	/// Attribute to specify the interface type to use for the handler of a <see cref="Widget"/>
	/// </summary>
	/// <remarks>
	/// Apply this to your widget-derived class to allow it to automatically create your handler.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class HandlerAttribute : Attribute
	{
		/// <summary>
		/// Gets the type of the handler required for the widget
		/// </summary>
		/// <value>The type of the handler</value>
		public Type Type { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.HandlerAttribute"/> class.
		/// </summary>
		/// <param name="type">Type of the handler to use for the applied widget</param>
		public HandlerAttribute(Type type)
		{
			Type = type;
		}
	}
}

