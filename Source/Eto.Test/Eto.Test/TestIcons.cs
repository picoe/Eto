using System;
using System.Reflection;
using Eto.Drawing;

namespace Eto.Test
{
	public static class TestIcons
	{
#if PCL
		static Assembly Assembly { get { return typeof(TestIcons).GetTypeInfo().Assembly; } } // Don't use GetExecutingAssembly, it is not cross-platform safe.
#else
		static Assembly Assembly { get { return typeof(TestIcons).Assembly; } } // Don't use GetExecutingAssembly, it is not cross-platform safe.
#endif
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? Assembly.GetName().Name + "."; } }


		public static string TestIconName = "TestIcon.ico";
		public static string TestImageName = "TestImage.png";
		public static string TexturesName = "Textures.png";

		/// <summary>
		/// An app can set this to translate resource names if they are linking them in.
		/// </summary>
		public static Func<string, string> TranslateResourceName { get; set; }

		static string GetTranslatedResourceName(string s)
		{
			return TranslateResourceName != null ? TranslateResourceName(s) : s;
		}

		public static Icon TestIcon(Generator generator = null)
		{
			return Icon.FromResource(Assembly, GetTranslatedResourceName(Prefix + TestIconName), generator);
		}

		public static Bitmap TestImage(Generator generator = null)
		{
			return Bitmap.FromResource(GetTranslatedResourceName(Prefix + TestImageName), Assembly, generator: generator);
		}

		public static Bitmap Textures(Generator generator = null)
		{
			return Bitmap.FromResource(GetTranslatedResourceName(Prefix + TexturesName), Assembly, generator: generator);
		}
	}
}
