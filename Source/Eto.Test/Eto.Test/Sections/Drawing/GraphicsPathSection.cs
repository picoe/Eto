using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	class GraphicsPathRendererConfig
	{
		public bool StartFigures { get; set; }

		public bool CloseFigures { get; set; }

		public bool ConnectPath { get; set; }

		public float PenThickness { get; set; }

		public PenLineJoin LineJoin { get; set; }

		public PenLineCap LineCap { get; set; }
	}

	public class GraphicsPathSection : Scrollable
	{
		GraphicsPathRenderer renderer;
		GraphicsPathRendererConfig config;

		public GraphicsPathSection() : this(null)
		{
		}

		public GraphicsPathSection(DrawingToolkit toolkit)
		{
			toolkit = toolkit ?? new DrawingToolkit();
			config = new GraphicsPathRendererConfig();
			config.StartFigures = true;
			config.PenThickness = 1;
			
			var layout = new DynamicLayout();

			layout.AddSeparateRow(null, StartFiguresControl(), CloseFiguresControl(), ConnectPathControl(), null);
			layout.AddSeparateRow(null, PenThicknessControl(), PenJoinControl(), PenCapControl(), null);
			renderer = new GraphicsPathRenderer(config); // A shared config. Create the renderer after the bindings have been created.
			layout.AddSeparateRow(null, Bounds(), CurrentPoint(), null);
			layout.BeginVertical();
			layout.AddRow(new Label { Text = "Draw Line Path" }, DrawLinePath(toolkit.Clone()));
			layout.AddRow(new Label { Text = "Fill Line Path" }, FillLinePath(toolkit.Clone()));
			layout.EndVertical();
			layout.Add(null);

			Content = layout;
		}

		Control StartFiguresControl()
		{
			var control = new CheckBox { Text = "Start Figures" };
			control.Bind(cb => cb.Checked, this.config, r => r.StartFigures);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control CloseFiguresControl()
		{
			var control = new CheckBox { Text = "Close Figures" };
			control.Bind(cb => cb.Checked, this.config, r => r.CloseFigures);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control ConnectPathControl()
		{
			var control = new CheckBox { Text = "Connect Paths" };
			control.Bind(cb => cb.Checked, this.config, r => r.ConnectPath);
			control.CheckedChanged += Refresh;
			return control;
		}

		Control PenThicknessControl()
		{
			var control = new NumericUpDown { MinValue = 1, MaxValue = 100 };
			control.Bind(c => c.Value, this.config, r => r.PenThickness);
			control.ValueChanged += Refresh;

			var layout = new DynamicLayout(Padding.Empty);
			layout.AddRow(new Label { Text = "Thickness:", VerticalAlign = VerticalAlign.Middle }, control);
			return layout;
		}

		Control PenJoinControl()
		{
			var control = new EnumComboBox<PenLineJoin>();
			control.Bind(c => c.SelectedValue, this.config, r => r.LineJoin);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control PenCapControl()
		{
			var control = new EnumComboBox<PenLineCap>();
			control.Bind(c => c.SelectedValue, this.config, r => r.LineCap);
			control.SelectedValueChanged += Refresh;
			return control;
		}

		Control Bounds()
		{
			var control = new Label();
			control.Text = string.Format("Bounds: {0}", renderer.Path.Bounds);
			return control;
		}

		Control CurrentPoint()
		{
			var control = new Label();
			control.Text = string.Format("CurrentPoint: {0}", renderer.Path.CurrentPoint);
			return control;
		}

		void Refresh(object sender, EventArgs e)
		{
			renderer.Refresh();
			foreach (var d in Children.OfType<Drawable> ())
			{
				d.Invalidate();
			}
		}

		Control DrawLinePath(DrawingToolkit toolkit)
		{
			var control = new Drawable { Size = new Size (550, 200) };
			toolkit.Initialize(control);
			control.Paint += (sender, e) => toolkit.Render(e.Graphics, g => renderer.Render(g, fill: false));
			return control;
		}

		Control FillLinePath(DrawingToolkit toolkit)
		{
			var control = new Drawable { Size = new Size (550, 200) };
			toolkit.Initialize(control);
			control.Paint += (sender, e) => toolkit.Render(e.Graphics, g => renderer.Render(g, fill: true));
			return control;
		}
	}

	class GraphicsPathRenderer
	{
		GraphicsPathRendererConfig config;

		GraphicsPath path;
		public GraphicsPath Path { get { return path = path ?? CreateMainPath(); } }

		public GraphicsPathRenderer(GraphicsPathRendererConfig config)
		{
			this.config = config;
			Refresh();
		}

		internal void Refresh()
		{
			path = null;
		}

		public void Render(Graphics g, bool fill)
		{
			if (fill)
			{
				g.FillPath(Brushes.Black(), Path);
			}
			else
			{
				var pen = new Pen(Colors.Black, config.PenThickness);
				pen.LineJoin = config.LineJoin;
				pen.LineCap = config.LineCap;
				g.DrawPath(pen, Path);
			}
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
			mainPath.AddPath(childPath, config.ConnectPath);

			mainPath.AddLine(370, 120, 370, 170);

			return mainPath;
		}

		static GraphicsPath CreatePath()
		{
			var newPath = new GraphicsPath();
			var start = config.StartFigures;
			var close = config.CloseFigures;

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
