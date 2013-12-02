using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		static string prefix;
		static string Prefix { get { return prefix = prefix ?? Assembly.GetExecutingAssembly().GetName().Name + "."; } }

#if IOS
		// Resources on iOS are stored without the extension
		public static string TestIconName = "TestIcon"; 
		public static string TestImageName = "TestImage";
		public static string TexturesName = "Textures";
#else
		public static string TestIconName = "TestIcon.ico"; 
		public static string TestImageName = "TestImage.png";
		public static string TexturesName = "Textures.png";
#endif
		static Icon testIcon;
		public static Icon TestIcon { get { return testIcon = testIcon ?? Icon.FromResource(Prefix + TestIconName); } }

		static Bitmap testImage;
		public static Bitmap TestImage { get { return testImage = testImage ?? Bitmap.FromResource(Prefix + TestImageName); } }

		static Bitmap textures;
		public static Bitmap Textures { get { return textures = textures ?? Bitmap.FromResource(Prefix + TexturesName); } }
	}
}
