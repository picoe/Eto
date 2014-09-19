using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
#if PCL
		static Assembly Assembly { get { return typeof(TestIcons).GetTypeInfo().Assembly; } }
#else
		static Assembly Assembly { get { return typeof(TestIcons).Assembly; } }
#endif
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? Assembly.GetName().Name + "."; } }

		public static Icon TestIcon
		{
			get { return Icon.FromResource(Prefix + "TestIcon.ico", Assembly); }
		}

		public static Bitmap TestImage
		{
			get { return Bitmap.FromResource(Prefix + "TestImage.png", Assembly); }
		}

		public static Bitmap Textures
		{
			get { return Bitmap.FromResource(Prefix + "Textures.png", Assembly); }
		}

		public static Bitmap TexturesIndexed
		{
			get { return Bitmap.FromResource(Prefix + "Textures.gif", Assembly); }
		}
	}
}
