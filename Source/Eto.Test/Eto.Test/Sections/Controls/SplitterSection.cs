using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class SplitterSection : Panel
	{
		public SplitterSection()
		{
			var layout = new DynamicLayout();
			layout.Add(null);
			layout.AddCentered(Test1WithSize());
			layout.AddCentered(Test1AutoSize());
			layout.AddCentered(Test2WithSize());
			layout.AddCentered(Test2AutoSize());
			layout.Add(null);
			Content = layout;
		}

		static Control Test1WithSize()
		{
			var control = new Button { Text = "Show splitter test 1 with ClientSize" };
			control.Click += (sender, e) => Test1(true).Show();
			return control;
		}

		static Control Test1AutoSize()
		{
			var control = new Button { Text = "Show splitter test 1 with auto size" };
			control.Click += (sender, e) => Test1(false).Show();
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

		static Form Test1(bool setSize)
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

			Label[] status = { new Label(), new Label(), new Label(), new Label(), new Label() };

			// Status bar
			var statusLayout = new DynamicLayout(Padding.Empty, Size.Empty);
			statusLayout.BeginHorizontal();
			for (var i = 0; i < status.Length; ++i)
				statusLayout.Add(status[i], xscale: true);
			statusLayout.EndHorizontal();

			// Splitter windows
			Panel[] p = { new Panel(), new Panel(), new Panel(), new Panel(), new Panel() };
			Color[] colors = { Colors.PaleTurquoise, Colors.Olive, Colors.NavajoWhite, Colors.Purple, Colors.Orange };
			var count = 0;
			for (var i = 0; i < p.Length; ++i)
			{
				var temp = i;
				//p[i].BackgroundColor = colors[i];
				var button = new Button { Text = "Click to update status " + i.ToString(), BackgroundColor = colors[i] };
				button.Click += (s, e) => status[temp].Text = "New count: " + (count++).ToString();
				p[i].Content = button;
			}

			var p0_1 = new Splitter { Panel1 = p[0], Panel2 = p[1], Orientation = SplitterOrientation.Vertical, Position = 200 };
			var p2_3 = new Splitter { Panel1 = p[2], Panel2 = p[3], Orientation = SplitterOrientation.Vertical, Position = 200 };
			var p01_23 = new Splitter { Panel1 = p0_1, Panel2 = p2_3, Orientation = SplitterOrientation.Horizontal, Position = 200 };
			var p0123_4 = new Splitter { Panel1 = p01_23, Panel2 = p[4], Orientation = SplitterOrientation.Horizontal, Position = 400 };

			// Main panel
			var mainPanel = new Panel();
			mainPanel.Content = p0123_4;

			// Form's content
			var layout = new DynamicLayout();
			layout.Add(mainPanel, yscale: true);
			layout.Add(statusLayout);
			layout.Generate();
			var form = new Form { Content = layout };
			if (setSize)
				form.ClientSize = new Size(800, 600);
			return form;
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
