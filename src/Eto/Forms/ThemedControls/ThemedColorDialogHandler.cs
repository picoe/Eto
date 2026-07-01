namespace Eto.Forms.ThemedControls;

using Eto.Drawing;

/// <summary>
/// A themed handler for the <see cref="ColorDialog"/> control that provides
/// a color spectrum, hue strip, RGB/Alpha sliders, hex input, and color preview.
/// </summary>
public class ThemedColorDialogHandler : WidgetHandler<Widget, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler, CommonDialog.ICancellableHandler
{
	Color _color = Colors.White;
	bool _allowAlpha;
	ThemedColorDialog _activeDialog;

	/// <inheritdoc/>
	public Color Color
	{
		get => _color;
		set => _color = value;
	}

	/// <inheritdoc/>
	public bool AllowAlpha
	{
		get => _allowAlpha;
		set => _allowAlpha = value;
	}

	/// <inheritdoc/>
	public bool SupportsAllowAlpha => true;

	/// <inheritdoc/>
	public DialogResult ShowDialog(Window parent)
	{
		if (parent?.HasFocus == false)
			parent.Focus();

		var lastColor = _color;
		var dialog = new ThemedColorDialog(_color, _allowAlpha);
		dialog.SelectedColorChanged += (_, __) =>
		{
			_color = dialog.SelectedColor;
			Callback.OnColorChanged(Widget, EventArgs.Empty);
		};

		_activeDialog = dialog;
		DialogResult result;
		try
		{
			result = parent != null ? dialog.ShowModal(parent) : dialog.ShowModal();
		}
		finally
		{
			_activeDialog = null;
		}

		if (result == DialogResult.Ok)
		{
			_color = dialog.SelectedColor;
			return DialogResult.Ok;
		}

		_color = lastColor;
		Callback.OnColorChanged(Widget, EventArgs.Empty);
		return DialogResult.Cancel;
	}

	/// <summary>
	/// Closes the dialog while it is being shown, allowing the asynchronous
	/// <see cref="CommonDialog.ShowDialogAsync(Window, CancellationToken)"/> to be cancelled. Because the dialog is a
	/// managed Eto <see cref="Dialog"/>, closing it ends the modal display directly without any native interop.
	/// </summary>
	public void CancelDialog() => _activeDialog?.Close();

	class ThemedColorDialog : Dialog<DialogResult>
	{
		const int SpectrumSize = 256;
		const int HueBarWidth = 24;

		readonly bool _allowAlpha;
		readonly Drawable _spectrumArea;
		readonly Drawable _hueStrip;
		readonly Drawable _previewSwatch;
		readonly Slider _rSlider, _gSlider, _bSlider, _aSlider;
		readonly NumericStepper _rStepper, _gStepper, _bStepper, _aStepper;
		readonly TextBox _hexBox;
		readonly Label _aLabel;
		readonly TableRow _alphaRow;

		Bitmap _spectrumBitmap;
		Bitmap _hueBitmap;
		float _hue;      // 0-360
		float _sat;       // 0-1
		float _bright;    // 0-1
		float _alpha;     // 0-1
		bool _updating;

		public event EventHandler<EventArgs> SelectedColorChanged;

		Color _selectedColor;
		public Color SelectedColor
		{
			get => _selectedColor;
			private set
			{
				if (_selectedColor != value)
				{
					_selectedColor = value;
					SelectedColorChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public ThemedColorDialog(Color initialColor, bool allowAlpha)
		{
			_allowAlpha = allowAlpha;
			Title = Application.Instance.Localize(typeof(ThemedColorDialogHandler), "Select Color");
			Resizable = false;
			ShowInTaskbar = false;

			// Initialize HSB from initial color
			var hsb = new ColorHSB(initialColor);
			_hue = hsb.H;
			_sat = hsb.S;
			_bright = hsb.B;
			_alpha = initialColor.A;
			SelectedColor = initialColor;

			// --- Spectrum area (Saturation x Brightness for current Hue) ---
			_spectrumArea = new Drawable { Size = new Size(SpectrumSize, SpectrumSize) };
			_spectrumArea.Paint += SpectrumArea_Paint;
			_spectrumArea.MouseDown += SpectrumArea_Mouse;
			_spectrumArea.MouseMove += SpectrumArea_Mouse;

			// --- Hue strip ---
			_hueStrip = new Drawable { Size = new Size(HueBarWidth, SpectrumSize) };
			_hueStrip.Paint += HueStrip_Paint;
			_hueStrip.MouseDown += HueStrip_Mouse;
			_hueStrip.MouseMove += HueStrip_Mouse;

			// --- Preview swatch ---
			_previewSwatch = new Drawable { Size = new Size(60, 20) };
			_previewSwatch.Paint += PreviewSwatch_Paint;

			// --- RGB sliders & steppers ---
			_rSlider = CreateSlider();
			_gSlider = CreateSlider();
			_bSlider = CreateSlider();
			_aSlider = CreateSlider();

			_rStepper = CreateStepper();
			_gStepper = CreateStepper();
			_bStepper = CreateStepper();
			_aStepper = CreateStepper();

			_rSlider.ValueChanged += (_, __) => { if (!_updating) OnRgbSliderChanged(); };
			_gSlider.ValueChanged += (_, __) => { if (!_updating) OnRgbSliderChanged(); };
			_bSlider.ValueChanged += (_, __) => { if (!_updating) OnRgbSliderChanged(); };
			_aSlider.ValueChanged += (_, __) => { if (!_updating) OnAlphaSliderChanged(); };

			_rStepper.ValueChanged += (_, __) => { if (!_updating) OnRgbStepperChanged(); };
			_gStepper.ValueChanged += (_, __) => { if (!_updating) OnRgbStepperChanged(); };
			_bStepper.ValueChanged += (_, __) => { if (!_updating) OnRgbStepperChanged(); };
			_aStepper.ValueChanged += (_, __) => { if (!_updating) OnAlphaStepperChanged(); };

			// --- Hex input ---
			_hexBox = new TextBox { Width = 80 };
			_hexBox.TextChanged += (_, __) =>
			{
				if (_updating) return;
				OnHexChanged();
			};

			// --- Buttons ---
			var okButton = new Button { Text = Application.Instance.Localize(typeof(ThemedColorDialogHandler), "OK") };
			okButton.Click += (_, __) => Close(DialogResult.Ok);

			var cancelButton = new Button { Text = Application.Instance.Localize(typeof(ThemedColorDialogHandler), "Cancel") };
			cancelButton.Click += (_, __) => Close(DialogResult.Cancel);

			DefaultButton = okButton;
			AbortButton = cancelButton;
			PositiveButtons.Add(okButton);
			NegativeButtons.Add(cancelButton);

			// --- Labels ---
			var rLabel = new Label { Text = "R:" };
			var gLabel = new Label { Text = "G:" };
			var bLabel = new Label { Text = "B:" };
			_aLabel = new Label { Text = "A:" };

			// --- Layout ---
			var slidersTable = new TableLayout
			{
				Spacing = new Size(6, 6)
			};

			slidersTable.Rows.Add(new TableRow(rLabel, new TableCell(_rSlider, true), _rStepper));
			slidersTable.Rows.Add(new TableRow(gLabel, new TableCell(_gSlider, true), _gStepper));
			slidersTable.Rows.Add(new TableRow(bLabel, new TableCell(_bSlider, true), _bStepper));

			_alphaRow = new TableRow(_aLabel, new TableCell(_aSlider, true), _aStepper);
			if (_allowAlpha)
				slidersTable.Rows.Add(_alphaRow);

			slidersTable.Rows.Add(null); // spacer

			var hexRow = new TableLayout(new TableRow(new Label { Text = "#" }, _hexBox, null));
			hexRow.Spacing = new Size(4, 0);

			var rightPanel = new TableLayout
			{
				Spacing = new Size(6, 8)
			};
			rightPanel.Rows.Add(new TableRow(new TableCell(slidersTable, true)));
			rightPanel.Rows.Add(new TableRow(new TableCell(
				new TableLayout(new TableRow(
					new Label { Text = Application.Instance.Localize(typeof(ThemedColorDialogHandler), "Hex:"), VerticalAlignment = VerticalAlignment.Center },
					_hexBox,
					new Panel { Width = 8 },
					new Label { Text = Application.Instance.Localize(typeof(ThemedColorDialogHandler), "Preview:"), VerticalAlignment = VerticalAlignment.Center },
					_previewSwatch,
					null
				))
				{ Spacing = new Size(4, 0) }
			)));
			rightPanel.Rows.Add(null);

			var mainLayout = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(10, 10)
			};
			mainLayout.Rows.Add(new TableRow(
				_spectrumArea,
				_hueStrip,
				new TableCell(rightPanel, true)
			));

			Content = mainLayout;

			BuildHueBitmap();
			RebuildSpectrumBitmap();
			UpdateControlsFromHsb();
		}

		static Slider CreateSlider()
		{
			return new Slider
			{
				MinValue = 0,
				MaxValue = 255,
				TickFrequency = 16,
				Width = 150
			};
		}

		static NumericStepper CreateStepper()
		{
			return new NumericStepper
			{
				MinValue = 0,
				MaxValue = 255,
				DecimalPlaces = 0,
				MaximumDecimalPlaces = 0,
				Increment = 1,
				Width = 80
			};
		}

		// ===================== Bitmap generation =====================

		void BuildHueBitmap()
		{
			_hueBitmap?.Dispose();
			_hueBitmap = new Bitmap(1, SpectrumSize, PixelFormat.Format32bppRgba);
			using var bd = _hueBitmap.Lock();
			for (int y = 0; y < SpectrumSize; y++)
			{
				float hue = y * 360f / SpectrumSize;
				var c = new ColorHSB(hue, 1f, 1f).ToColor();
				bd.SetPixel(0, y, c);
			}
		}

		void RebuildSpectrumBitmap()
		{
			_spectrumBitmap?.Dispose();
			_spectrumBitmap = new Bitmap(SpectrumSize, SpectrumSize, PixelFormat.Format32bppRgba);
			using var bd = _spectrumBitmap.Lock();
			for (int y = 0; y < SpectrumSize; y++)
			{
				float brightness = 1f - (float)y / (SpectrumSize - 1);
				for (int x = 0; x < SpectrumSize; x++)
				{
					float saturation = (float)x / (SpectrumSize - 1);
					var c = new ColorHSB(_hue, saturation, brightness).ToColor();
					bd.SetPixel(x, y, c);
				}
			}
		}

		// ===================== Paint handlers =====================

		void SpectrumArea_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			if (_spectrumBitmap != null)
				g.DrawImage(_spectrumBitmap, 0, 0, SpectrumSize, SpectrumSize);

			// Draw crosshair at current S,B position
			float cx = _sat * (SpectrumSize - 1);
			float cy = (1f - _bright) * (SpectrumSize - 1);
			var outerColor = _bright > 0.5f ? Colors.Black : Colors.White;
			g.DrawEllipse(outerColor, cx - 5, cy - 5, 10, 10);
			g.DrawEllipse(outerColor == Colors.Black ? Colors.White : Colors.Black, cx - 4, cy - 4, 8, 8);
		}

		void HueStrip_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			if (_hueBitmap != null)
				g.DrawImage(_hueBitmap, 0, 0, HueBarWidth, SpectrumSize);

			// Draw indicator at current hue
			float y = _hue / 360f * (SpectrumSize - 1);
			g.DrawLine(Colors.Black, new PointF(0, y), new PointF(HueBarWidth, y));
			g.DrawLine(Colors.White, new PointF(0, y - 1), new PointF(HueBarWidth, y - 1));
			g.DrawLine(Colors.White, new PointF(0, y + 1), new PointF(HueBarWidth, y + 1));
		}

		void PreviewSwatch_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			var sw = _previewSwatch.Size;

			// Draw checkerboard for alpha
			if (_allowAlpha)
			{
				int checkSize = 6;
				for (int cy = 0; cy < sw.Height; cy += checkSize)
				{
					for (int cx = 0; cx < sw.Width; cx += checkSize)
					{
						bool dark = ((cx / checkSize) + (cy / checkSize)) % 2 == 1;
						g.FillRectangle(dark ? Colors.LightGrey : Colors.White,
							cx, cy,
							Math.Min(checkSize, sw.Width - cx),
							Math.Min(checkSize, sw.Height - cy));
					}
				}
			}

			g.FillRectangle(SelectedColor, 0, 0, sw.Width, sw.Height);
			g.DrawRectangle(Colors.DarkGray, 0, 0, sw.Width - 1, sw.Height - 1);
		}

		// ===================== Mouse handlers =====================

		void SpectrumArea_Mouse(object sender, MouseEventArgs e)
		{
			if (!e.Buttons.HasFlag(MouseButtons.Primary))
				return;

			float x = Math.Max(0, Math.Min(SpectrumSize - 1, e.Location.X));
			float y = Math.Max(0, Math.Min(SpectrumSize - 1, e.Location.Y));

			_sat = x / (SpectrumSize - 1);
			_bright = 1f - y / (SpectrumSize - 1);

			UpdateColorFromHsb();
			UpdateControlsFromHsb();
			_spectrumArea.Invalidate();
			_previewSwatch.Invalidate();
		}

		void HueStrip_Mouse(object sender, MouseEventArgs e)
		{
			if (!e.Buttons.HasFlag(MouseButtons.Primary))
				return;

			float y = Math.Max(0, Math.Min(SpectrumSize - 1, e.Location.Y));
			_hue = y / (SpectrumSize - 1) * 360f;

			RebuildSpectrumBitmap();
			UpdateColorFromHsb();
			UpdateControlsFromHsb();
			_spectrumArea.Invalidate();
			_hueStrip.Invalidate();
			_previewSwatch.Invalidate();
		}

		// ===================== Update logic =====================

		void UpdateColorFromHsb()
		{
			var rgb = new ColorHSB(_hue, _sat, _bright).ToColor();
			SelectedColor = new Color(rgb, _alpha);
		}

		void UpdateControlsFromHsb()
		{
			_updating = true;
			var c = SelectedColor;

			_rSlider.Value = c.Rb;
			_gSlider.Value = c.Gb;
			_bSlider.Value = c.Bb;
			_aSlider.Value = c.Ab;

			_rStepper.Value = c.Rb;
			_gStepper.Value = c.Gb;
			_bStepper.Value = c.Bb;
			_aStepper.Value = c.Ab;

			_hexBox.Text = _allowAlpha ? c.ToHex(true) : c.ToHex(false);

			_updating = false;
		}

		void OnRgbSliderChanged()
		{
			var c = Color.FromArgb(_rSlider.Value, _gSlider.Value, _bSlider.Value, _aSlider.Value);
			SetColorAndUpdateHsb(c);
		}

		void OnRgbStepperChanged()
		{
			var c = Color.FromArgb((int)_rStepper.Value, (int)_gStepper.Value, (int)_bStepper.Value, (int)_aStepper.Value);
			SetColorAndUpdateHsb(c);
		}

		void OnAlphaSliderChanged()
		{
			_alpha = _aSlider.Value / 255f;
			UpdateColorFromHsb();
			UpdateControlsFromHsb();
			_previewSwatch.Invalidate();
		}

		void OnAlphaStepperChanged()
		{
			_alpha = (float)_aStepper.Value / 255f;
			UpdateColorFromHsb();
			UpdateControlsFromHsb();
			_previewSwatch.Invalidate();
		}

		void SetColorAndUpdateHsb(Color c)
		{
			SelectedColor = c;
			var hsb = new ColorHSB(c);
			_hue = hsb.H;
			_sat = hsb.S;
			_bright = hsb.B;
			_alpha = c.A;

			RebuildSpectrumBitmap();

			_updating = true;
			_rSlider.Value = c.Rb;
			_gSlider.Value = c.Gb;
			_bSlider.Value = c.Bb;
			_aSlider.Value = c.Ab;
			_rStepper.Value = c.Rb;
			_gStepper.Value = c.Gb;
			_bStepper.Value = c.Bb;
			_aStepper.Value = c.Ab;
			_hexBox.Text = _allowAlpha ? c.ToHex(true) : c.ToHex(false);
			_updating = false;

			_spectrumArea.Invalidate();
			_hueStrip.Invalidate();
			_previewSwatch.Invalidate();
		}

		void OnHexChanged()
		{
			var text = _hexBox.Text?.Trim();
			if (string.IsNullOrEmpty(text))
				return;

			// Allow input with or without '#'
			if (!text.StartsWith("#"))
				text = "#" + text;

			if (Color.TryParse(text, out var c))
			{
				if (!_allowAlpha)
					c = new Color(c, 1f);
				SetColorAndUpdateHsb(c);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_spectrumBitmap?.Dispose();
				_hueBitmap?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
