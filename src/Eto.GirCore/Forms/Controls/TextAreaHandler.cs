using System.Runtime.CompilerServices;
using Range = Eto.Forms.Range<int>;

namespace Eto.GirCore.Forms.Controls
{
	public class TextAreaHandler : GirControl<Gtk.TextView, TextArea, TextArea.ICallback>, TextArea.IHandler
	{
		static readonly object BorderKey = new object();

		readonly Gtk.ScrolledWindow scroll;
		Gtk.CssProvider? cssProvider;
		readonly Dictionary<string, string> styleCache = new Dictionary<string, string>();
		int suppressSelectionAndTextChanged;
		Color textColor = Colors.Black;
		BorderType border = BorderType.Bezel;
		bool acceptsReturn = true;

		public override Gtk.Widget ContainerControl => scroll;

		public override Size DefaultSize => new Size(100, 60);

		public TextAreaHandler()
		{
			scroll = Gtk.ScrolledWindow.New();
			scroll.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic);
			scroll.HasFrame = true;

			Control = Gtk.TextView.New();
			scroll.SetChild(Control);

			Wrap = true;
			AcceptsTab = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Size = DefaultSize;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Buffer.OnChanged += HandleBufferChanged;
					break;
				case TextArea.SelectionChangedEvent:
					Control.Buffer.OnMarkSet += HandleSelectionChanged;
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.Buffer.OnMarkSet += HandleCaretIndexChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		void AddStyle(string style, [CallerMemberName] string? caller = null)
		{
			if (caller == null)
				return;
			if (cssProvider == null)
			{
				cssProvider = Gtk.CssProvider.New();
				Control.GetStyleContext().AddProvider(cssProvider, 600);
			}
			styleCache[caller] = style;
			cssProvider.LoadFromString(string.Join("\n", styleCache.Values));
		}

		void HandleBufferChanged(GObject.Object sender, EventArgs e)
		{
			if (suppressSelectionAndTextChanged == 0)
				Callback.OnTextChanged(Widget, EventArgs.Empty);
		}

		void HandleSelectionChanged(GObject.Object sender, Gtk.TextBuffer.MarkSetSignalArgs e)
		{
			if (suppressSelectionAndTextChanged == 0)
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
		}

		void HandleCaretIndexChanged(GObject.Object sender, Gtk.TextBuffer.MarkSetSignalArgs e)
		{
			if (suppressSelectionAndTextChanged == 0)
				Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
		}

		public string Text
		{
			get => Control.Buffer.Text;
			set
			{
				var selection = Selection;
				suppressSelectionAndTextChanged++;
				Control.Buffer.Text = value ?? string.Empty;
				suppressSelectionAndTextChanged--;
				Callback.OnTextChanged(Widget, EventArgs.Empty);
				if (selection != Selection)
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public Color TextColor
		{
			get => textColor;
			set
			{
				textColor = value;
				AddStyle($"textview text {{ color: {value.ToHex()}; }}");
			}
		}

		public bool ReadOnly
		{
			get => !Control.Editable;
			set => Control.Editable = !value;
		}

		public bool Wrap
		{
			get => Control.WrapMode != Gtk.WrapMode.None;
			set => Control.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None;
		}

		public void Append(string text, bool scrollToCursor)
		{
			Control.Buffer.GetEndIter(out var end);
			var value = text ?? string.Empty;
			Control.Buffer.Insert(end, value, value.Length);
			if (scrollToCursor)
			{
				var mark = Control.Buffer.CreateMark("append-end", end, false);
				Control.ScrollToMark(mark, 0, false, 0, 0);
			}
		}

		public string SelectedText
		{
			get
			{
				if (Control.Buffer.GetSelectionBounds(out var start, out var end))
					return Control.Buffer.GetText(start, end, false);
				return string.Empty;
			}
			set
			{
				suppressSelectionAndTextChanged++;
				if (Control.Buffer.GetSelectionBounds(out var start, out var end))
				{
					var startOffset = start.GetOffset();
					Control.Buffer.Delete(start, end);
					if (value != null)
					{
						Control.Buffer.GetIterAtOffset(out start, startOffset);
						Control.Buffer.Insert(start, value, value.Length);
						Control.Buffer.GetIterAtOffset(out start, startOffset);
						Control.Buffer.GetIterAtOffset(out end, startOffset + value.Length);
						Control.Buffer.SelectRange(start, end);
					}
				}
				else if (value != null)
				{
					Control.Buffer.InsertAtCursor(value, value.Length);
				}
				suppressSelectionAndTextChanged--;
				Callback.OnTextChanged(Widget, EventArgs.Empty);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public Range Selection
		{
			get
			{
				if (Control.Buffer.GetSelectionBounds(out var start, out var end))
					return new Range(start.GetOffset(), end.GetOffset() - 1);
				return Eto.Forms.Range.FromLength(Control.Buffer.CursorPosition, 0);
			}
			set
			{
				suppressSelectionAndTextChanged++;
				Control.Buffer.GetIterAtOffset(out var start, value.Start);
				Control.Buffer.GetIterAtOffset(out var end, value.End + 1);
				Control.Buffer.SelectRange(start, end);
				suppressSelectionAndTextChanged--;
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public void SelectAll()
		{
			Control.Buffer.GetStartIter(out var start);
			Control.Buffer.GetEndIter(out var end);
			Control.Buffer.SelectRange(start, end);
		}

		public int CaretIndex
		{
			get
			{
				Control.Buffer.GetIterAtMark(out var iter, Control.Buffer.GetInsert());
				return iter.GetOffset();
			}
			set
			{
				Control.Buffer.GetIterAtOffset(out var iter, value);
				Control.Buffer.SelectRange(iter, iter);
			}
		}

		public bool AcceptsTab
		{
			get => Control.AcceptsTab;
			set => Control.AcceptsTab = value;
		}

		public bool AcceptsReturn
		{
			get => acceptsReturn;
			set
			{
				if (acceptsReturn == value)
					return;
				if (!acceptsReturn)
					Widget.KeyDown -= HandleKeyDown;
				acceptsReturn = value;
				if (!acceptsReturn)
					Widget.KeyDown += HandleKeyDown;
			}
		}

		void HandleKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
				e.Handled = true;
		}

		public TextAlignment TextAlignment
		{
			get => Control.Justification switch
			{
				Gtk.Justification.Left => TextAlignment.Left,
				Gtk.Justification.Center => TextAlignment.Center,
				Gtk.Justification.Right => TextAlignment.Right,
				_ => TextAlignment.Left,
			};
			set => Control.Justification = value.ToGtk();
		}

		public bool SpellCheck
		{
			get => false;
			set { }
		}

		public bool SpellCheckIsSupported => false;

		public TextReplacements TextReplacements
		{
			get => TextReplacements.None;
			set { }
		}

		public TextReplacements SupportedTextReplacements => TextReplacements.None;

		public BorderType Border
		{
			get => border;
			set
			{
				border = value;
				Widget.Properties.Set(BorderKey, value);
				scroll.HasFrame = value != BorderType.None;
			}
		}

		public int TextLength => Control.Buffer.GetCharCount();

		public void ScrollTo(Range range)
		{
			Control.Buffer.GetIterAtOffset(out var iter, range.Start + range.Length());
			var mark = Control.Buffer.CreateMark("scroll-range", iter, false);
			Control.ScrollToMark(mark, 0, false, 0, 0);
		}

		double GetScrollX() => Control.Justification switch
		{
			Gtk.Justification.Right => scroll.Hadjustment.Upper,
			Gtk.Justification.Center => (scroll.Hadjustment.Upper - scroll.Hadjustment.Lower - scroll.Hadjustment.PageSize) / 2,
			_ => scroll.Hadjustment.Lower,
		};

		public void ScrollToEnd()
		{
			scroll.Vadjustment.Value = scroll.Vadjustment.Upper - scroll.Vadjustment.PageSize;
			scroll.Hadjustment.Value = GetScrollX();
		}

		public void ScrollToStart()
		{
			scroll.Vadjustment.Value = scroll.Vadjustment.Lower;
			scroll.Hadjustment.Value = GetScrollX();
		}
	}
}
