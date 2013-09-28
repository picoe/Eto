using System;
using Eto.Forms;
using Eto.Drawing;

namespace Tutorial2
{
	public class MyForm : Form
	{
		public MyForm()
		{
			this.ClientSize = new Size(600, 400);
			this.Title = "Dynamic Layout";

			// Using a DynamicLayout for a simple table is actually a lot easier to maintain than using a TableLayout 
			// and having to specify the x/y co-ordinates for each control added.

			// 1. Create a new DynamicLayout object

			var layout = new DynamicLayout();

			// 2. Begin a horizontal row of controls

			layout.BeginHorizontal();

			// 3. Add controls for each column.  We are setting xscale to true to make each column use an equal portion
			// of the available space.

			layout.Add(new Label { Text = "First Column" }, xscale: true);
			layout.Add(new Label { Text = "Second Column" }, xscale: true);
			layout.Add(new Label { Text = "Third Column" }, xscale: true);

			// 4. End the horizontal section

			layout.EndHorizontal();

			// 5. To add a new row, begin another horizontal section and add more controls:

			layout.BeginHorizontal();
			layout.Add(new TextBox { Text = "Second Row, First Column" });
			layout.Add(new ComboBox { DataStore = new ListItemCollection { new ListItem { Text = "Second Row, Second Column" } } });
			layout.Add(new CheckBox { Text = "Second Row, Third Column" });
			layout.EndHorizontal();

			// 6. By default, the last row & column of a table expands to fill the rest of the space.  We can add one 
			// last row with nothing in it to make the space empty.  Since we are not in a horizontal group, calling 
			// Add() adds a new row.

			layout.Add(null);

			// 7. Set the content of the form to use the layout

			Content = layout;

			GenerateActions();
		}

		void GenerateActions()
		{
			var actions = new GenerateActionArgs(this);
			Application.Instance.GetSystemActions(actions, true);

			var quitAction = new ButtonAction { Text = "Quit", Accelerator = Application.Instance.CommonModifier | Key.Q };
			quitAction.Activated += (sender, e) => Application.Instance.Quit();
				
			// add action to file sub-menu
			var file = actions.Menu.FindAddSubMenu("&File");
			file.Actions.Add(quitAction);

			// generate menu
			this.Menu = actions.Menu.GenerateMenuBar();
		}
	}

	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var app = new Application();
			
			app.Initialized += delegate
			{
				app.MainForm = new MyForm();
				app.MainForm.Show();
			};
			app.Run(args);
		}
	}
}
