using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Password box handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PasswordBoxHandler : WpfControl<swc.PasswordBox, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler
	{
		protected override wf.Size DefaultSize => new wf.Size(80, double.NaN);

		public PasswordBoxHandler()
		{
			Control = new swc.PasswordBox();
		}

		public override wf.Size GetPreferredSize(wf.Size constraint)
		{
			return base.GetPreferredSize(Conversions.ZeroSize);
		}

		public override bool UseMousePreview { get { return true; } }

		public override bool UseKeyPreview { get { return true; } }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.PasswordChanged += delegate
					{
						Callback.OnTextChanged(Widget, EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return false; }
			set { }
		}

		public char PasswordChar
		{
			get { return Control.PasswordChar[0]; }
			set { Control.PasswordChar = value.ToString(); }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string Text
		{
			get { return Control.Password; }
			set { Control.Password = value; }
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}
	}
}
