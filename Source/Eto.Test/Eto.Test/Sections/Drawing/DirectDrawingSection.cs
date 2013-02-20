using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace Eto.Test.Sections.Drawing
{
	public class DirectDrawingSection : Drawable
	{
		SizeF increment = new SizeF (3, 3);
		List<Box> boxes = new List<Box> ();
		UITimer timer;

		class Box
		{
			static Random random = new Random ();
			RectangleF position;
			SizeF increment;
			Color color;
			float rotation;
			float angle;
			Action<Graphics, Color> draw;

			public SizeF Increment { get { return increment; } set { increment = value; } }

			public RectangleF Position { get { return position; } set { position = value; } }

			public Box (Size size)
			{
				position = new RectangleF (new SizeF(random.Next (50) + 50, random.Next (50) + 50));
				position.Location = new PointF(random.Next (size.Width - (int)position.Width), random.Next (size.Height - (int)position.Height));
				increment = new SizeF (random.Next (3) + 1, random.Next (3) + 1);
				if (random.Next (2) == 1)
					increment.Width = -increment.Width;
				if (random.Next (2) == 1)
					increment.Height = -increment.Height;

				angle = random.Next (360);
				rotation = (random.Next (20) - 10f) / 4f;

				color = Color.FromArgb (random.Next (byte.MaxValue), random.Next (byte.MaxValue), random.Next (byte.MaxValue));
				switch (random.Next (2)) {
				case 0:
					draw = (g, c) => g.DrawRectangle (c, position);
					break;
				case 1:
					draw = (g, c) => g.DrawEllipse (c, position);
					break;
				}
			}

			public void Move (Size bounds)
			{
				position.Offset (increment);
				if (increment.Width > 0 && position.Right >= bounds.Width)
					increment.Width = -increment.Width;
				else if (increment.Width < 0 && position.Left < 0)
					increment.Width = -increment.Width;
				
				if (increment.Height > 0 && position.Bottom >= bounds.Height)
					increment.Height = -increment.Height;
				else if (increment.Height < 0 && position.Top < 0)
					increment.Height = -increment.Height;
				angle += rotation;
			}

			public void Erase (Graphics graphics)
			{
				graphics.SaveTransform ();
				graphics.MultiplyTransform (Matrix.FromRotationAt (angle, position.Center));
				draw (graphics, Colors.Black);
				graphics.RestoreTransform ();
			}

			public void Draw (Graphics graphics)
			{
				graphics.SaveTransform ();
				graphics.MultiplyTransform (Matrix.FromRotationAt (angle, position.Center));
				draw (graphics, color);
				graphics.RestoreTransform ();
			}

		}

		void InitializeBoxes ()
		{
			var size = this.Size;
			for (int i = 0; i < 20; i++) {
				boxes.Add (new Box (size));
			}
		}

		public DirectDrawingSection ()
		{
			this.BackgroundColor = Colors.Black;
			timer = new UITimer {
				Interval = 0.01
			};
			timer.Elapsed += (sender, e) => {
				if (this.ParentWindow == null) {
					timer.Stop ();
					return;
				}

				if (boxes.Count == 0)
					InitializeBoxes ();

				var bounds = this.Size;
				try {
					using (var graphics = this.CreateGraphics ()) {
						graphics.Antialias = false;
						foreach (var box in boxes) {
							box.Erase (graphics);
							box.Move (bounds);
							box.Draw (graphics);
						} 
					}
				} catch (NotSupportedException) {
					timer.Stop ();
					this.AddDockedControl (new Label { Text = "This platform does not support direct drawing", TextColor = Colors.White });
				}
			};
		}

		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			timer.Start ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing)
				timer.Stop ();
		}
	}
}

