﻿using System;
using Eto.Drawing;
using System.IO;
using System.Collections.Generic;

namespace Eto.Forms
{
	/// <summary>
	/// Format for loading and saving text from the <see cref="RichTextArea"/>
	/// </summary>
	public enum RichTextAreaFormat
	{
		/// <summary>
		/// Standard Rich Text format
		/// </summary>
		Rtf,
		/// <summary>
		/// Plain Text only
		/// </summary>
		PlainText
	}

	/// <summary>
	/// Interface for a formatted text buffer
	/// </summary>
	public interface ITextBuffer
	{
		/// <summary>
		/// Sets the bold flag for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the font weight.</param>
		/// <param name="bold">If set to <c>true</c>, then text will be bold.</param>
		void SetBold(Range<int> range, bool bold);

		/// <summary>
		/// Sets the italic flag for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the italics.</param>
		/// <param name="italic">If set to <c>true</c>, then text will be italic.</param>
		void SetItalic(Range<int> range, bool italic);

		/// <summary>
		/// Sets the underline flag for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the underline.</param>
		/// <param name="underline">If set to <c>true</c>, then the text will be underline.</param>
		void SetUnderline(Range<int> range, bool underline);

		/// <summary>
		/// Sets the strikethrough for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the strikethrough.</param>
		/// <param name="strikethrough">If set to <c>true</c>, then the text will be strikethrough.</param>
		void SetStrikethrough(Range<int> range, bool strikethrough);

		/// <summary>
		/// Sets the font for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the font.</param>
		/// <param name="font">Font for the text in the range.</param>
		void SetFont(Range<int> range, Font font);

		/// <summary>
		/// Sets the foreground color for the specified text range.
		/// </summary>
		/// <param name="range">Range to set the foreground color.</param>
		/// <param name="color">Color to set the text foreground in the range.</param>
		void SetForeground(Range<int> range, Color color);

		/// <summary>
		/// Sets the background color for the specified range.
		/// </summary>
		/// <param name="range">Range to set the background color.</param>
		/// <param name="color">Color to set the text background in the range.</param>
		void SetBackground(Range<int> range, Color color);

		/// <summary>
		/// Gets an enumeration of formats supported for the <see cref="Load"/> and <see cref="Save"/> methods.
		/// </summary>
		/// <value>The supported formats for loading and saving.</value>
		IEnumerable<RichTextAreaFormat> SupportedFormats { get; }

		/// <summary>
		/// Loads the specified format from the stream, replacing the content of the buffer.
		/// </summary>
		/// <param name="stream">Stream to load from.</param>
		/// <param name="format">Format of the stream to load.</param>
		void Load(Stream stream, RichTextAreaFormat format);

		/// <summary>
		/// Saves the buffer into a stream with the specified format.
		/// </summary>
		/// <param name="stream">Stream to save to.</param>
		/// <param name="format">Format to save into the stream.</param>
		void Save(Stream stream, RichTextAreaFormat format);

		/// <summary>
		/// Clears the buffer of all text and formatting.
		/// </summary>
		void Clear();

		/// <summary>
		/// Deletes text from the specified range
		/// </summary>
		/// <param name="range">Range of the text to delete.</param>
		void Delete(Range<int> range);

		/// <summary>
		/// Inserts text with the format of the text at the specified position.
		/// </summary>
		/// <param name="position">Position to insert the text.</param>
		/// <param name="text">Text to insert.</param>
		void Insert(int position, string text);
	}

	/// <summary>
	/// Text area with ability to specify rich text formatting such as font attributes and colors.
	/// </summary>
	[Handler(typeof(IHandler))]
	public class RichTextArea : TextArea
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets the font of the selected text or insertion point.
		/// </summary>
		/// <value>The font of the selection.</value>
		public Font SelectionFont
		{
			get { return Handler.SelectionFont; }
			set { Handler.SelectionFont = value; }
		}

		/// <summary>
		/// Gets or sets the foreground color of the selected text or insertion point.
		/// </summary>
		/// <value>The foreground color of the selection.</value>
		public Color SelectionForeground
		{
			get { return Handler.SelectionForeground; }
			set { Handler.SelectionForeground = value; }
		}

		/// <summary>
		/// Gets or sets the background color of the selected text or insertion point.
		/// </summary>
		/// <value>The background color of the selection.</value>
		public Color SelectionBackground
		{
			get { return Handler.SelectionBackground; }
			set { Handler.SelectionBackground = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the selected text or insertion point has bold text.
		/// </summary>
		/// <value><c>true</c> if selected text is bold; otherwise, <c>false</c>.</value>
		public bool SelectionBold
		{
			get { return Handler.SelectionBold; }
			set { Handler.SelectionBold = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the selected text or insertion point has italic style.
		/// </summary>
		/// <value><c>true</c> if selected text is italic; otherwise, <c>false</c>.</value>
		public bool SelectionItalic
		{
			get { return Handler.SelectionItalic; }
			set { Handler.SelectionItalic = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the selected text or insertion point has underline decorations.
		/// </summary>
		/// <value><c>true</c> if selected text is underline; otherwise, <c>false</c>.</value>
		public bool SelectionUnderline
		{
			get { return Handler.SelectionUnderline; }
			set { Handler.SelectionUnderline = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the selected text or insertion point has strikethrough decorations.
		/// </summary>
		/// <value><c>true</c> if selected text is strikethrough; otherwise, <c>false</c>.</value>
		public bool SelectionStrikethrough
		{
			get { return Handler.SelectionStrikethrough; }
			set { Handler.SelectionStrikethrough = value; }
		}

		/// <summary>
		/// Gets the formatted text buffer to set formatting and load/save to file.
		/// </summary>
		/// <remarks>
		/// The text buffer allows you to control the formatting of the text.
		/// </remarks>
		/// <value>The text buffer.</value>
		public ITextBuffer Buffer
		{
			get { return Handler.Buffer; }
		}

		/// <summary>
		/// Handler interface for the <see cref="RichTextArea"/>.
		/// </summary>
		public new interface IHandler : TextArea.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether the selected text or insertion point has bold text.
			/// </summary>
			/// <value><c>true</c> if selected text is bold; otherwise, <c>false</c>.</value>
			bool SelectionBold { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the selected text or insertion point has italic style.
			/// </summary>
			/// <value><c>true</c> if selected text is italic; otherwise, <c>false</c>.</value>
			bool SelectionItalic { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the selected text or insertion point has underline decorations.
			/// </summary>
			/// <value><c>true</c> if selected text is underline; otherwise, <c>false</c>.</value>
			bool SelectionUnderline { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether the selected text or insertion point has strikethrough decorations.
			/// </summary>
			/// <value><c>true</c> if selected text is strikethrough; otherwise, <c>false</c>.</value>
			bool SelectionStrikethrough { get; set; }

			/// <summary>
			/// Gets or sets the font of the selected text or insertion point.
			/// </summary>
			/// <value>The font of the selection.</value>
			Font SelectionFont { get; set; }

			/// <summary>
			/// Gets or sets the foreground color of the selected text or insertion point.
			/// </summary>
			/// <value>The foreground color of the selection.</value>
			Color SelectionForeground { get; set; }

			/// <summary>
			/// Gets or sets the background color of the selected text or insertion point.
			/// </summary>
			/// <value>The background color of the selection.</value>
			Color SelectionBackground { get; set; }

			/// <summary>
			/// Gets the formatted text buffer to set formatting and load/save to file.
			/// </summary>
			/// <remarks>
			/// The text buffer allows you to control the formatting of the text.
			/// </remarks>
			/// <value>The text buffer.</value>
			ITextBuffer Buffer { get; }
		}
	}
}

