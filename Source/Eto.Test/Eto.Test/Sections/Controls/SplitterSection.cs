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
			var layout = new DynamicLayout();
			layout.Add(null);
			layout.AddCentered(Test1WithSize());
			layout.AddCentered(Test1AutoSize());
			layout.AddCentered(Test1WithFullScreenAndSize());
			layout.AddCentered(Test1FullScreenAndAutoSize());
			layout.AddCentered(Test2WithSize());
			layout.AddCentered(Test2AutoSize());
			layout.Add(null);
			Content = layout;
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
			Label[] status = { new Label(), new Label(), new Label(), new Label(), new Label() };
			var statusLayout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			statusLayout.BeginHorizontal();
			for (var i = 0; i < status.Length; ++i)
				statusLayout.Add(status[i], xscale: true);
			statusLayout.EndHorizontal();

			// Main panel
			var mainPanel = new Panel();
			layoutContent(status, mainPanel);

			// Form's content
			var layout = new DynamicLayout();
			layout.Add(mainPanel, yscale: true);
			layout.Add(statusLayout);
			layout.Create();
			var form = new Form { Content = layout };
			if (setSize)
				form.ClientSize = new Size(800, 600);
			return form;
		}

		private static void LayoutContent(Label[] status, Panel mainPanel)
		{
			var count = 0;
			var splitLayout = new SplitLayout();
			mainPanel.Content = splitLayout.Layout(
				i => {
					var button = new Button { Text = "Click to update status " + i, BackgroundColor = splitLayout.PanelColors[i] };
					button.Click += (s, e) => status[i].Text = "New count: " + (count++);
					return button;
				});
		}

		private static void LayoutContent2(Label[] status, Panel mainPanel)
		{
			LayoutContent2(status, mainPanel, null);
		}

		private static void LayoutContent2(Label[] status, Panel mainPanel, Panel[] panels)
		{
			var splitLayout = new SplitLayout(panels);
			var isFullScreen = false;
			mainPanel.Content = splitLayout.Layout(
				i => {
					var button = new Button { Text = "Click to make full screen" + i, BackgroundColor = splitLayout.PanelColors[i] };
					button.Click += (s, e) => {
						if (isFullScreen)
							LayoutContent2(status, mainPanel, splitLayout.Panels); // recursive
						else
							mainPanel.Content = splitLayout.Panels[i];
						isFullScreen = !isFullScreen;
					};
					return button;
				});
		}

		private class SplitLayout
		{
			public Control Root { get; private set; }
			public Panel[] Panels { get; private set; }
			public Color[] PanelColors = { Colors.PaleTurquoise, Colors.Olive, Colors.NavajoWhite, Colors.Purple, Colors.Orange };

			public SplitLayout(Panel[] panels = null)
			{
				this.Panels = panels ?? new Panel[] { new Panel(), new Panel(), new Panel(), new Panel(), new Panel() };
			}
			
			public Control Layout(Func<int, Control>getContent)
			{
				// Add splitters like this:
				// |---------------------------
				// |        |      |          |
				// |  P0    |  P2  |   P4     |
				// | -------|      |          |  <== These are on MainPanel
				// |  P1    |------|          |
				// |        |  P3  |          |
				// |---------------------------
				// |         status0..4,      |  <== These are on StatusPanel
				// ----------------------------

				for (var i = 0; i < Panels.Length; ++i)
					Panels[i].Content = getContent(i);

				var p0_1 = new Splitter { Panel1 = Panels[0], Panel2 = Panels[1], Orientation = SplitterOrientation.Vertical, Position = 200 };
				var p2_3 = new Splitter { Panel1 = Panels[2], Panel2 = Panels[3], Orientation = SplitterOrientation.Vertical, Position = 200, FixedPanel = SplitterFixedPanel.Panel2 };
				var p01_23 = new Splitter { Panel1 = p0_1, Panel2 = p2_3, Orientation = SplitterOrientation.Horizontal, Position = 200 };
				var p0123_4 = new Splitter { Panel1 = p01_23, Panel2 = Panels[4], Orientation = SplitterOrientation.Horizontal, Position = 400 };
				return this.Root = p0123_4;
			}
		}

		static ComboBox ComboWithItems()
		{
			var combo = new ComboBox();
			combo.Items.Add("hello");
			combo.Items.Add("there");
			return combo;
		}

		static Form Test2(bool setSize)
		{
			var leftPane = new DynamicLayout { Padding = Padding.Empty, DefaultPadding = Padding.Empty };
			leftPane.AddColumn(new TreeGridView());

			var rightTop = new DynamicLayout();
			rightTop.AddColumn(ComboWithItems(), new Panel());

			var rightBottom = new DynamicLayout();
			rightBottom.AddRow(new ComboBox(), ComboWithItems(), new Button(), new CheckBox(), null);

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
}
