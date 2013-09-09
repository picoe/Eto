using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	public class DrawableSection : Scrollable
	{
		public DrawableSection ()
		{
			var layout = new DynamicLayout (this);
			
			layout.BeginVertical ();
			layout.BeginHorizontal ();
			layout.Add (new Label { Text = "Default" });
			layout.Add (this.Default (), true);
			layout.Add (new Label { Text = "With Background" });
			layout.Add (this.WithBackground (), true);
			layout.EndHorizontal ();
			layout.EndVertical ();
			layout.BeginVertical ();
			layout.AddRow (new Label { Text = "Large Canvas" }, DockLayout.CreatePanel(this.LargeCanvas ()));
			layout.EndVertical ();

			layout.Add (null);
		}

		Control Default ()
		{
			var control = new Drawable {
				Size = new Size (50, 50)
			};
			control.Paint += delegate (object sender, PaintEventArgs pe) {
				pe.Graphics.DrawLine (Pens.Black (), Point.Empty, new Point (control.Size));
			};
			LogEvents (control);

			return control;
		}
		
		Control WithBackground ()
		{
			var control = new Drawable {
				Size = new Size (50, 50),
				BackgroundColor = Colors.Lime
			};
			control.Paint += delegate (object sender, PaintEventArgs pe) {
				pe.Graphics.DrawLine (Pens.Black(), Point.Empty, new Point (control.Size));
			};
			LogEvents (control);

			return control;
		}

		Control LargeCanvas ()
		{
			var control = new Drawable{
				Size = new Size (1000, 1000),
				BackgroundColor = Colors.Blue
			};
			var image = TestIcons.TestImage;
			control.Paint += delegate(object sender, PaintEventArgs pe) {
				pe.Graphics.FillRectangle (Brushes.Black (), new Rectangle (150, 150, 100, 100));
				var whitePen = Pens.White ();
				var inc = 400;
				for (int i = 0; i <= control.Size.Width / inc; i++) {
					var pos = i * inc;
					pe.Graphics.DrawLine (whitePen, new Point (pos, 0), new Point (pos + control.Size.Width, control.Size.Height));
					pe.Graphics.DrawLine (whitePen, new Point (pos, 0), new Point (pos - control.Size.Width, control.Size.Height));
				}
				var lpos = 100;
				pe.Graphics.DrawLine (whitePen, new Point (0, lpos), new Point (control.Size.Width, lpos));
				pe.Graphics.DrawLine (whitePen, new Point (lpos, 0), new Point (lpos, control.Size.Height));
				pe.Graphics.DrawImage (image, 100, 10);
				pe.Graphics.DrawImage (image, 250, 10, 80, 20);
			};
			LogEvents (control);

			var layout = new PixelLayout (new Scrollable {
				Size = new Size (250, 250)
			});
			layout.Add (control, 25, 25);
			return layout.Container;
		}

		void LogEvents (Drawable control)
		{
			control.Paint += delegate (object sender, PaintEventArgs pe) {
				Log.Write (control, "Paint, ClipRectangle: {0}", pe.ClipRectangle);
			};
		}
	}
}

