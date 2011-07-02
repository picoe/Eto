using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Interface.Controls
{
	public class ScrollableSection : Panel
	{
		public ScrollableSection ()
		{
			var layout = new TableLayout(this, 4, 2);
			
			layout.SetColumnScale (1);
			layout.SetColumnScale (3);
			layout.SetRowScale (0);
			layout.SetRowScale (1);
			
			layout.Add (new Label{ Text = "Default" }, 0, 0);
			layout.Add (DefaultScrollable(), 1, 0);
			
			layout.Add (new Label{ Text = "No Border" }, 2, 0);
			layout.Add (NoBorderScrollable (), 3, 0);
			
			layout.Add (new Label{ Text = "Bezeled" }, 0, 1);
			layout.Add (BezelScrollable (), 1, 1);
			
			layout.Add (new Label{ Text = "Line" }, 2, 1);
			layout.Add (LineScrollable (), 3, 1);
		}
		
		Control DefaultScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200) };
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
		
		Control NoBorderScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.None };
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}

		Control BezelScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.Bezel };
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
		
		Control LineScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.Line };
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
	}
}

