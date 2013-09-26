using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	public class SplitterSection : WindowSectionMethod
	{
		protected override Forms.Window GetWindow()
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

			Label[] status = new Label[] { new Label(), new Label(), new Label(), new Label(), new Label() };

			// Status bar
			var statusPanel = new Panel { };
			var statusLayout = new DynamicLayout(Padding.Empty, Size.Empty);
			statusLayout.BeginHorizontal();
			for (var i = 0; i < status.Length; ++i)
				statusLayout.Add(status[i], xscale: true);
			statusLayout.EndHorizontal();
			statusPanel.Content = statusLayout;

			// Splitter windows
			Panel[] p = new Panel[] { new Panel(), new Panel(), new Panel(), new Panel(), new Panel() };
			var colors = new Color[] { Colors.PaleTurquoise, Colors.Olive, Colors.NavajoWhite, Colors.Purple, Colors.Orange };
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
			var p01_23 = new Splitter { Panel1 = p0_1, Panel2 = p2_3, Orientation = SplitterOrientation.Horizontal, Position = 200};
			var p0123_4 = new Splitter { Panel1 = p01_23, Panel2 = p[4], Orientation = SplitterOrientation.Horizontal, Position = 400 };

			// Main panel
			var mainPanel = new Panel();
			mainPanel.Content = p0123_4;

			// Form's content
			var layout = new DynamicLayout();
			layout.Add(mainPanel, yscale: true);
			layout.Add(statusPanel);
			layout.Generate();
			var form = new Form 
			{ 
				Size = new Size(800, 600),
				Content = layout
			};
			return form;
		}
	}
}
