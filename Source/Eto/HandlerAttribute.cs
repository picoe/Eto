using System;
using System.ComponentModel;

namespace Eto
{
	[AttributeUsage(AttributeTargets.Class)]
	public class HandlerAttribute : Attribute
	{
		public Type Type { get; private set; }

		public HandlerAttribute(Type type)
		{
			Type = type;
		}
	}
}

