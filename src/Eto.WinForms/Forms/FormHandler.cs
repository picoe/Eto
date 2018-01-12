using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class FormHandler : WindowHandler<swf.Form, Form, Form.ICallback>, Form.IHandler
	{
		public class EtoForm : swf.Form
		{
			bool hideFromAltTab;

			public bool HideFromAltTab
			{
				get { return hideFromAltTab; }
				set
				{
					if (hideFromAltTab != value)
					{
						hideFromAltTab = value;
						SetStyle(Win32.WS_EX.TOOLWINDOW, value);
					}
				}
			}

			public bool ShouldShowWithoutActivation { get; set; }

			protected override bool ShowWithoutActivation => ShouldShowWithoutActivation;

			protected override swf.CreateParams CreateParams
			{
				get
				{
					var createParams = base.CreateParams;
                    
					if (hideFromAltTab)
						createParams.ExStyle |= (int)Win32.WS_EX.TOOLWINDOW;
					if (!ShouldAllowFocus)
					{
						createParams.ExStyle |= (int)Win32.WS_EX.NOACTIVATE;
						createParams.Style |= (int)Win32.WS.CHILD;
					}

					return createParams;
				}
			}

			bool shouldAllowFocus = true;
			public bool ShouldAllowFocus
			{
				get { return shouldAllowFocus; }
				set
				{
					if (shouldAllowFocus != value)
					{
						shouldAllowFocus = value;
						SetStyle(Win32.WS_EX.NOACTIVATE, !value);
						SetStyle(Win32.WS.CHILD, !value);
					}
				}
			}

			void SetStyle(Win32.WS_EX style, bool value)
			{
				if (IsHandleCreated)
				{
					var styleInt = Win32.GetWindowLong(Handle, Win32.GWL.EXSTYLE);
					if (value)
						styleInt |= (uint)style;
					else
						styleInt &= (uint)~style;

					Win32.SetWindowLong(Handle, Win32.GWL.EXSTYLE, styleInt);
				}
			}

			void SetStyle(Win32.WS style, bool value)
			{
				if (IsHandleCreated)
				{
					var styleInt = Win32.GetWindowLong(Handle, Win32.GWL.STYLE);
					if (value)
						styleInt |= (uint)style;
					else
						styleInt &= (uint)~style;

					Win32.SetWindowLong(Handle, Win32.GWL.STYLE, styleInt);
				}
			}
		}

		public FormHandler(swf.Form form)
		{
			Control = form;
		}

		public FormHandler()
		{
			Control = new EtoForm
			{
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			Resizable = true;
		}

		public void Show()
		{
			Control.Show();
		}

		public override bool ShowInTaskbar
		{
			get { return base.ShowInTaskbar; }
			set
			{
				base.ShowInTaskbar = value;
				var etoForm = Control as EtoForm;
				if (etoForm != null)
					etoForm.HideFromAltTab = !value;
			}
		}

		public Color TransparencyKey
		{
			get { return Control.TransparencyKey.ToEto(); }
			set { Control.TransparencyKey = value.ToSD(); }
		}


		public bool KeyPreview
		{
			get { return Control.KeyPreview; }
			set { Control.KeyPreview = value; }
		}

		public bool ShowActivated
		{
			get { return EtoFormControl?.ShouldShowWithoutActivation != true; }
			set
			{
				var etoForm = EtoFormControl;
				if (etoForm != null)
					etoForm.ShouldShowWithoutActivation = !value;
			}
		}

		public bool CanFocus
		{
			get { return EtoFormControl?.ShouldAllowFocus == true; }
			set {
				var etoForm = EtoFormControl;
				if (etoForm != null)
					etoForm.ShouldAllowFocus = value;
			}
		}

		EtoForm EtoFormControl => Control as EtoForm;
	}
}
