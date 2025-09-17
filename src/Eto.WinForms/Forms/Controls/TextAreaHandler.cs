namespace Eto.WinForms.Forms.Controls
{
	public class TextAreaHandler : TextAreaHandler<TextArea, TextArea.ICallback>
	{

	}

	public class EtoRichTextBox : swf.RichTextBox
	{
		public bool AcceptsReturn { get; set; }

		protected override bool IsInputKey(swf.Keys keyData)
		{
			if (!AcceptsTab &&
				(keyData & ~swf.Keys.Modifiers) == swf.Keys.Tab &&
				(keyData & (swf.Keys.Control | swf.Keys.Alt)) == 0
			)
				return false;

			if (!AcceptsReturn && keyData == swf.Keys.Return)
				return false;

			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(swf.KeyEventArgs e)
		{
			if (!AcceptsReturn && e.KeyData == swf.Keys.Return)
			{
				e.Handled = true;
				return;
			}

			base.OnKeyDown(e);
		}
	}

	public class TextAreaHandler<TWidget, TCallback> : WindowsControl<EtoRichTextBox, TWidget, TCallback>, TextArea.IHandler
		where TWidget : TextArea
		where TCallback : TextArea.ICallback
	{
		int? lastCaretIndex;
		swf.TableLayoutPanel container;

		internal override bool SetFontTwiceForSomeReason => true;

		public static Size DefaultMinimumSize = new Size(100, 60);

		public override Size? GetDefaultSize(Size availableSize)
		{
			return DefaultMinimumSize;
		}

		public override swf.Control ContainerControl
		{
			get { return container; }
		}

		public TextAreaHandler()
		{
			Control = new EtoRichTextBox
			{
				Size = sd.Size.Empty,
				Multiline = true,
				AcceptsTab = true,
				AcceptsReturn = true,
				Dock = swf.DockStyle.Fill,
				BorderStyle = swf.BorderStyle.None,
				ScrollBars = swf.RichTextBoxScrollBars.Both,
				LanguageOption = swf.RichTextBoxLanguageOptions.DualFont,
				DetectUrls = false
			};
			container = new swf.TableLayoutPanel
			{
				MinimumSize = sd.Size.Empty,
				BorderStyle = swf.BorderStyle.FixedSingle,
				Size = DefaultMinimumSize.ToSD()
			};
			container.ColumnStyles.Add(new swf.ColumnStyle(swf.SizeType.AutoSize, 1));
			container.RowStyles.Add(new swf.RowStyle(swf.SizeType.AutoSize, 1));
			container.Controls.Add(Control, 0, 0);
		}

		/// <summary>
		/// Supresses the selection/caret changed events if greater than zero
		/// </summary>
		protected int SuppressSelectionChanged { get; set; }

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextArea.SelectionChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						if (SuppressSelectionChanged <= 0)
							Callback.OnSelectionChanged(Widget, EventArgs.Empty);
					};
					break;
				case TextArea.CaretIndexChangedEvent:
					Control.SelectionChanged += (sender, e) =>
					{
						var caretIndex = CaretIndex;
						if (SuppressSelectionChanged <= 0 && caretIndex != lastCaretIndex)
						{
							Callback.OnCaretIndexChanged(Widget, EventArgs.Empty);
							lastCaretIndex = caretIndex;
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public bool ReadOnly
		{
			get { return Control.ReadOnly; }
			set { Control.ReadOnly = value; }
		}

		public bool Wrap
		{
			get { return Control.WordWrap; }
			set { Control.WordWrap = value; }
		}

		public override Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				SuppressSelectionChanged++;
				var val = value ?? string.Empty;
				base.Text = val;
				if (!Control.IsHandleCreated) // correct??
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				Selection = Eto.Forms.Range.FromLength(val.Length, 0); // Fully qualified because System.Range was introduced in .NET Core 3.0
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				SuppressSelectionChanged--;
			}
		}

		public void Append(string text, bool scrollToCursor)
		{
			if (scrollToCursor)
			{
				Control.SelectionStart = Control.TextLength;
				Control.SelectedText = text;
				if (EtoEnvironment.Platform.IsMono)
					Control.ScrollToCaret();
				else
					Control.FastScrollToCaret(); // use a faster method for large amounts of text
			}
			else
				Control.AppendText(text);
		}

		public string SelectedText
		{
			get { return Control.SelectedText; }
			set
			{
				var start = Control.SelectionStart;
				SuppressSelectionChanged++;
				var val = value ?? string.Empty;
				Control.SelectedText = val;
				Control.Select(start, val.Length);
				Callback.OnSelectionChanged(Widget, EventArgs.Empty);
				SuppressSelectionChanged--;
			}
		}

		public Range<int> Selection
		{
			get { return new Range<int>(Control.SelectionStart, Control.SelectionStart + Control.SelectionLength - 1); }
			set { Control.Select(value.Start, value.End - value.Start + 1); }
		}

		public void SelectAll()
		{
			Control.SelectAll();
		}

		public int CaretIndex
		{
			get { return Control.SelectionStart; }
			set { Control.Select(value, 0); }
		}

		public bool AcceptsTab
		{
			get { return Control.AcceptsTab; }
			set { Control.AcceptsTab = value; }
		}

		public bool AcceptsReturn
		{
			get { return Control.AcceptsReturn; }
			set { Control.AcceptsReturn = value; }
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}

		TextAlignment _textAlignment;

		public TextAlignment TextAlignment
		{
			get => _textAlignment;
			set
			{
				if (value == TextAlignment) return;
				_textAlignment = value;
				var sel = Selection;
				Control.SelectAll();
				Control.SelectionAlignment = value.ToSWF();
				Selection = sel;
			}
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

		public BorderType Border
		{
			get => container.BorderStyle.ToEto();
			set => container.BorderStyle = value.ToSWF();
		}

		public int TextLength => Control.TextLength;

		public void ScrollTo(Range<int> range)
		{
			var pos = Control.GetPositionFromCharIndex(range.End);
			sd.Point scrollPosition = sd.Point.Empty;
			Win32.SendMessage(Control.Handle, Win32.WM.EM_GETSCROLLPOS, IntPtr.Zero, ref scrollPosition);

			var si = new Win32.SCROLLINFO();
			si.cbSize = Marshal.SizeOf(si);
			si.fMask = (int)Win32.ScrollInfoMask.SIF_ALL;
			Win32.GetScrollInfo(Control.Handle, (int)Win32.SBOrientation.SB_VERT, ref si);

			if (si.nPage > 0)
				scrollPosition.Y = Math.Min(si.nMax - si.nPage, Math.Max(si.nMin, scrollPosition.Y + pos.Y));

			Win32.GetScrollInfo(Control.Handle, (int)Win32.SBOrientation.SB_HORZ, ref si);

			// only scroll X if not in view already
			if (si.nPage > 0 && (pos.X < si.nPos || pos.X > si.nPos + si.nPage))
				scrollPosition.X = Math.Min(si.nMax - si.nPage, Math.Max(si.nMin, scrollPosition.X + pos.X));

			Win32.SendMessage(Control.Handle, Win32.WM.EM_SETSCROLLPOS, IntPtr.Zero, ref scrollPosition);
		}

		Win32.SB GetScrollX() => Control.RightToLeft switch
		{
			swf.RightToLeft.Yes => Win32.SB.RIGHT,
			_ => Win32.SB.LEFT
		};

		public void ScrollToStart()
		{
			Win32.SendMessage(Control.Handle, Win32.WM.VSCROLL, (IntPtr)Win32.SB.TOP, IntPtr.Zero);
			Win32.SendMessage(Control.Handle, Win32.WM.HSCROLL, (IntPtr)GetScrollX(), IntPtr.Zero);
		}

		public void ScrollToEnd()
		{
			Win32.SendMessage(Control.Handle, Win32.WM.VSCROLL, (IntPtr)Win32.SB.BOTTOM, IntPtr.Zero);
			Win32.SendMessage(Control.Handle, Win32.WM.HSCROLL, (IntPtr)GetScrollX(), IntPtr.Zero);
		}

		protected override swf.ContextMenuStrip GetDefaultContextMenu() => CreateDefaultContextMenu(Control);

		private swf.ContextMenuStrip CreateDefaultContextMenu(swf.TextBoxBase control)
		{
			var menu = new swf.ContextMenuStrip();

			var undoItem = new swf.ToolStripMenuItem("Undo", null, (s, e) => control.Undo());
			var cutItem = new swf.ToolStripMenuItem("Cut", null, (s, e) => control.Cut());
			var copyItem = new swf.ToolStripMenuItem("Copy", null, (s, e) => control.Copy());
			var pasteItem = new swf.ToolStripMenuItem("Paste", null, (s, e) => control.Paste());
			var deleteItem = new swf.ToolStripMenuItem("Delete", null, (s, e) => control.SelectedText = "");
			var selectAllItem = new swf.ToolStripMenuItem("Select All", null, (s, e) => control.SelectAll());

			menu.Items.AddRange(new swf.ToolStripItem[] {
				undoItem,
				new swf.ToolStripSeparator(),
				cutItem,
				copyItem,
				pasteItem,
				deleteItem,
				new swf.ToolStripSeparator(),
				selectAllItem
			});

			// Dynamically enable/disable items when menu opens
			menu.Opening += (s, e) =>
			{
				undoItem.Enabled = control.CanUndo;
				cutItem.Enabled = !string.IsNullOrEmpty(control.SelectedText);
				copyItem.Enabled = !string.IsNullOrEmpty(control.SelectedText);
				pasteItem.Enabled = swf.Clipboard.ContainsText();
				deleteItem.Enabled = !string.IsNullOrEmpty(control.SelectedText);
				selectAllItem.Enabled = control.TextLength > 0 && control.SelectionLength < control.TextLength;
			};

			return menu;
		}

	}
}