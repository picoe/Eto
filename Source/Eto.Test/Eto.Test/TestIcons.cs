using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		public static string TestIconName = "Eto.Test.TestIcon.ico"; // static not const so test apps can override
		public static Icon TestIcon { get { return Icon.FromResource(TestIconName); } }

		public static string TestImageName = "Eto.Test.TestImage.png"; // static not const so test apps can override
		public static Bitmap TestImage { get { return Bitmap.FromResource(TestImageName); } }

		public static string TexturesName = "Eto.Test.Textures.png"; // static not const so test apps can override
		public static Bitmap Textures { get { return Bitmap.FromResource(TexturesName); } }
	}
}
