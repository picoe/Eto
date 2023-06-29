using System.Runtime.CompilerServices;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Windows")]
	public class WindowsSection : Panel, INotifyPropertyChanged
	{
		Window child;
		Button bringToFrontButton;
		EnumRadioButtonList<WindowStyle> styleCombo;
		EnumRadioButtonList<WindowState> stateCombo;
		EnumRadioButtonList<WindowType> typeRadio;
		CheckBox setOwnerCheckBox;
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

			var typeControls = CreateTypeControls();

			layout.AddSeparateRow(null, Resizable(), Minimizable(), Maximizable(), MovableByWindowBackground(), null);
			layout.AddSeparateRow(null, ShowInTaskBar(), CloseableCheckBox(), TopMost(), VisibleCheckbox(), CreateShowActivatedCheckbox(), CreateCanFocus(), null);
			layout.AddSeparateRow(null, "Type", typeControls, null);
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

			DataContext = settings = new SettingsWindow();
		}

		SettingsWindow settings;

		enum WindowType
		{
			Form,
			FloatingForm,
			Dialog
		}

		class SettingsWindow : INotifyPropertyChanged
		{
			public bool ThreeState => true; // enable three state for these settings
			public bool? Resizable { get; set; }
			public bool? CanFocus { get; set; }
			public bool? Minimizable { get; set; }
			public bool? Maximizable { get; set; }
			public bool? Closeable { get; set; }
			public bool? MovableByWindowBackground { get; set; }
			public bool? ShowInTaskbar { get; set; }
			public bool? ShowActivated { get; set; }
			public bool? Topmost { get; set; }
			public WindowStyle WindowStyle { get; set; }
			public WindowType WindowType { get; set; }

			bool windowStyleEnabled;
			public bool WindowStyleEnabled
			{
				get => windowStyleEnabled;
				set => Set(ref windowStyleEnabled, value);
			}

			public event PropertyChangedEventHandler PropertyChanged;

			void Set<T>(ref T prop, T value, [CallerMemberName] string propertyName = null)
			{
				if (!Equals(prop, value))
				{
					prop = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
				}
			}
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
			menu.Items.Add(new SubMenuItem { Text = "&File", Items = {
				new ButtonMenuItem { Text = "Click Me" }
			}});
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
			typeRadio = new EnumRadioButtonList<WindowType>();

			typeRadio.SelectedValueBinding.BindDataContext((SettingsWindow m) => m.WindowType);
			typeRadio.BindDataContext(c => c.Enabled, Binding.Delegate((object m) => m is SettingsWindow));

			setOwnerCheckBox = new CheckBox { Text = "Set Owner" };
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
			var enableStyle = new CheckBox { Checked = false };
			styleCombo = new EnumRadioButtonList<WindowStyle>();
			styleCombo.SelectedValueBinding.BindDataContext((SettingsWindow w) => w.WindowStyle);

			var enabledBinding = Binding.Property<SettingsWindow, bool?>(w => w.WindowStyleEnabled);
			var enabledBindingElseTrue = enabledBinding.Convert<bool?>(r => r ?? true, r => r);
			var enabledBindingCanToggle = enabledBinding.Convert<bool?>(r => r != null);

			styleCombo.BindDataContext(c => c.Enabled, enabledBindingElseTrue);
			enableStyle.CheckedBinding.BindDataContext(enabledBindingElseTrue);
			enableStyle.BindDataContext(c => c.Enabled, enabledBindingCanToggle);
			return new TableLayout(new TableRow(enableStyle, styleCombo));
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
			var resizableCheckBox = new CheckBox { Text = "Resizable" };
			resizableCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			resizableCheckBox.CheckedBinding.BindDataContext((Window w) => w.Resizable);
			return resizableCheckBox;
		}

		Control Maximizable()
		{
			var maximizableCheckBox = new CheckBox { Text = "Maximizable" };
			maximizableCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			maximizableCheckBox.CheckedBinding.BindDataContext((Window w) => w.Maximizable);
			return maximizableCheckBox;
		}

		Control MovableByWindowBackground()
		{
			var movableByWindowBackgroundCheckBox = new CheckBox { Text = "MovableByWindowBackground" };
			movableByWindowBackgroundCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			movableByWindowBackgroundCheckBox.CheckedBinding.BindDataContext((Window w) => w.MovableByWindowBackground);
			return movableByWindowBackgroundCheckBox;
		}
		
		Control Minimizable()
		{
			var minimizableCheckBox = new CheckBox { Text = "Minimizable" };
			minimizableCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			minimizableCheckBox.CheckedBinding.BindDataContext((Window w) => w.Minimizable);
			return minimizableCheckBox;
		}

		Control ShowInTaskBar()
		{
			var showInTaskBarCheckBox = new CheckBox { Text = "Show In TaskBar" };
			showInTaskBarCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			showInTaskBarCheckBox.CheckedBinding.BindDataContext((Window w) => w.ShowInTaskbar);
			return showInTaskBarCheckBox;
		}

		Control CloseableCheckBox()
		{
			var closeableCheckBox = new CheckBox { Text = "Closeable" };
			closeableCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			closeableCheckBox.CheckedBinding.BindDataContext((Window w) => w.Closeable);
			return closeableCheckBox;
		}

		Control CreateCanFocus()
		{
			var canFocusCheckBox = new CheckBox { Text = "CanFocus" };
			canFocusCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			canFocusCheckBox.CheckedBinding.BindDataContext((Form w) => w.CanFocus);
			return canFocusCheckBox;
		}

		Control TopMost()
		{
			var topMostCheckBox = new CheckBox { Text = "Top Most" };
			topMostCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			topMostCheckBox.CheckedBinding.BindDataContext((Window w) => w.Topmost);
			return topMostCheckBox;
		}

		Control VisibleCheckbox()
		{
			var visibleCheckBox = new CheckBox { Text = "Visible" };
			visibleCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow w) => w.ThreeState);
			visibleCheckBox.CheckedBinding.BindDataContext((Window w) => w.Visible);
			return visibleCheckBox;
		}

		Control CreateShowActivatedCheckbox()
		{
			var showActivatedCheckBox = new CheckBox { Text = "ShowActivated" };
			showActivatedCheckBox.BindDataContext(c => c.ThreeState, (SettingsWindow s) => s.ThreeState);
			showActivatedCheckBox.CheckedBinding.BindDataContext((Form w) => w.ShowActivated);
			showActivatedCheckBox.Bind(c => c.Enabled, typeRadio, Binding.Property((RadioButtonList t) => t.SelectedKey).ToBool("dialog").Convert(v => !v));
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

			switch (settings.WindowType)
			{
				default:
				case WindowType.Form:
					{
						var form = new Form();
						child = form;
						show = form.Show;
						if (settings.ShowActivated != null)
							form.ShowActivated = settings.ShowActivated == true;
						if (settings.CanFocus != null)
							form.CanFocus = settings.CanFocus == true;
					}
					break;
				case WindowType.FloatingForm:
					{
						var form = new FloatingForm();
						child = form;
						show = form.Show;
						if (settings.ShowActivated != null)
							form.ShowActivated = settings.ShowActivated == true;
						if (settings.CanFocus != null)
							form.CanFocus = settings.CanFocus == true;
					}
					break;
				case WindowType.Dialog:
					{
						var dialog = new Dialog();

						dialog.DefaultButton = new Button { Text = "Default" };
						dialog.DefaultButton.Click += (sender, e) => Log.Write(dialog, "Default button clicked");

						dialog.AbortButton = new Button { Text = "Abort" };
						dialog.AbortButton.Click += (sender, e) => Log.Write(dialog, "Abort button clicked");

						layout.AddSeparateRow(null, dialog.DefaultButton, dialog.AbortButton, null);

						child = dialog;
						show = dialog.ShowModal;

						if (dialogDisplayModeDropDown.SelectedValue != null)
							dialog.DisplayMode = dialogDisplayModeDropDown.SelectedValue ?? DialogDisplayMode.Default;
					}
					break;
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
			if (styleCombo.Enabled)
				child.WindowStyle = styleCombo.SelectedValue;
			child.WindowState = stateCombo.SelectedValue;
			if (settings.Topmost != null)
				child.Topmost = settings.Topmost ?? false;
			if (settings.Resizable != null)
				child.Resizable = settings.Resizable ?? false;
			if (settings.Maximizable != null)
				child.Maximizable = settings.Maximizable ?? false;
			if (settings.Minimizable != null)
				child.Minimizable = settings.Minimizable ?? false;
			if (settings.ShowInTaskbar != null)
				child.ShowInTaskbar = settings.ShowInTaskbar ?? false;
			if (settings.Closeable != null)
				child.Closeable = settings.Closeable ?? false;
			if (settings.MovableByWindowBackground != null)
				child.MovableByWindowBackground = settings.MovableByWindowBackground ?? false;
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
			DataContext = child;
			show();
			//visibleCheckBox.Checked = child?.Visible == true; // child will be null after it is shown
			// show that the child is now referenced
			Log.Write(null, "Open Windows: {0}", Application.Instance.Windows.Count());
		}

		void child_Closed(object sender, EventArgs e)
		{
			Log.Write(child, "Closed");
			DataContext = settings;
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

		void child_Shown(object sender, EventArgs e)
		{
			var child = (Window)sender;
			Log.Write(child, "Shown");
			OnDataContextChanged(EventArgs.Empty);
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