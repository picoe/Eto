namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Key Events")]
	public class KeyEventsSection : AllControlsBase
	{
		CheckBox handleEvents;
		CheckBox showParentEvents;
		CheckBox showWindowEvents;
		CheckBox cancelTextInput;

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);
			LogEvents(this);
			LogEvents(ParentWindow);
		}

		protected override void OnUnLoad(System.EventArgs e)
		{
			base.OnUnLoad(e);
			ParentWindow.KeyDown -= control_KeyDown;
			ParentWindow.KeyUp -= control_KeyUp;
		}

		void LogKeyEvent(object sender, string type, KeyEventArgs e)
		{
			if (!showParentEvents.Checked == true && sender == this)
				return;
			if (!showWindowEvents.Checked == true && sender == this.ParentWindow)
				return;
			Log.Write(sender, "{0}, Key: {1}, Char: {2}, Handled: {3}", type, e.KeyData, e.IsChar ? e.KeyChar.ToString() : "no char", e.Handled);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		protected override void LogEvents(Control control)
		{
			base.LogEvents(control);

			control.KeyDown += control_KeyDown;

			control.KeyUp += control_KeyUp;
			
			// if (control is Drawable || control is Button)
				control.TextInput += control_TextInput;
		}

		private void control_TextInput(object sender, TextInputEventArgs e)
		{
			Log.Write(sender, $"TextInput: {e.Text}");
			if (cancelTextInput.Checked == true)
				e.Cancel = true;
		}

		void control_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyEventType != KeyEventType.KeyUp)
				Log.Write(sender, "INCORRECT: KeyUp event should always have a KeyEventType = KeyUp");
			LogKeyEvent(sender, e.KeyEventType.ToString(), e);
		}

		void control_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyEventType != KeyEventType.KeyDown)
				Log.Write(sender, "INCORRECT: KeyDown event should always have a KeyEventType = KeyDown");
			LogKeyEvent(sender, e.KeyEventType.ToString(), e);
		}

		Control ShowParentEvents()
		{
			return showParentEvents = new CheckBox { Text = "Show parent events" };
		}

		Control ShowWindowEvents()
		{
			return showWindowEvents = new CheckBox { Text = "Show window events" };
		}

		protected override Control CreateOptions()
		{
			var layout = new DynamicLayout { Spacing = new Size(5, 5) };

			layout.AddRow(null, Handled(), CancelTextInput(), ShowParentEvents(), ShowWindowEvents(), null);
			layout.Add(null);

			return layout;
		}

		Control Handled()
		{
			return handleEvents = new CheckBox { Text = "Handle key events" };
		}
		Control CancelTextInput()
		{
			return cancelTextInput = new CheckBox { Text = "Cancel TextInput events" };
		}
	}
}

