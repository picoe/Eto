namespace Eto.GtkSharp.Forms.Controls
{
	public class EtoEntry : Gtk.Entry
	{
		public bool AlwaysShowSelection { get; set; }

		public WeakReference WeakHandler { get; set; }

		protected override bool OnDrawn(Cairo.Context cr)
		{
			var ret = base.OnDrawn(cr);

			if (!AlwaysShowSelection || HasVisibleFocus)
				return ret;

			// can have selection even without focus			
			GetSelectionBounds(out var selectionStart, out var selectionEnd);
			if (selectionStart != selectionEnd)
				return ret;

			// Gtk.Entry has a ScrollOffset property in GTK3/GTK4
			var xOffset = ScrollOffset;

			// Get the layout and entry padding
			var layout = Layout;

			// Get layout size for alignment
			layout.GetPixelSize(out int layoutWidth, out int layoutHeight);
			var padding = StyleContext.GetPadding(StateFlags);

			int xPadding = padding.Left;
			int yPadding = (Allocation.Height - layoutHeight) / 2;

			var selection = (WeakHandler?.Target as TextBox.IHandler)?.Selection ?? new Range<int>(0, 0);

			// Draw selection manually if not focused
			if (selection.Length() > 0)
			{
				// Get selection bounds in layout
				var startRect = layout.IndexToPos(selection.Start);
				var endRect = layout.IndexToPos(selection.End + 1);

				// Convert from Pango units to pixels
				double x0 = startRect.X / Pango.Scale.PangoScale;
				double x1 = endRect.X / Pango.Scale.PangoScale;

				int selectionX = (int)x0 + xPadding - xOffset;
				int selectionWidth = (int)(x1 - x0);
				int selectionY = yPadding;
				int selectionHeight = layoutHeight;

				// Draw selection background, but since this is actually drawn overtop, set alpha so we can still see the text
				var color = new Color(SystemColors.Highlight, 0.4f).ToCairo();
				cr.SetSourceColor(color);
				cr.Rectangle(selectionX, selectionY, selectionWidth, selectionHeight);
				cr.Fill();
			}

			return ret;
		}
	}
	public class TextBoxHandler : TextBoxHandler<Gtk.Entry, TextBox, TextBox.ICallback>
	{
		internal static object DisableTextChanged_Key = new object();

		public TextBoxHandler()
		{
			Control = new EtoEntry { WeakHandler = new WeakReference(this) };
			Control.WidthRequest = 100;
			Control.WidthChars = 0;
		}

	}

	public class TextBoxHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, TextBox.IHandler
		where TControl : Gtk.Entry
		where TWidget : TextBox
		where TCallback : TextBox.ICallback
	{
		string placeholderText;
		Range<int>? lastSelection;
		Range<int>? initialSelection;
		bool textChangedPending;

		protected override void Initialize()
		{
			base.Initialize();
			Control.ActivatesDefault = true;
			HandleEvent(Eto.Forms.Control.GotFocusEvent);
			HandleEvent(Eto.Forms.Control.LostFocusEvent);
		}

		void SetSelection()
		{
			if (AutoSelectMode == AutoSelectMode.Always)
			{
				Application.Instance.AsyncInvoke(SelectAll);
			}
			else if (AutoSelectMode == AutoSelectMode.Never)
			{
				if (lastSelection == null)
				{
					var text = Text;
					lastSelection = new Range<int>(text.Length, text.Length - 1);
				}
				initialSelection = null;
				var selection = lastSelection; // Gtk on some platforms (macOS) fire LostFocus after this??
				Application.Instance.AsyncInvoke(() =>
				{
					if (selection != null && Selection.Length() == Text.Length)
						Selection = selection.Value;
				});
			}
			else if (initialSelection != null)
			{
				Application.Instance.AsyncInvoke(() =>
				{
					if (initialSelection != null && Selection.Length() == Text.Length)
						Selection = initialSelection.Value;
					initialSelection = null;
				});
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += Connector.HandleTextChanged;
					break;
				case TextBox.TextChangingEvent:
					Control.ClipboardPasted += Connector.HandleClipboardPasted;
					Control.TextDeleted += Connector.HandleTextDeleted;
					HandleEvent(Eto.Forms.Control.TextInputEvent);
					break;
				case Eto.Forms.Control.TextInputEvent:
					// A native GtkEntry commits text -- ordinary keystrokes, IME/dead-key composition,
					// drag/drop -- through its OWN system input-method context, which the base
					// IMContextSimple-based TextInput path never observes (see the comment on the shadow
					// IMContextSimple in GtkControl). Hook the entry's insert-text signal instead so
					// TextInput fires for every committed insertion, not just simple keystrokes. The base
					// keypress path is suppressed for TextBox (see TextBoxConnector.HandleKeyPressEvent) so
					// plain keystrokes don't fire TextInput twice.
					Control.TextInserted += Connector.HandleTextInserted;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		// A native GtkEntry can't distinguish an IME/composition commit from ordinary typing (its
		// preedit state isn't exposed), so per the TextChangeSource contract a key-driven insert is
		// reported as Keyboard; anything else (drag/drop, primary-selection paste, dictation) is Unknown.
		TextChangeSource GetInsertSource()
		{
			var currentEvent = Gtk.Application.CurrentEvent;
			if (currentEvent != null && currentEvent.Type == Gdk.EventType.KeyPress)
				return TextChangeSource.Keyboard;
			return TextChangeSource.Unknown;
		}

		protected new TextBoxConnector Connector { get { return (TextBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextBoxConnector();
		}

		protected int DisableTextChanged
		{
			get => Widget.Properties.Get<int>(TextBoxHandler.DisableTextChanged_Key);
			set => Widget.Properties.Set(TextBoxHandler.DisableTextChanged_Key, value);
		}


		protected class TextBoxConnector : GtkControlConnector
		{
			public new TextBoxHandler<TControl, TWidget, TCallback> Handler { get { return (TextBoxHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleTextChanged(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				if (h.DisableTextChanged > 0)
					return;

				// Gtk raises the native "changed" signal once per edit primitive, so a
				// single keystroke that replaces the current selection emits it TWICE
				// (delete-selection, then insert-character). WPF and Mac raise TextChanged
				// once per keystroke. Without coalescing, a handler that synchronously
				// rewrites Text from the intermediate (post-delete) state -- e.g. the
				// command line's autocomplete ghost-completion (AutoCompletePopup.SetInput)
				// -- corrupts the pending insert and the typed character is dropped.
				// Coalesce consecutive native changes within the same main-loop iteration
				// into a single TextChanged so consumers see one change per keystroke.
				//
				// Only the native path is coalesced: programmatic Text/SelectedText writes
				// raise OnTextChanged directly (guarded here by DisableTextChanged), so they
				// stay synchronous.
				if (h.textChangedPending)
					return;
				h.textChangedPending = true;
				Application.Instance.AsyncInvoke(() =>
				{
					var handler = Handler;
					if (handler == null)
						return;
					handler.textChangedPending = false;
					handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
				});
			}

			static Clipboard clipboard;

			static Clipboard Clipboard
			{
				get { return clipboard ?? (clipboard = new Clipboard()); }
			}

			// Suppress the base IMContextSimple-based TextInput path for a native TextBox. The GtkEntry
			// owns its own system input-method context and inserts committed text (plain, IME-composed,
			// dead-key, etc.) itself; Eto observes those via the entry's insert-text signal
			// (HandleTextInserted). Feeding a second shadow IMContextSimple here would double-fire
			// TextInput for ordinary keystrokes and still miss IME text entirely.
			[GLib.ConnectBefore]
			public override void HandleKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var e = args.Event.ToEto();
				if (e != null)
				{
					handler.Callback.OnKeyDown(handler.Widget, e);
					args.RetVal = e.Handled;
				}
			}

			bool inserting;
			bool pasting;

			// Fires for every committed insertion into the entry regardless of origin -- ordinary typing,
			// IME/dead-key composition, and drag/drop -- which is the only path that also sees IME text.
			[GLib.ConnectBefore]
			public void HandleTextInserted(object o, Gtk.TextInsertedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				// Programmatic Text changes raise TextChanging(Programmatic) explicitly under
				// DisableTextChanged; don't report their native insert a second time.
				if (handler.DisableTextChanged > 0)
					return;
				// A paste is already reported by HandleClipboardPasted with the correct source.
				if (pasting || inserting)
					return;
				inserting = true;
				try
				{
					var text = args.NewText ?? string.Empty;
					if (text.Length == 0)
						return;
					// Pure insertion at the caret -- any selected text was already removed via delete-text.
					var range = new Range<int>(args.Position, args.Position - 1);
					var cancel = false;
					if (handler.IsEventHandled(TextBox.TextChangingEvent))
					{
						var tia = new TextChangingEventArgs(text, range, handler.GetInsertSource());
						handler.Callback.OnTextChanging(handler.Widget, tia);
						cancel = tia.Cancel;
					}
					// TextInput fires after TextChanging (matching WPF/Mac ordering) and can also cancel.
					if (!cancel && handler.IsEventHandled(Eto.Forms.Control.TextInputEvent))
					{
						var tie = new TextInputEventArgs(text);
						handler.Callback.OnTextInput(handler.Widget, tie);
						cancel = tie.Cancel;
					}
					if (cancel)
						NativeMethods.g_signal_stop_emission_by_name(handler.Control.Handle, "insert-text");
				}
				finally
				{
					inserting = false;
				}
			}

			[GLib.ConnectBefore]
			public void HandleClipboardPasted(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				// The pasted characters also arrive through insert-text; flag the paste so
				// HandleTextInserted doesn't report them a second time as a keyboard change. Cleared on
				// the next main-loop iteration, after the synchronous insert paste-clipboard performs.
				pasting = true;
				Application.Instance.AsyncInvoke(() => pasting = false);
				var tia = new TextChangingEventArgs(Clipboard.Text, h.Selection, TextChangeSource.Paste);
				Handler.Callback.OnTextChanging(h.Widget, tia);
				if (tia.Cancel)
					NativeMethods.g_signal_stop_emission_by_name(Handler.Control.Handle, "paste-clipboard");
			}

			bool deleting;

			[GLib.ConnectBefore]
			public void HandleTextDeleted(object o, Gtk.TextDeletedArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (!deleting)
				{
					deleting = true;
					if (args.StartPos < args.EndPos)
					{
						// The delete-text signal doesn't identify the origin (keyboard, cut, etc.); that it's a
						// deletion is derivable from the empty replacement text.
						var tia = new TextChangingEventArgs(string.Empty, new Range<int>(args.StartPos, Math.Min(args.EndPos - 1, handler.Control.Text.Length - 1)), TextChangeSource.Unknown);
						handler.Callback.OnTextChanging(handler.Widget, tia);
						if (tia.Cancel)
							args.RetVal = true;
					}
					deleting = false;
				}
			}

			public override void FocusInEvent(object o, Gtk.FocusInEventArgs args)
			{
				base.FocusInEvent(o, args);
				Handler?.SetSelection();
			}

			public override void FocusOutEvent(object o, Gtk.FocusOutEventArgs args)
			{
				base.FocusOutEvent(o, args);
				var handler = Handler;
				if (handler != null)
				{
					handler.lastSelection = null;
					handler.lastSelection = handler.Selection;
				}
			}

#if GTK2

			public virtual void HandleExposeEvent(object o, Gtk.ExposeEventArgs args)
			{
				var handler = Handler;
				if (handler == null)
					return;
				var control = handler.Control;
				if (!string.IsNullOrEmpty(control.Text) || args.Event.Window == control.GdkWindow)
					return;

				if (handler.placeholderLayout == null)
				{
					handler.placeholderLayout = new Pango.Layout(control.PangoContext);
					handler.placeholderLayout.FontDescription = control.PangoContext.FontDescription.Copy();
				}
				handler.placeholderLayout.SetText(handler.placeholderText);

				int currentHeight, currentWidth;
				args.Event.Window.GetSize(out currentWidth, out currentHeight);

				int width, height;
				handler.placeholderLayout.GetPixelSize(out width, out height);

				var style = control.Style;
				var bc = style.Base(Gtk.StateType.Normal);
				var tc = style.Text(Gtk.StateType.Normal);

				using (var gc = new Gdk.GC(args.Event.Window))
				{
					gc.Copy(style.TextGC(Gtk.StateType.Normal));

					gc.RgbFgColor = new Gdk.Color((byte)(((int)bc.Red + tc.Red) / 2 / 256), (byte)(((int)bc.Green + (int)tc.Green) / 2 / 256), (byte)((bc.Blue + tc.Blue) / 2 / 256));

					args.Event.Window.DrawLayout(gc, 2, (currentHeight - height) / 2 + 1, handler.placeholderLayout);
				}
			}

#endif
		}
#if GTK2
		Pango.Layout placeholderLayout;

		public override Eto.Drawing.Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				placeholderLayout = null;
			}
		}
#else
		protected override void SetBackgroundColor(Eto.Drawing.Color? color)
		{
		}
#endif

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				Control.WidthChars = (value.Width == -1) ? -1 : 0;
				base.Size = value;
			}
		}

		public override string Text
		{
			get { return Control.Text; }
			set
			{
				var oldText = Control.Text;
				var newText = value ?? string.Empty;
				if (newText != oldText)
				{
					var args = new TextChangingEventArgs(oldText, newText, TextChangeSource.Programmatic);
					Callback.OnTextChanging(Widget, args);
					if (args.Cancel)
						return;
					DisableTextChanged++;
					Control.Text = newText;
					lastSelection = null;
					initialSelection = null;
					DisableTextChanged--;
					if (AutoSelectMode == AutoSelectMode.Never)
					{
						Selection = Eto.Forms.Range.FromLength(newText.Length, 0);
					}
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public virtual bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength == -1 ? 0 : Control.MaxLength; }
			set { Control.MaxLength = value == 0 ? -1 : value; }
		}

		public string PlaceholderText
		{
			get { return placeholderText; }
			set
			{
#if GTK2
				if (!string.IsNullOrEmpty(placeholderText))
					Control.ExposeEvent -= Connector.HandleExposeEvent;
				placeholderText = value;
				if (!string.IsNullOrEmpty(placeholderText))
					Control.ExposeEvent += Connector.HandleExposeEvent;
				if (Widget.Loaded)
					Invalidate(false);
#elif GTKCORE
				placeholderText = value;
				Control.PlaceholderText = value;
#else
				placeholderText = value;

				NativeMethods.gtk_entry_set_placeholder_text(Control.Handle, value);
#endif
			}
		}

		public void SelectAll()
		{
			if (!string.IsNullOrEmpty(Control.Text))
				Selection = new Range<int>(0, Control.Text.Length - 1);
		}

		public Color TextColor
		{
			get { return Control.GetTextColor(); }
			set { Control.SetTextColor(value); }
		}

		public override Color BackgroundColor
		{
			get { return Control.GetBackground(); }
			set
			{
				Control.SetBackground(value);
				Control.SetBase(value);
			}
		}

		public int CaretIndex
		{
			get
			{
				if (!HasFocus && initialSelection != null)
					return initialSelection.Value.Start;
				int start, end;
				Control.GetSelectionBounds(out start, out end);
				return Math.Min(start, end);
			}
			set
			{
				Control.SelectRegion(value, value);
				lastSelection = new Range<int>(value, value - 1);
				if (!HasFocus)
					initialSelection = lastSelection;
			}
		}

		public Range<int> Selection
		{
			get
			{
				if (!HasFocus)
				{
					if (lastSelection != null)
						return lastSelection.Value;
					if (initialSelection != null)
						return initialSelection.Value;
				}
				Control.GetSelectionBounds(out var start, out var end);
				return new Range<int>(Math.Min(start, end), Math.Max(start, end) - 1);
			}
			set
			{
				Control.SelectRegion(value.Start, value.End + 1);
				lastSelection = value;
				if (!HasFocus)
				{
					initialSelection = lastSelection;
					if (AlwaysShowSelection)
						Control.QueueDraw();
				}
			}
		}

		public int GetCharacterIndex(PointF location)
		{
			var text = Control.Text ?? string.Empty;
			if (text.Length == 0)
				return 0;

			Control.GetLayoutOffsets(out var layoutX, out var layoutY);
			var x = (int)((location.X - layoutX + Control.ScrollOffset) * Pango.Scale.PangoScale);
			var y = (int)((location.Y - layoutY) * Pango.Scale.PangoScale);

			if (Control.Layout.XyToIndex(x, y, out var index, out var trailing))
				return Control.LayoutIndexToTextIndex(index + trailing);

			return location.X <= layoutX ? 0 : text.Length;
		}

		public TextAlignment TextAlignment
		{
			get => Control.Alignment < 0.5f ? TextAlignment.Left
						  : Control.Alignment > 0.5f ? TextAlignment.Right
						  : TextAlignment.Center;
			set => Control.Alignment = value switch
			{
				TextAlignment.Left => 0,
				TextAlignment.Center => 0.5f,
				TextAlignment.Right => 1,
				_ => throw new NotSupportedException(),
			};
		}

		public override bool ShowBorder
		{
			get { return Control.HasFrame; }
			set { Control.HasFrame = value; }
		}

		public AutoSelectMode AutoSelectMode { get; set; }
		public bool AlwaysShowSelection
		{
			get => Control is EtoEntry entry && entry.AlwaysShowSelection;
			set
			{
				if (Control is EtoEntry entry)
				{
					entry.AlwaysShowSelection = value;
					entry.QueueDraw();
				}
				else
					Debug.WriteLine("This control does not support AlwaysShowSelection");
			}
		}
	}
}
