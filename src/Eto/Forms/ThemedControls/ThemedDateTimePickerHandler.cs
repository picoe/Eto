namespace Eto.Forms.ThemedControls;

/// <summary>
/// Themed implementation of the <see cref="DateTimePicker"/> control using a <see cref="MaskedTextStepper{T}"/>
/// and a culture-aware fixed format provider.
/// </summary>
public class ThemedDateTimePickerHandler : ThemedControlHandler<DateTimeMaskedTextStepper, DateTimePicker, DateTimePicker.ICallback>, DateTimePicker.IHandler
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ThemedDateTimePickerHandler"/> class.
	/// </summary>
	public ThemedDateTimePickerHandler()
	{
		Control = new DateTimeMaskedTextStepper();
		Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
	}

	/// <inheritdoc/>
	protected override void Initialize()
	{
		base.Initialize();
		SetPreferredSize();
	}

	
	static readonly object PreferredSize_Key = new object();
	Size PreferredSize
	{
		get => Widget.Properties.Get<Size?>(PreferredSize_Key) ?? new Size(-1, -1);
		set => Widget.Properties.Set(PreferredSize_Key, value);
	}
	
	void SetPreferredSize()
	{
		var size = PreferredSize;
		if (size.Width < 0)
		{
			size.Width = Control.Mode switch
			{
				DateTimePickerMode.Date => 100,
				DateTimePickerMode.Time => 80,
				DateTimePickerMode.DateTime => 160,
				_ => 100
			};
		}
		base.Size = size;
	}

	/// <inheritdoc/>
	public override void AttachEvent(string id)
	{
		switch (id)
		{
			default:
				base.AttachEvent(id);
				break;
		}
	}

	/// <inheritdoc/>
	public DateTime? Value
	{
		get => Control.Value;
		set => Control.Value = value;
	}

	/// <inheritdoc/>
	public DateTime MinDate
	{
		get => Control.MinDate;
		set => Control.MinDate = value;
	}

	/// <inheritdoc/>
	public DateTime MaxDate
	{
		get => Control.MaxDate;
		set => Control.MaxDate = value;
	}

	/// <inheritdoc/>
	public DateTimePickerMode Mode
	{
		get => Control.Mode;
		set
		{
			Control.Mode = value;
			SetPreferredSize();
		}
	}

	/// <inheritdoc/>
	public Color TextColor
	{
		get => Control.TextColor;
		set => Control.TextColor = value;
	}

	/// <inheritdoc/>
	public bool ShowBorder
	{
		get => Control.ShowBorder;
		set => Control.ShowBorder = value;
	}

	/// <inheritdoc/>
	public Font Font
	{
		get => Control.Font;
		set => Control.Font = value;
	}

	/// <inheritdoc/>
	protected override Control KeyboardControl => Control;

	/// <inheritdoc/>
	public override void Focus() => Control.Focus();

	/// <inheritdoc/>
	public override bool HasFocus => base.HasFocus || Control.HasFocus;
	
	/// <inheritdoc/>
	public override Size Size
	{
		get => base.Size;
		set
		{
			PreferredSize = value;
			SetPreferredSize();
		}
	}

	/// <inheritdoc/>
	public override int Width
	{
		get => base.Width;
		set
		{
			PreferredSize = new Size(value, PreferredSize.Height);
			SetPreferredSize();
		}
	}
	
	/// <inheritdoc/>
	override public int Height
	{
		get => base.Height;
		set
		{
			PreferredSize = new Size(PreferredSize.Width, value);
			base.Height = value;
		}
	}

}
