using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(Drawable))]
	public class DrawableSection : Scrollable
	{
		public DrawableSection()
		{
			Content = new TableLayout
			{
				Padding = new Padding(10),
				Spacing = new Size(10,10),
				Rows =
				{
					TableLayout.HorizontalScaled(
						10,
						new TableLayout(
							"Default",
							Default()
						),
						new TableLayout(
							"With Background",
							WithBackground()
						)
					),

					new TableLayout(
						"Large Canvas",
						// use a separate containing panel to test calculations in those cases
						new Panel { Content = LargeCanvas() }
					),

					new TableRow(TableLayout.Horizontal(
						10,
						new TableLayout(
							"Nested",
							Nested()
						),
						new TableLayout(
							"Transparent",
							Transparent()
						),
						new TableLayout(
							"Tools",
							TableLayout.Horizontal(
								Tools(1), Tools(2), Tools(0)
							),
							Tools(3),
							Tools(0)
						)
					)),

					(Platform.SupportedFeatures & PlatformFeatures.DrawableWithTransparentContent) == 0 ?
					new TableRow(
						"(Transparent content on drawable not supported on this platform)"
					) : null,

					null
				}
			};
		}

		Control Default()
		{
			var control = new Drawable
			{
				Size = new Size(50, 50)
			};
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				pe.Graphics.DrawLine(Pens.Black, Point.Empty, new Point(control.Size));
			};
			LogEvents(control, "Default");

			return control;
		}

		Control WithBackground()
		{
			var control = new Drawable
			{
				Size = new Size(50, 50),
				BackgroundColor = Colors.Lime
			};
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				pe.Graphics.DrawLine(Pens.Black, Point.Empty, new Point(control.Size));
			};
			LogEvents(control, "With Background");

			return control;
		}

		Control LargeCanvas()
		{
			var control = new Drawable
			{
				Size = new Size(1000, 1000),
				BackgroundColor = Colors.Blue
			};
			var image = TestIcons.TestImage;
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				pe.Graphics.FillRectangle(Brushes.Black, new Rectangle(150, 150, 100, 100));
				var whitePen = Pens.White;
				const int inc = 400;
				for (int i = 0; i <= control.Size.Width / inc; i++)
				{
					var pos = i * inc;
					pe.Graphics.DrawLine(whitePen, new Point(pos, 0), new Point(pos + control.Size.Width, control.Size.Height));
					pe.Graphics.DrawLine(whitePen, new Point(pos, 0), new Point(pos - control.Size.Width, control.Size.Height));
				}
				const int lpos = 100;
				pe.Graphics.DrawLine(whitePen, new Point(0, lpos), new Point(control.Size.Width, lpos));
				pe.Graphics.DrawLine(whitePen, new Point(lpos, 0), new Point(lpos, control.Size.Height));
				pe.Graphics.DrawImage(image, 100, 10);
				pe.Graphics.DrawImage(image, 250, 10, 80, 20);
			};
			LogEvents(control, "Large Canvas");

			var layout = new PixelLayout();
			layout.Add(control, 25, 25);
			return new Scrollable
			{
				Size = new Size(250, 250),
				Content = layout
			};
		}

		Control Nested()
		{
			var control = new Drawable
			{
				BackgroundColor = Colors.Black,
				Padding = 10,
				Content = new Drawable
				{
					BackgroundColor = Colors.White,
					Padding = 10,
					Content = "Black Border"
				}
			};

			return control;
		}

		Control Transparent()
		{
			//NOTE: do not try to remove those `Size = new Size(10, 10)`
			//..... it would really kill the app in WinForms
			return new Drawable
			{
				BackgroundColor = Colors.White,
				Padding = 10,
				Content = new TableLayout(
					true,
					TableLayout.HorizontalScaled
					(
						new Drawable
						{
							BackgroundColor = Color.FromArgb(255, 0, 0, 128),
							Content = new TableLayout(
								true,
								null,
								TableLayout.HorizontalScaled(
									null,
									new Panel
									{
										BackgroundColor = Color.FromArgb(255, 0, 0),
										Size = new Size(10, 10)
									}
								)
							)
						},
						new Drawable
						{
							BackgroundColor = Color.FromArgb(0, 255, 0, 128),
							Content = new TableLayout(
								true,
								null,
								TableLayout.HorizontalScaled(
									new Panel
									{
										BackgroundColor = Color.FromArgb(0, 255, 0),
										Size = new Size(10, 10)
									},
									null
								)
							)
						}
					),
					TableLayout.HorizontalScaled
					(
						new Drawable
						{
							BackgroundColor = Color.FromArgb(0, 0, 255, 128),
							Content = new TableLayout(
								true,
								TableLayout.HorizontalScaled(
									null,
									new Panel
									{
										BackgroundColor = Color.FromArgb(0, 0, 255),
										Size = new Size(10, 10)
									}
								),
								null
							)
						},
						new Drawable
						{
							BackgroundColor = Color.FromArgb(0, 0, 0, 128),
							Content = new TableLayout(
								true,
								TableLayout.HorizontalScaled(
									new Panel
									{
										BackgroundColor = Color.FromArgb(0, 0, 0),
										Size = new Size(10, 10)
									},
									null
								),
								null
							),
						}
					)
				)
			}.With(it => it.Paint += (s,pe) =>
			{
				using (var p = new Pen(Colors.Black, 3f))
				{
					for(int i = 4, n = Math.Max(it.Width, it.Height); i < n; i += 8)
					{
						pe.Graphics.DrawLine(p, i, 0, i + n, n);
						pe.Graphics.DrawLine(p, 0, i, n, i + n);
					}
				}
			});
		}

		Control Tools(int n)
		{
			var stack = new StackLayout
			{
				BackgroundColor = Colors.Transparent,
				Orientation = Orientation.Horizontal,
				VerticalContentAlignment = VerticalAlignment.Center,
				Padding = 4,
				Spacing = 4,
			};
			if (n > 0)
			{
				stack.Items.Add("Label:");
				for (int i = 0; i < n; i++)
				{
					stack.Items.Add(new TextBox
					{
						Text = "Box" + ( i + 1 )
					});
				}
			}
			var control = new Drawable
			{
				Content = stack
			};
			control.Paint += (s, pe) =>
			{
				int w = control.Width;
				int h = control.Height;
				var c1 = Color.FromGrayscale(0.8f);
				var c2 = Color.FromGrayscale(0.6f);
				using (var b = new LinearGradientBrush(
					c1, c2, new PointF(1, 0), new PointF(1, h)))
					pe.Graphics.FillRectangle(b, 1, 0, w-2, h);
				pe.Graphics.DrawLine(c1, 0, 0, 0, h);
				pe.Graphics.DrawLine(c2, w-1, 0, w-1, h);
			};
			return control;
		}

		void LogEvents(Drawable control, string name)
		{
			control.Paint += delegate(object sender, PaintEventArgs pe)
			{
				Log.Write(name, "Paint, ClipRectangle: {0}", pe.ClipRectangle);
			};
		}
	}
}

