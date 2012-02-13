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
			items.Add (new GridItem ("Col 1", null, "Col 3", image1));
			items.Add (new GridItem ("Col 1", false, null, image2));
			items.Add (new GridItem ("Col 1", true, "Col 3", null));
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

