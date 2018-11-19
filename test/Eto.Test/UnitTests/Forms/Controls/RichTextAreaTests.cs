using System;
using Eto.Forms;
using NUnit.Framework;
using Eto.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class RichTextAreaTests : TextAreaTests<RichTextArea>
	{

		[Test]
		[InvokeOnUI]
		public void CheckSelectionTextCaretAfterSettingRtf()
		{
			// not supported in GTK yet
			if (Platform.Instance.IsGtk)
				Assert.Inconclusive("Gtk does not support RTF format");
			int selectionChanged = 0;
			int textChanged = 0;
			int caretChanged = 0;
			string val;
			var textArea = new RichTextArea();
			textArea.TextChanged += (sender, e) => textChanged++;
			textArea.SelectionChanged += (sender, e) => selectionChanged++;
			textArea.CaretIndexChanged += (sender, e) => caretChanged++;
			Assert.AreEqual(Range.FromLength(0, 0), textArea.Selection, "#1");

			textArea.Rtf = @"{\rtf1\ansi {Hello \ul Underline \i Italic \b Bold \strike Strike}}";
			Assert.AreEqual(val = "Hello Underline Italic Bold Strike", textArea.Text.TrimEnd(), "#2-1");
			Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#2-2");
			Assert.AreEqual(val.Length, textArea.CaretIndex, "#2-3");
			Assert.AreEqual(1, textChanged, "#2-4");
			Assert.AreEqual(1, selectionChanged, "#2-5");
			Assert.AreEqual(1, caretChanged, "#2-6");

			textArea.Selection = Range.FromLength(6, 5);
			Assert.AreEqual(Range.FromLength(6, 5), textArea.Selection, "#3-1");
			Assert.AreEqual(6, textArea.CaretIndex, "#3-2");
			Assert.AreEqual(1, textChanged, "#3-3");
			Assert.AreEqual(2, selectionChanged, "#3-4");
			Assert.AreEqual(2, caretChanged, "#3-5");

			textArea.Rtf = @"{\rtf1\ansi {Some \b other \i text}}";
			Assert.AreEqual(val = "Some other text", textArea.Text.TrimEnd(), "#4-1");
			Assert.AreEqual(Range.FromLength(val.Length, 0), textArea.Selection, "#4-2");
			Assert.AreEqual(val.Length, textArea.CaretIndex, "#4-3");
			Assert.AreEqual(2, textChanged, "#4-4");
			Assert.AreEqual(3, selectionChanged, "#4-5");
			Assert.AreEqual(3, caretChanged, "#4-6");
		}

		public static void TestSelectionAttributes(RichTextArea richText, string tag, bool italic = false, bool underline = false, bool bold = false, bool strikethrough = false)
		{
			Assert.AreEqual(italic, richText.SelectionItalic, tag + "-1");
			Assert.AreEqual(underline, richText.SelectionUnderline, tag + "-2");
			Assert.AreEqual(bold, richText.SelectionBold, tag + "-3");
			Assert.AreEqual(strikethrough, richText.SelectionStrikethrough, tag + "-4");
		}

		[Test]
		[InvokeOnUI]
		public void SelectionAttributesShouldBeCorrectWithLoadedRtf()
		{
			// not supported in GTK yet
			if (Platform.Instance.IsGtk)
				Assert.Inconclusive("Gtk does not support RTF format");

			var richText = new RichTextArea();
			richText.Rtf = @"{\rtf1\ansi {Hello \ul Underline \i Italic \b Bold \strike Strike}}";
			Assert.AreEqual("Hello Underline Italic Bold Strike", richText.Text.TrimEnd(), "#1");
			richText.CaretIndex = 5;
			TestSelectionAttributes(richText, "#2");
			richText.CaretIndex = 7;
			TestSelectionAttributes(richText, "#3", underline: true);
			richText.CaretIndex = 17;
			TestSelectionAttributes(richText, "#4", underline: true, italic: true);
			richText.CaretIndex = 24;
			TestSelectionAttributes(richText, "#5", underline: true, italic: true, bold: true);
			richText.CaretIndex = 29;
			TestSelectionAttributes(richText, "#6", underline: true, italic: true, bold: true, strikethrough: true);
		}

		[Test]
		[InvokeOnUI]
		public void NewLineAtEndShouldNotBeRemoved()
		{
			string val;
			var richText = new RichTextArea();
			var nl = "\n";

			if (!Platform.Instance.IsWpf)
			{
				// why does WPF always add a newline even when the content doesn't have a newline?
				richText.Text = val = $"This is{nl}some text";
				Assert.AreEqual(val, richText.Text, "#1");
			}

			richText.Text = val = $"This is{nl}some text{nl}";
			Assert.AreEqual(val, richText.Text, "#2");
		}

		[Test]
		[InvokeOnUI]
		public void SelectionRangeShouldIncludeNewlines()
		{
			Range<int> range;
			var richText = new RichTextArea();

			var text = "Hello\nThere\nThis is some text";

			richText.Text = text;
			Assert.AreEqual(text, richText.Text.TrimEnd(), "#1");

			richText.Selection = range = GetRange(text, "There");
			Assert.AreEqual("There", richText.SelectedText, "#2.2");
			Assert.AreEqual(range, richText.Selection, "#2.1");

			richText.Selection = range = GetRange(text, "is some text");
			Assert.AreEqual("is some text", richText.SelectedText, "#3.2");
			Assert.AreEqual(range, richText.Selection, "#3.1");
		}

		public class FontVariantInfo
		{
			public string FamilyName { get; set; }
			public string FaceName { get; set; }
			public bool WithBold { get; set; }
			public bool WithItalic { get; set; }
			public string FontNameSuffix { get; set; }

			string _rtfFontName;
			bool _hasSpecificRtfName;
			Lazy<FontFamily> _family;
			Lazy<FontTypeface> _typeface;
			Lazy<FontTypeface> _baseTypeface;
			public FontFamily Family => _family.Value;
			public FontTypeface Typeface => _typeface.Value;
			public FontTypeface BaseTypeface => _baseTypeface.Value;

			public FontVariantInfo()
			{
				_family = new Lazy<FontFamily>(GetFamily);
				_typeface = new Lazy<FontTypeface>(() => GetTypeface(Family, WithBold, WithItalic));
				_baseTypeface = new Lazy<FontTypeface>(() => GetTypeface(Family, false, false));
			}

			public bool IsFound => Family != null && Typeface != null;

			FontFamily GetFamily()
			{
				var family = Fonts.AvailableFontFamilies.FirstOrDefault(r => r.Name == FamilyName);
				if (FaceName != null)
				{
					var face = GetTypeface(family, WithBold, WithItalic);
					if (face == null)
					{
						var fontName = $"{FamilyName} {FaceName}";
						FamilyName = fontName;
						FaceName = null;

						// some faces may have a separate family for the font, instead of grouping it into the same family.
						family = Fonts.AvailableFontFamilies.FirstOrDefault(r => r.Name == fontName);
					}
				}
				return family;
			}

			FontTypeface GetTypeface(FontFamily family, bool bold, bool italic)
			{
				var name = FaceName;
				if (bold)
					name = name == null ? "Bold" : name + " Bold";
				if (italic)
					name = name == null ? "Italic" : name + " Italic";

				if (name == null)
					return family?
						.Typefaces
						.FirstOrDefault(r => IsRegular(r.Name));

				// gar, some fonts have "Regular Italic", some have "Italic", etc.
				var typeface = family?.Typefaces.FirstOrDefault(r => r.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
				if (typeface == null && FaceName == null)
				{
					var thename = "Regular " + name;
					typeface = family?.Typefaces.FirstOrDefault(r => r.Name.Equals(thename, StringComparison.CurrentCultureIgnoreCase));
				}

				if (typeface == null && FaceName == null)
				{
					var thename = "Normal " + name;
					typeface = family?.Typefaces.FirstOrDefault(r => r.Name.Equals(thename, StringComparison.CurrentCultureIgnoreCase));
				}

				return typeface;
			}


			bool IsRegular(string name)
			{
				return (name.Equals("Regular", StringComparison.OrdinalIgnoreCase))
					|| (name.Equals("Normal", StringComparison.OrdinalIgnoreCase))
					|| (name.Equals("None", StringComparison.CurrentCultureIgnoreCase)); // winforms
			}

			bool IsBoldOrItalic(string name)
			{
				return (WithBold && !WithItalic && name.Equals("Bold", StringComparison.OrdinalIgnoreCase))
					|| (WithItalic && !WithBold && (
						name.Equals("Italic", StringComparison.OrdinalIgnoreCase)
						|| name.Equals("Regular Italic", StringComparison.OrdinalIgnoreCase)
						|| name.Equals("Normal Italic", StringComparison.OrdinalIgnoreCase))
						)
					|| (WithBold && WithItalic && name.Equals("Bold Italic", StringComparison.OrdinalIgnoreCase));
			}

			public string RtfFontName
			{
				get
				{
					if (_rtfFontName != null)
						return _rtfFontName;
					var typeface = BaseTypeface;
					if (typeface == null || IsRegular(typeface.Name) || IsBoldOrItalic(typeface.Name))
						return _rtfFontName = $"{Family.Name}{FontNameSuffix}";

					return _rtfFontName = $"{Family.Name} {typeface.Name}{FontNameSuffix}";
				}
				set
				{
					_rtfFontName = value;
					_hasSpecificRtfName = !string.IsNullOrEmpty(value);
				}
			}

			public bool HasSpecificRtfName => _hasSpecificRtfName;

			public string RtfFlags
			{
				get
				{
					var flags = string.Empty;

					if (WithBold)
						flags += @"\b";
					if (WithItalic)
						flags += @"\i";
					return flags;
				}
			}

			public string RegexFontName
			{
				get
				{
					// apparently each platform likes to write RTF differently
					// macOS writes "Klavika-MediumItalic", with \i rtf command
					// windows writes "Klavika Medium", with \i rtf command
					var sb = new StringBuilder();

					var familyName = Family.Name.Replace(" ", "[ ]?");

					// some fonts are written to rtf without spaces or hyphens.
					// e.g. on macOS:
					//   "Arial Narrow" family is written as "ArialNarrow"
					//   "Arial Black" family is written as "Arial-Black"
					//
					// I think it is using the postscript name from the font, perhaps that needs to be exposed in Eto..
					var familyWithFaceName = $"({Regex.Replace(Family.Name, "[ -]", "[ -]?")})";

					void AddEntry(string entry)
					{
						if (sb.Length > 0)
							sb.Append("|");
						sb.Append(entry);
					}

					void AddTypeface(string name)
					{
						var typefaceName = name.Replace(" ", "[ ]?");
						var typefaceNameOption = "";
						// regular/normal we don't write the name
						if (BaseTypeface != null && IsRegular(name))
						{
							typefaceNameOption = "?";
							AddEntry(familyWithFaceName);
						}
						// bold, italic, bold+italic
						if (IsBoldOrItalic(name))
						{
							typefaceNameOption = "?"; // if it is in RTF, it's okay.. but if not, still okay
						}
						AddEntry($"({familyName}([ -]{typefaceName}){typefaceNameOption}{FontNameSuffix})");
					}

					if (HasSpecificRtfName)
						AddEntry(Regex.Escape(RtfFontName));

					if (BaseTypeface != null)
						AddTypeface(BaseTypeface.Name);
					if (Typeface != null)
						AddTypeface(Typeface.Name);
					else
						AddEntry(familyWithFaceName);

					return sb.ToString();
				}
			}


			public override string ToString()
			{
				var sep = FaceName != null ? "-" : "";
				var sb = new StringBuilder();
				sb.Append($"Font: {FamilyName}{sep}{FaceName}, HasBold: {WithBold}, HasItalic: {WithItalic}");

				if (_rtfFontName != null)
					sb.Append($", RtfFontName: {_rtfFontName}");

				return sb.ToString();
			}
		}

		public static IEnumerable<FontVariantInfo> GetFontVariants()
		{
			var arialBaseName = EtoEnvironment.Platform.IsMac ? "MT" : null;

			yield return new FontVariantInfo { FamilyName = "Arial", FontNameSuffix = arialBaseName };
			yield return new FontVariantInfo { FamilyName = "Arial", FontNameSuffix = arialBaseName, WithBold = true };
			yield return new FontVariantInfo { FamilyName = "Arial", FontNameSuffix = arialBaseName, WithItalic = true };
			yield return new FontVariantInfo { FamilyName = "Arial", FontNameSuffix = arialBaseName, WithBold = true, WithItalic = true };

			yield return new FontVariantInfo { FamilyName = "Arial", FaceName = "Narrow" };
			yield return new FontVariantInfo { FamilyName = "Arial", FaceName = "Narrow", WithItalic = true };

			yield return new FontVariantInfo { FamilyName = "Arial", FaceName = "Black" };

			yield return new FontVariantInfo { FamilyName = "BolsterBold", WithBold = true };

			if (EtoEnvironment.Platform.IsMac)
			{
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue" };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", WithBold = true };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", WithItalic = true };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", WithBold = true, WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", RtfFontName = "HelveticaNeue-UltraLight", FaceName = "UltraLight" };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "UltraLight", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Thin" };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Thin", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Light" };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Light", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Medium" };
				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Medium", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Condensed Bold" };

				yield return new FontVariantInfo { FamilyName = "Helvetica Neue", FaceName = "Condensed Black" };
			}

			if (EtoEnvironment.Platform.IsWindows)
			{
				yield return new FontVariantInfo { FamilyName = "Segoe UI" };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", WithBold = true };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", WithItalic = true };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", WithBold = true, WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Black" };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Black", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Light" };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Light", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Semibold" };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Semibold", WithItalic = true };

				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Semilight" };
				yield return new FontVariantInfo { FamilyName = "Segoe UI", FaceName = "Semilight", WithItalic = true };
			}

			yield return new FontVariantInfo { FamilyName = "Klavika", RtfFontName = "Klavika Rg" };
			yield return new FontVariantInfo { FamilyName = "Klavika", WithBold = true, RtfFontName = "Klavika Bd" };
			yield return new FontVariantInfo { FamilyName = "Klavika", WithItalic = true, RtfFontName = "Klavika Rg" };
			yield return new FontVariantInfo { FamilyName = "Klavika", WithBold = true, WithItalic = true, RtfFontName = "Klavika Bd" };

			/*
			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Bold", RtfFontName = "Klavika Bd" };
			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Bold", WithItalic = true, RtfFontName = "Klavika Bd" };
			*/

			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Light", RtfFontName = "Klavika Lt" };
			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Light", WithItalic = true, RtfFontName = "Klavika Lt" };

			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Medium", RtfFontName = "Klavika Md" };
			yield return new FontVariantInfo { FamilyName = "Klavika", FaceName = "Medium", WithItalic = true, RtfFontName = "Klavika Md" };
		}

		/// <summary>
		/// Tests font variants that should be part of the family name in RTF.
		/// </summary>
		/// <remarks>
		/// In WPF, font variants by default are not saved correctly.
		/// This tests to ensure that the font variants are saved properly in the rtf family name.
		/// </remarks>
		[TestCaseSource(nameof(GetFontVariants))]
		[InvokeOnUI]
		public void FontVariantsShouldCorrectlySaveToRtf(FontVariantInfo info)
		{
			if (Platform.Instance.IsGtk)
				Assert.Inconclusive("Gtk does not support RTF format");

			if (!info.IsFound)
				Assert.Inconclusive("Font cannot be found on this system");

			var text = "This is some Font Variant text.";

			var richText = new RichTextArea();
			richText.Text = text;
			Assert.AreEqual(text, richText.Text.TrimEnd(), "#1");

			richText.Selection = GetRange(text, "Font Variant");
			Assert.AreEqual("Font Variant", richText.SelectedText, "#2");

			if (info.BaseTypeface != null)
			{
				// test base typeface (non-bold/italic)
				richText.SelectionTypeface = info.BaseTypeface;
				Assert.AreEqual(info.BaseTypeface.Name, richText.SelectionTypeface.Name, "#3.1");
				Assert.AreEqual(info.BaseTypeface.Name, richText.SelectionFont.Typeface.Name, "#3.2");
			}
			else
			{
				richText.SelectionTypeface = info.Typeface;
			}
			Assert.AreEqual(info.Family.Name, richText.SelectionFamily.Name, "#3.3");
			Assert.AreEqual(info.Family.Name, richText.SelectionFont.FamilyName, "#3.4");

			// setting these should not affect font name in RTF as it uses \b and \i to specify that
			if (info.WithBold)
				richText.SelectionBold = true;

			if (info.WithItalic)
				richText.SelectionItalic = true;

			// test it is using the right typeface
			Assert.AreEqual(info.Typeface.Name, richText.SelectionTypeface.Name, "#4.1");
			Assert.AreEqual(info.Typeface.Name, richText.SelectionFont.Typeface.Name, "#4.2");

			// ensure the generated RTF contains the correct font variant name
			var rtf = richText.Rtf;
			Console.WriteLine($"Generated RTF:");
			Console.WriteLine(rtf);
			var reg = $@"(?<={{\\fonttbl.*)\\f\d+[^}};]* ({info.RegexFontName});";
			Assert.IsTrue(Regex.IsMatch(rtf, reg), $"#5 - Variant '{info}' does not exist in RTF:\n{rtf}");
		}

		/// <summary>
		/// Tests font variants that should be part of the family name in RTF.
		/// </summary>
		/// <remarks>
		/// In WPF, font variants by default are not saved correctly.
		/// This tests to ensure that the font variants are loaded properly when specified in the rtf family name.
		/// </remarks>
		[TestCaseSource(nameof(GetFontVariants))]
		[InvokeOnUI]
		public void FontVariantsShouldCorrectlyLoadFromRtf(FontVariantInfo info)
		{
			if (Platform.Instance.IsGtk)
				Assert.Inconclusive("Gtk does not support RTF format");

			if (!info.IsFound)
				Assert.Inconclusive("Font cannot be found on this system");


			var text = "This is some Font Variant text.";
			var rtf = @"{\rtf1\ansi
{\fonttbl\f0\fswiss\fcharset0 Arial;\f1\fswiss\fcharset0 " + info.RtfFontName + @";}
{\f0\fs24 \cf0 This is some 
\f1" + info.RtfFlags + @" Font Variant
\f0\b0  text.}}";

			Console.WriteLine("Loading rtf");
			Console.WriteLine(rtf);
			var richText = new RichTextArea();
			richText.Rtf = rtf;

			Assert.AreEqual(text, richText.Text.TrimEnd(), "#1");

			// select Font Variant text and ensure it is correctly set
			richText.Selection = GetRange(text, "Font Variant");
			Assert.AreEqual("Font Variant", richText.SelectedText, "#2");

			Assert.AreEqual(info.Family.Name, richText.SelectionFamily.Name, "#3.1");
			Assert.AreEqual(info.Family.Name, richText.SelectionFont.FamilyName, "#3.2");
			Assert.AreEqual(info.Typeface.Name, richText.SelectionTypeface.Name, "#3.3");
			Assert.AreEqual(info.Typeface.Name, richText.SelectionFont.Typeface.Name, "#3.4");
		}

		[Test]
		[InvokeOnUI]
		public void SelectionBoldItalicUnderlineShouldTriggerTextChanged()
		{
			int textChangedCount = 0;
			var richText = new RichTextArea();
			richText.TextChanged += (sender, e) => textChangedCount++;

			string text = "This is some underline, strikethrough, bold, and italic text. This is green, background blue text.";

			richText.Text = text;
			Assert.AreEqual(1, textChangedCount);

			richText.Selection = GetRange(text, "underline");
			richText.SelectionUnderline = true;
			Assert.AreEqual(2, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionUnderline");
			Assert.AreEqual(true, richText.SelectionUnderline);
			Assert.AreEqual(false, richText.SelectionStrikethrough);
			Assert.AreEqual(false, richText.SelectionBold);
			Assert.AreEqual(false, richText.SelectionItalic);

			richText.Selection = GetRange(text, "strikethrough");
			richText.SelectionStrikethrough = true;
			Assert.AreEqual(3, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionStrikethrough");
			Assert.AreEqual(false, richText.SelectionUnderline);
			Assert.AreEqual(true, richText.SelectionStrikethrough);
			Assert.AreEqual(false, richText.SelectionBold);
			Assert.AreEqual(false, richText.SelectionItalic);

			richText.Selection = GetRange(text, "bold");
			richText.SelectionBold = true;
			Assert.AreEqual(4, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionBold");
			Assert.AreEqual(false, richText.SelectionUnderline);
			Assert.AreEqual(false, richText.SelectionStrikethrough);
			Assert.AreEqual(true, richText.SelectionBold);
			Assert.AreEqual(false, richText.SelectionItalic);

			richText.Selection = GetRange(text, "italic");
			richText.SelectionItalic = true;
			Assert.AreEqual(5, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionItalic");
			Assert.AreEqual(false, richText.SelectionUnderline);
			Assert.AreEqual(false, richText.SelectionStrikethrough);
			Assert.AreEqual(false, richText.SelectionBold);
			Assert.AreEqual(true, richText.SelectionItalic);

			richText.Selection = GetRange(text, "green");
			richText.SelectionForeground = Colors.Green;
			Assert.AreEqual(6, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionForeground");
			Assert.AreEqual(Colors.Green, richText.SelectionForeground);

			richText.Selection = GetRange(text, "green");
			richText.SelectionBackground = Colors.Blue;
			Assert.AreEqual(7, textChangedCount, "RichTextArea.TextChanged did not fire when setting SelectionBackground");
			Assert.AreEqual(Colors.Blue, richText.SelectionBackground);
		}

		[TestCase(true)]
		[TestCase(false)]
		[InvokeOnUI]
		public void PlainTextShouldInheritBaseFont(bool withFont)
		{
			var richText = new RichTextArea();
			float expectedFontSize;
			if (withFont)
			{
				expectedFontSize = 24;
				richText.Font = Fonts.Sans(expectedFontSize);
			}
			else
				expectedFontSize = richText.Font.Size;
			var text = "Hello then";
			var textBuffer = Encoding.UTF8.GetBytes(text);
			var ms = new MemoryStream(textBuffer);
			richText.Buffer.Load(ms, RichTextAreaFormat.PlainText);


			richText.Selection = GetRange(text, "Hello");
			Assert.AreEqual(expectedFontSize, richText.SelectionFont.Size);
		}

		static Range<int> GetRange(string text, string s) => Range.FromLength(text.IndexOf(s, StringComparison.Ordinal), s.Length);

		[Test]
		[InvokeOnUI]
		public void ItalicTypefaceShouldApply()
		{
			var richText = new RichTextArea();

			var text = "Some Text";
			var rtf = @"{\rtf1\deff0{\fonttbl{\f0 Arial;}{\f1 Arial Black;}}\fs40 {\f1\i Some Text}\par}";
			richText.Rtf = rtf;
			richText.Selection = GetRange(text, "Text");
			Assert.IsTrue(richText.SelectionItalic, "#1");
			Assert.AreEqual("Arial", richText.SelectionFamily.Name, "#2");
			Assert.AreEqual("Black Oblique", richText.SelectionTypeface.Name, "#3");
		}
	}
}
