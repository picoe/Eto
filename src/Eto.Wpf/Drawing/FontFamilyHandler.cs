using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using swm = System.Windows.Media;
using sw = System.Windows;
using swd = System.Windows.Documents;
using System.Diagnostics;
using System.Globalization;
using Eto.Forms;
using System.IO;
using System;
using Eto.Shared.Drawing;
using Eto.Wpf.CustomControls.FontDialog;

namespace Eto.Wpf.Drawing
{

	public class FontFamilyHandler : WidgetHandler<swm.FontFamily, FontFamily>, FontFamily.IHandler
	{
		FontTypeface[] _typefaces;

		~FontFamilyHandler()
		{
			Dispose(false);
		}
		
		public FontFamilyHandler()
		{
		}

		public FontFamilyHandler(swm.FontFamily wpfFamily)
		{
			Control = wpfFamily;
			var familyMapName = NameDictionaryExtensions.GetEnglishName(Control.FamilyNames);
			Name = familyMapName ?? Control.Source;
		}

		internal void SetTypefaces(FontTypeface[] typefaces) => _typefaces = typefaces;

		public FontFamilyHandler(swd.TextSelection range, sw.Controls.RichTextBox control)
		{
			Control = range.GetPropertyValue(swd.TextElement.FontFamilyProperty) as swm.FontFamily ?? swd.TextElement.GetFontFamily(control);
			var familyMapName = NameDictionaryExtensions.GetEnglishName(Control.FamilyNames);
			Name = familyMapName ?? Control.Source;
		}

		public void Create(string familyName)
		{
			Name = familyName;
			switch (familyName.ToUpperInvariant())
			{
				case FontFamilies.MonospaceFamilyName:
					familyName = "Courier New";
					break;
				case FontFamilies.SansFamilyName:
					familyName = "Tahoma, Arial, Verdana, Trebuchet, MS Sans Serif, Helvetica";
					break;
				case FontFamilies.SerifFamilyName:
					familyName = "Times New Roman";
					break;
				case FontFamilies.CursiveFamilyName:
					familyName = "Comic Sans MS, Monotype Corsiva, Papryus";
					break;
				case FontFamilies.FantasyFamilyName:
					familyName = "Impact, Juice ITC";
					break;
			}
			Control = new swm.FontFamily(familyName);
		}

		public string Name { get; set; }


		static readonly object LocalizedName_Key = new object();

		public string LocalizedName
		{
			get
			{
				if (Widget.Properties.ContainsKey(LocalizedName_Key))
					return Widget.Properties.Get<string>(LocalizedName_Key);

				var localizedName = CustomControls.FontDialog.NameDictionaryExtensions.GetDisplayName(Control.FamilyNames);
				Widget.Properties.Set(LocalizedName_Key, localizedName);
				return localizedName;
			}
		}

		public IEnumerable<FontTypeface> Typefaces => _typefaces ?? (_typefaces = GetTypefaces().ToArray());
		
		IEnumerable<FontTypeface> GetTypefaces()
		{
			foreach (var type in Control.GetTypefaces())
			{
				if (!FontHandler.ShowSimulatedFonts && (type.IsBoldSimulated || type.IsObliqueSimulated))
					continue;
				yield return new FontTypeface(Widget, new FontTypefaceHandler(type));
			}
		}

		public void Apply(sw.Documents.TextRange control)
		{
			control.ApplyPropertyValue(swd.TextElement.FontFamilyProperty, Control);
		}


		public void CreateFromFiles(IEnumerable<string> fileNames)
		{
			// add to private font collection
			string fontPath = null;

			bool useFallback = false;
			foreach (var fileName in fileNames)
			{
				var currentPath = Path.GetDirectoryName(fileName);
				if (fontPath == null)
					fontPath = currentPath;
				else if (fontPath != currentPath)
					throw new InvalidOperationException("All fonts in the family must be in the same directory.");

				var fontInfos = OpenTypeFontInfo.FromFile(fileName);
				foreach (var fontInfo in fontInfos)
				{
					if (fontInfo == null)
					{
						// for some reason can't read info from this file.. fallback to dumb way.
						useFallback = true;
						Name = null;
					}
					if (!useFallback)
					{
						var currentName = fontInfo.TypographicFamilyName ?? fontInfo.FamilyName;
						if (Name == null)
							Name = currentName;
						else if (Name != currentName)
							throw new InvalidOperationException($"Family name of the supplied font files do not match. '{Name}' and '{currentName}'");
					}
				}
			}
			
			if (useFallback)
			{
				// do we need this??
				// get the font family name using System.Drawing

				var fontCollection = new System.Drawing.Text.PrivateFontCollection();
				foreach (var fileName in fileNames)
				{
					fontCollection.AddFontFile(fileName);
				}

				var families = fontCollection.Families;
				var shortest = families.OrderBy(r => r.Name.Length).First();
				if (!families.All(r => r.Name.StartsWith(shortest.Name)))
				{
					const string RegularSuffix = " Regular";
					var regular = families.Where(r => r.Name.EndsWith(RegularSuffix)).FirstOrDefault();
					if (regular != null)
					{
						var familyName = regular.Name.Substring(regular.Name.Length - RegularSuffix.Length);
						if (!families.All(r => r.Name.StartsWith(familyName)))
						{
							throw new ArgumentException("Fonts must all be in the same family");
						}
						Name = familyName;
					}
				}
				else
					Name = shortest.Name;
			}
			var name = $"file:///{fontPath.Replace("\\", "/")}/#{Name}";
			Control = new swm.FontFamily(name);
		}

		string fontTempDirectory;

		public void CreateFromStreams(IEnumerable<Stream> streams)
		{
			fontTempDirectory = FontTypefaceHandler.CreateTempDirectoryForFonts();
			var fileNames = new List<string>();
			foreach (var stream in streams)
			{
				// assume everything is an otf so WPF picks it up.. We should detect the format somehow. :/
				var fileName = Path.Combine(fontTempDirectory, Guid.NewGuid().ToString() + ".otf");
				fileNames.Add(fileName);
				using (var fs = File.Create(fileName))
				{
					stream.CopyTo(fs);
				}
			}
			CreateFromFiles(fileNames);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (fontTempDirectory != null && Directory.Exists(fontTempDirectory))
			{
				try
				{
					Directory.Delete(fontTempDirectory, true);
				}
				catch
				{
				}
				fontTempDirectory = null;
			}
		}
	}
}
