using System;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp.Drawing
{
	/// <summary>
	/// Pen handler
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class PenHandler : IPenHandler
	{
		double[] cairodashes;
		float thickness;
		double cairooffset;
		DashStyle dashStyle;

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

		public PenLineCap LineCap { get; set; }

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

				if (LineCap == PenLineCap.Butt) {
					cairodashes = Array.ConvertAll (dashes, x => (double)x * Thickness);
				}
				else {
					if (Thickness == 1)
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

			graphics.Control.Color = Color.ToCairo ();
			graphics.Control.LineWidth = this.Thickness;
			graphics.Control.LineCap = LineCap.ToCairo ();
			graphics.Control.LineJoin = LineJoin.ToCairo ();
			if (cairodashes != null)
				graphics.Control.SetDash (cairodashes, cairooffset);
			graphics.Control.MiterLimit = MiterLimit;
		}
	}
}

