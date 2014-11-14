using System;

namespace Eto.Forms
{
	/// <summary>
	/// Single line text box control
	/// </summary>
	/// <seealso cref="TextArea"/>
	[Handler(typeof(TextBox.IHandler))]
	public class TextBox : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBox"/> class.
		/// </summary>
		public TextBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public TextBox(Generator generator)
			: this(generator, typeof(IHandler))
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TextBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TextBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
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
		/// Gets or sets the maximum length of the text that can be entered in the control.
		/// </summary>
		/// <remarks>
		/// This typically does not affect the value set using <see cref="TextControl.Text"/>, only the limit of what the user can 
		/// enter into the control.
		/// </remarks>
		/// <value>The maximum length of the text in the control.</value>
		public int MaxLength
		{
			get { return Handler.MaxLength; }
			set { Handler.MaxLength = value; }
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
		}
	}
}
