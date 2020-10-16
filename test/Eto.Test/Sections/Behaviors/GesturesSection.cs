using System;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;

namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Gestures")]
	public class GesturesSection : Scrollable
	{
		readonly RectangleF displayBounds = Screen.DisplayBounds;
		readonly Screen[] screens;
		Drawable gesturesDrawable;
		Window parentWindow;
		Label windowPositionLabel;
		Label mousePositionLabel;
		bool enableGestures;
		
		private double swipe_x;
		private double swipe_y;
		private double zoom_scale;
		private double rotate_angle;
		private bool long_pressed;
		private double pan_x;
		private double pan_y;
		private bool pan_active;
		private bool zoom_active;
		private bool rotate_active;
		protected bool EnableGestureHandling
		{
			get => enableGestures;
			set
			{
				enableGestures = value;
				gesturesDrawable.Invalidate(false);
			}
		}

		public GesturesSection()
		{
			zoom_scale = 1.0;
			rotate_angle = 0;
			
			var layout = new DynamicLayout { DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			var enableGestureHandling = new CheckBox { Text = "Handle Events" };
			enableGestureHandling.CheckedBinding.Bind(this, c => c.EnableGestureHandling);
			layout.Add(enableGestureHandling);

			var area = DefineGestureWindow();
			
			LogEvents(area);
			
			layout.Add(area);

			Content = layout;
		}
		
		protected void LogEvents(Control control)
		{			

			control.Swipe += delegate(object sender, SwipeGestureEventArgs e)
			{
				swipe_x = e.VelocityX / 20.0;
				swipe_y = e.VelocityY / 20.0;
				LogSwipeEvent(control, "Swipe Gesture", e);
				gesturesDrawable.Invalidate();

			};
			
			control.Longpress += delegate(object sender, LongPressGestureEventArgs e)
			{
				long_pressed = e.Pressed;
				LogLongpressEvent(control, "Longpress Gesture", e);
				gesturesDrawable.Invalidate();
			};

			control.PanH += HandlePan;
			control.PanV += HandlePan;
			
			control.Rotate += delegate(object sender, RotateGestureEventArgs e)
			{
				rotate_angle = e.AngleDelta;

				rotate_active = true;

				LogRotateEvent(control, "Rotate Gesture", e);
				gesturesDrawable.Invalidate();
			};
			
			control.ZoomExpand += delegate(object sender, ZoomGestureEventArgs e)
			{
				zoom_scale = e.ScaleDelta;
				zoom_active = true;
				LogZoomEvent(control, "Zoom Gesture", e);
				gesturesDrawable.Invalidate();
			};
		}

		private void HandlePan(object sender, PanGestureEventArgs e)
		{
			Control control = sender as Control;
			pan_active = true;
			switch (e.Direction)
			{
				case PanDirection.Down:
					pan_y = e.Offset;
					break;
				case PanDirection.Up:
					pan_y = -1 * e.Offset;
					break;
				case PanDirection.Left:
					pan_x = -1 * e.Offset;
					break;
				case PanDirection.Right:
					pan_x = e.Offset;
					break;
			}
			LogPanEvent(control, "Pan Gesture", e);
			gesturesDrawable.Invalidate();
		}

		Control DefineGestureWindow()
		{
			gesturesDrawable = new Drawable();
			gesturesDrawable.Paint += (sender, args) =>
			{
				Drawable control = sender as Drawable;
				double center_x = control.Size.Width / 2;
				double center_y = control.Size.Height / 2;

				args.Graphics.FillRectangle(Brushes.Black, args.ClipRectangle);

				// rotate zoom box
				if (rotate_active || zoom_active)
				{
					args.Graphics.SaveTransform();
					args.Graphics.TranslateTransform((float)(center_x), (float)(center_y));
					args.Graphics.RotateTransform((float)(rotate_angle * (180 / Math.PI)));
					args.Graphics.ScaleTransform((float)zoom_scale, (float)zoom_scale);
					RectangleF bound = new RectangleF(-30, -30, 60, 60);
					args.Graphics.FillRectangle(Colors.CadetBlue, bound);
					args.Graphics.RestoreTransform();
				}


				// label
				var size = args.Graphics.MeasureString(SystemFonts.Label(), "Gestures Drawable");
				var p1 = new PointF((float)center_x, (float)center_y);
				args.Graphics.DrawText(SystemFonts.Label(), Brushes.White, (PointF)((control.Size - size) / 2), "Gestures Drawable");

				// swipe bar
				if (swipe_x != 0 || swipe_y != 0)
				{				
					var p2 = new PointF((float)(swipe_x+center_x), (float)(swipe_y+center_y));
					args.Graphics.DrawLine(Colors.Red, p1, p2);
				}

				// pan circle (horizontal)
				if (pan_active)
				{
					if (pan_x < (-1 * center_x + 20))
					{
						pan_x = center_x * -1;
					}
					if (pan_x > (center_x))
					{
						pan_x = center_x;
					}
					if (pan_y < (-1 * center_y + 20))
					{
						pan_y = center_y * -1;
					}
					if (pan_y > (center_y))
					{
						pan_y = center_y;
					}
					double cx = center_x + pan_x;
					double cy = center_y + pan_y;
					RectangleF bound = new RectangleF((float)(cx - 10), (float)(cy - 10), 20, 20);

					var p2 = new PointF((float)(swipe_x+center_x), (float)(swipe_y+center_y));
					args.Graphics.DrawEllipse(Colors.Aquamarine, bound);
				}

				// long press
				if (long_pressed)
				{

					RectangleF bound = new RectangleF((float)(center_x - 25), (float)(center_y - 25), 50, 50);

					var p2 = new PointF((float)(swipe_x+center_x), (float)(swipe_y+center_y));
					args.Graphics.DrawEllipse(Colors.Gold, bound);
				}
			};
			return gesturesDrawable;
		}
		
		void LogSwipeEvent(object sender, string type, SwipeGestureEventArgs e)
		{
			Console.WriteLine("{0}, VelocityX: {1}, VelocityY: {2}", type, e.VelocityX, e.VelocityY);
			if (enableGestures == true)
				e.Handled = true;
		}

		void LogLongpressEvent(object sender, string type, LongPressGestureEventArgs e)
		{
			Console.WriteLine("{0}, Pressed: {1}, N Press: {2} X: {3} Y: {4}", type, e.Pressed, e.NPress, e.X, e.Y);
			if (enableGestures == true)
				e.Handled = true;
		}

		void LogPanEvent(object sender, string type, PanGestureEventArgs e)
		{

			Console.WriteLine("{0}, Direction: {1}, Offset: {2}", type, e.Direction, e.Offset);
			if (enableGestures == true)
				e.Handled = true;
		}

		void LogRotateEvent(object sender, string type, RotateGestureEventArgs e)
		{

			Console.WriteLine("{0}, Angle Delta: {1} Rotation {2}", type, e.AngleDelta, rotate_angle);
			if (enableGestures == true)
				e.Handled = true;
		}

		void LogZoomEvent(object sender, string type, ZoomGestureEventArgs e)
		{
			Console.WriteLine("{0}, Scale Delta: {1}", type, e.ScaleDelta);
			if (enableGestures == true)
				e.Handled = true;
		}

	}
}

