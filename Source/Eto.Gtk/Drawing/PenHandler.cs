using System;
using Eto.Drawing;

namespace Eto.GtkSharp.Drawing
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : Pen.IHandler
	{
		class PenObject
		{
			double[] cairodashes;
			float thickness;
			double cairooffset;
			DashStyle dashStyle;

			public Cairo.Color Color { get; set; }

			public float Thickness
			{
				get { return thickness; }
				set {
					thickness = value;
					SetDashStyle ();
				}
			}

			public Cairo.LineJoin LineJoin { get; set; }

			public Cairo.LineCap LineCap { get; set; }

			public float MiterLimit { get; set; }

			public DashStyle DashStyle
			{
				get { return dashStyle; }
				set
				{
					dashStyle = value;
					SetDashStyle ();
				}
			}

			void SetDashStyle ()
			{
				if (dashStyle == null || dashStyle.IsSolid)
					cairodashes = null;
				else {
					var dashes = DashStyle.Dashes;
					cairooffset = DashStyle.Offset * Thickness;

					if (LineCap == Cairo.LineCap.Butt) {
						cairodashes = Array.ConvertAll (dashes, x => (double)x * Thickness);
					}
					else {
						if (Math.Abs(Thickness - 1) < 0.01f)
							cairooffset += Thickness / 2;
						cairodashes = new double[dashes.Length];
						for (int i = 0; i < cairodashes.Length; i++) {
							var dash = dashes [i] * Thickness;
							if ((i % 2) == 1) {
								// gap must include square/round thickness
								dash += Thickness;
							} else {
								// dash must exclude square/round thickness
								dash -= Thickness;
							}
							cairodashes [i] = dash;
						}
					}
				}
			}

			public void Apply (GraphicsHandler graphics)
			{
				graphics.Control.SetSourceColor(Color);
				graphics.Control.LineWidth = Thickness;
				graphics.Control.LineCap = LineCap;
				graphics.Control.LineJoin = LineJoin;
				if (cairodashes != null)
					graphics.Control.SetDash (cairodashes, cairooffset);
				graphics.Control.MiterLimit = MiterLimit;
				graphics.Control.Stroke ();
			}
		}

		public object Create (Color color, float thickness)
		{
			return new PenObject {
				Color = color.ToCairo (),
				Thickness = thickness,
				MiterLimit = 10f,
				LineCap = PenLineCap.Square.ToCairo ()
			};
		}

		public Color GetColor (Pen widget)
		{
			return ((PenObject)widget.ControlObject).Color.ToEto ();
		}

		public void SetColor (Pen widget, Color color)
		{
			((PenObject)widget.ControlObject).Color = color.ToCairo ();
		}

		public float GetThickness (Pen widget)
		{
			return ((PenObject)widget.ControlObject).Thickness;
		}

		public void SetThickness (Pen widget, float thickness)
		{
			((PenObject)widget.ControlObject).Thickness = thickness;
		}

		public PenLineJoin GetLineJoin (Pen widget)
		{
			return ((PenObject)widget.ControlObject).LineJoin.ToEto ();
		}

		public void SetLineJoin (Pen widget, PenLineJoin lineJoin)
		{
			((PenObject)widget.ControlObject).LineJoin = lineJoin.ToCairo ();
		}

		public PenLineCap GetLineCap (Pen widget)
		{
			return ((PenObject)widget.ControlObject).LineCap.ToEto ();
		}

		public void SetLineCap (Pen widget, PenLineCap lineCap)
		{
			((PenObject)widget.ControlObject).LineCap = lineCap.ToCairo ();
		}

		public float GetMiterLimit (Pen widget)
		{
			return ((PenObject)widget.ControlObject).MiterLimit;
		}

		public void SetMiterLimit (Pen widget, float miterLimit)
		{
			((PenObject)widget.ControlObject).MiterLimit = miterLimit;
		}

		public void SetDashStyle (Pen widget, DashStyle dashStyle)
		{
			((PenObject)widget.ControlObject).DashStyle = dashStyle;
		}

		public void Apply (Pen widget, GraphicsHandler graphics)
		{
			((PenObject)widget.ControlObject).Apply (graphics);
		}
	}
}

