using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using Eto.Forms;
using Eto.Drawing;
using Windows.UI.Xaml;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Text box handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class TextBoxHandler : WpfControl<swc.TextBox, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		bool textChanging;
		protected override Size DefaultSize { get { return new Size(80, -1); } }

		public TextBoxHandler ()
		{
			Control = new swc.TextBox();
		}

		protected override void Initialize()
		{
			base.Initialize();
#if TODO_XAML
			Control.GotKeyboardFocus += Control_GotKeyboardFocus;
#endif
		}

#if TODO_XAML
		void Control_GotKeyboardFocus(object sender, sw.Input.KeyboardFocusChangedEventArgs e)
		{
			Control.SelectAll();
			Control.GotKeyboardFocus -= Control_GotKeyboardFocus;
		}
#endif

		public override wf.Size GetPreferredSize(wf.Size constraint)
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
							Callback.OnTextChanged (Widget, EventArgs.Empty);
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
#if TODO_XAML
				if (value != null)
					Control.CaretIndex = value.Length;
#endif
				textChanging = false;
			}
		}

		public string PlaceholderText
		{
#if TODO_XAML
			get { return Control.Watermark as string; }
			set { Control.Watermark = value; }
#else
			get; set;
#endif
		}

		public void SelectAll ()
		{
			Control.Focus (FocusState.Programmatic);
			Control.SelectAll ();
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}
    }
}
