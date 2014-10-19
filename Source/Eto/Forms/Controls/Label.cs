using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Specifies the horizontal alignment for a <see cref="Label"/>
	/// </summary>
	public enum HorizontalAlign
	{
		/// <summary>
		/// Text will be aligned to the left
		/// </summary>
		Left,
		/// <summary>
		/// Text will be aligned in the center of the label
		/// </summary>
		Center,
		/// <summary>
		/// Text will be aligned to the right
		/// </summary>
		Right
	}

	/// <summary>
	/// Specifies the vertical alignment for a <see cref="Label"/>
	/// </summary>
	public enum VerticalAlign
	{
		/// <summary>
		/// Text will be aligned to the top of the label
		/// </summary>
		Top,
		/// <summary>
		/// Text will be aligned to the middle of the label
		/// </summary>
		Middle,
		/// <summary>
		/// Text will be aligned to the bottom of the label
		/// </summary>
		Bottom
	}

	/// <summary>
	/// Specifies the wrapping mode for the text of a <see cref="Label"/>
	/// </summary>
	/// <remarks>
	/// Regardless of the mode, you can always add hard wraps by inserting newline characters.
	/// </remarks>
	public enum WrapMode
	{
		/// <summary>
		/// No wrapping, the text will clip when smaller than the required space for the text.
		/// </summary>
		None,
		/// <summary>
		/// Text will wrap by word to fit the horizontal space available
		/// </summary>
		Word,
		/// <summary>
		/// Text will wrap by character to fit the horizontal space available
		/// </summary>
		Character
	}

	/// <summary>
	/// Displays a string of text on the form
	/// </summary>
	[Handler(typeof(Label.IHandler))]
	public class Label : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Label"/> class.
		/// </summary>
		public Label()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Label"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public Label(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Label"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected Label(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the wrap mode for the text
		/// </summary>
		/// <remarks>
		/// This defines the soft wrapping for the label's text. 
		/// Hard wraps can be placed in the text with newline characters.
		/// 
		/// Wrapping will only occur if the label's width is smaller than the space needed for the text.
		/// If you are autosizing your control, it may get autosized to the width so you will have to add constraints
		/// to the container or explicitly set the size.
		/// </remarks>
		/// <value>The wrapping mode for the text.</value>
		public WrapMode Wrap
		{
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The horizontal alignment.</value>
		public HorizontalAlign HorizontalAlign
		{
			get { return Handler.HorizontalAlign; }
			set { Handler.HorizontalAlign = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the text.
		/// </summary>
		/// <remarks>
		/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
		/// </remarks>
		/// <value>The vertical alignment.</value>
		public VerticalAlign VerticalAlign
		{
			get { return Handler.VerticalAlign; }
			set { Handler.VerticalAlign = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="Label"/>
		/// </summary>
		public new interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets the horizontal alignment of the text.
			/// </summary>
			/// <remarks>
			/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
			/// </remarks>
			/// <value>The horizontal alignment.</value>
			HorizontalAlign HorizontalAlign { get; set; }

			/// <summary>
			/// Gets or sets the vertical alignment of the text.
			/// </summary>
			/// <remarks>
			/// When auto sizing the label, this won't have an affect unless the label's container is larger than the text.
			/// </remarks>
			/// <value>The vertical alignment.</value>
			VerticalAlign VerticalAlign { get; set; }

			/// <summary>
			/// Gets or sets the wrap mode for the text
			/// </summary>
			/// <remarks>
			/// This defines the soft wrapping for the label's text. 
			/// Hard wraps can be placed in the text with newline characters.
			/// 
			/// Wrapping will only occur if the label's width is smaller than the space needed for the text.
			/// If you are autosizing your control, it may get autosized to the width so you will have to add constraints
			/// to the container or explicitly set the size.
			/// </remarks>
			/// <value>The wrapping mode for the text.</value>
			WrapMode Wrap { get; set; }
		}
	}
}
