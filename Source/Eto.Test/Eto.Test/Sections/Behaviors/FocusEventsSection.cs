using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Focus Events")]
	public class FocusEventsSection : AllControlsBase
	{
		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			control.GotFocus += delegate
			{
				Log.Write(control, "GotFocus");
			};
			control.LostFocus += delegate
			{
				Log.Write(control, "LostFocus");
			};
		}
	}
}

