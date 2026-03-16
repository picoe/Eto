namespace Eto.Forms.ThemedControls;

/// <summary>
/// A themed handler for the <see cref="FontDialog"/> control.
/// </summary>
public class ThemedFontDialogHandler : WidgetHandler<Widget, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
{
	Font _font;

	/// <inheritdoc/>
	public Font Font
	{
		get => _font ??= SystemFonts.Default();
		set => _font = value;
	}

	/// <inheritdoc/>
	public override void AttachEvent(string id)
	{
		switch (id)
		{
			case FontDialog.FontChangedEvent:
				break;
			default:
				base.AttachEvent(id);
				break;
		}
	}

	/// <inheritdoc/>
	public DialogResult ShowDialog(Window parent)
	{
		if (parent?.HasFocus == false)
			parent.Focus();

		var lastFont = Font;
		var dialog = new ThemedFontDialog(lastFont);
		dialog.SelectedFontChanged += (_, __) =>
		{
			_font = dialog.SelectedFont;
			Callback.OnFontChanged(Widget, EventArgs.Empty);
		};

		var result = parent != null ? dialog.ShowModal(parent) : dialog.ShowModal();

		if (result == DialogResult.Ok)
		{
			_font = dialog.SelectedFont;
			Callback.OnFontChanged(Widget, EventArgs.Empty);
			return DialogResult.Ok;
		}

		_font = lastFont;
		Callback.OnFontChanged(Widget, EventArgs.Empty);
		return DialogResult.Cancel;
	}

	class ThemedFontDialog : Dialog<DialogResult>
	{
		const string PreviewSample = "The quick brown fox jumps over the lazy dog";

		readonly ListBox _familyDropDown;
		readonly ListBox _typefaceDropDown;
		readonly NumericStepper _sizeStepper;
		readonly TextBox _previewBox;
		readonly List<FontFamily> _families;
		List<FontTypeface> _typefaces;
		FontDecoration _decoration;
		bool _updating;
		Font _selectedFont;

		public event EventHandler<EventArgs> SelectedFontChanged;

		public Font SelectedFont
		{
			get => _selectedFont;
			set
			{
				if (_selectedFont != value)
				{
					_selectedFont = value;
					SelectedFontChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public ThemedFontDialog(Font initialFont)
		{
			Title = "Select Font";
			ClientSize = new Size(560, 400);
			Resizable = true;
			ShowInTaskbar = false;

			_familyDropDown = new ListBox();
			_typefaceDropDown = new ListBox();
			_sizeStepper = new NumericStepper
			{
				MinValue = 1,
				MaxValue = 200,
				Increment = 0.5,
				DecimalPlaces = 1
			};
			_previewBox = new TextBox
			{
				Height = 80,
				ReadOnly = true,
				Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), PreviewSample)
			};

			_families = Fonts.AvailableFontFamilies
				.OrderBy(r => r.LocalizedName, StringComparer.CurrentCultureIgnoreCase)
				.ToList();
			_familyDropDown.DataStore = _families.Select(r => r.LocalizedName).ToList();

			_familyDropDown.SelectedIndexChanged += (_, __) =>
			{
				if (_updating)
					return;
				PopulateTypefaces();
				UpdateSelectedFont();
			};
			_typefaceDropDown.SelectedIndexChanged += (_, __) =>
			{
				if (_updating)
					return;
				UpdateSelectedFont();
			};
			_sizeStepper.ValueChanged += (_, __) =>
			{
				if (_updating)
					return;
				UpdateSelectedFont();
			};

			var okButton = new Button { Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), "OK") };
			okButton.Click += (_, __) => Close(DialogResult.Ok);

			var cancelButton = new Button { Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), "Cancel") };
			cancelButton.Click += (_, __) => Close(DialogResult.Cancel);

			DefaultButton = okButton;
			AbortButton = cancelButton;
			PositiveButtons.Add(okButton);
			NegativeButtons.Add(cancelButton);

			var layout = new DynamicLayout
			{
				Padding = 10,
				DefaultSpacing = new Size(8, 8)
			};

			layout.BeginVertical(yscale: true);
			layout.BeginHorizontal();
			{
				layout.BeginVertical(xscale: true);
				{
					layout.Add(new Label { Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), "Font family") }, yscale: false);
					layout.Add(_familyDropDown, yscale: true);
				}
				layout.EndVertical();
				layout.BeginVertical(xscale: true);
				{
					layout.Add(new Label { Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), "Font style") }, yscale: false);
					layout.Add(_typefaceDropDown, yscale: true);
				}
				layout.EndVertical();
			}
			layout.EndHorizontal();
			layout.EndVertical();

			layout.AddSeparateRow(null, new Label { Text = Application.Instance.Localize(typeof(ThemedFontDialogHandler), "Size") }, _sizeStepper);
			layout.Add(_previewBox, yscale: false);
			Content = layout;

			ApplyFont(initialFont ?? SystemFonts.Default());
		}

		void ApplyFont(Font font)
		{
			_updating = true;
			_decoration = font.FontDecoration;

			var familyIndex = _families.FindIndex(r => r == font.Family);
			if (familyIndex < 0 && _families.Count > 0)
				familyIndex = 0;
			_familyDropDown.SelectedIndex = familyIndex;

			PopulateTypefaces();

			var typefaceIndex = _typefaces.FindIndex(r => r == font.Typeface);
			if (typefaceIndex < 0 && _typefaces.Count > 0)
				typefaceIndex = 0;
			_typefaceDropDown.SelectedIndex = typefaceIndex;

			_sizeStepper.Value = Math.Max(1, font.Size);
			_updating = false;
			UpdateSelectedFont();
		}

		void PopulateTypefaces()
		{
			var family = GetSelectedFamily();
			_typefaces = family?.Typefaces
				.OrderBy(r => r.LocalizedName, StringComparer.CurrentCultureIgnoreCase)
				.ToList() ?? new List<FontTypeface>();
			_typefaceDropDown.DataStore = _typefaces.Select(r => r.LocalizedName).ToList();
			if (_typefaces.Count > 0)
				_typefaceDropDown.SelectedIndex = 0;
		}

		FontFamily GetSelectedFamily()
		{
			var index = _familyDropDown.SelectedIndex;
			return index >= 0 && index < _families.Count ? _families[index] : null;
		}

		FontTypeface GetSelectedTypeface()
		{
			var index = _typefaceDropDown.SelectedIndex;
			return index >= 0 && index < _typefaces.Count ? _typefaces[index] : null;
		}

		void UpdateSelectedFont()
		{
			var family = GetSelectedFamily();
			var typeface = GetSelectedTypeface();
			var size = (float)_sizeStepper.Value;

			var font = typeface != null
				? new Font(typeface, size, _decoration)
				: family != null
					? new Font(family, size, FontStyle.None, _decoration)
					: SystemFonts.Default();

			_previewBox.Font = font;
			SelectedFont = font;
		}
	}
}
