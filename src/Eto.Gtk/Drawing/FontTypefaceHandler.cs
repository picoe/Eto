using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Eto.GtkSharp.Drawing
{
	public class FontTypefaceHandler : WidgetHandler<Pango.FontFace, FontTypeface>, FontTypeface.IHandler
	{
		public FontTypefaceHandler(Pango.FontFace pangoFace)
		{
			Control = pangoFace;
		}

		public FontTypefaceHandler()
		{
		}

		public string Name => Control?.FaceName;

		public string LocalizedName => Name;

		public FontStyle FontStyle
		{
			get
			{
				var style = FontStyle.None;
				if (Control == null)
					return style;

				var description = Control.Describe();
				if (description.Style == Pango.Style.Italic || description.Style == Pango.Style.Oblique)
					style |= FontStyle.Italic;
				if ((int)description.Weight >= (int)Pango.Weight.Semibold)
					style |= FontStyle.Bold;
				return style;
			}
		}

		public bool IsSymbol => false; // todo, how do we get font info?

		public FontFamily Family { get; private set; }

		static Pango.AttrList noFallbackAttributes;
		static object noFallbackLock = new object();

		public bool HasCharacterRanges(IEnumerable<Range<int>> ranges)
		{
			var desc = Control.Describe();

			if (noFallbackAttributes == null)
			{
				lock (noFallbackLock)
				{
					if (noFallbackAttributes != null)
					{
						noFallbackAttributes = new Pango.AttrList();
						noFallbackAttributes.Change(new Pango.AttrFallback(false));
					}
				}
			}

			using (var layout = new Pango.Layout(FontsHandler.Context))
			{
				layout.FontDescription = desc;
				layout.Attributes = noFallbackAttributes;
				foreach (var range in ranges)
				{
					var text = new string(Enumerable.Range(range.Start, range.Length()).Select(c => (char)c).ToArray());
					layout.SetText(text);
					if (layout.UnknownGlyphsCount > 0)
						return false;
				}
			}
			return true;
		}

		public void Create(Stream stream)
		{
			var familyName = LoadFontFromStream(stream);
			FontsHandler.ResetFontMap();
			CreateFromFamilyName(familyName);
		}

		public unsafe void Create(string fileName)
		{
			var familyName = LoadFontFromFile(fileName);
			FontsHandler.ResetFontMap();
			CreateFromFamilyName(familyName);
		}

		private void CreateFromFamilyName(string familyName)
		{
			var familyHandler = new FontFamilyHandler(familyName, this);
			Family = new FontFamily(familyHandler);
			if (familyHandler.Control == null)
				throw new ArgumentException("Font could not be loaded");
			Control = familyHandler.Control.Faces[0];
		}

		internal static unsafe string LoadFontFromFile(string fileName)
		{
#if GTKCORE
			// note: FontMap is null on Mac currently.  It's likely a bug.
			if (FontsHandler.Context.FontMap?.NativeType.ToString() == "PangoCairoFcFontMap")
			{
				var fcconfig = NativeMethods.FcConfigGetCurrent();

				if (!NativeMethods.FcConfigAppFontAddFile(fcconfig, fileName))
					throw new ArgumentException(nameof(fileName), "Could not add font to fontconfig");

				var fcfontsPtr = NativeMethods.FcConfigGetFonts(fcconfig, NativeMethods.FcSetName.FcSetApplication);
				var fcfonts = Marshal.PtrToStructure<NativeMethods.FcFontSet>(fcfontsPtr);
				IntPtr[] fonts = new IntPtr[fcfonts.nfont];
				Marshal.Copy(fcfonts.fonts, fonts, 0, fcfonts.nfont);

				// we're assuming, but probably correct that the last font added goes to the last entry in the array.
				var fontDescriptionPtr = NativeMethods.pango_fc_font_description_from_pattern(fonts[fonts.Length - 1], false);
				var fontdesc = new Pango.FontDescription(fontDescriptionPtr);
				return fontdesc.Family;

			}
			else if (EtoEnvironment.Platform.IsMac)
			{
				IntPtr provider = NativeMacMethods.CGDataProviderCreateWithFilename(fileName);
				var cgfont = NativeMacMethods.CGFontCreateWithDataProvider(provider);
				NativeMacMethods.CGDataProviderRelease(provider);
				var ctfont = NativeMacMethods.CTFontCreateWithGraphicsFont(cgfont, 10, IntPtr.Zero, IntPtr.Zero);
				var fontFamily = NativeMacMethods.CTFontCopyName(ctfont, NativeMacMethods.CTFontNameKeyFamily);
				NativeMacMethods.CFRelease(ctfont);

				NativeMacMethods.CTFontManagerRegisterGraphicsFont(cgfont, out var error);

				return NativeMacMethods.CFStringToString(fontFamily);
			}
			
			// todo: What do we do on Windows?? Maybe if someone cares enough they can help here..
#endif
			throw new NotSupportedException("This platform does not support loading fonts directly");
		}

		internal static unsafe string LoadFontFromStream(Stream stream)
		{
#if GTKCORE
			// note: FontMap is null on Mac currently.  It's likely a bug.
			if (FontsHandler.Context.FontMap?.NativeType.ToString() == "PangoCairoFcFontMap")
			{
				// need to save to a temp file and use that.
				// https://gitlab.freedesktop.org/fontconfig/fontconfig/-/issues/12
				var tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
				using (var fs = File.Create(tempFileName))
				{
					stream.CopyTo(fs);
				}
				return LoadFontFromFile(tempFileName);
			}
			else if (EtoEnvironment.Platform.IsMac)
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					var buffer = ms.ToArray();
					IntPtr provider;
					fixed (byte* p = &buffer[0])
					{
						provider = NativeMacMethods.CGDataProviderCreateWithData(IntPtr.Zero, (IntPtr)p, new IntPtr(buffer.Length), IntPtr.Zero);
					}
					var cgfont = NativeMacMethods.CGFontCreateWithDataProvider(provider);
					NativeMacMethods.CGDataProviderRelease(provider);
					var ctfont = NativeMacMethods.CTFontCreateWithGraphicsFont(cgfont, 10, IntPtr.Zero, IntPtr.Zero);
					var fontFamily = NativeMacMethods.CTFontCopyName(ctfont, NativeMacMethods.CTFontNameKeyFamily);
					NativeMacMethods.CFRelease(ctfont);

					NativeMacMethods.CTFontManagerRegisterGraphicsFont(cgfont, out var error);

					return NativeMacMethods.CFStringToString(fontFamily);
				}
			}
			// todo: What do we do on Windows?? Maybe if someone cares enough they can help here..
#endif
			throw new NotSupportedException("This platform does not support loading fonts directly");
		}

		public void Create(FontFamily family)
		{
			Family = family;
		}
	}
}

