using System;
using Eto.Forms;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Addin.Shared
{
	static class Extensions
	{
		public static bool ToBool(this string value)
		{
			bool result;
			return bool.TryParse(value, out result) && result;
		}
	}
	
}