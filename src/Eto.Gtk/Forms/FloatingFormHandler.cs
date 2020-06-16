using Eto.Forms;
using System;

namespace Eto.GtkSharp.Forms
{
	public class FloatingFormHandler : FormHandler, FloatingForm.IHandler
	{
		protected override void Initialize()
		{
			base.Initialize();

			Topmost = true;
			Minimizable = false;
			Maximizable = false;
			ShowInTaskbar = false;
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
			var lastAcceptFocus = Control.AcceptFocus;
			Control.AcceptFocus = false;

			base.Visible = ApplicationHandler.Instance.IsActive && _visible;
			Control.AcceptFocus = lastAcceptFocus;
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
