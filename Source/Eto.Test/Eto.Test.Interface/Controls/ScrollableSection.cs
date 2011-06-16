using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class ScrollableSection : Panel
	{
		public ScrollableSection ()
		{
			var layout = new TableLayout(this, 1, 10);
			
			var scrollable = new Scrollable{ Size = new Size(100, 200) };
			var playout = new TableLayout(scrollable, 3, 1);
			playout.SetColumnScale (0);
			playout.SetColumnScale (2);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 1, 0);
			layout.Add (scrollable, 0, 0);
			scrollable.UpdateScrollSizes ();
		}
		
		Control FormControl()
		{
		
			var layout = new TableLayout(new Panel(), 2, 10);

			
			return layout.Container;
		}
	}
}

