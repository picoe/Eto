namespace Eto.Test.Sections.Behaviors
{
	[Section("Behaviors", "Gestures")]
	public class GesturesSection : Panel
	{
		readonly Drawable drawable;
		readonly Label scaleLabel;
		readonly Label magnificationLabel;
		readonly Label rotationLabel;
		readonly Label angleLabel;
		readonly Label translationLabel;
		readonly Label velocityLabel;
		readonly Label scrollDeltaLabel;
		readonly Label scrollVelocityLabel;
		readonly Label scrollInvertedLabel;
		readonly CheckBox allowSimultaneousGesturesCheckBox;
		readonly MagnificationGesture magnificationGesture;
		readonly RotationGesture rotateGesture;
		readonly PanGesture panGesture;
		readonly ScrollGesture scrollGesture;
		float scale = 1f;
		float lastMagnification;
		float angle;
		float lastRotation;
		PointF offset;
		PointF lastTranslation;
		PointF lastVelocity;
		SizeF lastScrollDelta;
		PointF lastScrollVelocity;
		bool lastScrollInverted;

		public GesturesSection()
		{
			scaleLabel = new Label();
			magnificationLabel = new Label();
			rotationLabel = new Label();
			angleLabel = new Label();
			translationLabel = new Label();
			velocityLabel = new Label();
			scrollDeltaLabel = new Label();
			scrollVelocityLabel = new Label();
			scrollInvertedLabel = new Label();
			allowSimultaneousGesturesCheckBox = new CheckBox { Text = "Allow Simultaneous Gestures", Checked = true };
			allowSimultaneousGesturesCheckBox.CheckedChanged += (sender, e) => SetSimultaneousGestures(allowSimultaneousGesturesCheckBox.Checked == true);

			drawable = new Drawable
			{
				Size = new Size(520, 360),
				BackgroundColor = Colors.White
			};
			drawable.Paint += Drawable_Paint;
			drawable.CanFocus = true;
			drawable.MouseDown += (sender, e) => Log.Write(this, $"MouseDown: {e.Buttons} at {e.Location}");
			// drawable.MouseMove += (sender, e) => Log.Write(this, $"MouseMove: {e.Buttons} at {e.Location}");
			drawable.MouseUp += (sender, e) => Log.Write(this, $"MouseUp: {e.Buttons} at {e.Location}");
			drawable.MouseWheel += (sender, e) => Log.Write(this, $"MouseWheel: {e.Delta} at {e.Location}, Inverted: {e.IsDirectionInverted}");

			if (Platform.Supports<MagnificationGesture>())
			{
				magnificationGesture = new MagnificationGesture();
				magnificationGesture.Activated += MagnificationGesture_Activated;
				drawable.Gestures.Add(magnificationGesture);
			}

			if (Platform.Supports<RotationGesture>())
			{
				rotateGesture = new RotationGesture();
				rotateGesture.Activated += RotateGesture_Activated;
				drawable.Gestures.Add(rotateGesture);
			}

			if (Platform.Supports<PanGesture>())
			{
				panGesture = new PanGesture();
				panGesture.Activated += PanGesture_Activated;
				drawable.Gestures.Add(panGesture);
			}

			if (Platform.Supports<ScrollGesture>())
			{
				scrollGesture = new ScrollGesture();
				scrollGesture.Activated += ScrollGesture_Activated;
				drawable.Gestures.Add(scrollGesture);
			}

			SetSimultaneousGestures(allowSimultaneousGesturesCheckBox.Checked == true);
			var panMouseButtonsSelector = panGesture != null ? CreatePanMouseButtonsSelector() : null;
			var wheelScrollAmountControl = scrollGesture != null ? CreateWheelScrollAmountControl() : null;

			var resetButton = new Button { Text = "Reset" };
			resetButton.Click += (sender, e) =>
			{
				scale = 1f;
				lastMagnification = 0f;
				angle = 0f;
				lastRotation = 0f;
				offset = PointF.Empty;
				lastTranslation = PointF.Empty;
				lastVelocity = PointF.Empty;
				lastScrollDelta = SizeF.Empty;
				lastScrollVelocity = PointF.Empty;
				lastScrollInverted = false;
				UpdateLabels();
				drawable.Invalidate();
			};

			UpdateLabels();

			Content = new DynamicLayout
			{
				Padding = new Padding(10),
				DefaultSpacing = new Size(5, 5),
				Rows =
				{
					new Label { Text = "Pinch over the drawable to scale the drawing. Rotate with two fingers to spin it. Pan to move it." },
					new TableLayout(
						TableLayout.Horizontal(10, scaleLabel, magnificationLabel, resetButton, allowSimultaneousGesturesCheckBox, panMouseButtonsSelector, null),
						TableLayout.Horizontal(10, angleLabel, rotationLabel, null),
						TableLayout.Horizontal(10, translationLabel, velocityLabel, null),
						TableLayout.Horizontal(10, scrollDeltaLabel, scrollVelocityLabel, scrollInvertedLabel, wheelScrollAmountControl, null),
						drawable
					),
					null
				}
			};
		}

		Control CreateWheelScrollAmountControl()
		{
			var stepper = new NumericStepper
			{
				MinValue = 0,
				MaxValue = 500,
				Increment = 1,
				DecimalPlaces = 1,
				Value = scrollGesture.WheelScrollAmount,
				Width = 70
			};
			stepper.ValueChanged += (sender, e) => scrollGesture.WheelScrollAmount = (float)stepper.Value;

			return TableLayout.Horizontal(5, new Label { Text = "Wheel Scroll Amount:" }, stepper);
		}

		Control CreatePanMouseButtonsSelector()
		{
			var menuItem = new MenuSegmentedItem { CanSelect = false };
			var menu = new ContextMenu();

			void AddMouseButtonsItem(MouseButtons buttons)
			{
				var item = new ButtonMenuItem { Text = FormatMouseButtons(buttons) };
				item.Click += (sender, e) =>
				{
					panGesture.Buttons = buttons;
					menuItem.Text = "Pan Mouse: " + FormatMouseButtons(buttons);
				};
				menu.Items.Add(item);
			}

			AddMouseButtonsItem(MouseButtons.Primary);
			AddMouseButtonsItem(MouseButtons.Alternate);
			AddMouseButtonsItem(MouseButtons.Middle);
			AddMouseButtonsItem(MouseButtons.Primary | MouseButtons.Alternate);
			AddMouseButtonsItem(MouseButtons.Primary | MouseButtons.Middle);
			AddMouseButtonsItem(MouseButtons.Alternate | MouseButtons.Middle);
			AddMouseButtonsItem(MouseButtons.Primary | MouseButtons.Alternate | MouseButtons.Middle);

			menuItem.Menu = menu;
			menuItem.Text = "Pan Mouse: " + FormatMouseButtons(panGesture.Buttons);

			return new SegmentedButton
			{
				SelectionMode = SegmentedSelectionMode.None,
				Items = { menuItem }
			};
		}

		static string FormatMouseButtons(MouseButtons buttons)
		{
			return buttons.ToString().Replace(", ", " + ");
		}

		void SetSimultaneousGestures(bool allow)
		{
			if (magnificationGesture != null && rotateGesture != null)
				SetSimultaneousGestures(magnificationGesture, rotateGesture, allow);
			if (magnificationGesture != null && panGesture != null)
				SetSimultaneousGestures(magnificationGesture, panGesture, allow);
			if (rotateGesture != null && panGesture != null)
				SetSimultaneousGestures(rotateGesture, panGesture, allow);
			if (scrollGesture != null && panGesture != null)
				SetSimultaneousGestures(scrollGesture, panGesture, allow);
		}

		static void SetSimultaneousGestures(Gesture first, Gesture second, bool allow)
		{
			if (allow)
				first.AllowSimultaneousWith(second);
			else
				first.DisallowSimultaneousWith(second);
		}

		void MagnificationGesture_Activated(object sender, EventArgs e)
		{
			lastMagnification = magnificationGesture.Magnification;
			scale *= 1f + lastMagnification;
			scale = Math.Max(0.25f, Math.Min(5f, scale));
			UpdateLabels();
			drawable.Invalidate();
		}

		void RotateGesture_Activated(object sender, EventArgs e)
		{
			lastRotation = rotateGesture.Rotation;
			angle += lastRotation;
			UpdateLabels();
			drawable.Invalidate();
		}

		void PanGesture_Activated(object sender, EventArgs e)
		{
			lastTranslation = panGesture.Translation;
			lastVelocity = panGesture.Velocity;
			offset += lastTranslation;
			UpdateLabels();
			drawable.Invalidate();
		}

		void ScrollGesture_Activated(object sender, EventArgs e)
		{
			lastScrollDelta = scrollGesture.Delta;
			lastScrollVelocity = scrollGesture.Velocity;
			lastScrollInverted = scrollGesture.IsDirectionInverted;
			offset += new PointF(lastScrollDelta.Width, lastScrollDelta.Height);
			UpdateLabels();
			drawable.Invalidate();
		}

		void UpdateLabels()
		{
			scaleLabel.Text = string.Format("Scale: {0:0.00}x", scale);
			magnificationLabel.Text = string.Format("Magnification: {0:+0.000;-0.000;0.000}", lastMagnification);
			angleLabel.Text = string.Format("Angle: {0:0.0}°", angle);
			rotationLabel.Text = string.Format("Rotation: {0:+0.000;-0.000;0.000}°", lastRotation);
			translationLabel.Text = string.Format("Translation: {0:+0.0;-0.0;0.0}, {1:+0.0;-0.0;0.0}", lastTranslation.X, lastTranslation.Y);
			velocityLabel.Text = string.Format("Velocity: {0:+0.0;-0.0;0.0}, {1:+0.0;-0.0;0.0}", lastVelocity.X, lastVelocity.Y);
			scrollDeltaLabel.Text = string.Format("Scroll Delta: {0:+0.0;-0.0;0.0}, {1:+0.0;-0.0;0.0}", lastScrollDelta.Width, lastScrollDelta.Height);
			scrollVelocityLabel.Text = string.Format("Scroll Velocity: {0:+0.0;-0.0;0.0}, {1:+0.0;-0.0;0.0}", lastScrollVelocity.X, lastScrollVelocity.Y);
			scrollInvertedLabel.Text = string.Format("Scroll Inverted: {0}", lastScrollInverted);
		}

		void Drawable_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			var bounds = new RectangleF(drawable.Size);
			var center = bounds.Center;

			DrawBackground(g, bounds);

			g.SaveTransform();
			g.TranslateTransform(offset);
			g.TranslateTransform(center);
			g.ScaleTransform(scale);
			g.RotateTransform(angle);
			g.TranslateTransform(-center);

			DrawScaledDrawing(g, center);

			g.RestoreTransform();
			DrawOverlay(g, bounds);
		}

		void DrawBackground(Graphics g, RectangleF bounds)
		{
			g.FillRectangle(Colors.White, bounds);

			using (var gridPen = new Pen(Color.FromArgb(0xe8, 0xec, 0xf0), 1))
			using (var axisPen = new Pen(Color.FromArgb(0x9a, 0xa8, 0xb3), 1))
			{
				for (int x = 0; x <= bounds.Width; x += 24)
					g.DrawLine(gridPen, x, 0, x, bounds.Height);
				for (int y = 0; y <= bounds.Height; y += 24)
					g.DrawLine(gridPen, 0, y, bounds.Width, y);

				g.DrawLine(axisPen, bounds.Width / 2, 0, bounds.Width / 2, bounds.Height);
				g.DrawLine(axisPen, 0, bounds.Height / 2, bounds.Width, bounds.Height / 2);
			}
		}

		void DrawScaledDrawing(Graphics g, PointF center)
		{
			var rect = new RectangleF(center.X - 85, center.Y - 55, 170, 110);
			using (var outlinePen = new Pen(Colors.DarkSlateBlue, 3))
			using (var accentPen = new Pen(Colors.OrangeRed, 4))
			using (var crossPen = new Pen(Colors.DarkGreen, 2))
			{
				g.FillEllipse(Color.FromArgb(0x9b, 0xd7, 0xea), rect);
				g.DrawEllipse(outlinePen, rect);
				g.DrawLine(accentPen, center.X - 120, center.Y, center.X + 120, center.Y);
				g.DrawLine(accentPen, center.X, center.Y - 90, center.X, center.Y + 90);

				g.DrawRectangle(crossPen, center.X - 42, center.Y - 42, 84, 84);
				g.DrawLine(crossPen, center.X - 42, center.Y - 42, center.X + 42, center.Y + 42);
				g.DrawLine(crossPen, center.X + 42, center.Y - 42, center.X - 42, center.Y + 42);
			}
		}

		void DrawOverlay(Graphics g, RectangleF bounds)
		{
			using (var borderPen = new Pen(Color.FromArgb(0x7c, 0x89, 0x94), 1))
			{
				g.DrawRectangle(borderPen, bounds);
			}
			g.DrawText(SystemFonts.Label(), Colors.Black, 8, 8, "MagnificationGesture + PanGesture + RotateGesture + ScrollGesture");
		}
	}
}
