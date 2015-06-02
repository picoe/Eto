using System;
using Eto.Forms;
using Eto.Drawing;
using System.Diagnostics;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// Wraps a drawable and renders directly to a passed
	/// Graphics or optionally via an intermediate bitmap.
	/// </summary>
	public class DrawableTarget
	{
		readonly Drawable drawable;
		Graphics bitmapGraphics;
		public Bitmap OffscreenBitmap { get; private set; }

		bool useOffScreenBitmap;
		public bool UseOffScreenBitmap
		{
			get { return useOffScreenBitmap; }
			set
			{
				useOffScreenBitmap = value;
				drawable.Invalidate();
			}
		}

		public DrawableTarget(Drawable drawable)
		{
			this.drawable = drawable;
			UseOffScreenBitmap = false;
		}

		Size renderSize;

		public Graphics BeginDraw(PaintEventArgs e)
		{
			if (UseOffScreenBitmap)
			{
				var screen = drawable.ParentWindow.Screen;
				var scale = screen.RealScale / screen.Scale;
				renderSize = Size.Round(e.ClipRectangle.Size * scale);
				if (OffscreenBitmap == null ||
					OffscreenBitmap.Size.Width < renderSize.Width ||
					OffscreenBitmap.Size.Height < renderSize.Height)
				{
					if (OffscreenBitmap != null)
						OffscreenBitmap.Dispose();

					OffscreenBitmap = new Bitmap(renderSize, PixelFormat.Format32bppRgba);
				}
				bitmapGraphics = new Graphics(OffscreenBitmap);
				bitmapGraphics.ScaleTransform(scale);
				bitmapGraphics.TranslateTransform(-e.ClipRectangle.Location);
				bitmapGraphics.SetClip(e.ClipRectangle); // should not be affected by transform
				bitmapGraphics.Clear(Brushes.Cached(drawable.BackgroundColor));
				return bitmapGraphics;
			}
			return e.Graphics;
		}

		public void EndDraw(PaintEventArgs e)
		{
			if (bitmapGraphics != null)
			{
				bitmapGraphics.Dispose();
				bitmapGraphics = null;
				e.Graphics.DrawImage(OffscreenBitmap, new RectangleF(renderSize), e.ClipRectangle);
				if (drawable.Platform.IsWpf)
				{
					// wpf runs out of resources fast here, so we garbage collect
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
		}

		public Control Checkbox()
		{
			var control = new CheckBox
			{
				Text = "Use Offscreen Bitmap",
				Checked = UseOffScreenBitmap,
			};
			control.CheckedChanged += (sender, e) => UseOffScreenBitmap = control.Checked ?? false;
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
			var drawable = new Drawable();
			var drawableTarget = new DrawableTarget(drawable);
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };
			layout.AddSeparateRow(null, drawableTarget.Checkbox(), null);
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
				var graphics = drawableTarget.BeginDraw(e);

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

				drawableTarget.EndDraw(e);
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
