using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;
using System.Diagnostics;
using sd = System.Drawing;

#if IOS
using MonoTouch.UIKit;
using NSView = MonoTouch.UIKit.UIView;
using IMacView = Eto.Platform.iOS.Forms.IiosView;
using MacContainer = Eto.Platform.iOS.Forms.iosLayout<MonoTouch.UIKit.UIView, Eto.Forms.TableLayout>;


#elif OSX
using MonoMac.AppKit;
using Eto.Platform.Mac.Forms.Controls;
using MacContainer = Eto.Platform.Mac.Forms.MacContainer<MonoMac.AppKit.NSView, Eto.Forms.TableLayout>;
#endif
namespace Eto.Platform.Mac.Forms
{
	public class TableLayoutHandler : MacContainer, ITableLayout
	{
		Control[,] views;
		bool[] xscaling;
		bool[] yscaling;
		int lastxscale;
		int lastyscale;
		Size spacing;
		Padding padding;
		sd.SizeF oldFrameSize;

		public override NSView ContainerControl { get { return Control; } }

		public Eto.Drawing.Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				if (Widget.Loaded)
					LayoutParent();
			}
		}

		public Padding Padding
		{
			get { return padding; }
			set
			{
				padding = value;
				if (Widget.Loaded)
					LayoutParent();
			}
		}

		public TableLayoutHandler()
		{
#if OSX
			Control = new MacEventView { Handler = this };
#elif IOS
			Control = new NSView();
#endif
		}

		protected override void Initialize()
		{
			base.Initialize();

			this.Spacing = TableLayout.DefaultSpacing;
			this.Padding = TableLayout.DefaultPadding;
			Widget.SizeChanged += HandleSizeChanged;
		}

		bool isResizing;

		void HandleSizeChanged(object sender, EventArgs e)
		{
			if (!isResizing)
			{
				isResizing = true;
				LayoutChildren();
				isResizing = false;
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			LayoutChildren();
		}

		public override Size GetPreferredSize(Size availableSize)
		{
			var baseSize = base.GetPreferredSize(availableSize);
			if (views == null)
				return baseSize;
			var heights = new float[views.GetLength(0)];
			var widths = new float[views.GetLength(1)];
			float totalxpadding = Padding.Horizontal + Spacing.Width * (widths.Length - 1);
			float totalypadding = Padding.Vertical + Spacing.Height * (heights.Length - 1);
			var requiredx = totalxpadding;
			var requiredy = totalypadding;

			for (int y = 0; y < heights.Length; y++)
			{
				heights[y] = 0;
			}
			for (int x = 0; x < widths.Length; x++)
			{
				widths[x] = 0;
			}

			for (int y = 0; y < heights.Length; y++)
				for (int x = 0; x < widths.Length; x++)
				{	
					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var size = view.GetPreferredSize(availableSize);
						if (size.Width > widths[x])
						{
							requiredx += size.Width - widths[x];
							widths[x] = size.Width;
						}
						if (size.Height > heights[y])
						{
							requiredy += size.Height - heights[y];
							heights[y] = size.Height;
						}
					}
				}
			return Size.Max(baseSize, new Size((int)requiredx, (int)requiredy));
		}

		public override void LayoutChildren()
		{
			if (!Widget.Loaded)
				return;
			var heights = new float[views.GetLength(0)];
			var widths = new float[views.GetLength(1)];
			var controlFrame = ContentControl.Frame;
			float totalxpadding = Padding.Horizontal + Spacing.Width * (widths.Length - 1);
			float totalypadding = Padding.Vertical + Spacing.Height * (heights.Length - 1);
			var totalx = controlFrame.Width - totalxpadding;
			var totaly = controlFrame.Height - totalypadding;
			var requiredx = totalxpadding;
			var requiredy = totalypadding;
			var numx = 0;
			var numy = 0;

			for (int y = 0; y < heights.Length; y++)
			{
				heights[y] = 0;
				if (yscaling[y] || lastyscale == y)
					numy++;
			}
			for (int x = 0; x < widths.Length; x++)
			{
				widths[x] = 0;
				if (xscaling[x] || lastxscale == x)
					numx++;
			}

			var availableSize = Size.Max(Size.Empty, Control.Frame.Size.ToEtoSize() - new Size((int)requiredx, (int)requiredy));
			for (int y = 0; y < heights.Length; y++)
				for (int x = 0; x < widths.Length; x++)
				{
					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var size = view.GetPreferredSize(availableSize);
						if (!xscaling[x] && lastxscale != x && widths[x] < size.Width)
						{
							requiredx += size.Width - widths[x];
							widths[x] = size.Width;
						}
						if (!yscaling[y] && lastyscale != y && heights[y] < size.Height)
						{
							requiredy += size.Height - heights[y];
							heights[y] = size.Height;
						}
					}
				}
			if (controlFrame.Width < requiredx)
			{
				totalx = requiredx - totalxpadding;
			}
			if (controlFrame.Height < requiredy)
			{
				totaly = requiredy - totalypadding;
			}

			for (int y = 0; y < heights.Length; y++)
				if (!yscaling[y] && lastyscale != y)
					totaly -= heights[y];
			for (int x = 0; x < widths.Length; x++)
				if (!xscaling[x] && lastxscale != x)
					totalx -= widths[x];

			var chunkx = (numx > 0) ? (float)Math.Truncate(Math.Max(totalx, 0) / numx) : totalx;
			var chunky = (numy > 0) ? (float)Math.Truncate(Math.Max(totaly, 0) / numy) : totaly;

#if OSX
			bool flipped = Control.IsFlipped;
#elif IOS
			bool flipped = !Control.Layer.GeometryFlipped;
#endif
			float starty = Padding.Top;
			for (int x = 0; x < widths.Length; x++)
			{
				if (xscaling[x] || lastxscale == x)
				{
					widths[x] = Math.Min(chunkx, totalx);
					totalx -= chunkx;
				}
			}

			for (int y = 0; y < heights.Length; y++)
			{
				if (yscaling[y] || lastyscale == y)
				{
					heights[y] = Math.Min(chunky, totaly);
					totaly -= chunky;
				}
				float startx = Padding.Left;
				for (int x = 0; x < widths.Length; x++)
				{
					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var nsview = view.GetContainerView();
						var frame = nsview.Frame;
						var oldframe = frame;
						frame.Width = widths[x];
						frame.Height = heights[y];
						frame.X = Math.Max(0, startx);
						frame.Y = flipped ? starty : controlFrame.Height - starty - frame.Height;
						if (frame != nsview.Frame)
							nsview.Frame = frame;
						else if (oldframe.Right > oldFrameSize.Width || oldframe.Bottom > oldFrameSize.Height
						         || frame.Right > oldFrameSize.Width || frame.Bottom > oldFrameSize.Height)
							nsview.SetNeedsDisplay();
						//Console.WriteLine("*** x:{2} y:{3} view: {0} size: {1} totalx:{4} totaly:{5}", view, view.Size, x, y, totalx, totaly);
					}
					startx += widths[x] + Spacing.Width;
				}
				starty += heights[y] + Spacing.Height;
			}
			oldFrameSize = controlFrame.Size;
		}

		public void Add(Control child, int x, int y)
		{
			var current = views[y, x];
			if (current != null)
			{
				var currentView = current.GetContainerView();
				if (currentView != null)
					currentView.RemoveFromSuperview();
			}
			views[y, x] = child;
			if (child != null)
			{
				var view = child.GetContainerView();
				if (Widget.Loaded)
					LayoutParent();
				Control.AddSubview(view);
			}
			else if (Widget.Loaded)
				LayoutParent();
		}

		public void Move(Control child, int x, int y)
		{
			var current = views[y, x];
			if (current != null)
			{
				var currentView = current.GetContainerView();
				if (currentView != null)
					currentView.RemoveFromSuperview();
			}
			for (int yy = 0; yy < views.GetLength(0); yy++)
				for (int xx = 0; xx < views.GetLength(1); xx++)
				{
					if (object.ReferenceEquals(views[yy, xx], child))
						views[yy, xx] = null;
				}

			views[y, x] = child;
			if (Widget.Loaded)
				LayoutParent();
		}

		public void Remove(Control child)
		{
			for (int y = 0; y < views.GetLength(0); y++)
				for (int x = 0; x < views.GetLength(1); x++)
				{
					if (object.ReferenceEquals(views[y, x], child))
					{
						var view = child.GetContainerView();
						view.RemoveFromSuperview();
						views[y, x] = null;
						if (Widget.Loaded)
							LayoutParent();
						return;
					}
				}
		}

		public void CreateControl(int cols, int rows)
		{
			views = new Control[rows, cols];
			xscaling = new bool[cols];
			lastxscale = cols - 1;
			yscaling = new bool[rows];
			lastyscale = rows - 1;
		}

		public void SetColumnScale(int column, bool scale)
		{
			xscaling[column] = scale;
			lastxscale = xscaling.Any(r => r) ? -1 : xscaling.Length - 1;
			if (Widget.Loaded)
				LayoutParent();
		}

		public bool GetColumnScale(int column)
		{
			return xscaling[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			yscaling[row] = scale;
			lastyscale = yscaling.Any(r => r) ? -1 : yscaling.Length - 1;
			if (Widget.Loaded)
				LayoutParent();
		}

		public bool GetRowScale(int row)
		{
			return yscaling[row];
		}
	}
}
