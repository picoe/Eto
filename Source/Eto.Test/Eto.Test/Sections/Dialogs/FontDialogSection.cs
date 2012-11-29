using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Test.Sections.Dialogs
{
	public class FontDialogSection : Scrollable
	{
		Font selectedFont;
		TextArea preview;
		ListBox fontList;
		ListBox fontStyles;
		ListBox fontSizes;
		bool updating;

		public FontDialogSection ()
		{
			var layout = new DynamicLayout (this, new Size (5, 5));
			layout.BeginVertical ();
			layout.AddRow (null, PickFont (), null);
			layout.AddRow (null, PickFontWithStartingFont (), null);
			layout.AddRow (null, SetToFontFamily (), null);

			layout.EndVertical ();

			layout.AddSeparateRow (null, FontList (), FontStyles (), FontSizes (), null);
			layout.AddSeparateRow (null, new Label { Text = "Style:"}, BoldFont (), ItalicFont (), null);
			layout.Add (Preview (), yscale: true);
			UpdatePreview (new Font(FontFamilies.Serif, 18, FontStyle.Bold));
		}

		Control PickFont ()
		{
			var button = new Button { Text = "Pick Font" };
			button.Click += delegate {
				var dialog = new FontDialog ();
				dialog.FontChanged += delegate {
					// you need to handle this event for OS X, where the dialog is a floating window
					UpdatePreview (dialog.Font);
					Log.Write (dialog, "FontChanged, Font: {0}", dialog.Font);
				};
				var result = dialog.ShowDialog (this.ParentWindow);
				Log.Write (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control PickFontWithStartingFont ()
		{
			var button = new Button { Text = "Pick Font with initial starting font" };
			button.Click += delegate {
				var dialog = new FontDialog {
					Font = selectedFont
				};
				dialog.FontChanged += delegate {
					// need to handle this event for OS X, where the dialog is a floating window
					UpdatePreview (dialog.Font);
					Log.Write (dialog, "FontChanged, Font: {0}", dialog.Font);
				};
				var result = dialog.ShowDialog (this.ParentWindow);
				// do not get the font here, it may return immediately with a result of DialogResult.None on certain platforms
				Log.Write (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control SetToFontFamily ()
		{
			var button = new Button { Text = "Set to a specific font family (Times New Roman 20pt)" };
			button.Click += delegate {
				var family = new FontFamily ("Times New Roman");
				var font = new Font (family, 20);
				UpdatePreview (font);
			};
			return button;
		}

		Control FontList ()
		{
			fontList = new ListBox { Size = new Size (300, 180) };
			var lookup = Fonts.AvailableFontFamilies ().ToDictionary (r => r.Name);
			fontList.Items.AddRange (lookup.Values.OrderBy (r => r.Name).Select (r => new ListItem { Text = r.Name, Key = r.Name }).OfType<IListItem>());
			fontList.SelectedIndexChanged += (sender, e) => {
				if (updating)
					return;
				var family = lookup[fontList.SelectedKey];
				UpdatePreview (new Font (family.Typefaces.First (), selectedFont.SizeInPoints));
			};

			return fontList;
		}

		Control FontStyles ()
		{
			fontStyles = new ListBox { Size = new Size (150, 100) };
			fontStyles.SelectedIndexChanged += (sender, e) => {
				if (updating)
					return;
				var face = selectedFont.Family.Typefaces.FirstOrDefault (r => r.Name == fontStyles.SelectedKey);
				if (face != null) {
					UpdatePreview (new Font (face, selectedFont.SizeInPoints));
				}
			};
			return fontStyles;
		}

		Control FontSizes ()
		{
			fontSizes = new ListBox { Size = new Size (60, 100) };
			for (int i = 6; i < 72; i++) {
				fontSizes.Items.Add (i.ToString (), i.ToString ());
			}
			fontSizes.SelectedIndexChanged += (sender, e) => {
				if (updating)
					return;
				float size;
				if (float.TryParse (fontSizes.SelectedKey, out size)) {
					UpdatePreview (new Font(selectedFont.Typeface, size));
				}
			};
			return fontSizes;
		}

		Control BoldFont ()
		{
			var control = new CheckBox { Text = "Bold", Enabled = false };
			control.Bind (r => r.Checked, (Font f) => f.Bold);
			return control;
		}

		Control ItalicFont ()
		{
			var control = new CheckBox { Text = "Italic", Enabled = false };
			control.Bind (r => r.Checked, (Font f) => f.Italic);
			return control;
		}

		void UpdatePreview (Font font)
		{
			if (updating)
				return;
			updating = true;
			var newFamily = selectedFont == null || selectedFont.Family != font.Family;
			selectedFont = font;
			DataContext = selectedFont;
			preview.Font = selectedFont;
			preview.Invalidate ();

			var family = selectedFont.Family;
			if (newFamily) {
				fontStyles.Items.Clear ();
				fontStyles.Items.AddRange (family.Typefaces.Select (r => new ListItem { Text = r.Name, Key = r.Name }).OfType<IListItem>());
			}
			fontStyles.SelectedKey = selectedFont.Typeface.Name;
			fontList.SelectedKey = family.Name;
			fontSizes.SelectedKey = font.SizeInPoints.ToString ();

			updating = false;
		}

		Control Preview ()
		{
			preview = new TextArea { Wrap = true, Size = new Size(-1, 100) };
			preview.Text = "The quick brown fox jumps over the lazy dog";

			var box = new GroupBox { Text = "Preview" };
			box.AddDockedControl (preview, new Padding(10));
			return box;
		}
	}
}

