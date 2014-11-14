using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	[Section("TableLayout", "Runtime Creation")]
	public class RuntimeSection : Panel
	{
		TableLayout mainTable;
		TableLayout middleTable;
		Panel rightSection;
		Panel topSection;
		bool toggle;

		public RuntimeSection()
		{
			var layout = new DynamicLayout();

			layout.AddCentered(ToggleButton());
			layout.Add(MainTable());

			Content = layout;
		}

		Control ToggleButton()
		{
			var control = new Button
			{
				Text = "Add Columns To Table"
			};

			Control left = null;

			control.Click += delegate
			{
				toggle = !toggle;
				SuspendLayout();
				if (toggle)
				{
					mainTable.Add(left = VerticalSection(), 0, 0);
					rightSection.Content = VerticalSection();
					middleTable.Add(HorizontalSection(), 0, 2);
					topSection.Content = HorizontalSection();
					control.Text = "Remove Columns";
				}
				else
				{
					if (left != null)
					{
						left.Detach();
						left = null;
					}
					rightSection.Content = null;
					middleTable.Add(null, 0, 2);
					topSection.Content = null;
					control.Text = "Add Columns To Table";
				}
				ResumeLayout();
			};

			return control;
		}

		Control MainTable()
		{
			mainTable = new TableLayout(3, 1);

			mainTable.Add(MiddleSection(), 1, 0, true, true);
			mainTable.Add(rightSection = new Panel(), 2, 0);

			return mainTable;
		}

		Control MiddleSection()
		{
			middleTable = new TableLayout(1, 3);

			middleTable.Add(new Label { Text = "Content", BackgroundColor = Colors.LightGrey, HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Middle }, 0, 1, true, true);
			middleTable.Add(topSection = new Panel(), 0, 0);

			return middleTable;
		}

		Control VerticalSection()
		{
			var layout = new DynamicLayout { BackgroundColor = Colors.Blue };
			layout.Add(new Panel { Size = new Size(50, 60), BackgroundColor = Colors.Lime });
			layout.Add(new Panel { Size = new Size(50, 60), BackgroundColor = Colors.Lime });
			return layout;
		}

		Control HorizontalSection()
		{
			var layout = new DynamicLayout { BackgroundColor = Colors.Blue };
			layout.BeginHorizontal();
			layout.Add(new Panel { Size = new Size(50, 60), BackgroundColor = Colors.Lime });
			layout.Add(new Panel { Size = new Size(50, 60), BackgroundColor = Colors.Lime });
			layout.EndHorizontal();
			return layout;
		}
	}
}