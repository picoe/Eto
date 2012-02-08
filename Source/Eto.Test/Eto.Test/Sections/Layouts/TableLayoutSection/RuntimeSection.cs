using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Layouts.TableLayoutSection
{
	public class RuntimeSection : Panel
	{
		TableLayout mainTable;
		TableLayout middleTable;
		Panel rightSection;
		Panel topSection;
		bool toggle;

		public RuntimeSection ()
		{
			var layout = new DynamicLayout (this);

			layout.AddCentered (ToggleButton ());
			layout.Add (MainTable ());
		}

		Control ToggleButton ()
		{
			var control = new Button {
				Text = "Add Columns To Table"
			};

			control.Click += delegate {
				toggle = !toggle;
				this.SuspendLayout ();
				if (toggle) {
					mainTable.Add (VerticalSection (), 0, 0);
					rightSection.AddDockedControl (VerticalSection ());
					middleTable.Add (HorizontalSection (), 0, 2);
					topSection.AddDockedControl (HorizontalSection ());
					control.Text = "Remove Columns";
				}
				else {
					mainTable.Add (null, 0, 0);
					rightSection.AddDockedControl (null);
					middleTable.Add (null, 0, 2);
					topSection.AddDockedControl (null);
					control.Text = "Add Columns To Table";
				}
				this.ResumeLayout ();
			};

			return control;
		}

		Control MainTable ()
		{
			mainTable = new TableLayout (new Panel (), 3, 1);

			mainTable.Add (MiddleSection (), 1, 0, true, true);
			mainTable.Add (rightSection = new Panel(), 2, 0);

			return mainTable.Container;
		}

		Control MiddleSection ()
		{
			middleTable = new TableLayout (new Panel (), 1, 3);

			middleTable.Add (new Label { Text = "Content", BackgroundColor = Color.LightGray, HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Middle }, 0, 1, true, true);
			middleTable.Add (topSection = new Panel(), 0, 0);

			return middleTable.Container;
		}

		Control VerticalSection ()
		{
			var layout = new DynamicLayout (new Panel { BackgroundColor = Color.Blue });
			layout.Add (new Panel { Size = new Size (50, 60), BackgroundColor = Color.Green });
			layout.Add (new Panel { Size = new Size (50, 60), BackgroundColor = Color.Green });
			return layout.Container;
		}

		Control HorizontalSection ()
		{
			var layout = new DynamicLayout (new Panel { BackgroundColor = Color.Blue });
			layout.BeginHorizontal ();
			layout.Add (new Panel { Size = new Size (50, 60), BackgroundColor = Color.Green });
			layout.Add (new Panel { Size = new Size (50, 60), BackgroundColor = Color.Green });
			layout.EndHorizontal ();
			return layout.Container;
		}
	}
}