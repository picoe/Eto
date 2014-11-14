using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using mwc = Xceed.Wpf.Toolkit;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Wpf.Forms.Controls
{
	public class TextBoxHandler : WpfControl<mwc.WatermarkTextBox, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		bool textChanging;
		protected override Size DefaultSize { get { return new Size(80, -1); } }

		class EtoWatermarkTextBox : mwc.WatermarkTextBox
		{
			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				if (IsLoaded && IsVisible)
				{
					constraint.Width = !double.IsNaN(constraint.Width) ? Math.Min(constraint.Width, ActualWidth) : ActualWidth;
					constraint.Height = !double.IsNaN(constraint.Height) ? Math.Min(constraint.Height, ActualHeight) : ActualHeight;
				}
				return base.MeasureOverride(constraint);
			}
		}

		public TextBoxHandler ()
		{
			Control = new EtoWatermarkTextBox();
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
							Callback.OnTextChanged(Widget, EventArgs.Empty);
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
