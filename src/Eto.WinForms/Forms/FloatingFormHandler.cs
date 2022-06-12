using System;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class FloatingFormHandler : FormHandler, FloatingForm.IHandler
	{
		protected override void Initialize()
		{
			base.Initialize();
			ShowInTaskbar = false;
			Minimizable = false;
			Maximizable = false;
			Topmost = true;
		}
		
		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Application.Instance.IsActiveChanged += Application_IsActiveChanged;
			if (!Application.Instance.IsActive)
				base.Visible = false;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			Application.Instance.IsActiveChanged -= Application_IsActiveChanged;
		}

		private void Application_IsActiveChanged(object sender, EventArgs e)
		{
			var showActivated = ShowActivated;
			ShowActivated = false;
			base.Visible = Application.Instance.IsActive && _visible;
			ShowActivated = showActivated;
		}

		bool _visible = true;

		public override bool Visible
		{
			get => _visible;
			set
			{
				_visible = value;
				if (Application.Instance.IsActive)
					base.Visible = value;
			}
		}

	}
}
