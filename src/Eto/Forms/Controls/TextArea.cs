using Eto.Drawing;
using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Text replacement options when entering text
	/// </summary>
	[Flags]
	public enum TextReplacements
	{
		/// <summary>
		/// Do not perform any automatic replacements based on user input
		/// </summary>
		None = 0,
		/// <summary>
		/// Perform text replacements, such as shortcuts
		/// </summary>
		Text = 1 << 0,
		/// <summary>
		/// Perform replacements of straight quotes to 'curly' quotes
		/// </summary>
		Quote = 1 << 1,
		/// <summary>
		/// Perform replacements of dashes '-' to em dash '—'.
		/// </summary>
		/// <remarks>
		/// Note that some platforms may do this automatically with a single dash, some may require the user to enter 
		/// double dashes.
		/// </remarks>
		Dash = 1 << 2,
		/// <summary>
		/// Perform automatic spelling correction
		/// </summary>
		Spelling = 1 << 3,
		/// <summary>
		/// All replacements enabled.
		/// </summary>
		All = Text | Quote | Dash | Spelling
	}

	/// <summary>
	/// Control for multi-line text
	/// </summary>
	/// <remarks>
	/// This differs from the <see cref="TextBox"/> in that it is used for multi-line text entry and can accept <see cref="Keys.Tab"/>
	/// and <see cref="Keys.Enter"/> input.
	/// </remarks>
	[Handler(typeof(TextArea.IHandler))]
	public class TextArea : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="SelectionChanged"/> event.
		/// </summary>
		public const string SelectionChangedEvent = "TextArea.SelectionChanged";

		/// <summary>
		/// Occurs when the <see cref="Selection"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="SelectionChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="CaretIndexChanged"/> event.
		/// </summary>
		public const string CaretIndexChangedEvent = "TextArea.CaretIndexChanged";

		/// <summary>
		/// Occurs when the <see cref="CaretIndex"/> has changed.
		/// </summary>
		public event EventHandler<EventArgs> CaretIndexChanged
		{
			add { Properties.AddHandlerEvent(CaretIndexChangedEvent, value); }
			remove { Properties.RemoveEvent(CaretIndexChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="CaretIndexChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnCaretIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(CaretIndexChangedEvent, this, e);
		}

		#endregion

		static TextArea()
		{
			EventLookup.Register<TextArea>(c => c.OnSelectionChanged(null), TextArea.SelectionChangedEvent);
			EventLookup.Register<TextArea>(c => c.OnCaretIndexChanged(null), TextArea.CaretIndexChangedEvent);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextArea"/> is read only.
		/// </summary>
		/// <remarks>
		/// A read only text box can be scrolled, text can be selected and copied, etc. However, the user
		/// will not be able to change any of the text.
		/// This differs from the <see cref="Control.Enabled"/> property, which disables all user interaction.
		/// </remarks>
		/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether text will wrap if lines are longer than the width of the control.
		/// </summary>
		/// <remarks>
		/// Typically, a platform will word wrap the text.
		/// </remarks>
		/// <value><c>true</c> to wrap the text; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool Wrap
		{
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}
		/// <summary>
		/// Gets or sets the selected text.
		/// </summary>
		/// <remarks>
		/// When setting the selected text, the text within the <see cref="Selection"/> range will be replaced with
		/// the new value.
		/// </remarks>
		/// <value>The selected text.</value>
		public string SelectedText
		{
			get { return Handler.SelectedText; }
			set { Handler.SelectedText = value; }
		}

		/// <summary>
		/// Gets or sets the range of selected text.
		/// </summary>
		/// <remarks>
		/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
		/// </remarks>
		/// <seealso cref="SelectAll"/>
		/// <value>The text selection.</value>
		public Range<int> Selection
		{
			get { return Handler.Selection; }
			set { Handler.Selection = value; }
		}

		/// <summary>
		/// Selects all text.
		/// </summary>
		/// <remarks>
		/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
		/// </remarks>
		public void SelectAll()
		{
			Handler.SelectAll();
		}

		/// <summary>
		/// Gets or sets the index of the insertion caret.
		/// </summary>
		/// <remarks>
		/// When setting the caret, the control will be focussed and the associated keyboard may appear on mobile platforms.
		/// </remarks>
		/// <value>The index of the insertion caret.</value>
		public int CaretIndex
		{
			get { return Handler.CaretIndex; }
			set { Handler.CaretIndex = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the tab key is inserted into the text area, or if it should be ignored by this control and used
		/// for navigating to the next control.
		/// </summary>
		/// <value><c>true</c> if the TextArea accepts tab key characters; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool AcceptsTab
		{
			get { return Handler.AcceptsTab; }
			set { Handler.AcceptsTab = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the return key is inserted into the text area, or if it should be ignored by this control.
		/// </summary>
		/// <value><c>true</c> if the TextArea accepts the return key; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool AcceptsReturn
		{
			get { return Handler.AcceptsReturn; }
			set { Handler.AcceptsReturn = value; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		/// <value>The horizontal alignment.</value>
		public TextAlignment TextAlignment
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		/// <value>The horizontal alignment.</value>
		[Obsolete("Since 2.1: Use TextAlignment instead")]
		public HorizontalAlign HorizontalAlign
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextArea"/> will perform spell checking.
		/// </summary>
		/// <remarks>
		/// When <c>true</c>, platforms will typically show misspelled or unknown words with a red underline.
		/// This is a hint, and is only supported by the platform when <see cref="SpellCheckIsSupported"/> is true.
		/// When not supported, setting this property will do nothing.
		/// </remarks>
		/// <value><c>true</c> if spell check; otherwise, <c>false</c>.</value>
		public bool SpellCheck
		{
			get { return Handler.SpellCheck; }
			set { Handler.SpellCheck = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="SpellCheck"/> property is supported on the control's platform.
		/// </summary>
		/// <value><c>true</c> if spell check is supported; otherwise, <c>false</c>.</value>
		public bool SpellCheckIsSupported
		{
			get { return Handler.SpellCheckIsSupported; }
		}

		/// <summary>
		/// Gets or sets a hint value indicating whether this <see cref="Eto.Forms.TextArea"/> will automatically correct text.
		/// </summary>
		/// <remarks>
		/// On some platforms, autocorrection or text replacements such as quotes, etc may be default.
		/// Set this to <see cref="Eto.Forms.TextReplacements.None"/> to disable any text replacement.
		/// 
		/// Note this is only supported on OS X currently, all other platforms will be ignored.
		/// </remarks>
		/// <value>Type of replacements to enable when entering text..</value>
		public TextReplacements TextReplacements
		{
			get { return Handler.TextReplacements; }
			set { Handler.TextReplacements = value; }
		}

		/// <summary>
		/// Gets the text replacements that this control supports on the current platform.
		/// </summary>
		/// <remarks>
		/// You can use this to determine which flags in the <see cref="TextReplacements"/> will take effect.
		/// </remarks>
		/// <value>The supported text replacements.</value>
		public TextReplacements SupportedTextReplacements
		{
			get { return Handler.SupportedTextReplacements; }
		}

		/// <summary>
		/// Append the specified text to the control and optionally scrolls to make the inserted text visible.
		/// </summary>
		/// <remarks>
		/// This is an optimized way of inserting text into a TextArea when its content gets large.
		/// </remarks>
		/// <param name="text">Text to insert.</param>
		/// <param name="scrollToCursor">If set to <c>true</c>, scroll to the inserted text.</param>
		public void Append(string text, bool scrollToCursor = false)
		{
			Handler.Append(text, scrollToCursor);
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback()
		{
			return callback;
		}

		/// <summary>
		/// Callback interface for the <see cref="TextArea"/>
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			void OnSelectionChanged(TextArea widget, EventArgs e);

			/// <summary>
			/// Raises the caret index changed event.
			/// </summary>
			void OnCaretIndexChanged(TextArea widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="TextArea"/>
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the selection changed event.
			/// </summary>
			public void OnSelectionChanged(TextArea widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnSelectionChanged(e);
			}

			/// <summary>
			/// Raises the caret index changed event.
			/// </summary>
			public void OnCaretIndexChanged(TextArea widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnCaretIndexChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="TextArea"/>
		/// </summary>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextArea"/> is read only.
			/// </summary>
			/// <remarks>
			/// A read only text box can be scrolled, text can be selected and copied, etc. However, the user
			/// will not be able to change any of the text.
			/// This differs from the <see cref="Control.Enabled"/> property, which disables all user interaction.
			/// </remarks>
			/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
			bool ReadOnly { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether text will wrap if lines are longer than the width of the control.
			/// </summary>
			/// <remarks>
			/// Typically, a platform will word wrap the text.
			/// </remarks>
			/// <value><c>true</c> to wrap the text; otherwise, <c>false</c>.</value>
			bool Wrap { get; set; }

			/// <summary>
			/// Append the specified text to the control and optionally scrolls to make the inserted text visible.
			/// </summary>
			/// <remarks>
			/// This is an optimized way of inserting text into a TextArea when its content gets large.
			/// </remarks>
			/// <param name="text">Text to insert.</param>
			/// <param name="scrollToCursor">If set to <c>true</c>, scroll to the inserted text.</param>
			void Append(string text, bool scrollToCursor);

			/// <summary>
			/// Gets or sets the selected text.
			/// </summary>
			/// <remarks>
			/// When setting the selected text, the text within the <see cref="Selection"/> range will be replaced with
			/// the new value.
			/// </remarks>
			/// <value>The selected text.</value>
			string SelectedText { get; set; }

			/// <summary>
			/// Gets or sets the range of selected text.
			/// </summary>
			/// <seealso cref="SelectAll"/>
			/// <value>The text selection.</value>
			Range<int> Selection { get; set; }

			/// <summary>
			/// Selects all text.
			/// </summary>
			/// <remarks>
			/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
			/// </remarks>
			void SelectAll();

			/// <summary>
			/// Gets or sets the index of the insertion caret.
			/// </summary>
			/// <remarks>
			/// When setting the caret, the control will be focussed and the associated keyboard may appear on mobile platforms.
			/// </remarks>
			/// <value>The index of the insertion caret.</value>
			int CaretIndex { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the tab key is inserted into the text area, or if it should be ignored by this control and used
			/// for navigating to the next control.
			/// </summary>
			/// <value><c>true</c> if the TextArea accepts tab key characters; otherwise, <c>false</c>.</value>
			bool AcceptsTab { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the return key is inserted into the text area, or if it should be ignored by this control.
			/// </summary>
			/// <value><c>true</c> if the TextArea accepts the return key; otherwise, <c>false</c>.</value>
			bool AcceptsReturn { get; set; }

			/// <summary>
			/// Gets or sets a hint value indicating whether this <see cref="Eto.Forms.TextArea"/> will automatically correct text.
			/// </summary>
			/// <remarks>
			/// On some platforms, autocorrection or text replacements such as quotes, etc may be default.
			/// Set this to <see cref="Eto.Forms.TextReplacements.None"/> to disable any text replacement.
			/// 
			/// Note this is only a hint and not all (or any) of the replacements may apply on some platforms.
			/// </remarks>
			/// <value>Type of replacements to enable when entering text..</value>
			TextReplacements TextReplacements { get; set; }

			/// <summary>
			/// Gets the text replacements that this control supports on the current platform.
			/// </summary>
			/// <remarks>
			/// You can use this to determine which flags in the <see cref="TextReplacements"/> will take effect.
			/// </remarks>
			/// <value>The supported text replacements.</value>
			TextReplacements SupportedTextReplacements { get; }

			/// <summary>
			/// Gets or sets the horizontal alignment of the text.
			/// </summary>
			/// <value>The horizontal alignment.</value>
			TextAlignment TextAlignment { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextArea"/> will perform spell checking.
			/// </summary>
			/// <remarks>
			/// When <c>true</c>, platforms will typically show misspelled or unknown words with a red underline.
			/// This is a hint, and is only supported by the platform when <see cref="SpellCheckIsSupported"/> is true.
			/// When not supported, setting this property will do nothing.
			/// </remarks>
			/// <value><c>true</c> if spell check; otherwise, <c>false</c>.</value>
			bool SpellCheck { get; set; }

			/// <summary>
			/// Gets a value indicating whether the <see cref="SpellCheck"/> property is supported on the control's platform.
			/// </summary>
			/// <value><c>true</c> if spell check is supported; otherwise, <c>false</c>.</value>
			bool SpellCheckIsSupported { get; }
		}
	}
}
