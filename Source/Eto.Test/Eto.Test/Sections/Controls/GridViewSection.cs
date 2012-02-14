using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class GridViewSection : Panel
	{
		public GridViewSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.AddRow (new Label { Text = "Default" }, Default ());
			layout.AddRow (new Label { Text = "No Header" }, NoHeader ());
		}
		
		GridView Default ()
		{
			var control = new GridView {
				Size = new Size (300, 100)
			};
			
			control.Columns.Add (new GridColumn{ HeaderText = "Default"});
			control.Columns.Add (new GridColumn{ DataCell = new CheckBoxCell (), Editable = true, AutoSize = true, Resizable = false});
			control.Columns.Add (new GridColumn{ HeaderText = "Editable", Editable = true});
			control.Columns.Add (new GridColumn{ HeaderText = "Image", DataCell = new ImageCell() });
			
			var image1 = Bitmap.FromResource ("Eto.Test.TestImage.png");
			var image2 = Icon.FromResource ("Eto.Test.TestIcon.ico");
			var items = new GridItemCollection ();
			var rand = new Random();
			for (int i = 0; i < 10000; i++) {
				var boolVal = rand.Next(3);
				var image = rand.Next (2) == 0 ? (Image)image1 : (Image)image2;
				var txt = string.Format ("Col 1 Row {0}", i);
				var editText = rand.Next (10) == 0 ? null : "Editable, Sometimes Null";
				items.Add (new GridItem (txt, boolVal == 0 ? (bool?)false : boolVal == 1 ? (bool?)true : null, editText, image));
			}
			control.DataStore = items;
			
			return control;
		}
		
		GridView NoHeader ()
		{
			var control = Default ();
			control.ShowHeader = false;
			return control;
		}
	}
}

