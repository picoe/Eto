using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using mwc = Xceed.Wpf.Toolkit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class TextBoxHandler : WpfControl<mwc.WatermarkTextBox, TextBox>, ITextBox
	{
		bool textChanging;
		protected override Size DefaultSize { get { return new Size(80, -1); } }

		public TextBoxHandler ()
		{
			Control = new mwc.WatermarkTextBox();
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Control.GotKeyboardFocus += Control_GotKeyboardFocus;
		}

		void Control_GotKeyboardFocus(object sender, sw.Input.KeyboardFocusChangedEventArgs e)
		{
			Control.SelectAll();
			Control.GotKeyboardFocus -= Control_GotKeyboardFocus;
		}

		public override sw.Size GetPreferredSize(sw.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public override void AttachEvent (string id)
		{
			switch (id) {
				case TextControl.TextChangedEvent:
					Control.TextChanged += delegate {
						if (!textChanging)
							Widget.OnTextChanged (EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent (id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.IsReadOnly; }
			set { Control.IsReadOnly = value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Text; }
			set {
				textChanging = true;
				Control.Text = value;
				if (value != null)
					Control.CaretIndex = value.Length;
				textChanging = false;
			}
		}

		public string PlaceholderText
		{
			get { return Control.Watermark as string; }
			set { Control.Watermark = value; }
		}

		public void SelectAll ()
		{
			Control.Focus ();
			Control.SelectAll ();
		}
    }
}
