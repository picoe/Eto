using System;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public class ScrollableHandler : WindowsContainer<ScrollableHandler.CustomScrollable, Scrollable>, IScrollable
	{
		public class CustomScrollable : System.Windows.Forms.ScrollableControl
		{
			public ScrollableHandler Handler { get; set; }
			
			protected override bool ProcessDialogKey (SWF.Keys keyData)
			{
				SWF.KeyEventArgs e = new SWF.KeyEventArgs (keyData);
				base.OnKeyDown (e);
				return e.Handled;
			}
			
			protected override void OnScroll (System.Windows.Forms.ScrollEventArgs se)
			{
				base.OnScroll (se);
				//Handler.Widget.OnScroll(new ScrollEventArgs(se.))
				
			}
			
			protected override SD.Point ScrollToControl(SWF.Control activeControl)
			{
				/*if (autoScrollToControl) return base.ScrollToControl(activeControl);
				else return this.AutoScrollPosition;*/
				return this.AutoScrollPosition;
			}
		}

		public ScrollableHandler ()
		{
			Control = new CustomScrollable{ Handler = this };
			Control.AutoScroll = true;
			Control.VerticalScroll.SmallChange = 5;
			Control.VerticalScroll.LargeChange = 10;
			Control.HorizontalScroll.SmallChange = 5;
			Control.HorizontalScroll.LargeChange = 10;
			//control.AutoScrollPosition = new SD.Point(0,0);
			//control.AutoScrollMinSize = new System.Drawing.Size(500,500);
			//control.DisplayRectangle = new System.Drawing.Rectangle(0,0,500,1000);
			//control.BackColor = System.Drawing.Color.Black;
		}

		public void UpdateScrollSizes ()
		{
			Control.PerformLayout ();
		}

		public Point ScrollPosition {
			get { return new Point (-Control.AutoScrollPosition.X, -Control.AutoScrollPosition.Y); }
			set { 
				Control.AutoScrollPosition = Generator.Convert (value);
			}
		}

		public Size ScrollSize {
			get { return Generator.Convert (this.Control.DisplayRectangle.Size); }
			set { Control.AutoScrollMinSize = Generator.Convert (value); }
		}

	}
}
