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
		CheckBox movableByWindowBackgroundCheckBox;
		CheckBox showInTaskBarCheckBox;
		CheckBox topMostCheckBox;
		CheckBox setOwnerCheckBox;
		CheckBox visibleCheckBox;
		CheckBox showActivatedCheckBox;
		CheckBox canFocusCheckBox;
		CheckBox createMenuBar;
		EnumCheckBoxList<MenuBarSystemItems> systemMenuItems;
		EnumDropDown<DialogDisplayMode?> dialogDisplayModeDropDown;

		static readonly object CancelCloseKey = new object();
		public bool CancelClose
		{
			get { return Properties.Get<bool>(CancelCloseKey); }
			set { Properties.Set(CancelCloseKey, value, PropertyChanged, false, "CancelClose"); }
		}

		public WindowsSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, Resizable(), Minimizable(), Maximizable(), MovableByWindowBackground(), null);
			layout.AddSeparateRow(null, ShowInTaskBar(), TopMost(), VisibleCheckbox(), CreateShowActivatedCheckbox(), CreateCanFocus(), null);
			layout.AddSeparateRow(null, "Type", CreateTypeControls(), null);
			layout.AddSeparateRow(null, "Window Style", WindowStyle(), null);
			layout.AddSeparateRow(null, "Window State", WindowState(), null);
			layout.AddSeparateRow(null, "Dialog Display Mode", DisplayModeDropDown(), null);
			layout.AddSeparateRow(null, CreateMenuBarControls(), null);
			layout.AddSeparateRow(null, CreateInitialLocationControls(), null);
			layout.AddSeparateRow(null, CreateSizeControls(), null);
			layout.AddSeparateRow(null, CreateClientSizeControls(), null);
			layout.AddSeparateRow(null, CreateMinimumSizeControls(), null);
			layout.AddSeparateRow(null, CreateCancelClose(), null);
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

		MenuBar CreateMenuBar()
		{
			var menu = new MenuBar();
			if (systemMenuItems.SelectedValues.Any())
				menu.IncludeSystemItems = GetMenuItems();
			return menu;
		}

		MenuBarSystemItems GetMenuItems()
		{
			var val = MenuBarSystemItems.None;
			foreach (var value in systemMenuItems.SelectedValues)
			{
				val |= value;
			}
			return val;
		}

		Control CreateMenuBarControls()
		{
			createMenuBar = new CheckBox { Text = "Create MenuBar" };
			createMenuBar.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.Menu = createMenuBar.Checked == true ? CreateMenuBar() : null;
			};

			systemMenuItems = new EnumCheckBoxList<MenuBarSystemItems>();
			systemMenuItems.IncludeNoneFlag = true;
			systemMenuItems.SelectedValuesChanged += (sender, e) =>
			{
				if (child?.Menu != null)
				{
					child.Menu.IncludeSystemItems = GetMenuItems();
				}

			};
			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = { createMenuBar, "MenuBarSystemItems:", systemMenuItems }
			};
		}

		Control CreateTypeControls()
		{
			typeRadio = new RadioButtonList
			{
				Items =
				{ 
					new ListItem { Text = "Form (modeless)", Key = "form" },
					new ListItem { Text = "Dialog (modal)", Key = "dialog" }
				},
				SelectedKey = "form"
			};

			setOwnerCheckBox = new CheckBox { Text = "Set Owner", Checked = false };
			setOwnerCheckBox.CheckedChanged += (sender, e) => 
			{
				if (child != null)
					child.Owner = setOwnerCheckBox.Checked ?? false ? ParentWindow : null;
			};

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = { typeRadio, setOwnerCheckBox }
			};
		}

		Control WindowStyle()
		{
			styleCombo = new EnumRadioButtonList<WindowStyle>
			{
				SelectedValue = Forms.WindowStyle.Default
			};
			styleCombo.SelectedIndexChanged += (sender, e) =>
			{
				if (child != null)
					child.WindowStyle = styleCombo.SelectedValue;
			};
			return styleCombo;
		}

		Control DisplayModeDropDown()
		{
			dialogDisplayModeDropDown = new EnumDropDown<DialogDisplayMode?>();
			dialogDisplayModeDropDown.Bind(c => c.Enabled, typeRadio, Binding.Property((RadioButtonList t) => t.SelectedKey).ToBool("dialog"));
			dialogDisplayModeDropDown.SelectedValueChanged += (sender, e) =>
			{
				if (child is Dialog dlg)
					dlg.DisplayMode = dialogDisplayModeDropDown.SelectedValue ?? DialogDisplayMode.Default;
			};
			return dialogDisplayModeDropDown;
		}

		Control WindowState()
		{
			stateCombo = new EnumRadioButtonList<WindowState>
			{
				SelectedValue = Forms.WindowState.Normal
			};
			stateCombo.SelectedIndexChanged += (sender, e) =>
			{
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
			resizableCheckBox.CheckedChanged += (sender, e) =>
			{
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
			maximizableCheckBox.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.Maximizable = maximizableCheckBox.Checked ?? false;
			};
			return maximizableCheckBox;
		}

		Control MovableByWindowBackground()
		{
			movableByWindowBackgroundCheckBox = new CheckBox
			{
				Text = "MovableByWindowBackground",
				Checked = false
			};
			movableByWindowBackgroundCheckBox.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.MovableByWindowBackground = movableByWindowBackgroundCheckBox.Checked ?? false;
			};
			return movableByWindowBackgroundCheckBox;
		}

		Control Minimizable()
		{
			minimizableCheckBox = new CheckBox
			{
				Text = "Minimizable",
				Checked = true
			};
			minimizableCheckBox.CheckedChanged += (sender, e) =>
			{
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
			showInTaskBarCheckBox.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.ShowInTaskbar = showInTaskBarCheckBox.Checked ?? false;
			};
			return showInTaskBarCheckBox;
		}

		Control CreateCanFocus()
		{
			canFocusCheckBox = new CheckBox {
				Text = "CanFocus",
				Checked = true
			};
			canFocusCheckBox.CheckedChanged += (sender, e) => {
				var form = child as Form;
				if (form != null)
					form.CanFocus = canFocusCheckBox.Checked ?? false;
			};
			return canFocusCheckBox;
		}

		Control TopMost()
		{
			topMostCheckBox = new CheckBox
			{
				Text = "Top Most",
				Checked = false
			};
			topMostCheckBox.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.Topmost = topMostCheckBox.Checked ?? false;
			};
			return topMostCheckBox;
		}

		Control VisibleCheckbox()
		{
			visibleCheckBox = new CheckBox
			{
				Text = "Visible",
				Checked = true
			};
			visibleCheckBox.CheckedChanged += (sender, e) =>
			{
				if (child != null)
					child.Visible = visibleCheckBox.Checked ?? false;
			};
			return visibleCheckBox;
		}

		Control CreateShowActivatedCheckbox()
		{
			showActivatedCheckBox = new CheckBox
			{
				Text = "Show Activated",
				Checked = true
			};
			showActivatedCheckBox.CheckedChanged += (sender, e) =>
			{
				var form = child as Form;
				if (form != null)
					form.ShowActivated = showActivatedCheckBox.Checked ?? false;
			};
			return showActivatedCheckBox;

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

			var left = new NumericStepper();
			left.Bind(c => c.Enabled, setLocationCheckBox, c => c.Checked);
			left.ValueBinding.Bind(() => initialLocation.X, v => initialLocation.X = (int)v);

			var top = new NumericStepper();
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

		bool setInitialSize;
		Size initialSize = new Size(300, 300);

		Control CreateSizeControls()
		{
			var setClientSize = new CheckBox { Text = "Size" };
			setClientSize.CheckedBinding.Bind(() => setInitialSize, v => setInitialSize = v ?? false);

			var left = new NumericStepper();
			left.Bind(c => c.Enabled, setClientSize, c => c.Checked);
			left.ValueBinding.Bind(() => initialSize.Width, v => initialSize.Width = (int)v);

			var top = new NumericStepper();
			top.Bind(c => c.Enabled, setClientSize, c => c.Checked);
			top.ValueBinding.Bind(() => initialSize.Height, v => initialSize.Height = (int)v);

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

		bool setInitialClientSize;
		Size initialClientSize = new Size(300, 300);

		Control CreateClientSizeControls()
		{
			var setClientSize = new CheckBox { Text = "ClientSize" };
			setClientSize.CheckedBinding.Bind(() => setInitialClientSize, v => setInitialClientSize = v ?? false);

			var left = new NumericStepper();
			left.Bind(c => c.Enabled, setClientSize, c => c.Checked);
			left.ValueBinding.Bind(() => initialClientSize.Width, v => initialClientSize.Width = (int)v);

			var top = new NumericStepper();
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

		Size initialMinimumSize = new Size(300, 300);
		bool setInitialMinimumSize;

		Control CreateMinimumSizeControls()
		{
			var setMinimumSize = new CheckBox { Text = "MinimumSize" };
			setMinimumSize.CheckedBinding.Bind(() => setInitialMinimumSize, v =>
			{
				setInitialMinimumSize = v ?? false;
				if (v == true && child != null)
					child.MinimumSize = initialMinimumSize;
			});

			var width = new NumericStepper();
			width.Bind(c => c.Enabled, setMinimumSize, c => c.Checked);
			width.ValueBinding.Bind(() => initialMinimumSize.Width, v =>
			{
				initialMinimumSize.Width = (int)v;
				if (child != null)
					child.MinimumSize = initialMinimumSize;
			});

			var height = new NumericStepper();
			height.Bind(c => c.Enabled, setMinimumSize, c => c.Checked);
			height.ValueBinding.Bind(() => initialMinimumSize.Height, v => initialMinimumSize.Height = (int)v);

			return new StackLayout
			{
				Orientation = Orientation.Horizontal,
				Items = 
				{
					setMinimumSize,
					width,
					height
				}
			};

		}

		void CreateChild()
		{
			if (child != null)
				child.Close();
			Action show;

			var layout = new DynamicLayout();
			layout.Add(null);
			layout.AddCentered(TestChangeSizeButton());
			layout.AddCentered(TestChangeClientSizeButton());
			layout.AddCentered(SendToBackButton());
			layout.AddCentered(CreateCancelClose());
			layout.AddCentered(CloseButton());

			if (typeRadio.SelectedKey == "form")
			{
				var form = new Form();
				child = form;
				show = form.Show;
				form.ShowActivated = showActivatedCheckBox.Checked == true;
				form.CanFocus = canFocusCheckBox.Checked == true;
			}
			else
			{
				var dialog = new Dialog();

				dialog.DefaultButton = new Button { Text = "Default" };
				dialog.DefaultButton.Click += (sender, e) => Log.Write(dialog, "Default button clicked");

				dialog.AbortButton = new Button { Text = "Abort" };
				dialog.AbortButton.Click += (sender, e) => Log.Write(dialog, "Abort button clicked");

				layout.AddSeparateRow(null, dialog.DefaultButton, dialog.AbortButton, null);

				child = dialog;
				show = dialog.ShowModal;

				dialog.DisplayMode = dialogDisplayModeDropDown.SelectedValue ?? DialogDisplayMode.Default;
			}

			layout.Add(null);
			child.Padding = 20;
			child.Content = layout;

			child.OwnerChanged += child_OwnerChanged;
			child.WindowStateChanged += child_WindowStateChanged;
			child.Closed += child_Closed;
			child.Closing += child_Closing;
			child.Shown += child_Shown;
			child.GotFocus += child_GotFocus;
			child.LostFocus += child_LostFocus;
			child.LocationChanged += child_LocationChanged;
			child.SizeChanged += child_SizeChanged;

			child.Title = "Child Window";
			child.WindowStyle = styleCombo.SelectedValue;
			child.WindowState = stateCombo.SelectedValue;
			child.Topmost = topMostCheckBox.Checked ?? false;
			child.Resizable = resizableCheckBox.Checked ?? false;
			child.Maximizable = maximizableCheckBox.Checked ?? false;
			child.Minimizable = minimizableCheckBox.Checked ?? false;
			child.ShowInTaskbar = showInTaskBarCheckBox.Checked ?? false;
			child.MovableByWindowBackground = movableByWindowBackgroundCheckBox.Checked ?? false;
			if (setInitialLocation)
				child.Location = initialLocation;
			if (setInitialClientSize)
				child.ClientSize = initialClientSize;
			if (setInitialSize)
				child.Size = initialSize;
			if (setInitialMinimumSize)
				child.MinimumSize = initialMinimumSize;
			if (setOwnerCheckBox.Checked ?? false)
				child.Owner = ParentWindow;
			if (createMenuBar.Checked ?? false)
				child.Menu = CreateMenuBar();
			bringToFrontButton.Enabled = true;
			show();
			//visibleCheckBox.Checked = child?.Visible == true; // child will be null after it is shown
			// show that the child is now referenced
			Log.Write(null, "Open Windows: {0}", Application.Instance.Windows.Count());
		}

		void child_Closed(object sender, EventArgs e)
		{
			Log.Write(child, "Closed");
			child.OwnerChanged -= child_OwnerChanged;
			child.WindowStateChanged -= child_WindowStateChanged;
			child.Closed -= child_Closed;
			child.Closing -= child_Closing;
			child.Shown -= child_Shown;
			child.GotFocus -= child_GotFocus;
			child.LostFocus -= child_LostFocus;
			child.LocationChanged -= child_LocationChanged;
			child.SizeChanged -= child_SizeChanged;
			bringToFrontButton.Enabled = false;
			child.Unbind();
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
				//child.Visible = false;
				Log.Write(child, "Cancelled Close");
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

		static void child_OwnerChanged(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "OwnerChanged: {0}", child.Owner);
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
			control.Click += (sender, e) =>
			{
				if (child != null)
					child.BringToFront();
			};
			return control;
		}

		Control CloseButton()
		{
			var control = new Button { Text = "Close Window" };
			control.Click += (sender, e) =>
			{
				if (child != null)
					child.Close();
			};
			return control;
		}

		Control SendToBackButton()
		{
			var control = new Button { Text = "Send to Back" };
			control.Click += (sender, e) =>
			{
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