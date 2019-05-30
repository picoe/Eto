using System;
using Eto.Drawing;
namespace Eto.Addin.MonoDevelop
{
	public static class Extensions
	{
		public static Color ToEto(this Xwt.Drawing.Color color)
		{
			return new Color((float)color.Red, (float)color.Green, (float)color.Blue, (float)color.Alpha);
		}
	}
}
