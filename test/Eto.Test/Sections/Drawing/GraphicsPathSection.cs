using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "GraphicsPath")]
	public class GraphicsPathSection : Scrollable
	{
		public bool StartFigures { get; set; }

		public bool CloseFigures { get; set; }

		public bool ConnectPath { get; set; }

		public float PenThickness { get; set; }

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap { get; set; }

		public bool AntiAlias { get; set; }

		public Action<GraphicsPath> PathChanged;

		GraphicsPath path;

		GraphicsPath Path
		{
			get
			{
				if (path == null)
				{
					path = CreateMainPath();
					if (PathChanged != null)
						PathChanged(path);
				}
				return path;
			}
		}

		public GraphicsPathSection()
		{
			StartFigures = true;
			PenThickness = 1;
			AntiAlias = true;

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddSeparateRow(null, StartFiguresControl(), CloseFiguresControl(), ConnectPathControl(), AntiAliasControl(), null);
			if (Platform.Instance.Supports<NumericStepper>())
				layout.AddSeparateRow(null, PenThicknessControl(), PenJoinControl(), PenCapControl(), null);
			layout.AddSeparateRow(null, ShowBounds(), CurrentPoint(), null);
			layout.BeginVertical();
			layout.AddRow(new Label { Text = "Draw Line Path" }, DrawLinePath());
			layout.AddRow(new Label { Text = "Fill Line Path" }, FillLinePath());
			layout.EndVertical();
			layout.Add(null);

			Content = layout;
		}

		Control AntiAliasControl()
		{
			var control = new CheckBox { Text = "AntiAlias" };
			control.CheckedBinding.Bind(() => AntiAlias, val => { AntiAlias = val ?? false; Refresh(); });
			return control;
		}

		Control StartFiguresControl()
		{
			var control = new CheckBox { Text = "Start Figures" };
			control.CheckedBinding.Bind(() => StartFigures, val => { StartFigures = val ?? false; Refresh(); });
			return control;
		}

		Control CloseFiguresControl()
		{
			var control = new CheckBox { Text = "Close Figures" };
			control.CheckedBinding.Bind(() => CloseFigures, val => { CloseFigures = val ?? false; Refresh(); });
			return control;
		}

		Control ConnectPathControl()
		{
			var control = new CheckBox { Text = "Connect Paths" };
			control.CheckedBinding.Bind(() => ConnectPath, val => { ConnectPath = val ?? false; Refresh(); });
			return control;
		}

		Control PenThicknessControl()
		{
			var control = new NumericStepper { MinValue = 1, MaxValue = 100 };
			control.ValueBinding.Bind(() => PenThickness, val => { PenThickness = (float)val; Refresh(); });

			var layout = new DynamicLayout { Padding = Padding.Empty };
			layout.AddRow(new Label { Text = "Thickness:", VerticalAlignment = VerticalAlignment.Center }, control);
			return layout;
		}

		Control PenJoinControl()
		{
			var control = new EnumDropDown<PenLineJoin>();
			control.SelectedValueBinding.Bind(() => LineJoin, val => { LineJoin = val; Refresh(); });
			return control;
		}

		Control PenCapControl()
		{
			var control = new EnumDropDown<PenLineCap>();
			control.SelectedValueBinding.Bind(() => LineCap, val => { LineCap = val; Refresh(); });
			return control;
		}

		Control ShowBounds()
		{
			var control = new Label();
			PathChanged += path => control.Text = string.Format("Bounds: {0}", path.Bounds);
			return control;
		}

		Control CurrentPoint()
		{
			var control = new Label();
			PathChanged += path => control.Text = string.Format("CurrentPoint: {0}", path.CurrentPoint);
			return control;
		}

		void Refresh()
		{
			path = null;
			foreach (var d in Children.OfType<Drawable>())
			{
				d.Invalidate();
			}
		}

		Control DrawLinePath()
		{
			var control = new Drawable { Size = new Size(550, 200), BackgroundColor = Colors.Black };
			control.Paint += (sender, e) =>
			{
				var pen = new Pen(Colors.White, PenThickness);
				pen.LineJoin = LineJoin;
				pen.LineCap = LineCap;
				e.Graphics.AntiAlias = AntiAlias;
				e.Graphics.DrawPath(pen, Path);
				e.Graphics.AntiAlias = false;
			};
			return control;
		}

		Control FillLinePath()
		{
			var control = new Drawable { Size = new Size(550, 200), BackgroundColor = Colors.Black };
			control.Paint += (sender, e) =>
			{
				e.Graphics.AntiAlias = AntiAlias;
				e.Graphics.FillPath(Brushes.White, Path);
				e.Graphics.AntiAlias = false;
			};
			return control;
		}

		GraphicsPath CreateMainPath()
		{
			var mainPath = CreatePath();

			mainPath.StartFigure();
			mainPath.AddLine(220, 120, 220, 170);

			// test adding child paths and transforms
			var childPath = CreatePath();
			var matrix = Matrix.Create();
			matrix.Translate(240, 120);
			matrix.Scale(0.25f);
			childPath.Transform(matrix);
			mainPath.AddPath(childPath, ConnectPath);

			mainPath.AddLine(370, 120, 370, 170);

			return mainPath;
		}

		GraphicsPath CreatePath()
		{
			var newPath = new GraphicsPath();
			var start = StartFigures;
			var close = CloseFigures;

			// connected segments

			newPath.MoveTo(10, 10);
			newPath.LineTo(20, 90);
			newPath.LineTo(10, 60);
			newPath.LineTo(90, 80);
			newPath.LineTo(60, 30);
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddArc(100, 0, 100, 50, 200, -160);
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddBezier(new PointF(200, 10), new PointF(285, 20), new PointF(210, 85), new PointF(300, 90));
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddCurve(new PointF(310, 90), new PointF(390, 90), new PointF(390, 10), new PointF(310, 10));
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddLine(410, 10, 410, 90);
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddLines(new PointF(420, 10), new PointF(420, 90));
			if (close && start)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddLines(new PointF(430, 10), new PointF(430, 90));
			if (close)
				newPath.CloseFigure();

			// separate segments

			if (start)
				newPath.StartFigure();
			newPath.AddEllipse(100, 100, 100, 45);
			if (close)
				newPath.CloseFigure();

			if (start)
				newPath.StartFigure();
			newPath.AddRectangle(10, 110, 80, 80);
			if (close)
				newPath.CloseFigure();

			// at the end, draw a line so we can potentially connect to parent path
			if (start)
				newPath.StartFigure();
			newPath.AddLines(new PointF(440, 10), new PointF(440, 90));
			if (close)
				newPath.CloseFigure();

			return newPath;
		}
	}
}
