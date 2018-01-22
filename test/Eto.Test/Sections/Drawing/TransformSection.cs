using Eto.Drawing;
using Eto.Forms;
using System;

namespace Eto.Test.Sections.Drawing
{
	[Section("Drawing", "Transforms")]
	public class TransformSection : Scrollable
	{
		Image image;
		Size canvasSize = new Size(500, 221);
		PointF rotatedLineCenter = new PointF(60, 60);
		PointF rotatedTextCenter = new PointF(100, 100);
		PointF imageScaleLocation = new PointF(240, 0);
		Font font;

		public TransformSection()
		{
			image = TestIcons.TestIcon;
			font = Fonts.Sans(10);

			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) =>
			{
				pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
				MatrixTests(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "Matrix" }, drawable);

			drawable = new Drawable { Size = canvasSize };
			drawable.Paint += (sender, pe) =>
			{
				pe.Graphics.FillRectangle(Brushes.Black, pe.ClipRectangle);
				DirectTests(pe.Graphics);
			};
			layout.AddRow(new Label { Text = "Direct" }, drawable);
			layout.Add(null);

			var m = Matrix.Create();
			m.Scale(100, 100);
			var m2 = m.Clone();
			m2.Translate(10, 10);

			if (m == m2)
				throw new ArgumentOutOfRangeException("Grr!");

			Content = layout;
		}

		void MatrixTests(Graphics g)
		{
			// test matrix RotateAt
			DrawRotatedLines(g, Colors.SkyBlue, rotatedLineCenter, (center, angle) =>
			{
				var m = Matrix.Create();
				m.RotateAt(angle, center);
				g.MultiplyTransform(m);
			});

			// test matrix scale/translate/rotate
			DrawRotatedLines(g, Colors.Salmon, rotatedLineCenter, (center, angle) =>
			{
				var m = Matrix.Create();
				m.Translate(center);
				m.Rotate(angle);
				m.Translate(-center);
				m.Scale(.4f);
				g.MultiplyTransform(m);
			});

			// test rotating arcs
			DrawRotatedArcs(g, Colors.LightBlue, rotatedLineCenter, (center, angle) =>
			{
				var m = Matrix.Create();
				m.RotateAt(angle, center);
				g.MultiplyTransform(m);
			});

			// test transformed text
			DrawRotatedText(g, Colors.Lime, rotatedTextCenter, (center, angle) =>
			{
				var m = Matrix.Create();
				m.RotateAt(angle, center - 40);
				g.MultiplyTransform(m);
			});

			// test image drawing
			{
				g.SaveTransform();
				g.MultiplyTransform(Matrix.FromScaleAt(new SizeF(1, -1), imageScaleLocation + image.Size / 2));
				g.DrawImage(image, imageScaleLocation);

				var m = Matrix.Create();
				m.Translate(0, -50);
				m.ScaleAt(0.3f, imageScaleLocation + image.Size / 2);
				g.MultiplyTransform(m);
				g.DrawImage(image, imageScaleLocation);
				g.RestoreTransform();
			}

			// test skewing
			{
				g.SaveTransform();
				var m = Matrix.Create();
				m.Skew(20, 20);
				g.MultiplyTransform(m);
				var textSize = g.MeasureString(font, "Skewed Text");
				g.DrawText(font, Colors.White, new PointF(110, 0), "Skewed Text");
				g.DrawLine(Pens.White, 110, textSize.Height + 2, 110 + textSize.Width, textSize.Height + 2);

				g.RestoreTransform();
			}

			// test more drawing operations
			{
				g.SaveTransform();
				var m = Matrix.Create();
				m.Translate(480, 20);
				m.Scale(0.4f);
				m.Rotate(90);
				g.MultiplyTransform(m);
				PixelOffsetSection.Draw(g);
				g.RestoreTransform();
			}
		}

		void DirectTests(Graphics g)
		{
			// test translate/rotate
			DrawRotatedLines(g, Colors.SkyBlue, rotatedLineCenter, (center, angle) =>
			{
				g.TranslateTransform(center);
				g.RotateTransform(angle);
				g.TranslateTransform(-center);
			});

			// test translate/rotate/scale
			DrawRotatedLines(g, Colors.Salmon, rotatedLineCenter, (center, angle) =>
			{
				g.TranslateTransform(center);
				g.RotateTransform(angle);
				g.TranslateTransform(-center);
				g.ScaleTransform(.4f);
			});

			// test rotating arcs
			DrawRotatedArcs(g, Colors.LightBlue, rotatedLineCenter, (center, angle) =>
			{
				g.TranslateTransform(center);
				g.RotateTransform(angle);
				g.TranslateTransform(-center);
			});

			// test transformed text
			DrawRotatedText(g, Colors.Lime, rotatedTextCenter, (center, angle) =>
			{
				g.TranslateTransform(center - 40);
				g.RotateTransform(angle);
				g.TranslateTransform(-center + 40);
			});

			// Test image drawing
			g.SaveTransform();
			g.TranslateTransform(imageScaleLocation + image.Size / 2);
			g.ScaleTransform(1, -1);
			g.TranslateTransform(-imageScaleLocation - image.Size / 2);
			g.DrawImage(image, imageScaleLocation);

			g.TranslateTransform(0, -50);
			g.TranslateTransform(imageScaleLocation + image.Size / 2);
			g.ScaleTransform(0.3f);
			g.TranslateTransform(-imageScaleLocation - image.Size / 2);
			g.DrawImage(image, imageScaleLocation);
			g.RestoreTransform();

			// test skewing
			g.SaveTransform();
			g.MultiplyTransform(Matrix.FromSkew(20, 20));
			var textSize = g.MeasureString(font, "Skewed Text");
			g.DrawText(font, Colors.White, new PointF(110, 0), "Skewed Text");
			g.DrawLine(Pens.White, 110, textSize.Height + 2, 110 + textSize.Width, textSize.Height + 2);

			g.RestoreTransform();

			// test more drawing operations
			g.SaveTransform();
			g.TranslateTransform(480, 20);
			g.ScaleTransform(0.4f);
			g.RotateTransform(90);
			PixelOffsetSection.Draw(g);
			g.RestoreTransform();
		}

		void DrawRotatedLines(Graphics g, Color color, PointF center, Action<PointF, float> action)
		{
			var pen = new Pen(color);
			for (float i = 0; i < 360f; i += 10)
			{
				g.SaveTransform();
				action(center, i);
				g.DrawLine(pen, center, center + new Size(40, 0));
				g.RestoreTransform();
			}
		}

		void DrawRotatedText(Graphics g, Color color, PointF center, Action<PointF, float> action)
		{
			for (float i = 0; i <= 90f; i += 10)
			{
				g.SaveTransform();
				action(center, i);
				g.DrawText(font, color, center, "Some Rotated Text");
				g.RestoreTransform();
			}
		}

		void DrawRotatedArcs(Graphics g, Color color, PointF center, Action<PointF, float> action)
		{
			for (float i = 0; i <= 360f; i += 90f)
			{
				g.SaveTransform();
				action(center, i);
				g.DrawArc(new Pen(color), RectangleF.FromCenter(center, new SizeF(50, 50)), 0, 45f);
				g.RestoreTransform();
			}
		}
	}
}
