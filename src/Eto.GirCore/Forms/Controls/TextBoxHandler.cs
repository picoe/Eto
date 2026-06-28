using Eto.Forms;
using Eto.Drawing;
using Eto.GirCore.Forms;
using Gtk;

namespace Eto.GirCore.Forms.Controls
{
	public class TextBoxHandler : GirControl<Gtk.Entry, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		public TextBoxHandler()
		{
			Control = Gtk.Entry.New();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextBox.TextChangedEvent:
					Control.OnChanged += (sender, e) => Callback.OnTextChanged(Widget, EventArgs.Empty);
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
			get => Control.MaxLength;
			set => Control.MaxLength = value;
		}

		public Range<int> Selection
		{
			get
			{
				Control.GetSelectionBounds(out var start, out var end);
				return new Range<int>(start, end);
			}
			set
			{
				Control.SelectRegion(value.Start, value.End);
			}
		}

		public int CaretIndex
		{
			get => Control.GetPosition();
			set => Control.SetPosition(value);
		}
		public bool ShowBorder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public TextAlignment TextAlignment
		{
			get => Control.GetAlignment() switch
			{
				0 => TextAlignment.Left,
				0.5f => TextAlignment.Center,
				1 => TextAlignment.Right,
				_ => throw new NotSupportedException()
			};
			set => Control.SetAlignment(value switch
			{
				TextAlignment.Left => 0,
				TextAlignment.Center => 0.5f,
				TextAlignment.Right => 1,
				_ => throw new NotSupportedException()
			});
		}

		public AutoSelectMode AutoSelectMode { get; set; }
		public bool AlwaysShowSelection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Color TextColor
		{
			get
			{
				Control.GetColor(out var color);
				return color.ToEto();
			}
			set
			{
				// todo
			}
		}

		public void SelectAll()
		{
			Control.SelectRegion(0, (int)Control.TextLength);
		}

	}
}
