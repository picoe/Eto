using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		public static Icon TestIcon
		{
			get { return Icon.FromResource("Eto.Test.TestIcon.ico"); }
		}

		public static Bitmap TestImage
		{
			get { return Bitmap.FromResource("Eto.Test.TestImage.png"); }
		}

		public static Bitmap Textures
		{
			get { return Bitmap.FromResource("Eto.Test.Textures.png"); }
		}

		public static Bitmap TexturesIndexed
		{
			get { return Bitmap.FromResource("Eto.Test.Textures.gif"); }
		}
	}
}
