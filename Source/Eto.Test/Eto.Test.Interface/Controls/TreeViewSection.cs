using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class TreeViewSection : Panel
	{
		int expanded;
		
		static Image Image = new Icon (null, "Eto.Test.Interface.TestIcon.ico");
		
		public TreeViewSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.BeginVertical();
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "Simple" });
			layout.Add (Default ());
			layout.EndHorizontal ();
			
			layout.BeginHorizontal ();
			layout.Add (new Label{ Text = "With Images" });
			layout.Add (Images ());
			layout.EndHorizontal ();
			
			layout.EndVertical ();
			
			layout.Add (null, true, true);
		}
		
		TreeItem CreateTreeItem (int level, string name, Image image)
		{
			var item = new TreeItem {
				Text = name,
				Expanded = expanded++ % 2 == 0,
				Image = image
			};
			if (level < 4) {
				for (int i = 0; i < 4; i++) {
					item.Children.Add (CreateTreeItem (level + 1, name + " " + i, image));
				}
			}
			return item;
		}
		
		Control Default ()
		{
			var control = new TreeView {
				Size = new Size(100, 150)
			};
			control.TopNode = CreateTreeItem (0, "Item", null);
			return control;
		}

		Control Images ()
		{
			var control = new TreeView {
				Size = new Size(100, 150)
			};
			control.TopNode = CreateTreeItem (0, "Item", Image);
			return control;
		}
	}
}

