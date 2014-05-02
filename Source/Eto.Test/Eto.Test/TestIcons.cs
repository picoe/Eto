using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? Assembly.GetExecutingAssembly().GetName().Name + "."; } }

		public static string TestIconName = "TestIcon.ico";
		public static string TestImageName = "TestImage.png";
		public static string TexturesName = "Textures.png";

		public static Icon TestIcon(Generator generator = null)
		{
			return Icon.FromResource(Prefix + TestIconName, generator);
		}

		public static Bitmap TestImage(Generator generator = null)
		{
			return Bitmap.FromResource(Prefix + TestImageName, generator: generator);
		}

		public static Bitmap Textures(Generator generator = null)
		{
			return Bitmap.FromResource(Prefix + TexturesName, generator: generator);
		}
	}
}
