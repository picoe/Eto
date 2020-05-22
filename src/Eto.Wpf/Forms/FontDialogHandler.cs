using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.CustomControls.FontDialog;
using Eto.Wpf.Drawing;
using System;

using sw = System.Windows;
namespace Eto.Wpf.Forms
{
	public class FontDialogHandler : WidgetHandler<FontChooser, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		Font _font;
		static readonly object DisableFontChanged_Key = new object();

		int DisableFontChanged
		{
			get => Widget.Properties.Get<int>(DisableFontChanged_Key);
			set => Widget.Properties.Set(DisableFontChanged_Key, value);
		}

		public FontDialogHandler()
		{
			Control = new FontChooser();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
					Widget.Properties.Set(FontChooser.SelectedFontFamilyProperty, PropertyChangeNotifier.Register(FontChooser.SelectedFontFamilyProperty, FontChanged, Control));
					Widget.Properties.Set(FontChooser.SelectedFontSizeProperty, PropertyChangeNotifier.Register(FontChooser.SelectedFontSizeProperty, FontChanged, Control));
					Widget.Properties.Set(FontChooser.SelectedFontStretchProperty, PropertyChangeNotifier.Register(FontChooser.SelectedFontStretchProperty, FontChanged, Control));
					Widget.Properties.Set(FontChooser.SelectedFontStyleProperty, PropertyChangeNotifier.Register(FontChooser.SelectedFontStyleProperty, FontChanged, Control));
					Widget.Properties.Set(FontChooser.SelectedFontWeightProperty, PropertyChangeNotifier.Register(FontChooser.SelectedFontWeightProperty, FontChanged, Control));
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void FontChanged(object sender, EventArgs e)
		{
			if (DisableFontChanged > 0)
				return;

			if (IsChanged(_font))
			{
				_font = null;
				Callback.OnFontChanged(Widget, EventArgs.Empty);
			}
		}

		public Font Font
		{
			get => _font ?? (_font = GetFont());
			set
			{
				if (!ReferenceEquals(_font, value))
				{
					_font = value;
					if (_font?.Handler is FontHandler fontHandler && IsChanged(_font))
					{
						DisableFontChanged++;
						Control.SelectedFontFamily = fontHandler.WpfFamily;
						Control.SelectedFontPointSize = fontHandler.Size;
						Control.SelectedFontStyle = fontHandler.WpfFontStyle;
						Control.SelectedFontWeight = fontHandler.WpfFontWeight;
						Control.SelectedFontStretch = fontHandler.WpfFontStretch;
						DisableFontChanged--;
						Callback.OnFontChanged(Widget, EventArgs.Empty);
					}
				}
			}
		}

		private bool IsChanged(Font font)
		{
			if (font?.Handler is FontHandler fontHandler)
			{
				return Control.SelectedFontFamily?.Source != fontHandler.WpfFamily?.Source
				|| Control.SelectedFontPointSize != fontHandler.Size
				|| Control.SelectedFontStyle != fontHandler.WpfFontStyle
				|| Control.SelectedFontWeight != fontHandler.WpfFontWeight
				|| Control.SelectedFontStretch != fontHandler.WpfFontStretch;
			}
			return true;
		}

		Font GetFont()
		{
			var fontHandler = new FontHandler(Control.SelectedFontFamily, Control.SelectedFontPointSize, Control.SelectedFontStyle, Control.SelectedFontWeight, Control.SelectedFontStretch);
			return new Font(fontHandler);
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent != null)
			{
				var owner = parent.ControlObject as sw.Window;
				Control.Owner = owner;
				Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
			}
			var lastFont = Font;

			var result = Control.ShowDialog();

			if (result != true)
			{
				// restore to original font when cancelled.
				Font = lastFont;
			}

			return result == true ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}
