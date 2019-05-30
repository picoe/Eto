using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "PixelOffsetMode Transforms")]
	public class PixelOffsetTransforms : StackLayout
	{
		class TestInfo
		{
			public string Name { get; set; }

			public Action<Graphics> Test { get; set; }
		}

		List<TestInfo> tests = new List<TestInfo> {
			new TestInfo { Name = "Scissors", Test = ScissorsTest },
			new TestInfo { Name = "Ellipse and Curve", Test = EllipseAndCurveTest }
		};

		public PixelOffsetTransforms()
		{
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			Spacing = 5;

			var canvas = new TestCanvas();

			var offsetMode = new EnumDropDown<PixelOffsetMode>();
			offsetMode.SelectedValueBinding.Bind(canvas, c => c.PixelOffsetMode);

			var testDropDown = new DropDown();
			testDropDown.ItemTextBinding = Binding.Property((TestInfo t) => t.Name);
			testDropDown.SelectedValueBinding.Cast<TestInfo>().Bind(canvas, c => c.Test);
			testDropDown.DataStore = tests;
			testDropDown.SelectedIndex = 0;

			var options = new StackLayout
			{
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Spacing = 5,
				Padding = new Padding(10),
				Items =
				{
					"PixelOffsetMode",
					offsetMode,
					"Test",
					testDropDown
				}
			};

			Items.Add(options);
			Items.Add(new StackLayoutItem(canvas, true));
		}

		class TestCanvas : Drawable
		{
			PixelOffsetMode pixelOffsetMode;
			TestInfo test;

			public PixelOffsetMode PixelOffsetMode
			{
				get { return pixelOffsetMode; }
				set
				{
					pixelOffsetMode = value;
					Invalidate();
				}
			}

			public TestInfo Test
			{
				get { return test; }
				set
				{
					test = value;
					Invalidate();
				}
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				var graphics = e.Graphics;
				graphics.PixelOffsetMode = pixelOffsetMode;

				graphics.Clear(Brushes.White);

				if (test != null)
					test.Test(e.Graphics);
			}
		}

		static void EllipseAndCurveTest(Graphics graphics)
		{
			var path = new GraphicsPath();
			path.FillMode = FillMode.Winding;
			path.AddBezier(new PointF(10f, 6f), new PointF(10f, 4f), new PointF(8f, 2f), new PointF(6f, 2f));
			path.CloseFigure();
			var m = Matrix.Create();
			m.Scale(60f, 60f);
			graphics.SaveTransform();
			graphics.MultiplyTransform(m);
			var brush = new SolidBrush(Colors.Black);
			var pen = new Pen(Colors.Red, 0.1f);
			graphics.FillPath(brush, path);
			graphics.DrawPath(pen, path);
			brush.Dispose();
			pen.Dispose();
			var brushY = new SolidBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x9F));
			var penB = new Pen(Color.FromArgb(0x00, 0x00, 0xFF, 0x9F), 0.1f);
			graphics.DrawEllipse(penB, 10f, 6f, 0.5f, 0.5f);
			graphics.FillEllipse(brushY, 10f, 6f, 0.5f, 0.5f);
			graphics.DrawEllipse(penB, 6f, 2f, 0.5f, 0.5f);
			graphics.FillEllipse(brushY, 6f, 2f, 0.5f, 0.5f);
			graphics.RestoreTransform();
		}

		static void ScissorsTest(Graphics graphics)
		{
			var t = new
			{
				CenterX = 0f,
				CenterY = 0f,
				OffsetX = 200f,
				OffsetY = 50f,
				RotateAngle = 0f,
				SkewAngleY = 10f,
				SkewAngleX = 10f,
				ScaleX = 16f,
				ScaleY = 16f
			};

			var path = new GraphicsPath();
			path.FillMode = FillMode.Winding;
			// draw some scissors
			path.AddBezier(new PointF(9.64f, 7.64f), new PointF(9.87f, 7.14f), new PointF(10f, 6.59f), new PointF(10f, 6f));
			path.AddBezier(new PointF(10f, 6f), new PointF(10f, 3.79f), new PointF(8.21f, 2f), new PointF(6f, 2f));
			path.AddBezier(new PointF(6f, 2f), new PointF(3.79f, 2f), new PointF(2f, 3.79f), new PointF(2f, 6f));
			path.AddBezier(new PointF(2f, 6f), new PointF(2f, 8.21f), new PointF(3.79f, 10f), new PointF(6f, 10f));
			path.AddBezier(new PointF(6f, 10f), new PointF(6.59f, 10f), new PointF(7.14f, 9.87f), new PointF(7.64f, 9.64f));
			path.AddLine(7.64f, 9.64f, 10f, 12f);
			path.AddLine(10f, 12f, 7.64f, 14.36f);
			path.AddBezier(new PointF(7.64f, 14.36f), new PointF(7.14f, 14.13f), new PointF(6.59f, 14f), new PointF(6f, 14f));
			path.AddBezier(new PointF(6f, 14f), new PointF(3.79f, 14f), new PointF(2f, 15.79f), new PointF(2f, 18f));
			path.AddBezier(new PointF(2f, 18f), new PointF(2f, 20.21f), new PointF(3.79f, 22f), new PointF(6f, 22f));
			path.AddBezier(new PointF(6f, 22f), new PointF(8.21f, 22f), new PointF(10f, 20.21f), new PointF(10f, 18f));
			path.AddBezier(new PointF(10f, 18f), new PointF(10f, 17.41f), new PointF(9.87f, 16.86f), new PointF(9.64f, 16.36f));
			path.AddLine(9.64f, 16.36f, 12f, 14f);
			path.AddLine(12f, 14f, 19f, 21f);
			path.AddLine(19f, 21f, 22f, 21f);
			path.AddLine(22f, 21f, 22f, 20f);
			path.AddLine(22f, 20f, 9.64f, 7.64f);
			path.CloseFigure();
			path.AddBezier(new PointF(6f, 8f), new PointF(4.9f, 8f), new PointF(4f, 7.11f), new PointF(4f, 6f));
			path.AddBezier(new PointF(4f, 6f), new PointF(4f, 4.89f), new PointF(4.9f, 4f), new PointF(6f, 4f));
			path.AddBezier(new PointF(6f, 4f), new PointF(7.1f, 4f), new PointF(8f, 4.89f), new PointF(8f, 6f));
			path.AddBezier(new PointF(8f, 6f), new PointF(8f, 7.11f), new PointF(7.1f, 8f), new PointF(6f, 8f));
			path.CloseFigure();
			path.AddBezier(new PointF(6f, 20f), new PointF(4.9f, 20f), new PointF(4f, 19.11f), new PointF(4f, 18f));
			path.AddBezier(new PointF(4f, 18f), new PointF(4f, 16.89f), new PointF(4.9f, 16f), new PointF(6f, 16f));
			path.AddBezier(new PointF(6f, 16f), new PointF(7.1f, 16f), new PointF(8f, 16.89f), new PointF(8f, 18f));
			path.AddBezier(new PointF(8f, 18f), new PointF(8f, 19.11f), new PointF(7.1f, 20f), new PointF(6f, 20f));
			path.CloseFigure();
			path.AddBezier(new PointF(12f, 12.5f), new PointF(11.72f, 12.5f), new PointF(11.5f, 12.28f), new PointF(11.5f, 12f));
			path.AddBezier(new PointF(11.5f, 12f), new PointF(11.5f, 11.72f), new PointF(11.72f, 11.5f), new PointF(12f, 11.5f));
			path.AddBezier(new PointF(12f, 11.5f), new PointF(12.28f, 11.5f), new PointF(12.5f, 11.72f), new PointF(12.5f, 12f));
			path.AddBezier(new PointF(12.5f, 12f), new PointF(12.5f, 12.28f), new PointF(12.28f, 12.5f), new PointF(12f, 12.5f));
			path.CloseFigure();
			path.AddLine(19f, 3f, 13f, 9f);
			path.AddLine(13f, 9f, 15f, 11f);
			path.AddLine(15f, 11f, 22f, 4f);
			path.AddLine(22f, 4f, 22f, 3f);
			path.CloseFigure();
			path.AddBezier(new PointF(9.64f, 7.64f), new PointF(9.87f, 7.14f), new PointF(10f, 6.59f), new PointF(10f, 6f));
			path.AddBezier(new PointF(10f, 6f), new PointF(10f, 3.79f), new PointF(8.21f, 2f), new PointF(6f, 2f));
			path.AddBezier(new PointF(6f, 2f), new PointF(3.79f, 2f), new PointF(2f, 3.79f), new PointF(2f, 6f));
			path.AddBezier(new PointF(2f, 6f), new PointF(2f, 8.21f), new PointF(3.79f, 10f), new PointF(6f, 10f));
			path.AddBezier(new PointF(6f, 10f), new PointF(6.59f, 10f), new PointF(7.14f, 9.87f), new PointF(7.64f, 9.64f));
			path.AddLine(7.64f, 9.64f, 10f, 12f);
			path.AddLine(10f, 12f, 7.64f, 14.36f);
			path.AddBezier(new PointF(7.64f, 14.36f), new PointF(7.14f, 14.13f), new PointF(6.59f, 14f), new PointF(6f, 14f));
			path.AddBezier(new PointF(6f, 14f), new PointF(3.79f, 14f), new PointF(2f, 15.79f), new PointF(2f, 18f));
			path.AddBezier(new PointF(2f, 18f), new PointF(2f, 20.21f), new PointF(3.79f, 22f), new PointF(6f, 22f));
			path.AddBezier(new PointF(6f, 22f), new PointF(8.21f, 22f), new PointF(10f, 20.21f), new PointF(10f, 18f));
			path.AddBezier(new PointF(10f, 18f), new PointF(10f, 17.41f), new PointF(9.87f, 16.86f), new PointF(9.64f, 16.36f));
			path.AddLine(9.64f, 16.36f, 12f, 14f);
			path.AddLine(12f, 14f, 19f, 21f);
			path.AddLine(19f, 21f, 22f, 21f);
			path.AddLine(22f, 21f, 22f, 20f);
			path.AddLine(22f, 20f, 9.64f, 7.64f);
			path.CloseFigure();
			path.AddBezier(new PointF(6f, 8f), new PointF(4.9f, 8f), new PointF(4f, 7.11f), new PointF(4f, 6f));
			path.AddBezier(new PointF(4f, 6f), new PointF(4f, 4.89f), new PointF(4.9f, 4f), new PointF(6f, 4f));
			path.AddBezier(new PointF(6f, 4f), new PointF(7.1f, 4f), new PointF(8f, 4.89f), new PointF(8f, 6f));
			path.AddBezier(new PointF(8f, 6f), new PointF(8f, 7.11f), new PointF(7.1f, 8f), new PointF(6f, 8f));
			path.CloseFigure();
			path.AddBezier(new PointF(6f, 20f), new PointF(4.9f, 20f), new PointF(4f, 19.11f), new PointF(4f, 18f));
			path.AddBezier(new PointF(4f, 18f), new PointF(4f, 16.89f), new PointF(4.9f, 16f), new PointF(6f, 16f));
			path.AddBezier(new PointF(6f, 16f), new PointF(7.1f, 16f), new PointF(8f, 16.89f), new PointF(8f, 18f));
			path.AddBezier(new PointF(8f, 18f), new PointF(8f, 19.11f), new PointF(7.1f, 20f), new PointF(6f, 20f));
			path.CloseFigure();
			path.AddBezier(new PointF(12f, 12.5f), new PointF(11.72f, 12.5f), new PointF(11.5f, 12.28f), new PointF(11.5f, 12f));
			path.AddBezier(new PointF(11.5f, 12f), new PointF(11.5f, 11.72f), new PointF(11.72f, 11.5f), new PointF(12f, 11.5f));
			path.AddBezier(new PointF(12f, 11.5f), new PointF(12.28f, 11.5f), new PointF(12.5f, 11.72f), new PointF(12.5f, 12f));
			path.AddBezier(new PointF(12.5f, 12f), new PointF(12.5f, 12.28f), new PointF(12.28f, 12.5f), new PointF(12f, 12.5f));
			path.CloseFigure();
			path.AddLine(19f, 3f, 13f, 9f);
			path.AddLine(13f, 9f, 15f, 11f);
			path.AddLine(15f, 11f, 22f, 4f);
			path.AddLine(22f, 4f, 22f, 3f);
			path.CloseFigure();
			var m = Matrix.Create();
			var c = new PointF((float)t.CenterX, (float)t.CenterY);
			// translate
			m.Translate((float)t.OffsetX, (float)t.OffsetY);
			// rotate
			m.RotateAt((float)t.RotateAngle, c);
			// skew
			m.Translate(-c.X, -c.Y);
			m.Prepend(Matrix.Create(1, (float)Math.Tan(Math.PI * t.SkewAngleY / 180.0), (float)Math.Tan(Math.PI * t.SkewAngleX / 180.0), 1, 0, 0));
			m.Translate(c.X, c.Y);
			// scale
			m.ScaleAt((float)t.ScaleX, (float)t.ScaleY, (float)t.CenterX, (float)t.CenterY);
			graphics.SaveTransform();
			graphics.MultiplyTransform(m);
			var brush = new SolidBrush(Colors.Black);
			var pen = new Pen(Colors.Red, 0.5f);
			graphics.FillPath(brush, path);
			graphics.DrawPath(pen, path);
			brush.Dispose();
			pen.Dispose();
			graphics.RestoreTransform();
		}
		}
}

