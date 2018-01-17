using System;
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

#if OSX

namespace Eto.Mac.Drawing
#elif IOS
using CoreGraphics;

namespace Eto.iOS.Drawing
#endif
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : Pen.IHandler
	{
		class PenControl {
			nfloat[] cgdashes;
			DashStyle dashStyle;
			CGLineCap lineCap;
			float thickness;
			float cgoffset;

			public Brush Brush { get; set; }

			public float Thickness
			{
				get { return thickness; }
				set {
					thickness = value;
					SetDashStyle ();
				}
			}
			
			public CGLineJoin LineJoin { get; set; }
			
			public CGLineCap LineCap
			{
				get { return lineCap; }
				set {
					lineCap = value;
					SetDashStyle ();
				}
			}
			
			public float MiterLimit { get; set; }
			
			public DashStyle DashStyle
			{
				get { return dashStyle; }
				set {
					dashStyle = value;
					SetDashStyle ();
				}
			}
			
			void SetDashStyle ()
			{
				if (DashStyle == null || DashStyle.IsSolid) {
					cgdashes = null;
				} else {
					// TODO: this is not quite perfect for Square/Round for small thicknesses
					
					var dashes = DashStyle.Dashes;
					cgoffset = DashStyle.Offset * Thickness;
					
					if (LineCap == CGLineCap.Butt)
						cgdashes = Array.ConvertAll (dashes, x => (nfloat)(x * Thickness));
					else {
						if (Math.Abs(Thickness - 1) < 0.01f)
							cgoffset += Thickness / 2;
						cgdashes = new nfloat[dashes.Length];
						for (int i = 0; i < cgdashes.Length; i++) {
							var dash = dashes [i] * Thickness;
							if ((i % 2) == 1) {
								// gap must include square/round thickness
								dash += Thickness;
							} else {
								// dash must exclude square/round thickness
								dash -= Thickness;
							}
							cgdashes [i] = dash;
						}
					}
				}
			}

			public void Apply (GraphicsHandler graphics)
			{
				graphics.Control.SetLineCap (LineCap);
				graphics.Control.SetLineJoin (LineJoin);
				graphics.Control.SetLineWidth (Thickness);
				graphics.Control.SetMiterLimit (MiterLimit);
				if (cgdashes != null)
					graphics.Control.SetLineDash (cgoffset, cgdashes);
			}

			public void Finish(GraphicsHandler graphics)
			{
				Brush.Draw(graphics, true, FillMode.Winding);
			}
		}

		public object Create(Brush brush, float thickness)
		{
			return new PenControl
			{
				Brush = brush,
				Thickness = thickness,
				MiterLimit = 10f,
				LineCap = PenLineCap.Square.ToCG()
			};
		}

		public Brush GetBrush (Pen widget)
		{
			return ((PenControl)widget.ControlObject).Brush;
		}

		public float GetThickness (Pen widget)
		{
			return ((PenControl)widget.ControlObject).Thickness;
		}

		public void SetThickness (Pen widget, float thickness)
		{
			((PenControl)widget.ControlObject).Thickness = thickness;
		}

		public PenLineJoin GetLineJoin (Pen widget)
		{
			return ((PenControl)widget.ControlObject).LineJoin.ToEto ();
		}

		public void SetLineJoin (Pen widget, PenLineJoin lineJoin)
		{
			((PenControl)widget.ControlObject).LineJoin = lineJoin.ToCG ();
		}

		public PenLineCap GetLineCap (Pen widget)
		{
			return ((PenControl)widget.ControlObject).LineCap.ToEto ();
		}

		public void SetLineCap (Pen widget, PenLineCap lineCap)
		{
			((PenControl)widget.ControlObject).LineCap = lineCap.ToCG ();
		}

		public float GetMiterLimit (Pen widget)
		{
			return ((PenControl)widget.ControlObject).MiterLimit;
		}

		public void SetMiterLimit (Pen widget, float miterLimit)
		{
			((PenControl)widget.ControlObject).MiterLimit = miterLimit;
		}

		public void SetDashStyle (Pen widget, DashStyle dashStyle)
		{
			((PenControl)widget.ControlObject).DashStyle = dashStyle;
		}

		public void Apply(Pen widget, GraphicsHandler graphics)
		{
			((PenControl)widget.ControlObject).Apply(graphics);
		}
		public void Finish(Pen widget, GraphicsHandler graphics)
		{
			((PenControl)widget.ControlObject).Finish(graphics);
		}
	}
}

