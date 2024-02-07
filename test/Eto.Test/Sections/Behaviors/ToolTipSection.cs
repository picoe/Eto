using Eto.Forms;
namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "ToolTip")]
	public class ToolTipSection : AllControlsBase
	{
		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);
			control.ToolTip = $"ToolTip for {control.GetType().Name}";
		}
	}
}

