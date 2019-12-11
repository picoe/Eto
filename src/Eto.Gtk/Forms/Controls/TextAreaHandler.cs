using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TextAreaHandler : TextAreaHandler<Gtk.TextView, TextArea, TextArea.ICallback>
	{
	}

	public class TextAreaHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, TextArea.IHandler
		where TControl: Gtk.TextView, new()
		where TWidget: TextArea
		where TCallback: TextArea.ICallback
	{
		int suppressSelectionAndTextChanged;
		readonly Gtk.ScrolledWindow scroll;
		Gtk.TextTag tag;

		public override Gtk.Widget ContainerControl
		{
			get { return scroll; }
		}

		public override Size DefaultSize { get { return new Size(100, 60); } }

		public TextAreaHandler()
		{
			scroll = new Gtk.ScrolledWindow();
			scroll.ShadowType = Gtk.ShadowType.In;
			Control = new TControl();
			scroll.Add(Control);
			Wrap = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Size = new Size(100, 60);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Buffer.Changed += Connector.HandleBufferChanged;
					break;
				case TextArea.SelectionChangedEvent:
					Control.Buffer.MarkSet += Connector.HandleSelectionChanged;
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.Buffer.MarkSet += Connector.HandleCaretIndexChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new TextAreaConnector Connector { get { return (TextAreaConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextAreaConnector();
		}

		protected class TextAreaConnector : GtkControlConnector
		{
			Range<int> lastSelection;
			int? lastCaretIndex;

			public new TextAreaHandler<TControl, TWidget, TCallback> Handler { get { return (TextAreaHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleBufferChanged(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler.suppressSelectionAndTextChanged == 0)
					handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
			}

			public void HandleSelectionChanged(object o, Gtk.MarkSetArgs args)
			{
				var handler = Handler;
				var selection = handler.Selection;
				if (handler.suppressSelectionAndTextChanged == 0 && selection != lastSelection)
				{
					handler.Callback.OnSelectionChanged(handler.Widget, EventArgs.Empty);
					lastSelection = selection;
				}
			}

			public void HandleCaretIndexChanged(object o, Gtk.MarkSetArgs args)
			{
				var handler = Handler;
				var caretIndex = handler.CaretIndex;
				if (handler.suppressSelectionAndTextChanged == 0 && caretIndex != lastCaretIndex)
				{
					handler.Callback.OnCaretIndexChanged(handler.Widget, EventArgs.Empty);
					lastCaretIndex = caretIndex;
				}
			}

			public void HandleApplyTag(object sender, EventArgs e)
			{
				var buffer = Handler.Control.Buffer;
				var tag = Handler.tag;
				buffer.ApplyTag(tag, buffer.StartIter, buffer.EndIter);
			}
		}

		public override string Text
		{
			get { return Control.Buffer.Text; }
			set
			{
				var sel = Selection;
				suppressSelectionAndTextChanged++;
				Control.Buffer.Text = value;
				if (tag != null)
					Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
				Callback.OnTextChanged(Widget, EventArgs.Empty);
				suppressSelectionAndTextChanged--;
				if (sel != Selection)
					Callback.OnSelectionChanged(Widget, EventArgs.Empty);
			}
		}

		public virtual Color TextColor
		{
			get { return Control.GetForeground(); }
			set
			{
				Control.SetForeground(value);
				Control.SetTextColor(value);
			}
		}

		public override Color BackgroundColor
		{
			get
			{
				return Control.GetBase();
			}
			set
			{
				Control.SetBackground(value);
				Control.SetBase(value);
			}
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public bool Wrap
		{
			get { return Control.WrapMode != Gtk.WrapMode.None; }
			set { Control.WrapMode = value ? Gtk.WrapMode.WordChar : Gtk.WrapMode.None; }
		}

		public void Append(string text, bool scrollToCursor)
		{
			var end = Control.Buffer.EndIter;
			Control.Buffer.Insert(ref end, text);
			if (scrollToCursor)
			{
				var mark = Control.Buffer.CreateMark(null, end, false);
				Control.ScrollToMark(mark, 0, false, 0, 0);
			}
		}

		public string SelectedText
		{
			get
			{
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
				{
					return Control.Buffer.GetText(start, end, false);
				}
				return string.Empty;
			}
			set
			{
				suppressSelectionAndTextChanged++;
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
				{
					var startOffset = start.Offset;
					Control.Buffer.Delete(ref start, ref end);
					if (value != null)
					{
						Control.Buffer.Insert(ref start, value);
						start = Control.Buffer.GetIterAtOffset(startOffset);
						end = Control.Buffer.GetIterAtOffset(startOffset + value.Length);
						Control.Buffer.SelectRange(start, end);
					}
				}
				else if (value != null)
					Control.Buffer.InsertAtCursor(value);
				if (tag != null)
					Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
				Callback.OnTextChanged(Widget, EventArgs.Empty);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				suppressSelectionAndTextChanged--;
			}
		}

		public Range<int> Selection
		{
			get
			{
				Gtk.TextIter start, end;
				if (Control.Buffer.GetSelectionBounds(out start, out end))
				{
					return new Range<int>(start.Offset, end.Offset - 1);
				}
				return Range.FromLength(Control.Buffer.CursorPosition, 0);
			}
			set
			{
				suppressSelectionAndTextChanged++;
				var start = Control.Buffer.GetIterAtOffset(value.Start);
				var end = Control.Buffer.GetIterAtOffset(value.End + 1);
				Control.Buffer.SelectRange(start, end);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				suppressSelectionAndTextChanged--;
			}
		}

		public void SelectAll()
		{
			Control.Buffer.SelectRange(Control.Buffer.StartIter, Control.Buffer.EndIter);
		}

		public int CaretIndex
		{
			get { return Control.Buffer.GetIterAtMark(Control.Buffer.InsertMark).Offset; }
			set
			{
				var ins = Control.Buffer.GetIterAtOffset(value);
				Control.Buffer.SelectRange(ins, ins);
			}
		}

		public bool AcceptsTab
		{
			get { return Control.AcceptsTab; }
			set { Control.AcceptsTab = value; }
		}

		bool acceptsReturn = true;

		public bool AcceptsReturn
		{
			get { return acceptsReturn; }
			set
			{
				if (value != acceptsReturn)
				{
					if (!acceptsReturn)
						Widget.KeyDown -= HandleKeyDown;
					//Control.KeyPressEvent -= PreventEnterKey;
					acceptsReturn = value;
					if (!acceptsReturn)
						Widget.KeyDown += HandleKeyDown;
					//Control.KeyPressEvent += PreventEnterKey;
				}
			}
		}

		void HandleKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
				e.Handled = true;
		}

		static void PreventEnterKey(object o, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return)
				args.RetVal = false;
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				if (value != null)
				{
					if (tag == null)
					{
						tag = new Gtk.TextTag("font");
						Control.Buffer.TagTable.Add(tag);
						Control.Buffer.Changed += Connector.HandleApplyTag;
						Control.Buffer.ApplyTag(tag, Control.Buffer.StartIter, Control.Buffer.EndIter);
					}
					value.Apply(tag);
				}
				else
				{
					Control.Buffer.RemoveAllTags(Control.Buffer.StartIter, Control.Buffer.EndIter);
				}
			}
		}

		public TextAlignment TextAlignment
		{
			get { return Control.Justification.ToEto(); }
			set { Control.Justification = value.ToGtk(); }
		}

		public bool SpellCheck
		{
			get { return false; }
			set { }
		}

		public bool SpellCheckIsSupported { get { return false; } }

		public TextReplacements TextReplacements
		{
			get { return TextReplacements.None; }
			set { }
		}

		public TextReplacements SupportedTextReplacements
		{
			get { return TextReplacements.None; }
		}
	}
}
