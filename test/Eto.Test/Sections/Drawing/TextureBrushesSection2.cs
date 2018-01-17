using System;
using Eto.Forms;
using Eto.Drawing;
using System.Diagnostics;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// Drawable with option to double buffer drawing operations
	/// </summary>
	public class BufferedDrawable : Drawable
	{
		Bitmap bitmap;
		Size renderSize;
		bool enableDoubleBuffering;

		public bool EnableDoubleBuffering
		{
			get { return enableDoubleBuffering; }
			set
			{
				enableDoubleBuffering = value;
				Invalidate();
			}
		}


		protected virtual void OnBufferedPaint(PaintEventArgs e)
		{
		}

		protected sealed override void OnPaint(PaintEventArgs e)
		{
			if (EnableDoubleBuffering)
			{
				var screen = ParentWindow.Screen;
				var scale = screen.RealScale / screen.Scale;
				renderSize = Size.Round(e.ClipRectangle.Size * scale);
				if (bitmap == null ||
					bitmap.Size.Width < renderSize.Width ||
					bitmap.Size.Height < renderSize.Height)
				{
					if (bitmap != null)
						bitmap.Dispose();

					bitmap = new Bitmap(renderSize, PixelFormat.Format32bppRgba);
				}
				var bitmapGraphics = new Graphics(bitmap);
				bitmapGraphics.Clear(Brushes.Cached(BackgroundColor));
				bitmapGraphics.ScaleTransform(scale);
				bitmapGraphics.TranslateTransform(-e.ClipRectangle.Location);
				bitmapGraphics.SetClip(e.ClipRectangle); // should be affected by transform

				var childArgs = new PaintEventArgs(bitmapGraphics, e.ClipRectangle);
				base.OnPaint(childArgs);

				OnBufferedPaint(childArgs);

				bitmapGraphics.Dispose();
				e.Graphics.DrawImage(bitmap, new RectangleF(renderSize), e.ClipRectangle);
				//Log.Write(this, $"Clip: {e.ClipRectangle}, renderSize: {renderSize}");
			}
			else
			{
				base.OnPaint(e);
				OnBufferedPaint(e);
			}
		}

		public Control Checkbox()
		{
			var control = new CheckBox { Text = "Use Double Buffer" };
			control.CheckedBinding.Bind(this, c => c.EnableDoubleBuffering);
			return control;
		}
	}

	[Section("Drawing", "TextureBrush 2")]
	public class TextureBrushesSection2 : Panel
	{
		readonly Bitmap image;
		PointF location = new PointF(100, 100);

		public TextureBrushesSection2()
		{
			image = TestIcons.Textures;
			var drawable = new BufferedDrawable();
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.AddSeparateRow(null, drawable.Checkbox(), null);
			layout.Add(drawable);
			this.Content = layout;

			var w = image.Size.Width / 3; // same as height
			var img = image.Clone(new Rectangle(w, w, w, w));
			var textureBrush = new TextureBrush(img);
			var solidBrush = new SolidBrush(Colors.Blue);
			var linearGradientBrush = new LinearGradientBrush(Colors.White, Colors.Black, PointF.Empty, new PointF(0, 100));
			var font = SystemFonts.Default();
			drawable.BackgroundColor = Colors.Green;
			drawable.MouseMove += HandleMouseMove;
			drawable.MouseDown += HandleMouseMove;

			drawable.Paint += (s, e) =>
			{
				var graphics = e.Graphics;
				graphics.DrawText(font, Colors.White, 3, 3, "Move the mouse in this area to move the shapes.");
				// texture brushes
				var temp = location;
				DrawShapes(textureBrush, temp, img.Size, graphics);
				// solid brushes
				temp = temp + new PointF(200, 0);
				DrawShapes(solidBrush, temp, img.Size, graphics);
				// linear gradient brushes
				temp = temp + new PointF(200, 0);
				DrawShapes(linearGradientBrush, temp, img.Size, graphics);
			};
		}

		void HandleMouseMove(object sender, MouseEventArgs e)
		{
			location = e.Location;
			((Control)sender).Invalidate();
			e.Handled = true;
		}

		void DrawShapes(Brush brush, PointF location, Size size, Graphics g)
		{
			g.SaveTransform();
			g.TranslateTransform(location);
			g.RotateTransform(20);

			// rectangle
			g.FillRectangle(brush, new RectangleF(size));

			// ellipse
			g.TranslateTransform(0, size.Height + 20);
			g.FillEllipse(brush, new RectangleF(size));

			// pie
			g.TranslateTransform(0, size.Height + 20);
			g.FillPie(brush, new RectangleF(new SizeF(size.Width * 2, size.Height)), 0, 360);

			// polygon
			g.TranslateTransform(0, size.Height + 20);
			var polygon = GetPolygon();
			g.FillPolygon(brush, polygon);

			g.RestoreTransform();
		}

		static PointF[] GetPolygon()
		{
			var polygon = new[] { new PointF(0, 50), new PointF(50, 100), new PointF(100, 50), new PointF(50, 0) };
			return polygon;
		}
	}
}
