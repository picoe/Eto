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
using BaseMacContainer = Eto.iOS.Forms.IosLayout<UIKit.UIView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;

#elif OSX
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using BaseMacContainer = Eto.Mac.Forms.MacContainer<AppKit.NSView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;
#else
using BaseMacContainer = Eto.Mac.Forms.MacContainer<MonoMac.AppKit.NSView, Eto.Forms.TableLayout, Eto.Forms.TableLayout.ICallback>;
#endif

#endif
namespace Eto.Mac.Forms
{
	public class TableLayoutHandler : BaseMacContainer, TableLayout.IHandler
	{
		Control[,] views;
		bool[] xscaling;
		bool[] yscaling;
		int lastxscale;
		int lastyscale;
		Size spacing;
		Padding padding;
		SizeF oldFrameSize;
		float[] pref_widths;
		float[] pref_heights;
		float[] final_widths;
		float[] final_heights;

		class EtoTableLayoutView : MacEventView
		{
			new TableLayoutHandler Handler => (TableLayoutHandler)base.Handler;

			public EtoTableLayoutView()
			{
				AutoresizesSubviews = false;
			}

			public override void Layout()
			{
				if (MacView.NewLayout)
					base.Layout();
				Handler?.PerformLayout();
				if (!MacView.NewLayout)
					base.Layout();
			}
		}

		public override NSView ContainerControl { get { return Control; } }

		public Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				InvalidateMeasure();
			}
		}

		public Padding Padding
		{
			get { return padding; }
			set
			{
				padding = value;
				InvalidateMeasure();
			}
		}

		protected override NSView CreateControl()
		{
			#if OSX
			return new EtoTableLayoutView();
			#elif IOS
			return new NSView();
			#endif
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			if (float.IsPositiveInfinity(availableSize.Width) && float.IsPositiveInfinity(availableSize.Height))
			{
				var naturalSizeInfinity = NaturalSizeInfinity;
				if (naturalSizeInfinity != null)
					return naturalSizeInfinity.Value;
				naturalSizeInfinity = NaturalSizeInfinity = Calculate(availableSize, false, ref pref_widths, ref pref_heights);
				return naturalSizeInfinity.Value;
			}

			var naturalSize = NaturalSize;
			var naturalAvailableSize = availableSize.TruncateInfinity();
			if (naturalSize != null && NaturalAvailableSize == naturalAvailableSize)
				return naturalSize.Value;
			NaturalAvailableSize = naturalAvailableSize;
	        naturalSize = NaturalSize = Calculate(availableSize, false, ref pref_widths, ref pref_heights);
			return naturalSize.Value;
		}

		SizeF Calculate(SizeF availableSize, bool final, ref float[] widths, ref float[] heights)
		{
			if (views == null)
				return SizeF.Empty;
			if (heights == null)
				heights = new float[yscaling.Length];
			else
				Array.Clear(heights, 0, heights.Length);
			if (widths == null)
				widths = new float[xscaling.Length];
			else
				Array.Clear(widths, 0, widths.Length);
			var totalpadding = Padding.Size + Spacing * new Size(widths.Length - 1, heights.Length - 1);
			var required = (SizeF)totalpadding;
			var numscaled = new Size();
			var available = availableSize;// final ? availableSize : SizeF.PositiveInfinity;

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
						var size = view.GetPreferredSize(available);
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
			var maxScaledSize = new SizeF();
			for (int y = 0; y < heights.Length; y++)
			{
				var yscaled = y == lastyscale || yscaling[y];

				availableControlSize.Height = yscaled ? remaining.Height : float.PositiveInfinity;

				for (int x = 0; x < widths.Length; x++)
				{	
					var xscaled = x == lastxscale || xscaling[x];

					if (!xscaled && !yscaled)
						continue;

					if (final && xscaled && yscaled)
						continue;

					availableControlSize.Width = xscaled ? remaining.Width : float.PositiveInfinity;

					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var size = view.GetPreferredSize(availableControlSize);
						if (xscaled)
						{
							maxScaledSize.Width = Math.Max(maxScaledSize.Width, size.Width);
						}
						if (size.Width > widths[x])
						{
							if (!xscaled)
								required.Width += size.Width - widths[x];
							widths[x] = size.Width;
						}
						if (yscaled)
						{
							maxScaledSize.Height = Math.Max(maxScaledSize.Height, size.Height);
						}
						if (size.Height > heights[y])
						{
							if (!yscaled)
								required.Height += size.Height - heights[y];
							heights[y] = size.Height;
						}
					}
				}
			}

			if (!final)
				required += maxScaledSize * numscaled;


			if (final)
			{
				// we are laying out for display, so scaled columns are forced to share remaining size
				var scaledSpace = SizeF.Max(availableSize - required, SizeF.Empty);
				remaining = Size.Truncate(scaledSpace / (SizeF)numscaled);
				var roundingSpace = SizeF.Max(Size.Truncate(scaledSpace - (remaining * numscaled)), SizeF.Empty);

				for (int y = 0; y < heights.Length; y++)
				{
					var yscaled = y == lastyscale || yscaling[y];
					if (!yscaled)
						continue;

					var height = remaining.Height;
					if (roundingSpace.Height > 0)
					{
						height++;
						roundingSpace.Height--;
					}

					heights[y] = height;
				}
				for (int x = 0; x < widths.Length; x++)
				{	
					var xscaled = x == lastxscale || xscaling[x];

					if (!xscaled)
						continue;

					var width = remaining.Width;
					if (roundingSpace.Width > 0)
					{
						width++;
						roundingSpace.Width--;
					}


					widths[x] = width;
				}
				return availableSize;
			}

			return required;
		}

		void PerformLayout()
		{
			if (views == null)
				return;

			//Console.WriteLine($"TableLayout {xscaling.Length}x{yscaling.Length} PerformLayout");
			var controlSize = ContentControl.Frame.Size.ToEto();

			Calculate(controlSize, true, ref final_widths, ref final_heights);

#if OSX
			bool flipped = Control.IsFlipped;
#elif IOS
			bool flipped = !Control.Layer.GeometryFlipped;
#endif

			float starty = Padding.Top;
			for (int y = 0; y < final_heights.Length; y++)
			{
				float startx = Padding.Left;
				for (int x = 0; x < final_widths.Length; x++)
				{
					var view = views[y, x];
					if (view != null && view.Visible)
					{
						var macView = view.GetMacViewHandler();
						var frame = macView.GetAlignmentFrame();
						var oldframe = frame;
						frame.Width = final_widths[x];
						frame.Height = final_heights[y];
						frame.X = (nfloat)Math.Round(Math.Max(0, startx));
						frame.Y = (nfloat)Math.Round(flipped ? starty : controlSize.Height - starty - frame.Height);
						if (frame != oldframe)
							macView.SetAlignmentFrame(frame);
						else if (oldframe.Right > oldFrameSize.Width || oldframe.Bottom > oldFrameSize.Height
							|| frame.Right > oldFrameSize.Width || frame.Bottom > oldFrameSize.Height)
							macView.ContainerControl.SetNeedsDisplay();
						//Console.WriteLine("*** x:{2} y:{3} view: {0} size: {1} totalx:{4} totaly:{5}", view, view.Size, x, y, totalx, totaly);
					}
					startx += final_widths[x] + Spacing.Width;
				}
				starty += final_heights[y] + Spacing.Height;
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
				InvalidateMeasure();
				Control.AddSubview(view);
			}
			else 
				InvalidateMeasure();
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
			InvalidateMeasure();
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
						InvalidateMeasure();
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
			InvalidateMeasure();
		}

		public bool GetColumnScale(int column)
		{
			return xscaling[column];
		}

		public void SetRowScale(int row, bool scale)
		{
			yscaling[row] = scale;
			lastyscale = yscaling.Any(r => r) ? -1 : yscaling.Length - 1;
			InvalidateMeasure();
		}

		public override void InvalidateMeasure()
		{
			base.InvalidateMeasure();
			Control.NeedsLayout = true;
		}

		public bool GetRowScale(int row)
		{
			return yscaling[row];
		}
	}
}
