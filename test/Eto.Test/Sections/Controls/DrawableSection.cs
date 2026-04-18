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

					new TableLayout(
						"IME Input",
						InputMethodDrawable()
					),

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
			var control = new Drawable(true)
			{
				Size = new Size(2000, 2000),
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

		Control InputMethodDrawable()
		{
			const int padding = 12;
			var text = string.Empty;
			var compositionText = string.Empty;
			var compositionActive = false;
			var drawable = new Drawable
			{
				CanFocus = true,
				Size = new Size(420, 60),
				BackgroundColor = Colors.White
			};
			var font = SystemFonts.Default();

			RectangleF getCaretRect(float pointsToPixels = 1f, bool forInputMethod = false)
			{
				var displayText = text;
				if (!forInputMethod || Platform.IsGtk)
					displayText += compositionText;
					
				var size = font.MeasureString(displayText);
				return new RectangleF(
					padding + size.Width,
					padding,
					2,
					font.LineHeight * pointsToPixels
				);
			}

			void updateCaret()
			{
				drawable.Invalidate();
			}

			drawable.Paint += (sender, pe) =>
			{
				var size = font.MeasureString(text);
				size.Height = font.LineHeight * pe.Graphics.PixelsPerPoint;
				pe.Graphics.Clear(Colors.White);
				pe.Graphics.DrawRectangle(Colors.DarkGray, 0, 0, drawable.Width - 1, drawable.Height - 1);
				pe.Graphics.DrawText(font, Colors.Black, padding, padding, text);
				if (compositionActive && !string.IsNullOrEmpty(compositionText))
				{
					var compositionX = padding + size.Width;
					pe.Graphics.DrawText(font, Colors.DarkSlateBlue, compositionX, padding, compositionText);
					var compositionWidth = Math.Max(1, font.MeasureString(compositionText).Width);
					var underlineY = padding + size.Height;
					pe.Graphics.DrawLine(Colors.DarkSlateBlue, compositionX, underlineY, compositionX + compositionWidth, underlineY);
				}
				if (drawable.HasFocus)
				{
					var caret = getCaretRect(pe.Graphics.PixelsPerPoint);
					pe.Graphics.FillRectangle(Colors.DarkBlue, caret);
				}
			};
			drawable.GotFocus += (sender, e) => drawable.Invalidate();
			drawable.LostFocus += (sender, e) => drawable.Invalidate();
			drawable.MouseDown += (sender, e) =>
			{
				if (e.Buttons == MouseButtons.Primary)
					drawable.CommitTextComposition();
				else
					drawable.CancelTextComposition();
				drawable.Focus();
				e.Handled = true;
			};
			drawable.TextInput += (sender, e) =>
			{
				if (!string.IsNullOrEmpty(e.Text))
				{
					text += e.Text;
					compositionText = string.Empty;
					compositionActive = false;
					Log.Write(sender, $"TextInput: '{e.Text}'");
					updateCaret();
				}
				e.Cancel = true;
			};
			drawable.TextComposition += (sender, e) =>
			{
				compositionText = e.Text ?? string.Empty;
				compositionActive = e.IsActive && !string.IsNullOrEmpty(compositionText);
				Log.Write(sender, $"TextComposition: '{compositionText}', Active={e.IsActive}");
				updateCaret();
				e.Handled = true;
			};
			drawable.TextInsertionBoundsRequested += (sender, e) => e.Bounds = getCaretRect(forInputMethod: true);
			drawable.KeyDown += (sender, e) =>
			{
				if (e.KeyData == Keys.Backspace)
				{
					if (text.Length > 0)
					{
						text = text.Substring(0, text.Length - 1);
						updateCaret();
					}
					e.Handled = true;
				}
				Log.Write(sender, $"KeyDown: {e.KeyData}");
			};

			updateCaret();

			return new StackLayout
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				Spacing = 6,
				Items =
				{
					"Click inside the drawable and type with an IME. The candidate/composition UI should follow the caret.",
					drawable
				}
			};
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
