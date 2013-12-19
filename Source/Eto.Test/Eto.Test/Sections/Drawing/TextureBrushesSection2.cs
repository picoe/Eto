using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Drawing
{
	/// <summary>
	/// Wraps a drawable and renders directly to a passed
	/// Graphics or optionally via an intermediate bitmap.
	/// </summary>
	class DrawableTarget
	{
		Drawable drawable;
		Graphics bitmapGraphics;
		Bitmap offscreenBitmap = null;

		bool useOffScreenBitmap;
		public bool UseOffScreenBitmap {
			get { return useOffScreenBitmap; } 
			set { 
				useOffScreenBitmap = value;
				drawable.Invalidate ();
			}
		}

		public DrawableTarget(Drawable drawable)
		{
			this.drawable = drawable;
			UseOffScreenBitmap = true;
		}

		public Graphics BeginDraw(Graphics g)
		{
			if(UseOffScreenBitmap)
			{
				if(offscreenBitmap == null ||
					offscreenBitmap.Size.Width < drawable.Size.Width ||
					offscreenBitmap.Size.Height < drawable.Size.Height)
				{
					if(offscreenBitmap != null)
						offscreenBitmap.Dispose();

					offscreenBitmap = new Bitmap(drawable.Size, PixelFormat.Format32bppRgba);
				}

				bitmapGraphics = new Graphics (offscreenBitmap);
				bitmapGraphics.Clear(Brushes.Cached(drawable.BackgroundColor) as SolidBrush);
				return bitmapGraphics;
			}
			else
				return g;
		}

		public void EndDraw (Graphics g)
		{
			if (UseOffScreenBitmap) {
				bitmapGraphics.Dispose ();
				bitmapGraphics = null;
				g.DrawImage (offscreenBitmap, PointF.Empty);
			}
		}

		public Control Checkbox()
		{
			var control = new CheckBox {
				Text = "Use Offscreen Bitmap",
				Checked = this.UseOffScreenBitmap,
			};
			control.CheckedChanged += (sender, e) => {
				this.UseOffScreenBitmap = control.Checked ?? false;
			};
			return control;
		}	
	}

	class TextureBrushesSection2 : Scrollable
	{
		Bitmap image = TestIcons.Textures;
		PointF location = new PointF(100, 100);

		public TextureBrushesSection2()
		{
			var drawable = new Drawable();
			var drawableTarget = new DrawableTarget (drawable);
			var layout = new DynamicLayout(new Padding(10));
			layout.AddSeparateRow(null, drawableTarget.Checkbox(), null);
			layout.Add(drawable);
			this.Content = layout;

			var w = image.Size.Width / 3; // same as height
			var img = image.Clone(new Rectangle(w, w, w, w));
			var textureBrush = new TextureBrush(img);
			var solidBrush = new SolidBrush(Colors.Blue);
			var linearGradientBrush = new LinearGradientBrush(Colors.White, Colors.Black, PointF.Empty, new PointF(0, 100));
			var font = new Font(SystemFont.Default);
			drawable.BackgroundColor = Colors.Green;
			drawable.MouseMove += HandleMouseMove;
			drawable.MouseDown += HandleMouseMove;

			Action<Graphics> render = g => {
				g.DrawText (font, Colors.White, 3, 3, "Move the mouse in this area to move the shapes.");
				// texture brushes
				var temp = location;
				DrawShapes (textureBrush, temp, img.Size, g);
				// solid brushes
				temp = temp + new PointF (200, 0);
				DrawShapes (solidBrush, temp, img.Size, g);
				// linear gradient brushes
				temp = temp + new PointF (200, 0);
				DrawShapes (linearGradientBrush, temp, img.Size, g);
			};
		
			drawable.Paint += (s, e) =>
			{
				render(drawableTarget.BeginDraw(e.Graphics));
				drawableTarget.EndDraw(e.Graphics);
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
			var polygon = new PointF[] { new PointF(0, 50), new PointF(50, 100), new PointF(100, 50), new PointF(50, 0) };
			return polygon;
		}
	}
}
