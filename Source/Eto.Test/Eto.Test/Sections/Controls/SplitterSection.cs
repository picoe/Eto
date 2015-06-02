using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Splitter))]
	public class SplitterSection : Panel
	{
		public SplitterSection()
		{
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.Add(null);
			layout.AddCentered(Test1WithSize());
			layout.AddCentered(Test1AutoSize());
			layout.AddCentered(Test1WithFullScreenAndSize());
			layout.AddCentered(Test1FullScreenAndAutoSize());
			layout.AddCentered(Test2WithSize());
			layout.AddCentered(Test2AutoSize());
			layout.AddCentered(TestDynamic());
			layout.AddCentered(TestInitResize());
			var xthemed = new CheckBox() { Text = "Use Themed Splitter" };
			layout.AddCentered(xthemed);
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
			var statusLayout = new DynamicLayout { };
			statusLayout.BeginHorizontal();
			for (var i = 0; i < status.Length; ++i)
				statusLayout.Add(status[i], xscale: true);
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
						var newTabpage = new TabPage
						{
							Text = "test",
							Content = new Splitter
							{
								Panel1 = new TreeView { Size = new Size(100, 100) },
								Panel2 = new GridView(),
								Orientation = SplitterOrientation.Horizontal,
								FixedPanel = SplitterFixedPanel.Panel1,
								Position = 100,
							}
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
				this.Panels = panels ?? new Panel[] {
					new Panel(), new Panel(), new Panel(), new Panel(), new Panel(), new Panel() };
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
						Orientation = SplitterOrientation.Vertical,
						Position = 200
					};
					// absolute position with height and second panel fixed (issue #309)
					var p2_3 = new Splitter
					{
						Panel1 = Panels[2],
						Panel2 = Panels[3],
						Orientation = SplitterOrientation.Vertical,
						FixedPanel = SplitterFixedPanel.Panel2,
						Position = 0,
						Height = 205 // ~ RelativePosition=200
					};
					// ratio mode (60%)
					var p4_5 = new Splitter
					{
						Panel1 = Panels[4],
						Panel2 = Panels[5],
						Orientation = SplitterOrientation.Vertical,
						FixedPanel = SplitterFixedPanel.None,
						RelativePosition = .6
					};
					// auto-size test
					var p01_23 = new Splitter
					{
						Panel1 = p0_1,
						Panel2 = p2_3,
						Orientation = SplitterOrientation.Horizontal,
					};
					// relative position with second panel fixed
					var p0123_45 = new Splitter
					{
						Panel1 = p01_23,
						Panel2 = p4_5,
						Orientation = SplitterOrientation.Horizontal,
						FixedPanel = SplitterFixedPanel.Panel2,
						RelativePosition = 150
					};
					return this.Root = p0123_45;
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
					Orientation = SplitterOrientation.Vertical,
					FixedPanel = SplitterFixedPanel.Panel2,
					Panel1 = rightTop,
					Panel2 = rightBottom,
					Position = 200,
				};

				var form = new Form
				{
					Padding = new Padding(5),
					Content = new Splitter
					{
						Orientation = SplitterOrientation.Horizontal,
						FixedPanel = SplitterFixedPanel.Panel1,
						BackgroundColor = Colors.Gray,
						Position = 200,
						Panel1 = leftPane,
						Panel2 = rightPane
					}
				};
				if (setSize)
					form.Size = new Size(600, 400);
				return form;
			}
		}

		static Control TestInitResize()
		{
			var control = new Button
			{
				Text = "Show splitter test of initial resize"
			};
			Func<Control> makebox = () =>
			{
				var area = new TextArea();
				area.SizeChanged += (s, e) =>
				{
					var split = area.Parent as Splitter;
					if (split == null)
						return;
					var size = area.Parent.Size;
					if (split.Orientation == SplitterOrientation.Horizontal)
						size.Width -= split.SplitterWidth;
					else
						size.Height -= split.SplitterWidth;
					if (size.Width <= 0 || size.Height <= 0)
						return;
					area.Text = string.Format(
						"W:{0} ({1}%)\r\nH:{2} ({3}%)",
						area.Width, (area.Width * 200 + size.Width) / (size.Width + size.Width),
						area.Height, (area.Height * 200 + size.Height) / (size.Height + size.Height));
				};
				return area;
			};
			Func<int, Form> makeform = (i) =>
			{
				var wa = new Rectangle(Screen.PrimaryScreen.WorkingArea);
				var form = new Form
				{
					Title = "Test Form #" + (i + 1).ToString(),
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
					var middle = new Splitter
					{
						FixedPanel = SplitterFixedPanel.Panel2,
						Width = 200,
						Position = 120 - main.SplitterWidth
					};
					var ltop = new Splitter
					{
						Orientation = SplitterOrientation.Vertical,
						Position = 80
					};
					var lbottom = new Splitter
					{
						Orientation = SplitterOrientation.Vertical,
						FixedPanel = SplitterFixedPanel.Panel2,
						RelativePosition = 80
					};
					var right = new Splitter
					{
						Orientation = SplitterOrientation.Vertical,
						FixedPanel = SplitterFixedPanel.None,
						Height = 300 + main.SplitterWidth,
						Position = 100 // ~33%
					};
					var center = new Splitter
					{
						FixedPanel = SplitterFixedPanel.None,
						RelativePosition = .4
					};
					main.Panel1 = ltop;
					main.Panel2 = middle;
					ltop.Panel1 = makebox();
					ltop.Panel2 = lbottom;
					lbottom.Panel1 = makebox();
					lbottom.Panel2 = makebox();
					middle.Panel1 = center;
					middle.Panel2 = right;
					right.Panel1 = makebox();
					right.Panel2 = makebox();
					center.Panel1 = makebox();
					center.Panel2 = makebox();
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
	}
}
