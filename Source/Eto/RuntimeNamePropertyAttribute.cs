using System;

namespace Eto
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RuntimeNamePropertyAttribute : Attribute
	{
		public string Name { get; private set; }

		public RuntimeNamePropertyAttribute(string name)
		{
			Name = name;
		}
	}
}