namespace Eto.Test
{
	public static class TestIcons
	{
		public static Icon TestIcon => Icon.FromResource("Eto.Test.Images.TestIcon.ico", typeof(TestIcons));

		public static Bitmap TestImage => Platform.Instance.Supports<Bitmap>() ? Bitmap.FromResource("Eto.Test.Images.TestImage.png", typeof(TestIcons)) : null;

		public static Bitmap Textures => Bitmap.FromResource("Eto.Test.Images.Textures.png", typeof(TestIcons));

		public static Bitmap TexturesIndexed => Bitmap.FromResource("Eto.Test.Images.Textures.gif", typeof(TestIcons));

		public static Icon Logo => Icon.FromResource("Eto.Test.Images.Logo.png", typeof(TestIcons));

		public static Bitmap LogoBitmap => Bitmap.FromResource("Eto.Test.Images.Logo.png", typeof(TestIcons));

		public static Icon Logo288 => Icon.FromResource("Eto.Test.Images.LogoWith288DPI.png", typeof(TestIcons));

		public static Bitmap Logo288Bitmap => Bitmap.FromResource("Eto.Test.Images.LogoWith288DPI.png", typeof(TestIcons));

		public static Cursor TestCursor => Cursor.FromResource("Eto.Test.Images.Busy.cur", typeof(TestIcons));
	}
}
