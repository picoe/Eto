using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
		static Assembly Assembly { get { return typeof(TestIcons).Assembly; } } // Don't use GetExecutingAssembly, it is not cross-platform safe.

		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? Assembly.GetName().Name + "."; } }

		public static string TestIconName = "TestIcon.ico";
		public static string TestImageName = "TestImage.png";
		public static string TexturesName = "Textures.png";

		public static Icon TestIcon(Generator generator = null)
		{
			return Icon.FromResource(Assembly, Prefix + TestIconName, generator);
		}

		public static Bitmap TestImage(Generator generator = null)
		{
			return Bitmap.FromResource(Prefix + TestImageName, Assembly, generator: generator);
		}

		public static Bitmap Textures(Generator generator = null)
		{
			return Bitmap.FromResource(Prefix + TexturesName, Assembly, generator: generator);
		}
	}
}
