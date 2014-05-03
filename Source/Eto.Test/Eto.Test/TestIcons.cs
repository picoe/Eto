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

		public static Icon TestIcon
		{
			get { return Icon.FromResource(GetTranslatedResourceName(Prefix + TestIconName), Assembly); }
		}

		public static Bitmap TestImage
		{
			get { return Bitmap.FromResource(GetTranslatedResourceName(Prefix + TestImageName), Assembly); }
		}

		public static Bitmap Textures
		{
			get { return Bitmap.FromResource(GetTranslatedResourceName(Prefix + TexturesName), Assembly); }
		}
	}
}
