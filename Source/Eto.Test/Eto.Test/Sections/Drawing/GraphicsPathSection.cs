using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	public class GraphicsPathSection : Scrollable
	{
		IGraphicsPath path;

		public bool StartFigures { get; set; }

		public bool CloseFigures { get; set; }

		public bool ConnectPath { get; set; }

		public int PenThickness { get; set; }

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap { get; set; }

		public GraphicsPathSection ()
		{
			StartFigures = true;
			PenThickness = 1;
			path = CreateMainPath ();
			var layout = new DynamicLayout (this);

			layout.AddSeparateRow (null, StartFiguresControl (), CloseFiguresControl (), ConnectPathControl (), null);
			layout.AddSeparateRow (null, PenThicknessControl (), PenJoinControl (), PenCapControl (), null);
			layout.AddSeparateRow (null, Bounds (), CurrentPoint (), null);
			layout.BeginVertical ();
			layout.AddRow (new Label { Text = "Draw Line Path" }, DrawLinePath ());
			layout.AddRow (new Label { Text = "Fill Line Path" }, FillLinePath ());
			layout.EndVertical ();

			layout.Add (null);

		}
		Control StartFiguresControl ()
		{
			var control = new CheckBox { Text = "Start Figures" };
			control.Bind (cb => cb.Checked, this, r => r.StartFigures);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control CloseFiguresControl ()
		{
			var control = new CheckBox { Text = "Close Figures" };
			control.Bind (cb => cb.Checked, this, r => r.CloseFigures);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control ConnectPathControl ()
		{
			var control = new CheckBox { Text = "Connect Paths" };
			control.Bind (cb => cb.Checked, this, r => r.ConnectPath);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control PenThicknessControl ()
		{
			var control = new NumericUpDown { MinValue = 1, MaxValue = 100 };
			control.Bind (c => c.Value, this, r => r.PenThickness);
			control.ValueChanged += Refresh;

			var layout = new DynamicLayout (new Panel(), Padding.Empty);
			layout.AddRow (new Label { Text = "Thickness:", VerticalAlign = VerticalAlign.Middle}, control);
			return layout.Container;
		}

		Control PenJoinControl ()
		{
			var control = new EnumComboBox<PenLineJoin>();
			control.Bind (c => c.SelectedValue, this, r => r.LineJoin);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control PenCapControl ()
		{
			var control = new EnumComboBox<PenLineCap>();
			control.Bind (c => c.SelectedValue, this, r => r.LineCap);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control Bounds ()
		{
			var control = new Label ();
			control.Text = string.Format ("Bounds: {0}", path.Bounds);
			return control;
		}
		Control CurrentPoint ()
		{
			var control = new Label ();
			control.Text = string.Format ("CurrentPoint: {0}", path.CurrentPoint);
			return control;
		}

		void Refresh (object sender, EventArgs e)
		{
			path = CreateMainPath ();
			foreach (var d in this.Children.OfType<Drawable> ()) {
				d.Invalidate ();
			}
		}

		Control DrawLinePath ()
		{
			var control = new Drawable { Size = new Size (550, 200) };

			control.Paint += (sender, e) => {
				var pen = Pen.Create (Colors.Black, PenThickness);
				pen.LineJoin = LineJoin;
				pen.LineCap = LineCap;
				e.Graphics.DrawPath (pen, path);
			};

			return control;
		}

		Control FillLinePath ()
		{
			var control = new Drawable { Size = new Size (550, 200) };

			control.Paint += (sender, e) => {
				e.Graphics.FillPath (Brushes.Black (), path);
			};

			return control;
		}

		IGraphicsPath CreateMainPath ()
		{
			var path = CreatePath ();

			path.StartFigure ();
			path.AddLine (220, 120, 220, 170);

			// test adding child paths and transforms
			var childPath = CreatePath();
			var matrix = Matrix.Create();
			matrix.Translate (240, 120);
			matrix.Scale(0.25f);
			childPath.Transform(matrix);
			path.AddPath (childPath, ConnectPath);

			path.AddLine (370, 120, 370, 170);

			return path;
		}

		IGraphicsPath CreatePath ()
		{
			var path = GraphicsPath.Create ();
			var start = StartFigures;
			var close = CloseFigures;

			// connected segments

			path.MoveTo (10, 10);
			path.LineTo (20, 90);
			path.LineTo (10, 60);
			path.LineTo (90, 80);
			path.LineTo (60, 30);
			if (close && start) path.CloseFigure ();

			if (start) path.StartFigure ();
			path.AddArc (100, 0, 100, 50, 200, -160);
			if (close && start) path.CloseFigure ();

			if (start) path.StartFigure ();
			path.AddBezier (new PointF (200, 10), new PointF (285, 20), new PointF (210, 85), new PointF (300, 90));
			if (close && start) path.CloseFigure ();

			if (start) path.StartFigure ();
			path.AddCurve (new PointF (310, 90), new PointF (390, 90), new PointF (390, 10), new PointF (310, 10));
			if (close && start) path.CloseFigure ();

			if (start) path.StartFigure ();
			path.AddLine (410, 10, 410, 90);
			if (close && start) path.CloseFigure ();
			
			if (start) path.StartFigure ();
			path.AddLines (new PointF (420, 10), new PointF (420, 90));
			if (close && start) path.CloseFigure ();
			
			if (start) path.StartFigure ();
			path.AddLines (new PointF (430, 10), new PointF (430, 90));
			if (close) path.CloseFigure ();

			// separate segments

			if (start) path.StartFigure ();
			path.AddEllipse (100, 100, 100, 45);
			if (close) path.CloseFigure ();

			if (start) path.StartFigure ();
			path.AddRectangle (10, 110, 80, 80);
			if (close) path.CloseFigure ();

			// at the end, draw a line so we can potentially connect to parent path
			if (start) path.StartFigure ();
			path.AddLines (new PointF (440, 10), new PointF (440, 90));
			if (close) path.CloseFigure ();

			return path;
		}

	}
}
