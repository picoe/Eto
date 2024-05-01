namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Focus Events")]
	public class FocusEventsSection : AllControlsBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ParentWindow.GotFocus += Window_GotFocus;
			ParentWindow.LostFocus += Window_LostFocus;
		}

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			ParentWindow.GotFocus -= Window_GotFocus;
			ParentWindow.LostFocus -= Window_LostFocus;
		}

		protected void Window_GotFocus(object sender, EventArgs e)
		{
			if (sender is Window window)
				Log.Write(sender, $"Window.GotFocus, HasFocus: {window.HasFocus}");
		}

		protected void Window_LostFocus(object sender, EventArgs e)
		{
			if (sender is Window window)
				Log.Write(sender, $"Window.LostFocus, HasFocus: {window.HasFocus}");
		}

		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			control.GotFocus += delegate
			{
				Log.Write(control, $"GotFocus, HasFocus: {control.HasFocus}");
			};
			control.LostFocus += delegate
			{
				Log.Write(control, $"LostFocus, HasFocus: {control.HasFocus}");
			};
		}
	}
}

