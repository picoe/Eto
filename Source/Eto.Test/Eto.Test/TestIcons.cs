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

		static Icon testIcon;
		public static Icon TestIcon { get { return testIcon = testIcon ?? Icon.FromResource(TestIconName); } }

		public static string TestImageName = "Eto.Test.TestImage.png"; // static not const so test apps can override
		static Bitmap testImage;
		public static Bitmap TestImage { get { return testImage = testImage ?? Bitmap.FromResource(TestImageName); } }

		public static string TexturesName = "Eto.Test.Textures.png"; // static not const so test apps can override
		static Bitmap textures;
		public static Bitmap Textures { get { return textures = textures ?? Bitmap.FromResource(TexturesName); } }
	}
}
