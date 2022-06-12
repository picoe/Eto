using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Enumeration of the modes for auto selecting text.
	/// </summary>
	public enum AutoSelectMode
	{
		/// <summary>
		/// Selects the text when the control recieves focus, unless the user
		/// clicks at a point in the text with the I beam cursor.
		/// </summary>
		OnFocus = 0,

		/// <summary>
		/// The text is never automatically selected.  When the text of the control is set
		/// to a different value, the cursor usually will be at the end of the text input.
		/// 
		/// The last selection of the control is also usually kept in this mode.
		/// </summary>
		Never = 1,

		/// <summary>
		/// Selects the text when the control recieves focus regardless of whether the user 
		/// clicked at a point in the text, or the last selection.
		/// 
		/// On macOS, if the user clicks and drags to select some text it will not select all text.
		/// </summary>
		Always = 2
	}

	/// <summary>
	/// Single line text box control
	/// </summary>
	/// <seealso cref="TextArea"/>
	[Handler(typeof(TextBox.IHandler))]
	public class TextBox : TextControl
	{
		static readonly object SuppressTextChanging_Key = new object();

		int SuppressTextChanging
		{
			get => Properties.Get<int>(SuppressTextChanging_Key);
			set => Properties.Set(SuppressTextChanging_Key, value);
		}

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		#region Events

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="TextChanging"/> event
		/// </summary>
		public const string TextChangingEvent = "TextBox.TextChanging";

		/// <summary>
		/// Event to handle before the text is changed to allow cancelling any change events triggered by the user.
		/// </summary>
		public event EventHandler<TextChangingEventArgs> TextChanging
		{
			add { Properties.AddHandlerEvent(TextChangingEvent, value); }
			remove { Properties.RemoveEvent(TextChangingEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="TextChanging"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnTextChanging(TextChangingEventArgs e)
		{
			Properties.TriggerEvent(TextChangingEvent, this, e);
		}

		#endregion

		static TextBox()
		{
			EventLookup.Register<TextBox>(c => c.OnTextChanging(null), TextBox.TextChangingEvent);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextBox"/> is read only.
		/// </summary>
		/// <remarks>
		/// A user can selected and copied text when the read only, however the user will not be able to change any of the text.
		/// This differs from the <see cref="Control.Enabled"/> property, which disables all user interaction.
		/// </remarks>
		/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		/// <summary>
		/// Gets or sets the maximum length of the text that can be entered in the control, 0 for no limit.
		/// </summary>
		/// <remarks>
		/// This typically does not affect the value set using <see cref="TextControl.Text"/>, only the limit of what the user can 
		/// enter into the control.
		/// </remarks>
		/// <value>The maximum length of the text in the control.</value>
		public int MaxLength
		{
			get { return Handler.MaxLength; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(value), "MaxLength must be greater or equal to zero.");
				Handler.MaxLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the placeholder text to show as a hint of what the user should enter.
		/// </summary>
		/// <remarks>
		/// Typically this will be shown when the control is blank, and will dissappear when the user enters text or if
		/// it has an existing value.
		/// </remarks>
		/// <value>The placeholder text.</value>
		public string PlaceholderText
		{
			get { return Handler.PlaceholderText; }
			set { Handler.PlaceholderText = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the control's border.
		/// </summary>
		/// <remarks>
		/// This is a hint to omit the border of the control and show it as plainly as possible.
		/// 
		/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
		/// </remarks>
		/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get { return Handler.ShowBorder; }
			set { Handler.ShowBorder = value; }
		}

		/// <summary>
		/// Gets or sets the alignment of the text in the entry box.
		/// </summary>
		/// <value>The text alignment.</value>
		public TextAlignment TextAlignment
		{
			get { return Handler.TextAlignment; }
			set { Handler.TextAlignment = value; }
		}

		/// <summary>
		/// Selects all of the text in the control.
		/// </summary>
		/// <remarks>
		/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
		/// </remarks>
		public void SelectAll()
		{
			Handler.SelectAll();
		}

		/// <summary>
		/// Gets or sets the index of the current insertion point.
		/// </summary>
		/// <remarks>
		/// When there is selected text, this is usually the start of the selection.
		/// </remarks>
		/// <value>The index of the current insertion point.</value>
		public int CaretIndex
		{
			get { return Handler.CaretIndex; }
			set { Handler.CaretIndex = value; }
		}

		/// <summary>
		/// Gets or sets the current text selection.
		/// </summary>
		/// <value>The text selection.</value>
		public Range<int> Selection
		{
			get { return Handler.Selection; }
			set { Handler.Selection = value; }
		}

		/// <summary>
		/// Gets or sets the selected text.
		/// </summary>
		/// <value>The selected text.</value>
		public string SelectedText
		{
			get
			{
				var selection = Selection;
				var len = selection.Length();
				return len > 0 ? Text.Substring(selection.Start, len) : string.Empty;
			}
			set
			{
				var selection = Selection;
				var len = selection.Length();
				if (len >= 0)
				{
					var text = Text;
					if (len > 0)
						text = text.Remove(selection.Start, len);
					text = text.Insert(selection.Start, value);
					Text = text;
				}
			}
		}

		/// <summary>
		/// Gets or sets the auto selection mode.
		/// </summary>
		/// <value>The auto selection mode.</value>
		public AutoSelectMode AutoSelectMode
		{
			get { return Handler.AutoSelectMode; }
			set { Handler.AutoSelectMode = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="TextBox"/>.
		/// </summary>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TextBox"/> is read only.
			/// </summary>
			/// <remarks>
			/// A user can selected and copied text when the read only, however the user will not be able to change any of the text.
			/// This differs from the <see cref="Control.Enabled"/> property, which disables all user interaction.
			/// </remarks>
			/// <value><c>true</c> if the control is read only; otherwise, <c>false</c>.</value>
			bool ReadOnly { get; set; }

			/// <summary>
			/// Gets or sets the maximum length of the text that can be entered in the control.
			/// </summary>
			/// <remarks>
			/// This typically does not affect the value set using <see cref="TextControl.Text"/>, only the limit of what the user can 
			/// enter into the control.
			/// </remarks>
			/// <value>The maximum length of the text in the control.</value>
			int MaxLength { get; set; }

			/// <summary>
			/// Selects all of the text in the control.
			/// </summary>
			/// <remarks>
			/// When setting the selection, the control will be focussed and the associated keyboard may appear on mobile platforms.
			/// </remarks>
			void SelectAll();

			/// <summary>
			/// Gets or sets the placeholder text to show as a hint of what the user should enter.
			/// </summary>
			/// <remarks>
			/// Typically this will be shown when the control is blank, and will dissappear when the user enters text or if
			/// it has an existing value.
			/// </remarks>
			/// <value>The placeholder text.</value>
			string PlaceholderText { get; set; }

			/// <summary>
			/// Gets or sets the index of the current insertion point.
			/// </summary>
			/// <remarks>
			/// When there is selected text, this is usually the start of the selection.
			/// </remarks>
			/// <value>The index of the current insertion point.</value>
			int CaretIndex { get; set; }

			/// <summary>
			/// Gets or sets the current text selection.
			/// </summary>
			/// <value>The text selection.</value>
			Range<int> Selection { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether to show the control's border.
			/// </summary>
			/// <remarks>
			/// This is a hint to omit the border of the control and show it as plainly as possible.
			/// 
			/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
			/// </remarks>
			/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
			bool ShowBorder { get; set; }

			/// <summary>
			/// Gets or sets the alignment of the text in the entry box.
			/// </summary>
			/// <value>The text alignment.</value>
			TextAlignment TextAlignment { get; set; }

			/// <summary>
			/// Gets or sets the auto selection mode.
			/// </summary>
			/// <value>The auto selection mode.</value>
			AutoSelectMode AutoSelectMode { get; set; }
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="TextBox"/> based controls
		/// </summary>
		public new interface ICallback : TextControl.ICallback
		{
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			void OnTextChanging(TextBox widget, TextChangingEventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="TextControl"/> based controls
		/// </summary>
		protected new class Callback : TextControl.Callback, ICallback
		{

			
			/// <summary>
			/// Raises the text changed event.
			/// </summary>
			public void OnTextChanging(TextBox widget, TextChangingEventArgs e)
			{
				if (widget.SuppressTextChanging == 0)
				{
					widget.SuppressTextChanging++;
					if (e.NeedsOldText)
						e.SetOldText(widget.Text);
					using (widget.Platform.Context)
						widget.OnTextChanging(e);
					widget.SuppressTextChanging--;
				}
			}
		}

		#endregion
	}
}
