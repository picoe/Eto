using System;
using Eto.Forms;
using System.Linq;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#elif OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

#if IOS
using UIKit;
using CoreGraphics;
using Eto.iOS;
using NSView = UIKit.UIView;
using IMacView = Eto.iOS.Forms.IIosView;
using MacContainer = Eto.iOS.Forms.IosLayout<UIKit.UIView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;

#elif OSX
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using MacContainer = Eto.Mac.Forms.MacContainer<AppKit.NSView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;
#else
using MacContainer = Eto.Mac.Forms.MacContainer<MonoMac.AppKit.NSView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;
#endif

#endif
namespace Eto.Mac.Forms
{
	public class TableLayoutHandler : MacContainer, TableLayout.IHandler
	{
		Control[,] views;
		bool[] xscaling;
		bool[] yscaling;
		int lastxscale;
		int lastyscale;
		Size spacing;
		Padding padding;
		SizeF oldFrameSize;

		public override NSView ContainerControl { get { return Control; } }

		public Size Spacing
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

		protected override NSView CreateControl()
		{
			#if OSX
			return new MacEventView();
			#elif IOS
			return new NSView();
			#endif
		}

		protected override void Initialize()
		{
			base.Initialize();

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

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			LayoutChildren();
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (views == null)
				return SizeF.Empty;
			float[] widths, heights;
			return Calculate(availableSize, out widths, out heights, false);
		}

		SizeF Calculate(SizeF availableSize, out float[] widths, out float[] heights, bool final)
		{
			heights = new float[yscaling.Length];
			widths = new float[xscaling.Length];
			var totalpadding = Padding.Size + Spacing * new Size(widths.Length - 1, heights.Length - 1);
			var required = (SizeF)totalpadding;
			var numscaled = new Size();

			// calculate all non-scaled controls
			for (int y = 0; y < heights.Length; y++)
			{
				var yscaled = y == lastyscale || yscaling[y];
				if (yscaled)
					numscaled.Height++;

				for (int x = 0; x < widths.Length; x++)
				{
					var xscaled = x == lastxscale || xscaling[x];
					if (y == 0 && xscaled)
						numscaled.Width++;

					if (xscaled && yscaled)
						continue;

					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var size = view.GetPreferredSize(Size.MaxValue);
						if (!xscaled && size.Width > widths[x])
						{
							required.Width += size.Width - widths[x];
							widths[x] = size.Width;
						}
						if (!yscaled && size.Height > heights[y])
						{
							required.Height += size.Height - heights[y];
							heights[y] = size.Height;
						}
					}
				}
			}

			var remaining = SizeF.Max((availableSize - required) / (SizeF)numscaled, SizeF.Empty);
			//Console.WriteLine($"available: {availableSize}, remaining: {remaining}, required: {required} numscaled: {numscaled}, size: {widths.Length}x{heights.Length}");

			// now, calculate any scaled control(s) now that we have the remaining space available
			var availableControlSize = new SizeF();
			for (int y = 0; y < heights.Length; y++)
			{
				var yscaled = y == lastyscale || yscaling[y];

				availableControlSize.Height = yscaled ? remaining.Height : int.MaxValue;

				for (int x = 0; x < widths.Length; x++)
				{	
					var xscaled = x == lastxscale || xscaling[x];

					if (!xscaled && !yscaled)
						continue;

					availableControlSize.Width = xscaled ? remaining.Width : int.MaxValue;

					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var size = view.GetPreferredSize(availableControlSize);
						if (size.Width > widths[x])
						{
							if (!final || !xscaled)
								required.Width += size.Width - widths[x];
							widths[x] = size.Width;
						}
						if (size.Height > heights[y])
						{
							if (!final || !yscaled)
								required.Height += size.Height - heights[y];
							heights[y] = size.Height;
						}
					}
				}
			}


			if (final)
			{
				// we are laying out for display, so scaled columns are forced to share remaining size
				remaining = SizeF.Max((availableSize - required) / (SizeF)numscaled, SizeF.Empty);
				for (int y = 0; y < heights.Length; y++)
				{
					var yscaled = y == lastyscale || yscaling[y];
					if (!yscaled)
						continue;

					heights[y] = remaining.Height;

				}
				for (int x = 0; x < widths.Length; x++)
				{	
					var xscaled = x == lastxscale || xscaling[x];

					if (!xscaled)
						continue;

					widths[x] = remaining.Width;
				}
				return availableSize;
			}

			return required;
		}

		public override void LayoutChildren()
		{
			if (!Widget.Loaded || views == null || NeedsQueue())
				return;

			var controlSize = ContentControl.Frame.Size.ToEto();

			float[] widths, heights;
			Calculate(controlSize, out widths, out heights, true);

			#if OSX
			bool flipped = Control.IsFlipped;
			#elif IOS
			bool flipped = !Control.Layer.GeometryFlipped;
			#endif

			float starty = Padding.Top;
			for (int y = 0; y < heights.Length; y++)
			{
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
						frame.Y = flipped ? starty : controlSize.Height - starty - frame.Height;
						if (frame != oldframe)
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
			oldFrameSize = controlSize;
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
