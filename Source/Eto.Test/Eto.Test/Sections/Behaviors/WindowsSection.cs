using Eto.Drawing;
using Eto.Forms;
using System;
using System.Linq;
using System.ComponentModel;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Windows")]
	public class WindowsSection : Panel, INotifyPropertyChanged
	{
		Window child;
		Button bringToFrontButton;
		EnumRadioButtonList<WindowStyle> styleCombo;
		EnumRadioButtonList<WindowState> stateCombo;
		RadioButtonList typeRadio;
		CheckBox resizableCheckBox;
		CheckBox maximizableCheckBox;
		CheckBox minimizableCheckBox;
		CheckBox showInTaskBarCheckBox;
		CheckBox topMostCheckBox;

		static readonly object CancelCloseKey = new object();
		public bool CancelClose
		{
			get { return Properties.Get<bool>(CancelCloseKey); }
			set { Properties.Set(CancelCloseKey, value, PropertyChanged, false, "CancelClose"); }
		}

		public WindowsSection()
		{
			var layout = new DynamicLayout();

			layout.AddSeparateRow(null, Resizable(), Minimizable(), Maximizable(), ShowInTaskBar(), TopMost(), CreateCancelClose(), null);
			layout.AddSeparateRow(null, "Type", CreateTypeControls(), null);
			layout.AddSeparateRow(null, "Window Style", WindowStyle(), null);
			layout.AddSeparateRow(null, "Window State", WindowState(), null);
			layout.AddSeparateRow(null, CreateInitialLocationControls(), null);
			layout.AddSeparateRow(null, CreateClientSizeControls(), null);
			layout.AddSeparateRow(null, CreateChildWindowButton(), null);
			layout.AddSeparateRow(null, BringToFrontButton(), null);
			layout.Add(null);

			Content = layout;
		}

		protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (child != null)
				child.Close();
		}

		Control CreateTypeControls()
		{
			return typeRadio = new RadioButtonList
			{
				Items =
				{ 
					new ListItem { Text = "Form (modeless)", Key = "form" },
					new ListItem { Text = "Dialog (modal)", Key = "dialog" }
				},
				SelectedKey = "form"
			};
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

		Control CreateCancelClose()
		{
			var cancelCloseCheckBox = new CheckBox { Text = "Cancel Close" };
			cancelCloseCheckBox.CheckedBinding.Bind(this, c => c.CancelClose);
			return cancelCloseCheckBox;
		}

		Point initialLocation = new Point(200, 200);
		bool setInitialLocation;

		Control CreateInitialLocationControls()
		{
			var setLocationCheckBox = new CheckBox { Text = "Initial Location" };
			setLocationCheckBox.CheckedBinding.Bind(() => setInitialLocation, v => setInitialLocation = v ?? false);

			var left = new NumericUpDown();
			left.Bind(c => c.Enabled, setLocationCheckBox, c => c.Checked);
			left.ValueBinding.Bind(() => initialLocation.X, v => initialLocation.X = (int)v);

			var top = new NumericUpDown();
			top.Bind(c => c.Enabled, setLocationCheckBox, c => c.Checked);
			top.ValueBinding.Bind(() => initialLocation.Y, v => initialLocation.Y = (int)v);

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = 
				{
					setLocationCheckBox,
					left,
					top
				}
			};
		}

		bool setInitialClientSize;
		Size initialClientSize = new Size(300, 300);

		Control CreateClientSizeControls()
		{
			var setClientSize = new CheckBox { Text = "ClientSize" };
			setClientSize.CheckedBinding.Bind(() => setInitialClientSize, v => setInitialClientSize = v ?? false);

			var left = new NumericUpDown();
			left.Bind(c => c.Enabled, setClientSize, c => c.Checked);
			left.ValueBinding.Bind(() => initialClientSize.Width, v => initialClientSize.Width = (int)v);

			var top = new NumericUpDown();
			top.Bind(c => c.Enabled, setClientSize, c => c.Checked);
			top.ValueBinding.Bind(() => initialClientSize.Height, v => initialClientSize.Height = (int)v);

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = 
				{
					setClientSize,
					left,
					top
				}
				};

		}

		void CreateChild()
		{
			if (child != null)
				child.Close();
			Action show;

			if (typeRadio.SelectedKey == "form")
			{
				var form = new Form();
				child = form;
				show = form.Show;
			}
			else
			{
				var dialog = new Dialog();
				child = dialog;
				show = () => dialog.ShowModal(this);
			}

			child.Title = "Child Window";
			child.WindowStyle = styleCombo.SelectedValue;
			child.WindowState = stateCombo.SelectedValue;
			child.Topmost = topMostCheckBox.Checked ?? false;
			child.Resizable = resizableCheckBox.Checked ?? false;
			child.Maximizable = maximizableCheckBox.Checked ?? false;
			child.Minimizable = minimizableCheckBox.Checked ?? false;
			child.ShowInTaskbar = showInTaskBarCheckBox.Checked ?? false;
			if (setInitialLocation)
				child.Location = initialLocation;
			if (setInitialClientSize)
				child.ClientSize = initialClientSize;
			var layout = new DynamicLayout();
			layout.Add(null);
			layout.AddCentered(TestChangeSizeButton());
			layout.AddCentered(TestChangeClientSizeButton());
			layout.AddCentered(SendToBackButton());
			layout.AddCentered(CreateCancelClose());
			layout.AddCentered(CloseButton());
			layout.Add(null);
			child.Content = layout;

			child.WindowStateChanged += child_WindowStateChanged;
			child.Closed += child_Closed;
			child.Closing += child_Closing;
			child.Shown += child_Shown;
			child.GotFocus += child_GotFocus;
			child.LostFocus += child_LostFocus;
			child.LocationChanged += child_LocationChanged;
			child.SizeChanged += child_SizeChanged;
			bringToFrontButton.Enabled = true;
			show();
			// show that the child is now referenced
			Log.Write(null, "Open Windows: {0}", Application.Instance.Windows.Count());
		}

		void child_Closed(object sender, EventArgs e)
		{
			Log.Write(child, "Closed");
			child.WindowStateChanged -= child_WindowStateChanged;
			child.Closed -= child_Closed;
			child.Closing -= child_Closing;
			child.Shown -= child_Shown;
			child.GotFocus -= child_GotFocus;
			child.LostFocus -= child_LostFocus;
			child.LocationChanged -= child_LocationChanged;
			child.SizeChanged -= child_SizeChanged;
			bringToFrontButton.Enabled = false;
			child = null;
			// write out number of open windows after the closed event is called
			Application.Instance.AsyncInvoke(() => Log.Write(null, "Open Windows: {0}", Application.Instance.Windows.Count()));
		}

		void child_Closing(object sender, CancelEventArgs e)
		{
			Log.Write(child, "Closing");
			Log.Write(child, "RestoreBounds: {0}", child.RestoreBounds);
			if (CancelClose)
			{
				e.Cancel = true;
			}
		}

		static void child_LocationChanged(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "LocationChanged: {0}", child.Location);
		}

		static void child_SizeChanged(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "SizeChanged: {0}", child.Size);
		}

		static void child_LostFocus(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "LostFocus");
		}

		static void child_GotFocus(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "GotFocus");
		}

		static void child_Shown(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "Shown");
		}

		static void child_WindowStateChanged(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "StateChanged: {0}", child.WindowState);
		}

		Control CreateChildWindowButton()
		{
			var control = new Button { Text = "Create Child Window" };
			control.Click += (sender, e) => CreateChild();
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

		Control TestChangeSizeButton()
		{
			var control = new Button { Text = "TestChangeSize" };
			control.Click += (sender, e) =>
			{
				if (child != null)
					child.Size = new Size(500, 500);
			};
			return control;
		}

		Control TestChangeClientSizeButton()
		{
			var control = new Button { Text = "TestChangeClientSize" };
			control.Click += (sender, e) =>
			{
				if (child != null)
					child.ClientSize = new Size(500, 500);
			};
			return control;
		}

		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}