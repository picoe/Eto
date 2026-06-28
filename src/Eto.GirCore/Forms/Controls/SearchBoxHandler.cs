namespace Eto.GirCore.Forms.Controls
{
	public class SearchBoxHandler : GirControl<Gtk.SearchEntry, SearchBox, SearchBox.ICallback>, SearchBox.IHandler
	{
		public SearchBoxHandler()
		{
			Control = Gtk.SearchEntry.New();
			Control.WidthChars = 12;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextBox.TextChangedEvent:
					Control.OnSearchChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public virtual string? Text
		{
			get => Control.GetText();
			set => Control.SetText(value);
		}

		public bool ReadOnly
		{
			get => !Control.Editable;
			set => Control.Editable = !value;
		}

		public string PlaceholderText
		{
			get => Control.PlaceholderText;
			set => Control.PlaceholderText = value;
		}

		public int MaxLength
		{
			get => Control.MaxWidthChars;
			set => Control.MaxWidthChars = value;
		}

		public Range<int> Selection
		{
			get
			{
				Control.GetSelectionBounds(out var start, out var end);
				return new Range<int>(start, end);
			}
			set => Control.SelectRegion(value.Start, value.End);
		}

		public int CaretIndex
		{
			get => Control.GetPosition();
			set => Control.SetPosition(value);
		}

		public bool ShowBorder { get; set; } = true;

		public TextAlignment TextAlignment
		{
			get => Control.Xalign switch
			{
				0 => TextAlignment.Left,
				0.5f => TextAlignment.Center,
				1 => TextAlignment.Right,
				_ => TextAlignment.Left
			};
			set => Control.Xalign = value switch
			{
				TextAlignment.Left => 0,
				TextAlignment.Center => 0.5f,
				TextAlignment.Right => 1,
				_ => 0
			};
		}

		public AutoSelectMode AutoSelectMode { get; set; }

		public bool AlwaysShowSelection { get; set; }

		public Color TextColor
		{
			get => Colors.Black;
			set
			{
			}
		}

		public void SelectAll() => Control.SelectRegion(0, Control.GetText()?.Length ?? 0);
	}
}
