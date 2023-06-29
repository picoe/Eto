namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="Form"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class FormHandler : AndroidWindow<Form, Form.ICallback>, Form.IHandler
	{
		public FormHandler()
		{
		}

		public void Show()
		{
			var ParentActivity = (Activity ?? ApplicationHandler.Instance.TopActivity);
			Activity = ParentActivity;
			ParentActivity.SetContentView(ContainerControl);
			
			Callback.OnShown(Widget, EventArgs.Empty);
		}

		public bool ShowActivated { get; set; }

		public bool CanFocus { get; set; }

		// Ignore setting size for forms - they must fill the activity exactly.
		public override Size Size { get => base.Size; set { } }
	}
}
