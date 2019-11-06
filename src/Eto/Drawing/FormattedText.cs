using System.ComponentModel;

namespace Eto.Drawing
{
	/// <summary>
	/// Trimming mode for the <see cref="FormattedText"/>.
	/// </summary>
	public enum FormattedTextTrimming
	{
		/// <summary>
		/// No trimming. Text will be clipped to the <see cref="FormattedText.MaximumSize"/>.
		/// </summary>
		None,
		/// <summary>
		/// Show an ellipsis after the last visible character, but not extending beyond the maximum size.
		/// </summary>
		CharacterEllipsis,
		/// <summary>
		/// Specify that the ellipsis should be shown after the last complete word
		/// </summary>
		/// <remarks>
		/// Note that some platforms may not support this and fallback to character ellipsis (E.g. Mac and Gtk).
		/// </remarks>
		WordEllipsis
	}

	/// <summary>
	/// Wrap mode for the <see cref="FormattedText"/>.
	/// </summary>
	public enum FormattedTextWrapMode
	{
		/// <summary>
		/// No wrapping.  Text will be clipped to the <see cref="FormattedText.MaximumWidth"/>.
		/// </summary>
		None,
		/// <summary>
		/// Wrap on the word boundaries.
		/// </summary>
		Word,
		/// <summary>
		/// Wrap text at the character boundaries.
		/// </summary>
		/// <remarks>
		/// Note that some platforms may not support this and will fallback to word wrapping (E.g. Wpf).
		/// </remarks>
		Character
	}

	/// <summary>
	/// Alignment mode for the <see cref="FormattedText"/>
	/// </summary>
	public enum FormattedTextAlignment
	{
		/// <summary>
		/// Left justified text
		/// </summary>
		Left,
		/// <summary>
		/// Right justified text
		/// </summary>
		Right,
		/// <summary>
		/// Center justified text
		/// </summary>
		Center,
		/// <summary>
		/// Justified to both the right and left.
		/// </summary>
		Justify
	}

	/// <summary>
	/// Low level object that provides drawing text with formatting.
	/// </summary>
	[Handler(typeof(IHandler))]
    public class FormattedText : Widget
    {
        new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets the wrapping mode.
		/// </summary>
		/// <remarks>
		/// Note that not all platforms support all modes and may fall back to a different mode.
		/// </remarks>
        public FormattedTextWrapMode Wrap
        {
            get => Handler.Wrap;
            set => Handler.Wrap = value;
        }

		/// <summary>
		/// Gets or sets the trimming mode.
		/// </summary>
		/// <remarks>
		/// Note that not all platforms support all modes and may fall back to a different mode.
		/// </remarks>
		public FormattedTextTrimming Trimming
        {
            get => Handler.Trimming;
            set => Handler.Trimming = value;
        }

		/// <summary>
		/// Gets or sets how the text should be aligned to <see cref="MaximumSize"/>
		/// </summary>
        public FormattedTextAlignment Alignment
        {
            get => Handler.Alignment;
            set => Handler.Alignment = value;
        }

		/// <summary>
		/// Gets or sets the font to use to draw the text.
		/// </summary>
        public Font Font
        {
            get => Handler.Font;
            set => Handler.Font = value;
        }

		/// <summary>
		/// Gets or sets the text to display
		/// </summary>
        public string Text
        {
            get => Handler.Text;
            set => Handler.Text = value;
        }

		/// <summary>
		/// Gets or sets the maximum width of the text.
		/// </summary>
		/// <remarks>
		/// The width is used when wrapping, trimming, and aligning the text.
		/// </remarks>
		[DefaultValue(float.PositiveInfinity)]
        public float MaximumWidth
        {
            get => MaximumSize.Width;
            set => MaximumSize = new SizeF(value, MaximumSize.Height);
        }

		/// <summary>
		/// Gets or sets the maximum height of the text.
		/// </summary>
		/// <remarks>
		/// The height is used when trimming the text. Only the last full visible line will be visible.
		/// </remarks>
		[DefaultValue(float.PositiveInfinity)]
		public float MaximumHeight
        {
            get => MaximumSize.Height;
            set => MaximumSize = new SizeF(MaximumSize.Width, value);
        }

		/// <summary>
		/// Gets or sets the maximum size of the text.
		/// </summary>
		/// <remarks>
		/// The width is used when wrapping, trimming, and aligning the text.
		/// The height is used when trimming the text. Only the last full visible line will be visible.
		/// </remarks>
		public SizeF MaximumSize
        {
            get => Handler.MaximumSize;
            set => Handler.MaximumSize = value;
        }

		/// <summary>
		/// Gets or sets the brush to use to draw the text.
		/// </summary>
        public Brush ForegroundBrush
        {
            get => Handler.ForegroundBrush;
            set => Handler.ForegroundBrush = value;
        }

		/* Future enhancement... 
		public int MaximumLineCount
		{
			get => Handler.MaximumLineCount;
			set => Handler.MaximumLineCount = value;
		}
		*/

		/// <summary>
		/// Gets the size needed to draw the text.
		/// </summary>
		/// <remarks>
		/// This may be smaller than the <see cref="MaximumSize"/>.
		/// </remarks>
		/// <returns>The size of the rectangle needed to draw the text</returns>
        public SizeF Measure() => Handler.Measure();

		/// <summary>
		/// Handler for implementations of the <see cref="FormattedText"/> class.
		/// </summary>
        public new interface IHandler : Widget.IHandler
        {
			/// <summary>
			/// Gets or sets the wrapping mode.
			/// </summary>
			/// <remarks>
			/// Note that not all platforms support all modes and may fall back to a different mode.
			/// </remarks>
			FormattedTextWrapMode Wrap { get; set; }
			/// <summary>
			/// Gets or sets the trimming mode.
			/// </summary>
			/// <remarks>
			/// Note that not all platforms support all modes and may fall back to a different mode.
			/// </remarks>
			FormattedTextTrimming Trimming { get; set; }
			/// <summary>
			/// Gets or sets how the text should be aligned to <see cref="MaximumSize"/>
			/// </summary>
			FormattedTextAlignment Alignment { get; set; }
			/// <summary>
			/// Gets or sets the font to use to draw the text.
			/// </summary>
			Font Font { get; set; }
			/// <summary>
			/// Gets or sets the text to display
			/// </summary>
			string Text { get; set; }
			/// <summary>
			/// Gets or sets the maximum size of the text.
			/// </summary>
			/// <remarks>
			/// The width is used when wrapping, trimming, and aligning the text.
			/// The height is used when trimming the text. Only the last full visible line will be visible.
			/// </remarks>
			SizeF MaximumSize { get; set; }
			/// <summary>
			/// Gets or sets the brush to use to draw the text.
			/// </summary>
			Brush ForegroundBrush { get; set; }
			//int MaximumLineCount { get; set; }

			/// <summary>
			/// Gets the size needed to draw the text.
			/// </summary>
			/// <remarks>
			/// This may be smaller than the <see cref="MaximumSize"/>.
			/// </remarks>
			/// <returns>The size of the rectangle needed to draw the text</returns>
			SizeF Measure();
        }
    }
}
