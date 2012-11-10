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
			var layout = new DynamicLayout (this, new Size (20, 20));
			layout.BeginVertical ();
			layout.AddRow (null, PickFont (), null);
			layout.AddRow (null, PickFontWithStartingFont (), null);

			layout.EndVertical ();

			layout.AddSeparateRow (null, FontList (), FontStyles (), FontSizes (), null);
			layout.AddSeparateRow (Preview ());

			layout.Add (null);
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

		Control FontList ()
		{
			fontList = new ListBox { Size = new Size (300, 200) };
			var lookup = Fonts.AvailableFontFamilies ().ToDictionary (r => r.Name);
			fontList.Items.AddRange (lookup.Values.OrderBy (r => r.Name).Select (r => new ListItem { Text = r.Name, Key = r.Name }).OfType<IListItem>());
			fontList.SelectedIndexChanged += (sender, e) => {
				if (updating)
					return;
				var family = lookup[fontList.SelectedKey];
				UpdatePreview (new Font (family.Typefaces.First (), selectedFont.Size));
			};

			return fontList;
		}

		Control FontStyles ()
		{
			fontStyles = new ListBox { Size = new Size (200, 100) };
			fontStyles.SelectedIndexChanged += (sender, e) => {
				if (updating)
					return;
				var face = selectedFont.Family.Typefaces.FirstOrDefault (r => r.Name == fontStyles.SelectedKey);
				if (face != null) {
					UpdatePreview (new Font (face, selectedFont.Size));
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

		void UpdatePreview (Font font)
		{
			if (updating)
				return;
			updating = true;
			var newFamily = selectedFont == null || selectedFont.Family != font.Family;
			selectedFont = font;
			preview.Font = selectedFont;
			preview.Invalidate ();

			var family = selectedFont.Family;
			if (newFamily) {
				fontStyles.Items.Clear ();
				fontStyles.Items.AddRange (family.Typefaces.Select (r => new ListItem { Text = r.Name, Key = r.Name }).OfType<IListItem>());
			}
			fontStyles.SelectedKey = selectedFont.Typeface.Name;
			fontList.SelectedKey = family.Name;
			fontSizes.SelectedKey = font.Size.ToString ();

			updating = false;
		}

		Control Preview ()
		{
			preview = new TextArea { Wrap = true, Size = new Size(-1, 100) };
			preview.Text = "The quick brown fox jumps over the lazy dog";
			return preview;
		}
	}
}

