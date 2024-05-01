namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Splitter))]
	public class SplitterSection : Panel
	{
		public SplitterSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.Add(null);
			var xthemed = new CheckBox { Text = "Use Themed Splitter" };
			layout.AddCentered(xthemed);
			layout.AddSeparateRow(null, Test1WithSize(), Test1AutoSize(), null);
			layout.AddSeparateRow(null, Test1WithFullScreenAndSize(), Test1FullScreenAndAutoSize(), null);
			layout.AddSeparateRow(null, Test2WithSize(), Test2AutoSize(), null);
			layout.AddCentered(TestDynamic());
			layout.AddCentered(TestInitResize());
			layout.AddCentered(TestHiding());
			layout.Add(null);
			Content = layout;

			xthemed.CheckedChanged += (s, e) =>
			{
				useThemed = xthemed.Checked == true;
			};
		}

		static bool useThemed;
		static Platform themedPlatform;
		static IDisposable Context
		{
			get
			{
				if (!useThemed)
					return Platform.Instance.Context;
				if (themedPlatform == null)
				{
					themedPlatform = (Platform)Activator.CreateInstance(Platform.Instance.GetType());
					themedPlatform.Add<Splitter.IHandler>(() => new Eto.Forms.ThemedControls.ThemedSplitterHandler());
				}
				return themedPlatform.Context;
			}
		}


		static Control Test1WithSize()
		{
			var control = new Button { Text = "Show splitter test 1 with ClientSize" };
			control.Click += (sender, e) => Test1(true, LayoutContent).Show();
			return control;
		}

		static Control Test1AutoSize()
		{
			var control = new Button { Text = "Show splitter test 1 with auto size" };
			control.Click += (sender, e) => Test1(false, LayoutContent).Show();
			return control;
		}

		static Control Test1WithFullScreenAndSize()
		{
			var control = new Button { Text = "Show splitter test 1 with Fullscreen and ClientSize" };
			control.Click += (sender, e) => Test1(true, LayoutContent2).Show();
			return control;
		}

		static Control Test1FullScreenAndAutoSize()
		{
			var control = new Button { Text = "Show splitter test 1 with FullScreen and auto size" };
			control.Click += (sender, e) => Test1(false, LayoutContent2).Show();
			return control;
		}


		static Control Test2WithSize()
		{
			var control = new Button { Text = "Show splitter test 2 with ClientSize" };
			control.Click += (sender, e) => Test2(true).Show();
			return control;
		}

		static Control Test2AutoSize()
		{
			var control = new Button { Text = "Show splitter test 2 with auto size" };
			control.Click += (sender, e) => Test2(false).Show();
			return control;
		}

		static Form Test1(bool setSize, Action<Label[], Panel> layoutContent)
		{
			// Status bar
			Label[] status = {
				new Label(), new Label(), new Label(),
				new Label(), new Label(), new Label()
			};
			var statusLayout = new DynamicLayout();
			statusLayout.BeginHorizontal();
			for (var i = 0; i < status.Length; ++i)
				statusLayout.Add(status[i], true);
			statusLayout.EndHorizontal();

			// Main panel
			var mainPanel = new Panel();
			layoutContent(status, mainPanel);

			// Form's content
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			layout.Add(mainPanel, yscale: true);
			layout.Add(statusLayout);
			layout.Create();
			var form = new Form { Content = layout };
			if (setSize)
				form.ClientSize = new Size(800, 600);
			return form;
		}

		static Control TestDynamic()
		{
			var control = new Button { Text = "Dynamic splitter creation" };
			control.Click += (sender, e) =>
			{
				var tabcontrol = new TabControl();
				tabcontrol.Pages.Add(new TabPage { Text = "Index" });

				var addTabButton = new Button { Text = "Add Tab With Splitter" };
				addTabButton.Click += (ss, ee) =>
				{
					using (Context)
					{
						var splitter = new Splitter
						{
							Panel1 = new TreeGridView { Size = new Size(100, 100) },
							Panel2 = new GridView(),
							Orientation = Orientation.Horizontal,
							FixedPanel = SplitterFixedPanel.Panel1,
							Position = 100,
						};
						LogEvents(splitter);
						var newTabpage = new TabPage
						{
							Text = "test",
							Content = splitter
						};
						tabcontrol.Pages.Add(newTabpage);
						tabcontrol.SelectedPage = newTabpage;
					}
				};

				var form = new Form
				{
					Padding = new Padding(5),
					Content = new TableLayout(
						TableLayout.AutoSized(addTabButton, centered: true),
						tabcontrol
					)
				};
				form.Size = new Size(600, 400);
				form.Show();
			};
			return control;
		}

		static void LayoutContent(Label[] status, Panel mainPanel)
		{
			var count = 0;
			var splitLayout = new SplitLayout();
			mainPanel.Content = splitLayout.Layout(
				i =>
				{
					var button = new Button
					{
						Text = "Click to update status " + i,
						BackgroundColor = splitLayout.PanelColors[i]
					};
					button.Click += (s, e) => status[i].Text = "New count: " + (count++);
					return button;
				});
		}

		static void LayoutContent2(Label[] status, Panel mainPanel)
		{
			LayoutContent2(status, mainPanel, null);
		}

		static void LayoutContent2(Label[] status, Panel mainPanel, Panel[] panels)
		{
			var splitLayout = new SplitLayout(panels);
			var isFullScreen = false;
			mainPanel.Content = splitLayout.Layout(
				i =>
				{
					var button = new Button { Text = "Click to make full screen" + i, BackgroundColor = splitLayout.PanelColors[i] };
					button.Click += (s, e) =>
					{
						if (isFullScreen)
							LayoutContent2(status, mainPanel, splitLayout.Panels); // recursive
						else
							mainPanel.Content = splitLayout.Panels[i];
						isFullScreen = !isFullScreen;
					};
					return button;
				});
		}

		class SplitLayout

		{
			public Control Root { get; private set; }

			public Panel[] Panels { get; private set; }

			public Color[] PanelColors = {
				Colors.PaleTurquoise, Colors.Olive, Colors.NavajoWhite,
				Colors.Purple, Colors.Orange, Colors.Aqua };

			public SplitLayout(Panel[] panels = null)
			{
				Panels = panels ?? new[]
				{
					new Panel(), new Panel(), new Panel(), new Panel(), new Panel(), new Panel()
				};
			}

			public Control Layout(Func<int, Control> getContent)
			{
				// Add splitters like this:
				// |---------------------------
				// |        |      |          |
				// |  P0    |  P2  |   P4     |
				// | -------|      |----------|  <== These are on MainPanel
				// |  P1    |------|   P5     |
				// |        |  P3  |          |
				// |---------------------------
				// |         status0..5,      |  <== These are on StatusPanel
				// ----------------------------

				for (var i = 0; i < Panels.Length; ++i)
					Panels[i].Content = getContent(i);

				using (Context)
				{
					// basic test (compatible mode)
					var p0_1 = new Splitter
					{
						Panel1 = Panels[0],
						Panel2 = Panels[1],
						Orientation = Orientation.Vertical,
						Position = 200
					};
					LogEvents(p0_1);
					// absolute position with height and second panel fixed (issue #309)
					var p2_3 = new Splitter
					{
						Panel1 = Panels[2],
						Panel2 = Panels[3],
						Orientation = Orientation.Vertical,
						FixedPanel = SplitterFixedPanel.Panel2,
						//Position = 0,
						Height = 205 // ~ RelativePosition=200
					};
					LogEvents(p2_3);
					// ratio mode (60%)
					var p4_5 = new Splitter
					{
						Panel1 = Panels[4],
						Panel2 = Panels[5],
						Orientation = Orientation.Vertical,
						FixedPanel = SplitterFixedPanel.None,
						RelativePosition = .6
					};
					LogEvents(p4_5);
					// auto-size test
					var p01_23 = new Splitter
					{
						Panel1 = p0_1,
						Panel2 = p2_3,
						Orientation = Orientation.Horizontal,
					};
					LogEvents(p01_23);
					// relative position with second panel fixed
					var p0123_45 = new Splitter
					{
						Panel1 = p01_23,
						Panel2 = p4_5,
						Orientation = Orientation.Horizontal,
						FixedPanel = SplitterFixedPanel.Panel2,
						RelativePosition = 150
					};
					LogEvents(p0123_45);
					return Root = p0123_45;
				}
			}
		}

		static DropDown ComboWithItems()
		{
			var combo = new DropDown();
			combo.Items.Add("hello");
			combo.Items.Add("there");
			return combo;
		}

		static Form Test2(bool setSize)
		{
			var leftPane = new DynamicLayout { DefaultSpacing = new Size(5, 5) };
			leftPane.AddColumn(new TreeGridView());

			var rightTop = new DynamicLayout();
			rightTop.AddColumn(ComboWithItems(), new Panel());

			var rightBottom = new DynamicLayout();
			rightBottom.AddRow(new DropDown(), ComboWithItems(), new Button(), new CheckBox(), null);

			using (Context)
			{
				var rightPane = new Splitter
				{
					Orientation = Orientation.Vertical,
					FixedPanel = SplitterFixedPanel.Panel2,
					Panel1 = rightTop,
					Panel2 = rightBottom,
					Position = 200,
				};
				LogEvents(rightPane);

				var mainPane = new Splitter
				{
					Orientation = Orientation.Horizontal,
					FixedPanel = SplitterFixedPanel.Panel1,
					BackgroundColor = Colors.Gray,
					Position = 200,
					Panel1 = leftPane,
					Panel2 = rightPane
				};
				LogEvents(mainPane);

				var form = new Form
				{
					Padding = new Padding(5),
					Content = mainPane
				};
				if (setSize)
					form.Size = new Size(600, 400);
				return form;
			}
		}

		static Control TestInitResize()
		{
			var app = Application.Instance;
			var control = new Button
			{
				Text = "Show splitter test of initial resize"
			};
			Func<Splitter, Control> makebox = split =>
			{
				var area = new TextArea();
				area.SizeChanged += (s, e) =>
				{
					var size = area.VisualParent.Size;
					if (split.Orientation == Orientation.Horizontal)
						size.Width -= split.SplitterWidth;
					else
						size.Height -= split.SplitterWidth;
					if (size.Width <= 0 || size.Height <= 0)
						return;
					app.AsyncInvoke(() =>
					{
						area.Text = string.Format(
							"W:{0} ({1}%)\r\nH:{2} ({3}%)",
							area.Width, (area.Width * 200 + size.Width) / (size.Width + size.Width),
							area.Height, (area.Height * 200 + size.Height) / (size.Height + size.Height));
					});
				};
				return area;
			};
			Func<int, Form> makeform = i =>
			{
				var wa = new Rectangle(Screen.PrimaryScreen.WorkingArea);
				var form = new Form
				{
					Title = "Test Form #" + (i + 1),
					Bounds = i == 0
					? new Rectangle(wa.X + 20, wa.Y + 20, wa.Width / 3, wa.Height / 3)
					: i == 1
					? new Rectangle(wa.X + 20, wa.Y + 40 + wa.Height / 3, wa.Width / 3, wa.Height * 2 / 3 - 60)
					: new Rectangle(wa.X + wa.Width / 3 + 40, wa.Y + 20, wa.Width * 2 / 3 - 60, wa.Height - 40)
				};
				using (Context)
				{
					var main = new Splitter
					{
						Position = 80
					};
					LogEvents(main);
					var middle = new Splitter
					{
						FixedPanel = SplitterFixedPanel.Panel2,
						Width = 200,
						Position = 120 - main.SplitterWidth
					};
					LogEvents(middle);
					var ltop = new Splitter
					{
						Orientation = Orientation.Vertical,
						Position = 80
					};
					LogEvents(ltop);
					var lbottom = new Splitter
					{
						Orientation = Orientation.Vertical,
						FixedPanel = SplitterFixedPanel.Panel2,
						RelativePosition = 80
					};
					LogEvents(lbottom);
					var right = new Splitter
					{
						Orientation = Orientation.Vertical,
						FixedPanel = SplitterFixedPanel.None,
						Height = 300 + main.SplitterWidth,
						Position = 100 // ~33%
					};
					LogEvents(right);
					var center = new Splitter
					{
						FixedPanel = SplitterFixedPanel.None,
						RelativePosition = .4
					};
					LogEvents(center);
					main.Panel1 = ltop;
					main.Panel2 = middle;
					ltop.Panel1 = makebox(ltop);
					ltop.Panel2 = lbottom;
					lbottom.Panel1 = makebox(lbottom);
					lbottom.Panel2 = makebox(lbottom);
					middle.Panel1 = center;
					middle.Panel2 = right;
					right.Panel1 = makebox(right);
					right.Panel2 = makebox(right);
					center.Panel1 = makebox(center);
					center.Panel2 = makebox(center);
					form.Content = main;
				}
				form.Show();
				return form;
			};
			control.Click += (sender, e) =>
			{
				var forms = new Form[3];
				for (int i = 0; i < 3; i++)
				{
					forms[i] = makeform(i);
					forms[i].Closed += (fs, fe) =>
					{
						var all = forms;
						forms = null;
						if (all != null)
							for (int j = 0; j < 3; j++)
								if (all[j] != fs)
									all[j].Close();
					};
				}
			};
			return control;
		}

		enum PanelVisible
		{
			Visible,
			Invisible,
			Null
		}

		// should support all transitions
		// Visible > Invisible
		// Invisible > Null
		// Null > Visible
		// Visible > Null
		// Null > Invisible
		// Invisible > Visible
		static PanelVisible[] modes = {
			PanelVisible.Visible,
			PanelVisible.Invisible,
			PanelVisible.Null,
			PanelVisible.Visible,
			PanelVisible.Null,
			PanelVisible.Invisible,
		};


		Button TestHiding()
		{
			var control = new Button { Text = "Test Hiding" };
			control.Click += (sender, e) =>
			{
				var form = new Form();
				using (Context)
				{
					var rnd = new Random();
					var splitter = new Splitter
					{
						Orientation = Orientation.Horizontal,
						FixedPanel = SplitterFixedPanel.None,
						RelativePosition = 0.5,
						Panel1 = new Panel { Padding = 20, BackgroundColor = Colors.Red, Content = new Panel { BackgroundColor = Colors.White, Size = new Size(200, 400) } },
						Panel2 = new Panel { Padding = 20, BackgroundColor = Colors.Blue, Content = new Panel { BackgroundColor = Colors.White, Size = new Size(200, 400) } }
					};
					LogEvents(splitter);

					var showPanel1 = new CheckBox { Text = "Panel1.Visible" };
					showPanel1.CheckedBinding.Bind(splitter.Panel1, r => r.Visible);

					var showPanel2 = new CheckBox { Text = "Panel2.Visible" };
					showPanel2.CheckedBinding.Bind(splitter.Panel2, r => r.Visible);

					var fixedPanel = new EnumDropDown<SplitterFixedPanel>();
					fixedPanel.SelectedValueBinding.Bind(splitter, r => r.FixedPanel);

					var orientation = new EnumDropDown<Orientation>();
					orientation.SelectedValueBinding.Bind(splitter, r => r.Orientation);


					int count = 0;
					Control CreatePanel(string desc, ref int mode, Color color)
					{
						mode++;
						if (mode >= modes.Length)
							mode = 0;
						var vis = modes[mode];
						switch (vis)
						{
							case PanelVisible.Visible:
							case PanelVisible.Invisible:
								bool isVisible = vis == PanelVisible.Visible;
								Log.Write(this, $"Replacing {desc} to a control with Visible = {isVisible} (mode {mode})");
								return new Panel { Padding = 20, BackgroundColor = color, Visible = isVisible, Content = new Panel { BackgroundColor = Colors.White, Content = $"Count: {count++}" } };
							default:
								Log.Write(this, $"Replacing {desc} with null (mode {mode})");
								return null;
						}
					}
					int panel1Mode = 0;
					int panel2Mode = 0;
					var replacePanel1Button = new Button { Text = "Replace Panel1" };
					replacePanel1Button.Click += (s, ee) =>
					{
						showPanel1.Unbind();
						splitter.Panel1 = CreatePanel("Panel1", ref panel1Mode, Colors.Red);
						showPanel1.CheckedBinding.Bind(splitter.Panel1, r => r.Visible);
					};

					var replacePanel2Button = new Button { Text = "Replace Panel2" };
					replacePanel2Button.Click += (s, ee) =>
					{
						showPanel2.Unbind();
						splitter.Panel2 = CreatePanel("Panel2", ref panel2Mode, Colors.Blue);
						showPanel2.CheckedBinding.Bind(splitter.Panel2, r => r.Visible);
					};


					var splitPanel = new Panel { Content = splitter };

					var showSplitter = new CheckBox { Text = "Show Splitter", Checked = true };
					showSplitter.CheckedChanged += (sender2, e2) =>
					{
						if (showSplitter.Checked == true)
							splitPanel.Content = splitter;
						else
							splitPanel.Content = null;
					};

					var buttons1 = TableLayout.Horizontal(null, showSplitter, showPanel1, showPanel2, fixedPanel, orientation, null);
					var buttons2 = TableLayout.Horizontal(null, replacePanel1Button, replacePanel2Button, null);

					form.Content = new StackLayout
					{
						HorizontalContentAlignment = HorizontalAlignment.Stretch,
						Items =
						{
							buttons1,
							buttons2,
							new StackLayoutItem(splitPanel, true)
						}
					};
				}
				form.Show();
			};

			return control;
		}

		static void LogEvents(Splitter splitter)
		{
			splitter.PositionChanged += (sender, e) => Log.Write(sender, $"PositionChanged: {splitter.Position}");
			splitter.PositionChanging += (sender, e) => Log.Write(sender, $"PositionChanging: New: {e.NewPosition}, Current: {splitter.Position}");
			splitter.PositionChangeStarted += (sender, e) => Log.Write(sender, $"PositionChangeStarted");
			splitter.PositionChangeCompleted += (sender, e) => Log.Write(sender, $"PositionChangeCompleted");
		}
	}
}
