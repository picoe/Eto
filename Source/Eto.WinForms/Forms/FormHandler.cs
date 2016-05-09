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
						if (IsHandleCreated)
						{
							var style = Win32.GetWindowLong(Handle, Win32.GWL.EXSTYLE);
							if (hideFromAltTab)
								style |= (uint)Win32.WS_EX.TOOLWINDOW;
							else
								style &= (uint)~Win32.WS_EX.TOOLWINDOW;

							Win32.SetWindowLong(Handle, Win32.GWL.EXSTYLE, style);
						}
					}
				}
			}

			public bool ShouldShowWithoutActivation { get; set; }

			protected override bool ShowWithoutActivation
			{
				get { return ShouldShowWithoutActivation; }
			}

			protected override swf.CreateParams CreateParams
			{
				get
				{
					var createParams = base.CreateParams;
                    
					if (hideFromAltTab)
						createParams.ExStyle |= (int)Win32.WS_EX.TOOLWINDOW;

					return createParams;
				}
			}

			protected override void SetVisibleCore(bool value)
			{
				base.SetVisibleCore(value);
			}

			protected override void OnActivated(EventArgs e)
			{
				int h = (int)Handle;
				base.OnActivated(e);
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
			get { return (Control as EtoForm)?.ShouldShowWithoutActivation != true; }
			set
			{
				var etoForm = Control as EtoForm;
				if (etoForm != null)
					etoForm.ShouldShowWithoutActivation = !value;
			}
		}
	}
}
