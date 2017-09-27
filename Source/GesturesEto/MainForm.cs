using System;
using Eto.Forms;
using Eto.Drawing;

namespace GesturesEto
{
	/// <summary>
	/// Your application's main form
	/// </summary>
	public  class MainForm : Form
	{
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

		private Drawable GestureDrawable;
		private CheckBox handleEvents;



		public MainForm()
		{
			zoom_scale = 1.0;
			rotate_angle = 0;

			Title = "Touch Gestures in Eto";

			ClientSize = new Size(600, 600);



			GestureDrawable = new Drawable { Size = new Size(550, 550), CanFocus = true };
			GestureDrawable.Paint += OnDrawn;
			LogEvents(GestureDrawable);

			handleEvents = new CheckBox { Text = "Handle Events" };

			// scrollable region as the main content
			Content = new Scrollable
			{
				// table with three rows
				Content = new TableLayout(
					handleEvents,
					GestureDrawable,
					null
				)
			};

			// create a few commands that can be used for the menu and toolbar
			var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
			clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

			var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command { MenuText = "About..." };
			aboutCommand.Executed += (sender, e) => MessageBox.Show(this, "About my app...");

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new ButtonMenuItem { Text = "&File", Items = { clickMe } },
					// new ButtonMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new ButtonMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};
		}


		void
		OnDrawn (object sender, PaintEventArgs args)
		{
			Drawable control = sender as Drawable;
			double center_x = control.Size.Width / 2;
			double center_y = control.Size.Height / 2;

			args.Graphics.FillRectangle(Brushes.DarkSlateGray, args.ClipRectangle);

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

				double cx = center_x + pan_x;
				RectangleF bound = new RectangleF((float)(cx - 10), (float)(center_y - 10), 20, 20);

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
		}

		protected void LogEvents(Control control)
		{			

			control.Swipe += delegate(object sender, SwipeGestureEventArgs e)
			{
				swipe_x = e.VelocityX / 20.0;
				swipe_y = e.VelocityY / 20.0;
				LogSwipeEvent(control, "Swipe Gesture", e);
				GestureDrawable.Invalidate();

			};
			control.Longpress += delegate(object sender, LongPressGestureEventArgs e)
			{
				long_pressed = e.Pressed;
				LogLongpressEvent(control, "Longpress Gesture", e);
				GestureDrawable.Invalidate();
			};
			control.PanH += delegate(object sender, PanGestureEventArgs e)
			{
				pan_active = true;
				pan_x = e.Offset;
				if (e.Direction == PanDirection.Left)
					pan_x *= -1;
				LogPanEvent(control, "PanH Gesture", e);
				GestureDrawable.Invalidate();
			};
			control.Rotate += delegate(object sender, RotateGestureEventArgs e)
			{
				rotate_angle = e.AngleDelta;

				rotate_active = true;

				LogRotateEvent(control, "Rotate Gesture", e);
				GestureDrawable.Invalidate();
			};
			control.ZoomExpand += delegate(object sender, ZoomGestureEventArgs e)
			{
				zoom_scale = e.ScaleDelta;
				zoom_active = true;
				LogZoomEvent(control, "Zoom Gesture", e);
				GestureDrawable.Invalidate();
			};


		}


		void LogSwipeEvent(object sender, string type, SwipeGestureEventArgs e)
		{
			Console.WriteLine("{0}, VelocityX: {1}, VelocityY: {2}", type, e.VelocityX, e.VelocityY);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		void LogLongpressEvent(object sender, string type, LongPressGestureEventArgs e)
		{
			Console.WriteLine("{0}, Pressed: {1}, N Press: {2} X: {3} Y: {4}", type, e.Pressed, e.NPress, e.X, e.Y);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		void LogPanEvent(object sender, string type, PanGestureEventArgs e)
		{
			
			Console.WriteLine("{0}, Direction: {1}, Offset: {2}", type, e.Direction, e.Offset);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		void LogRotateEvent(object sender, string type, RotateGestureEventArgs e)
		{
			
			Console.WriteLine("{0}, Angle Delta: {1} Rotation {2}", type, e.AngleDelta, rotate_angle);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

		void LogZoomEvent(object sender, string type, ZoomGestureEventArgs e)
		{
			Console.WriteLine("{0}, Scale Delta: {1}", type, e.ScaleDelta);
			if (handleEvents.Checked == true)
				e.Handled = true;
		}

	}
}
