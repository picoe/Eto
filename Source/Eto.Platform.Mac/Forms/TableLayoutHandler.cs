using System;
using Eto.Forms;
using MonoMac.AppKit;
using System.Linq;
using Eto.Drawing;

namespace Eto.Platform.Mac
{
	public class TableLayoutHandler : MacLayout<NSView, TableLayout>, ITableLayout
	{
		Control[,] views;
		bool[] xscaling;
		bool[] yscaling;
		Size spacing;
		Padding padding;
		
		public void Update ()
		{
			Layout();
		}
		
		public Eto.Drawing.Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				Layout();
			}
		}
		
		public Padding Padding
		{
			get { return padding; }
			set
			{
				padding = value;
				Layout();
			}
		}
		
		public override NSView Control {
			get {
				return (NSView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}
		
		/*
		public override object LayoutObject
		{
			get	{ return Control; }
		}*/

		public TableLayoutHandler()
		{
			/*Control = new NSView();
			Control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver(NSView.NSViewFrameDidChangeNotification, delegate { 
				Layout();
			});*/
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			this.Spacing = TableLayout.DefaultSpacing;
			this.Padding = TableLayout.DefaultPadding;

			Control.PostsFrameChangedNotifications = true;
			this.AddObserver(NSView.NSViewFrameDidChangeNotification, delegate { 
				Layout();
			});
		}
		
		public override void SizeToFit ()
		{
			if (views == null) return;
			var heights = new float[views.GetLength(0)];
			var widths = new float[views.GetLength(1)];
			var controlFrame = Control.Frame;
			float totalxpadding = Padding.Horizontal + Spacing.Width * (widths.Length - 1);
			float totalypadding = Padding.Vertical + Spacing.Height * (heights.Length - 1);
			var requiredx = totalxpadding;
			var requiredy = totalypadding;
			var numx = 0;
			var numy = 0;
			
			for (int y=0; y<heights.Length; y++) { heights[y] = 0; if (yscaling[y]) numy++; }
			for (int x=0; x<widths.Length; x++) { widths[x] = 0; if (xscaling[x]) numx++; }
			
			for (int y=0; y<heights.Length; y++)
			for (int x=0; x<widths.Length; x++) {
				var view = views[y, x];
				if (view != null && view.Visible) {
					AutoSize(view);
					if (!xscaling[x] && widths[x] < view.Size.Width) { 
						requiredx += view.Size.Width - widths[x];
						widths[x] = view.Size.Width; 
					}
					if (!yscaling[y] && heights[y] < view.Size.Height) { 
						requiredy += view.Size.Height - heights[y];
						heights[y] = view.Size.Height; 
					}
				}
			}
			controlFrame.Width = requiredx;
			controlFrame.Height = requiredy;
			Control.Frame = controlFrame;
		}
		
		
		void AutoSize(Control view)
		{
			var c = view.ControlObject as NSControl;
			var mh = view.Handler as IMacView;
			if (mh != null && !mh.AutoSize) return;

			var container = view as Container;
			if (container != null && container.Layout != null)
			{
				var layout = container.Layout.Handler as IMacLayout;
				if (layout != null) layout.SizeToFit();
			}

			if (c != null) c.SizeToFit();
		}
		
		void Layout()
		{
			if (views == null) return;
			var heights = new float[views.GetLength(0)];
			var widths = new float[views.GetLength(1)];
			var controlFrame = Control.Frame;
			float totalxpadding = Padding.Horizontal + Spacing.Width * (widths.Length - 1);
			float totalypadding = Padding.Vertical + Spacing.Height * (heights.Length - 1);
			var totalx = controlFrame.Width - totalxpadding;
			var totaly = controlFrame.Height - totalypadding;
			var requiredx = totalxpadding;
			var requiredy = totalypadding;
			var numx = 0;
			var numy = 0;
			
			for (int y=0; y<heights.Length; y++) { heights[y] = 0; if (yscaling[y]) numy++; }
			for (int x=0; x<widths.Length; x++) { widths[x] = 0; if (xscaling[x]) numx++; }
			
			for (int y=0; y<heights.Length; y++)
			for (int x=0; x<widths.Length; x++) {
				var view = views[y, x];
				if (view != null && view.Visible) {
					AutoSize(view);
					if (!xscaling[x] && widths[x] < view.Size.Width) { 
						
						widths[x] = view.Size.Width; requiredx += view.Size.Width - widths[x];
					}
					if (!yscaling[y] && heights[y] < view.Size.Height) { 
						
						heights[y] = view.Size.Height; requiredy += view.Size.Height - heights[y];
					}
				}
			}
			if (controlFrame.Width < requiredx) {
				totalx = requiredx - totalxpadding;
				controlFrame.Width = requiredx;
			}
			if (controlFrame.Height < requiredy) {
				//Console.WriteLine("restricting y from {0} to {1}", controlFrame.Height, requiredy);
				totaly = requiredy - totalypadding;
				controlFrame.Height = requiredy;
			}
			
			if (numy > 0) { for (int y=0; y<heights.Length; y++) if (!yscaling[y]) totaly -= heights[y]; }
			else {
				if (heights.Length > 1) {
					for (int y=0; y<heights.Length-1; y++) totaly -= heights[y];
				}
				heights[heights.Length-1] = totaly;
			}
			if (numx > 0) { for (int x=0; x<widths.Length; x++) if (!xscaling[x]) totalx -= widths[x]; }
			else { 
				if (widths.Length > 1) {
					for (int x=0; x<widths.Length-1; x++) totalx -= widths[x];
				}
				widths[widths.Length-1] = totalx;
			}
			if (numx > 0) totalx = Math.Max(totalx, 0) / numx;
			if (numy > 0) totaly = Math.Max(totaly, 0) / numy;
			
			float starty = Padding.Top;
			for (int y=0; y<heights.Length; y++) {
				if (yscaling[y]) heights[y] = totaly;
				float startx = Padding.Left;
				for (int x=0; x<widths.Length; x++)
				{
					if (xscaling[x]) widths[x] = totalx;
					var view = views[y, x];
					if (view != null && view.Visible) {
						var nsview = view.ControlObject as NSView;
						var frame = nsview.Frame;
						frame.Width = widths[x];
						frame.Height = heights[y];
						frame.X = startx;
						frame.Y = starty; //Control.Frame.Height - starty - frame.Height;
						nsview.Frame = frame;
					}
					startx += widths[x] + Spacing.Width;
				}
				starty += heights[y] + Spacing.Height;
			}
			if (controlFrame != Control.Frame) 
			{
				//Control.Frame = controlFrame; // no need as it is a container that will be handled separately!
				//Console.WriteLine("Setting frame to {0}", controlFrame);
			}
		}


		public void Add(Control child, int x, int y)
		{
			var current = views[y,x];
			if (current != null) {
				var currentView = (NSView)current.ControlObject;
				if (currentView != null) currentView.RemoveFromSuperview();	
			}
			var view = (NSView)child.ControlObject;
			views[y, x] = child;
			Layout();
			Control.AddSubview(view);
		}
		public void Move(Control child, int x, int y)
		{
			var current = views[y,x];
			if (current != null) {
				var currentView = (NSView)current.ControlObject;
				if (currentView != null) currentView.RemoveFromSuperview();	
			}

			views[y, x] = child;
			Layout();
		}
		
		public void Remove (Control child)
		{
			var view = (NSView)child.ControlObject;
			view.RemoveFromSuperview();
			for (int y=0; y<views.GetLength(0); y++)
			for (int x=0; x<views.GetLength(1); x++)
			{
				if (views[y,x] == child) views[y,x] = null;
			}
			Layout();
		}

		public void CreateControl(int cols, int rows)
		{
			views = new Control[rows,cols];
			xscaling = new bool[cols];
			yscaling = new bool[rows];
		}

		public void SetColumnScale(int column, bool scale)
		{
			xscaling[column] = scale;
		}
		
		public void SetRowScale(int row, bool scale)
		{
			yscaling[row] = scale;
		}
	}
}
