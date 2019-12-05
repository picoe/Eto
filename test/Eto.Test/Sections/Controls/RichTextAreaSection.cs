using System;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
using System.Text;
using System.Linq;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(RichTextArea))]
	public class RichTextAreaSection : Panel
	{
		public static string LoremText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

		public static string RtfString = "{\\rtf1\\ansi\\ansicpg1252\\cocoartf1343\\cocoasubrtf160\r\n{\\fonttbl\\f0\\fswiss\\fcharset0 Helvetica;}\r\n{\\colortbl;\\red255\\green255\\blue255;}\r\n\\margl1440\\margr1440\\vieww10800\\viewh8400\\viewkind0\r\n\\pard\\tx566\\tx1133\\tx1700\\tx2267\\tx2834\\tx3401\\tx3968\\tx4535\\tx5102\\tx5669\\tx6236\\tx6803\\pardirnatural\r\n\r\n\\f0\\fs24 \\cf0 This is some \r\n\\b bold\r\n\\b0 , \r\n\\i italic\r\n\\i0 , and \\ul underline\\ulnone  text! \\\r\n\\\r\n\\pard\\tx566\\tx1133\\tx1700\\tx2267\\tx2834\\tx3401\\tx3968\\tx4535\\tx5102\\tx5669\\tx6236\\tx6803\\pardirnatural\\qr\r\n\\cf0 Some other text}";

		public RichTextAreaSection()
		{
			var richText = new RichTextArea();
			richText.Size = new Size(-1, 300);
			//richText.Font = new Font("Arial", 10);

			var buffer = richText.Buffer;

			/**/
			richText.Text = LoremText;

			var range = new Range<int>(6, 10);
			buffer.SetFont(range, Fonts.Cursive(20, FontStyle.Bold, FontDecoration.Underline));
			buffer.SetForeground(range, Colors.Blue);
			buffer.SetBackground(range, Colors.Yellow);

			buffer.SetBold(new Range<int>(11, 16), true);
			buffer.SetItalic(new Range<int>(18, 20), true);
			buffer.SetUnderline(new Range<int>(22, 25), true);
			buffer.SetStrikethrough(new Range<int>(28, 38), true);

			richText.CaretIndex = LoremText.Length - 1;
			/**/


			richText.SelectionChanged += (sender, e) =>
			{
				UpdateBindings(BindingUpdateMode.Destination);
				Log.Write(sender, "SelectionChanged: {0}", richText.Selection);
			};

			var boldButton = new CheckBox { Text = "Bold" };
			boldButton.CheckedBinding.Bind(richText, r => r.SelectionBold);
			boldButton.CheckedChanged += (sender, e) =>
			{
				richText.Focus();
				UpdateBindings(BindingUpdateMode.Destination);
			};

			var italicButton = new CheckBox { Text = "Italic" };
			italicButton.CheckedBinding.Bind(richText, r => r.SelectionItalic);
			italicButton.CheckedChanged += (sender, e) =>
			{
				richText.Focus();
				UpdateBindings(BindingUpdateMode.Destination);
			};

			var underlineButton = new CheckBox { Text = "Underline" };
			underlineButton.CheckedBinding.Bind(richText, r => r.SelectionUnderline);
			underlineButton.CheckedChanged += (sender, e) => richText.Focus();

			var strikethroughButton = new CheckBox { Text = "Strikethrough" };
			strikethroughButton.CheckedBinding.Bind(richText, r => r.SelectionStrikethrough);
			strikethroughButton.CheckedChanged += (sender, e) => richText.Focus();

			var backgroundButton = new ColorPicker { };
			backgroundButton.ValueBinding.Bind(() => richText.SelectionBackground, val => richText.SelectionBackground = val, h => Binding.AddPropertyEvent(richText, r => r.SelectionBackground, h), h => Binding.RemovePropertyEvent(richText, h));
			backgroundButton.ValueChanged += (sender, e) => richText.Focus();

			var foregroundButton = new ColorPicker { };
			foregroundButton.ValueBinding.Bind(richText, r => r.SelectionForeground);
			foregroundButton.ValueChanged += (sender, e) => richText.Focus();

			var fontButton = new FontPicker();
			fontButton.ValueBinding.Bind(richText, r => r.SelectionFont);
			fontButton.ValueChanged += (sender, e) =>
			{
				richText.Focus();
				UpdateBindings(BindingUpdateMode.Destination);
			};

			var typefaceDropDown = new DropDown();
			typefaceDropDown.ItemKeyBinding = Binding.Property((FontTypeface f) => f.Name);
			typefaceDropDown.DataStore = richText.SelectionFamily.Typefaces;
			var tyepfaceBinding = typefaceDropDown.SelectedValueBinding.Bind(richText, r => r.SelectionTypeface);
			typefaceDropDown.SelectedValueChanged += (sender, e) =>
			{
				richText.Focus();
				UpdateBindings(BindingUpdateMode.Destination);
			};

			var familyDropDown = new DropDown();
			familyDropDown.ItemTextBinding = Binding.Property((FontFamily f) => f.LocalizedName);
			familyDropDown.DataStore = Fonts.AvailableFontFamilies.OrderBy(r => r.LocalizedName);
			familyDropDown.SelectedValueBinding.Bind(richText, r => r.SelectionFamily);
			familyDropDown.SelectedValueChanged += (sender, e) =>
			{
				var family = familyDropDown.SelectedValue as FontFamily;
				//tyepfaceBinding.Mode = DualBindingMode.Manual;
				typefaceDropDown.DataStore = family?.Typefaces;
				//tyepfaceBinding.Mode = DualBindingMode.TwoWay;
				richText.Focus();
				UpdateBindings(BindingUpdateMode.Destination);
			};

			var formatEnum = new EnumDropDown<RichTextAreaFormat>();
			formatEnum.SelectedValue = RichTextAreaFormat.Rtf;

			var loadButton = new Button { Text = "Load" };
			loadButton.Enabled = buffer.SupportedFormats.Contains(formatEnum.SelectedValue);
			loadButton.Click += (sender, e) => buffer.Load(new MemoryStream(Encoding.UTF8.GetBytes(formatEnum.SelectedValue == RichTextAreaFormat.Rtf ? RtfString : LoremText)), formatEnum.SelectedValue);

			var loadFileButton = new Button { Text = "Load File..." };
			loadFileButton.Enabled = buffer.SupportedFormats.Contains(formatEnum.SelectedValue);
			loadFileButton.Click += (sender, e) =>
			{
				var dlg = new OpenFileDialog { Filters = { new FileFilter("RTF", ".rtf"), new FileFilter("TXT", "txt") } };
				if (dlg.ShowDialog(this) == DialogResult.Ok)
				{
					var useRtf = string.Equals(Path.GetExtension(dlg.FileName), ".rtf", StringComparison.OrdinalIgnoreCase);
					var fs = File.OpenRead(dlg.FileName);
					buffer.Load(fs, useRtf ? RichTextAreaFormat.Rtf : RichTextAreaFormat.PlainText);
				}
			};

			var saveButton = new Button { Text = "Save" };
			saveButton.Enabled = buffer.SupportedFormats.Contains(formatEnum.SelectedValue);
			saveButton.Click += (sender, e) =>
			{
				var stream = new MemoryStream();
				buffer.Save(stream, formatEnum.SelectedValue);
				stream.Position = 0;
				if (formatEnum.SelectedValue == RichTextAreaFormat.Rtf)
					RtfString = Encoding.UTF8.GetString(stream.ToArray());
				else
					LoremText = Encoding.UTF8.GetString(stream.ToArray());
				Log.Write(richText, "Saved {0}:\n{1}", formatEnum.SelectedValue, new StreamReader(stream).ReadToEnd());
			};

			formatEnum.SelectedValueChanged += (sender, e) => saveButton.Enabled = loadButton.Enabled = buffer.SupportedFormats.Contains(formatEnum.SelectedValue);

			var clearButton = new Button { Text = "Clear" };
			clearButton.Click += (sender, e) => buffer.Clear();

			var formatting1 = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
				    null,
				    boldButton,
				    italicButton,
				    underlineButton,
				    strikethroughButton,
				    null
				}
			};

			var formatting2 = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
					null,
					new Label { Text = "Foreground", VerticalAlignment = VerticalAlignment.Center },
					TableLayout.AutoSized(foregroundButton, centered: true),
					new Label { Text = "Background", VerticalAlignment = VerticalAlignment.Center },
					TableLayout.AutoSized(backgroundButton, centered: true),
					null
				}
			};
			var formatting3 = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
				    null,
				    fontButton,
				    familyDropDown,
					typefaceDropDown,
				    null
				}
			};

			var buttons = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Spacing = 5,
				Items =
				{
				    null,
				    formatEnum,
				    loadButton,
				    saveButton,
					loadFileButton,
					clearButton,
				    null
				}
			};

			Content = new StackLayout
			{
				Padding = new Padding(10),
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Items =
				{
					buttons,
					TextAreaSection.TextAreaOptions(richText),
					TextAreaSection.TextAreaOptions2(richText),
					TextAreaSection.TextAreaOptions3(richText),
					formatting1,
					formatting2,
					formatting3,
					new StackLayoutItem(richText, expand: true)
				}
			};
		}
	}
}

