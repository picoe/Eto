namespace Eto.Mac.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<FontTypeface>, FontTypeface.IHandler
	{
		NSFont _font;
		CGFont _cgfont;
		string _name;
		NSFontTraitMask? _traits;
		int? _weight;
		static readonly object LocalizedName_Key = new object();

		public NSFont Font => _font ?? (_font = CreateFont(10));

		public string PostScriptName { get; private set; }

		public int Weight
		{
			get => _weight ?? (_weight = (int)NSFontManager.SharedFontManager.WeightOfFont(Font)).Value;
			private set => _weight = value;
		}

		public NSFontTraitMask Traits
		{
			get => _traits ?? (_traits = NSFontManager.SharedFontManager.TraitsOfFont(Font)).Value;
			private set => _traits = value;
		}

		public FontTypefaceHandler(NSArray descriptor)
		{
			PostScriptName = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(0));
			_name = (string)Messaging.GetNSObject<NSString>(descriptor.ValueAt(1));
			Weight = Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(2))?.Int32Value ?? 1;
			Traits = (NSFontTraitMask)(Messaging.GetNSObject<NSNumber>(descriptor.ValueAt(3))?.Int32Value ?? 0);
		}

		public FontTypefaceHandler(NSFont font, NSFontTraitMask? traits = null)
		{
			_font = font;
			var descriptor = font.FontDescriptor;
			PostScriptName = descriptor.PostscriptName;
		}

		public FontTypefaceHandler(string postScriptName, string name, NSFontTraitMask traits, int weight)
		{
			PostScriptName = postScriptName;
			_name = name;
			Weight = weight;
			Traits = traits;
		}

		public FontTypefaceHandler()
		{
		}

		public FontTypefaceHandler(CGFont cgfont, string faceName)
		{
			_cgfont = cgfont;
			_name = faceName;
		}

		internal static NSString GetName(IntPtr fontHandle)
		{
			var namePtr = CTFontCopyName(fontHandle, CTFontNameKeySubFamily.Handle);
			return Runtime.GetNSObject<NSString>(namePtr);
		}

		public string Name
		{
			get
			{
				if (_name == null)
				{
					_name = GetName(Font.Handle);

					/*
					var manager = NSFontManager.SharedFontManager;
					// no attribute, find font face based on postscript name
					var members = manager.AvailableMembersOfFontFamily(PostScriptName);
					var member = members.FirstOrDefault(r => (string)Runtime.GetNSObject<NSString>(r.ValueAt(0)) == PostScriptName);
					if (member != null)
					{
						name = (string)Runtime.GetNSObject<NSString>(member.ValueAt(1));
					}*/
				}
				return _name;
			}
		}

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		internal static extern IntPtr CTFontCopyName(IntPtr font, IntPtr nameKey);

		[DllImport("/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText")]
		static extern IntPtr CTFontCopyLocalizedName(IntPtr font, IntPtr nameKey, out IntPtr actualLanguage);

		static NSString CTFontNameKeySubFamily = Dlfcn.GetStringConstant(Messaging.CoreTextHandle, "kCTFontSubFamilyNameKey");


		public string LocalizedName => Widget.Properties.Create<string>(LocalizedName_Key, GetLocalizedName);

		string GetLocalizedName()
		{
			// no (easy) way to get a CTFont from an NSFont
			var localizedNamePtr = CTFontCopyLocalizedName(Font.Handle, CTFontNameKeySubFamily.Handle, out var actualLanguagePtr);
			var actualLanguage = Runtime.GetNSObject<NSString>(actualLanguagePtr);
			var localizedName = Runtime.GetNSObject<NSString>(localizedNamePtr);

			return localizedName;
		}

		public FontStyle FontStyle => Traits.ToEto();

		static string SystemFontName => NSFont.SystemFontOfSize(NSFont.SystemFontSize).FontDescriptor.PostscriptName;
		static string BoldSystemFontName => NSFont.BoldSystemFontOfSize(NSFont.SystemFontSize).FontDescriptor.PostscriptName;

		public bool IsSymbol => Font.FontDescriptor.SymbolicTraits.HasFlag(NSFontSymbolicTraits.SymbolicClass);

		public FontFamily Family { get; private set; }

		public NSFont CreateFont(float size)
		{
			if (_cgfont != null)
			{
				var ctfont = new CTFont(_cgfont, size, null);
				return Runtime.GetNSObject<NSFont>(ctfont.Handle);
			}
			// we have a postcript name, use that to create the font
			if (!string.IsNullOrEmpty(PostScriptName))
			{
				// if we try to get a system font by name we get errors now..
				if (PostScriptName == SystemFontName)
					return NSFont.SystemFontOfSize(size);
				if (PostScriptName == BoldSystemFontName)
					return NSFont.BoldSystemFontOfSize(size);

				// always return something...
				var font = NSFont.FromFontName(PostScriptName, size) ?? NSFont.UserFontOfSize(size);
				return font;
			}

			var family = (FontFamilyHandler)Widget.Family.Handler;
			return family.CreateFont(size, Traits, Weight);
		}

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			var charSet = Font.CoveredCharacterSet;

			foreach (var range in ranges)
			{
				for (int i = range.Start; i <= range.End; i++)
				{
					if (!charSet.Contains((char)i))
						return false;
				}
			}
			return true;
		}

		public void Create(Stream stream)
		{
			
			using (var ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				var bytes = ms.ToArray();
				using (var dataProvider = new CGDataProvider(bytes, 0, bytes.Length))
				{
					_cgfont = CGFont.CreateFromProvider(dataProvider);
				}
			}
			Family = new FontFamily(new FontFamilyHandler(_cgfont, this));
		}

		public void Create(string fileName)
		{
			using (var dataProvider = new CGDataProvider(fileName))
			{
				_cgfont = CGFont.CreateFromProvider(dataProvider);
			}
			Family = new FontFamily( new FontFamilyHandler(_cgfont, this));
		}

		public void Create(FontFamily family)
		{
			Family = family;
		}
	}
}

