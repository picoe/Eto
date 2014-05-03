using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Eto
{
	[AttributeUsage(AttributeTargets.Interface)]
	public class AutoInitializeAttribute : Attribute
	{
		public bool Initialize { get; private set; }

		public AutoInitializeAttribute(bool initialize)
		{
			Initialize = initialize;
		}
	}
}
