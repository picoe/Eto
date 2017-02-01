using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		public static Icon TestIcon
		{
			get { return Icon.FromResource("Eto.Test.Images.TestIcon.ico"); }
		}

		public static Bitmap TestImage
		{
			get { return Bitmap.FromResource("Eto.Test.Images.TestImage.png"); }
		}

		public static Bitmap Textures
		{
			get { return Bitmap.FromResource("Eto.Test.Images.Textures.png"); }
		}

		public static Bitmap TexturesIndexed
		{
			get { return Bitmap.FromResource("Eto.Test.Images.Textures.gif"); }
		}

		public static Icon Logo
		{
			get { return Icon.FromResource("Eto.Test.Images.Logo.png"); }
		}
	
	}
}
