using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.Controls;

namespace Eto.Test.Sections.Controls
{
	public class ScrollableSection : SectionBase
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
			LogEvents (scrollable);
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
		
		Control NoBorderScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.None };
			LogEvents (scrollable);
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}

		Control BezelScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.Bezel };
			LogEvents (scrollable);
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
		
		Control LineScrollable()
		{
			var scrollable = new Scrollable{ Size = new Size(100, 200), Border = BorderType.Line };
			LogEvents (scrollable);
			var playout = new PixelLayout(scrollable);
			playout.Add (new LabelSection{ Size = new Size(400, 400)}, 0, 0);
			return playout.Container;
		}
		
		void LogEvents (Scrollable control)
		{
			control.Scroll += delegate(object sender, ScrollEventArgs e) {
				Log.Write (control, "Scroll, ScrollPosition: {0}", e.ScrollPosition);
			};
		}
	}
}

