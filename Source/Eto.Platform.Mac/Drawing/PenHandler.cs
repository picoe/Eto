using System;
using Eto.Drawing;

#if OSX
using MonoMac.CoreGraphics;

namespace Eto.Platform.Mac.Drawing
#elif IOS
using MonoTouch.CoreGraphics;

namespace Eto.Platform.iOS.Drawing
#endif
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPenHandler
	{
		CGColor cgcolor;
		float[] cgdashes;
		DashStyle dashStyle;
		PenLineCap lineCap;
		float thickness;
		float cgoffset;

		public Color Color { get; set; }

		public float Thickness
		{
			get { return thickness; }
			set {
				thickness = value;
				SetDashStyle ();
			}
		}

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap
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

				if (LineCap == PenLineCap.Butt)
					cgdashes = Array.ConvertAll (dashes, x => x * Thickness);
				else {
					if (Thickness == 1)
						cgoffset += Thickness / 2;
					cgdashes = new float[dashes.Length];
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

		public void Create (Color color, float thickness)
		{
			this.Color = color;
			this.Thickness = thickness;
			this.MiterLimit = 10f;
			this.LineCap = PenLineCap.Square;
		}

		public void Dispose ()
		{
		}

		public object ControlObject { get { return this; } }

		public void Apply (GraphicsHandler graphics)
		{
			cgcolor = cgcolor ?? Color.ToCGColor ();
			graphics.Control.SetStrokeColor (cgcolor);
			graphics.Control.SetLineCap (LineCap.ToCG ());
			graphics.Control.SetLineJoin (LineJoin.ToCG ());
			graphics.Control.SetLineWidth (Thickness);
			graphics.Control.SetMiterLimit (MiterLimit);
			if (cgdashes != null)
				graphics.Control.SetLineDash (cgoffset, cgdashes);
		}
	}
}

