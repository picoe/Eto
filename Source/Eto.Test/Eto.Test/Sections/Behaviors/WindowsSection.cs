#if DESKTOP
using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.Test.Sections.Behaviors
{
	public class WindowsSection : Panel
	{
		Form child;
		Button bringToFrontButton;
		EnumRadioButtonList<WindowStyle> styleCombo;
		EnumRadioButtonList<WindowState> stateCombo;
		CheckBox resizableCheckBox;
		CheckBox maximizableCheckBox;
		CheckBox minimizableCheckBox;
		CheckBox showInTaskBarCheckBox;
		CheckBox topMostCheckBox;

		public WindowsSection()
		{
			var layout = new DynamicLayout();

			layout.AddSeparateRow(null, Resizable(), Minimizable(), Maximizable(), ShowInTaskBar(), TopMost(), null);
			layout.AddSeparateRow(null, new Label { Text = "Window Style" }, WindowStyle(), null);
			layout.AddSeparateRow(null, new Label { Text = "Window State" }, WindowState(), null);
			layout.AddSeparateRow(null, CreateChildWindowButton(), null);
			layout.AddSeparateRow(null, BringToFrontButton(), null);
			layout.Add(null);

			Content = layout;
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (child != null)
				child.Close();
		}

		Control WindowStyle()
		{
			styleCombo = new EnumRadioButtonList<WindowStyle>
			{
				SelectedValue = Forms.WindowStyle.Default
			};
			styleCombo.SelectedIndexChanged += (sender, e) => {
				if (child != null)
					child.WindowStyle = styleCombo.SelectedValue;
			};
			return styleCombo;
		}

		Control WindowState()
		{
			stateCombo = new EnumRadioButtonList<WindowState>
			{
				SelectedValue = Forms.WindowState.Normal
			};
			stateCombo.SelectedIndexChanged += (sender, e) => {
				if (child != null)
					child.WindowState = stateCombo.SelectedValue;
			};
			return stateCombo;
		}

		Control Resizable()
		{
			resizableCheckBox = new CheckBox
			{
				Text = "Resizable",
				Checked = true
			};
			resizableCheckBox.CheckedChanged += (sender, e) => {
				if (child != null)
					child.Resizable = resizableCheckBox.Checked ?? false;
			};
			return resizableCheckBox;
		}

		Control Maximizable()
		{
			maximizableCheckBox = new CheckBox
			{
				Text = "Maximizable",
				Checked = true
			};
			maximizableCheckBox.CheckedChanged += (sender, e) => {
				if (child != null)
					child.Maximizable = maximizableCheckBox.Checked ?? false;
			};
			return maximizableCheckBox;
		}

		Control Minimizable()
		{
			minimizableCheckBox = new CheckBox
			{
				Text = "Minimizable",
				Checked = true
			};
			minimizableCheckBox.CheckedChanged += (sender, e) => {
				if (child != null)
					child.Minimizable = minimizableCheckBox.Checked ?? false;
			};
			return minimizableCheckBox;
		}

		Control ShowInTaskBar()
		{
			showInTaskBarCheckBox = new CheckBox
			{
				Text = "Show In TaskBar",
				Checked = true
			};
			showInTaskBarCheckBox.CheckedChanged += (sender, e) => {
				if (child != null)
					child.ShowInTaskbar = showInTaskBarCheckBox.Checked ?? false;
			};
			return showInTaskBarCheckBox;
		}

		Control TopMost()
		{
			topMostCheckBox = new CheckBox
			{
				Text = "Top Most",
				Checked = false
			};
			topMostCheckBox.CheckedChanged += (sender, e) => {
				if (child != null)
					child.Topmost = topMostCheckBox.Checked ?? false;
			};
			return topMostCheckBox;
		}

		void CreateChild()
		{
			if (child != null)
				child.Close();
			child = new Form
			{ 
				Title = "Child Window",
				ClientSize = new Size (300, 200),
				WindowStyle = styleCombo.SelectedValue,
				WindowState = stateCombo.SelectedValue,
				Topmost = topMostCheckBox.Checked ?? false,
				Resizable = resizableCheckBox.Checked ?? false,
				Maximizable = maximizableCheckBox.Checked ?? false,
				Minimizable = minimizableCheckBox.Checked ?? false,
				ShowInTaskbar = showInTaskBarCheckBox.Checked ?? false
			};
			var layout = new DynamicLayout();
			layout.Add(null);
			layout.AddCentered(SendToBackButton());
			layout.AddCentered(CloseButton());
			layout.Add(null);
			child.Content = layout;

			child.WindowStateChanged += child_WindowStateChanged;
			child.Closed += child_Closed;
			child.Shown += child_Shown;
			child.GotFocus += child_GotFocus;
			child.LostFocus += child_LostFocus;
			child.LocationChanged += child_LocationChanged;
			child.SizeChanged += child_SizeChanged;
			bringToFrontButton.Enabled = true;
			child.Show();
		}

		void child_Closed(object sender, EventArgs e)
		{
			Log.Write(child, "Closed");
			child.WindowStateChanged -= child_WindowStateChanged;
			child.Closed -= child_Closed;
			child.Shown -= child_Shown;
			child.GotFocus -= child_GotFocus;
			child.LostFocus -= child_LostFocus;
			child.LocationChanged -= child_LocationChanged;
			child.SizeChanged -= child_SizeChanged;
			bringToFrontButton.Enabled = false;
			child = null;
		}

		void child_LocationChanged(object sender, EventArgs e)
		{
			Log.Write(child, "LocationChanged: {0}", child.Location);
		}

		void child_SizeChanged(object sender, EventArgs e)
		{
			Log.Write(child, "SizeChanged: {0}", child.Size);
		}

		void child_LostFocus(object sender, EventArgs e)
		{
			Log.Write(child, "LostFocus");
		}

		void child_GotFocus(object sender, EventArgs e)
		{
			Log.Write(child, "GotFocus");
		}

		void child_Shown(object sender, EventArgs e)
		{
			Log.Write(child, "Shown");
		}

		void child_WindowStateChanged(object sender, EventArgs e)
		{
			Log.Write(child, "StateChanged: {0}", child.WindowState);
		}

		Control CreateChildWindowButton()
		{
			var control = new Button { Text = "Create Child Window" };
			control.Click += (sender, e) => {
				CreateChild();
			};
			return control;
		}

		Control BringToFrontButton()
		{
			var control = bringToFrontButton = new Button { Text = "Bring to Front", Enabled = false };
			control.Click += (sender, e) => {
				if (child != null)
					child.BringToFront();
			};
			return control;
		}

		Control CloseButton()
		{
			var control = new Button { Text = "Close Window" };
			control.Click += (sender, e) => {
				if (child != null)
					child.Close();
			};
			return control;
		}

		Control SendToBackButton()
		{
			var control = new Button { Text = "Send to Back" };
			control.Click += (sender, e) => {
				if (child != null)
					child.SendToBack();
			};
			return control;
		}
	}
}
#endif