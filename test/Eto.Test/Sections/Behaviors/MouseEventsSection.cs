using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Mouse Events")]
	public class MouseEventsSection : AllControlsBase
	{
		CheckBox handleEvents;
		CheckBox showParentEvents;

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);
			LogEvents(this);
		}

		void LogMouseEvent(object sender, string type, MouseEventArgs e)
		{
			if (!showParentEvents.Checked == true && sender == this)
				return;
			Log.Write(sender, "{0}, Location: {1}, Buttons: {2}, Modifiers: {3}, Delta: {4}", type, e.Location, e.Buttons, e.Modifiers, e.Delta);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		protected override Control CreateOptions()
		{
			var layout = new DynamicLayout { Spacing = new Size(5, 5) };

			layout.AddRow(null, Handled(), ShowParentEvents(), null);
			layout.Add(null);

			return layout;
		}

		Control Handled()
		{
			return handleEvents = new CheckBox { Text = "Handle mouse events" };
		}

		Control ShowParentEvents()
		{
			return showParentEvents = new CheckBox { Text = "Show parent events (bubbled)" };
		}

		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			control.MouseDoubleClick += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseDoubleClick", e);
			};
			control.MouseWheel += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseWheel", e);
			};
			control.MouseMove += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseMove", e);
			};
			control.MouseUp += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseUp", e);
			};
			control.MouseDown += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseDown", e);
			};
			control.MouseEnter += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseEnter", e);
			};
			control.MouseLeave += delegate(object sender, MouseEventArgs e)
			{
				LogMouseEvent(control, "MouseLeave", e);
			};
		}
	}
}

