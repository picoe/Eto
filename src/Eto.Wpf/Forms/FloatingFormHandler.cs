using System;
using Eto.Forms;

namespace Eto.Wpf.Forms
{
	public class FloatingFormHandler : FormHandler, FloatingForm.IHandler
	{
		protected override void Initialize()
		{
			base.Initialize();
			// defaults for a floating form
			Maximizable = false;
			Minimizable = false;
			ShowInTaskbar = false;
			Topmost = true;
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ApplicationHandler.Instance.IsActiveChanged += Application_IsActiveChanged;
			if (!ApplicationHandler.Instance.IsActive)
				base.Visible = false;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			ApplicationHandler.Instance.IsActiveChanged -= Application_IsActiveChanged;
		}

		private void Application_IsActiveChanged(object sender, EventArgs e)
		{
			base.Visible = ApplicationHandler.Instance.IsActive && _visible;
		}

		bool _visible = true;

		public override bool Visible
		{
			get => _visible;
			set
			{
				_visible = value;
				if (ApplicationHandler.Instance.IsActive)
					base.Visible = value;
			}
		}
	}
}