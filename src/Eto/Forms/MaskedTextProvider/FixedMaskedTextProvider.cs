using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Eto.Forms
{
	/// <summary>
	/// Implements a fixed masked text provider that converts to the specified type of <typeparamref name="T"/>, using the standard System.ComponentModel.MaskedTextProvider.
	/// </summary>
	public class FixedMaskedTextProvider<T> : FixedMaskedTextProvider, IMaskedTextProvider<T>
	{
		/// <summary>
		/// Gets or sets a delegate to convert the mask string to a value of type <typeparamref name="T"/>
		/// </summary>
		/// <value>The delegate to convert text to the value.</value>
		public Func<string, T> ConvertToValue { get; set; }

		/// <summary>
		/// Gets or sets a delegate to convert the a value of type <typeparamref name="T"/> to the mask string.
		/// </summary>
		/// <value>The delegate to convert the value to text.</value>
		public Func<T, string> ConvertToText { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FixedMaskedTextProvider{T}"/> class.
		/// </summary>
		/// <param name="mask">Mask for the input. See <see cref="FixedMaskedTextProvider.Mask"/> for mask format.</param>
		/// <param name="culture">Culture to format date/time separators and numeric placeholders.</param>
		/// <param name="allowPromptAsInput">If set to <c>true</c>, then allow the <see cref="FixedMaskedTextProvider.PromptChar"/> as valid input.</param>
		/// <param name="restrictToAscii">If set to <c>true</c>, restrict input characters to ASCII only (a-z or A-Z).</param>
		public FixedMaskedTextProvider(string mask, CultureInfo culture = null, bool allowPromptAsInput = true, bool restrictToAscii = false)
			: base(mask, culture, allowPromptAsInput, restrictToAscii)
		{
		}

		/// <summary>
		/// Gets or sets the translated value of the mask.
		/// </summary>
		/// <value>The value of the mask.</value>
		public T Value
		{
			get { return ConvertToValue != null ? ConvertToValue(Text) : default(T); }
			set { Text = ConvertToText != null ? ConvertToText(value) : Convert.ToString(value); }
		}
	}

	/// <summary>
	/// Implements a fixed masked text provider, using the standard System.ComponentModel.MaskedTextProvider.
	/// </summary>
	/// <remarks>
	/// This wraps the standard provider in an interface used by the <see cref="MaskedTextBox"/> so that we can provide
	/// different implementations of masked text providers.
	/// 
	/// The implementation of this is defined in each platform assembly so that we can use this from a PCL assembly.
	/// </remarks>
	public class FixedMaskedTextProvider : IMaskedTextProvider
	{
		readonly IHandler handler;

		/// <summary>
		/// Gets the culture for the mask, as specified in the constructor.
		/// </summary>
		/// <value>The culture for the mask.</value>
		public CultureInfo Culture
		{
			get { return handler.Culture; }
		}

		/// <summary>
		/// Gets the mask for this provider, as specified in the constructor.
		/// </summary>
		/// <remarks>
		/// The mask format can consist of the following characters:
		/// 
		/// 0 - Required digit from 0-9.
		/// 9 - Optional digit or space.
		/// # - Optional digit, space, or sign (+/-).  If blank, then it is output as a space in the Text value.
		/// L - Required upper or lowercase letter.
		/// ? - Optional upper or lowercase letter.
		/// &amp; - Required character. If <see cref="AsciiOnly"/> is true, then behaves like L.
		/// C - Optional character. If <see cref="AsciiOnly"/> is true, then behaves like ?.
		/// A - Required alphanumeric character. If <see cref="AsciiOnly"/> is true, then behaves like L.
		/// a - Optional alphanumeric. If <see cref="AsciiOnly"/> is true, then behaves like ?.
		/// . - Decimal placeholder based on the specified <see cref="Culture"/> for the mask.
		/// , - Thousands placeholder based on the specified <see cref="Culture"/> for the mask.
		/// : - Time separator based on the specified <see cref="Culture"/> for the mask.
		/// / - Date separator based on the specified <see cref="Culture"/> for the mask.
		/// $ - Currency symbol based on the specified <see cref="Culture"/> for the mask.
		/// &lt; - Shift all characters that follow to lower case.
		/// &gt; - Shift all characters that follow to upper case.
		/// | - Disables a previous shift to upper or lower case.
		/// \ - Escape the following character into a literal.
		/// All other characters are treated as literal and cannot be moved or deleted.
		/// </remarks>
		/// <value>The text mask.</value>
		public string Mask
		{
			get { return handler.Mask; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="PromptChar"/> can be a valid input character by the user.
		/// </summary>
		/// <value><c>true</c> to allow prompt as input; otherwise, <c>false</c>.</value>
		public bool AllowPromptAsInput
		{
			get { return handler.AllowPromptAsInput; }
		}

		/// <summary>
		/// Gets or sets the character to show for each unfilled edit position in the mask.
		/// </summary>
		/// <value>The prompt character.</value>
		[DefaultValue('_')]
		public char PromptChar
		{
			get { return handler.PromptChar; }
			set { handler.PromptChar = value; }
		}

		/// <summary>
		/// In password mode, gets or sets the character to show for filled edit characters in the mask.
		/// </summary>
		/// <value>The password character.</value>
		public char PasswordChar
		{
			get { return handler.PasswordChar; }
			set { handler.PasswordChar = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the input only accepts ascii characters A-Z or a-z.
		/// </summary>
		/// <value><c>true</c> if only ASCII characters are accepted; otherwise, <c>false</c>.</value>
		public bool AsciiOnly
		{
			get { return handler.AsciiOnly; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.FixedMaskedTextProvider"/> class.
		/// </summary>
		/// <param name="mask">Mask for the input. See <see cref="Mask"/> for mask format.</param>
		/// <param name="culture">Culture to format date/time separators and numeric placeholders.</param>
		/// <param name="allowPromptAsInput">If set to <c>true</c>, then allow the <see cref="PromptChar"/> as valid input.</param>
		/// <param name="restrictToAscii">If set to <c>true</c>, restrict input characters to ASCII only (a-z or A-Z).</param>
		public FixedMaskedTextProvider(string mask, CultureInfo culture = null, bool allowPromptAsInput = true, bool restrictToAscii = false)
		{
			handler = Platform.Instance.Create<IHandler>();
			handler.Create(mask, culture, allowPromptAsInput, restrictToAscii);
		}

		/// <summary>
		/// Called to insert a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the insertion was successful, or <c>false</c> if it failed.</returns>
		public virtual bool Insert(char character, ref int position)
		{
			return handler.Insert(character, ref position);
		}

		/// <summary>
		/// Called to replace a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the replacement was successful, or <c>false</c> if it failed.</returns>
		public virtual bool Replace(char character, ref int position)
		{
			return handler.Replace(character, ref position);
		}

		/// <summary>
		/// Called to delete a range of characters at the specified position in the masked text.
		/// </summary>
		/// <param name="position">Position to delete at.</param>
		/// <param name="length">Length of text (in the mask) to delete</param> 
		/// <param name="forward"><c>true</c> to delete the text forward, or <c>false</c> to delete backward</param>
		/// <returns><c>true</c> when the deletion was successful, or <c>false</c> if it failed.</returns>
		public virtual bool Delete(ref int position, int length, bool forward)
		{
			return handler.Delete(ref position, length, forward);
		}

		/// <summary>
		/// Called to clear a range of characters at the specified position in the masked text.
		/// </summary>
		/// <remarks>
		/// The cleared characters usually show the prompt character after cleared.
		/// This is useful for fixed length mask providers.  For variable length, this is usually the same
		/// as calling <see cref="Delete"/>.
		/// </remarks>
		/// <param name="position">Position to clear at.</param>
		/// <param name="length">Length of text (in the mask) to clear</param> 
		/// <param name="forward"><c>true</c> to delete the text forward, or <c>false</c> to delete backward</param>
		/// <returns><c>true</c> when the deletion was successful, or <c>false</c> if it failed.</returns>
		public bool Clear(ref int position, int length, bool forward)
		{
			return handler.Clear(ref position, length, forward);
		}

		/// <summary>
		/// Gets the display text, including prompt characters.
		/// </summary>
		/// <value>The display text.</value>
		public virtual string DisplayText
		{
			get { return handler.DisplayText; }
		}

		/// <summary>
		/// Gets or sets the text, usually excluding prompt or literal characters depending on the mask provider.
		/// </summary>
		/// <value>The text value for the mask.</value>
		public virtual string Text
		{
			get { return handler.Text; }
			set { handler.Text = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the mask has all required text to pass its validation.
		/// </summary>
		public virtual bool MaskCompleted
		{
			get { return handler.MaskCompleted; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the mask should be password protected.
		/// </summary>
		/// <value><c>true</c> if this instance is password; otherwise, <c>false</c>.</value>
		public bool IsPassword
		{
			get { return handler.IsPassword; }
			set { handler.IsPassword = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the <see cref="Text"/> property includes literals in the mask.
		/// </summary>
		/// <value><c>true</c> to include literals; otherwise, <c>false</c>.</value>
		public bool IncludeLiterals
		{
			get { return handler.IncludeLiterals; }
			set { handler.IncludeLiterals = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the <see cref="Text"/> property includes prompt characters for each
		/// edit position in the mask.
		/// </summary>
		/// <value><c>true</c> to include prompt; otherwise, <c>false</c>.</value>
		public bool IncludePrompt
		{
			get { return handler.IncludePrompt; }
			set { handler.IncludePrompt = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the user can type literals to skip them in the mask.
		/// </summary>
		/// <value><c>true</c> to allow the user to enter literals to skip them; otherwise, <c>false</c>.</value>
		public bool SkipLiterals
		{
			get { return handler.SkipLiterals; }
			set { handler.SkipLiterals = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating that the cursor position will advance past literals to the next available 
		/// edit position automatically.
		/// </summary>
		/// <value><c>true</c> to auto advance past literals; otherwise, <c>false</c>.</value>
		public bool AutoAdvance
		{
			get { return handler.AutoAdvance; }
			set { handler.AutoAdvance = value; }
		}

		/// <summary>
		/// Gets an enumeration of all valid edit positions in the mask.
		/// </summary>
		/// <value>The valid edit positions.</value>
		public IEnumerable<int> EditPositions
		{
			get { return handler.EditPositions; }
		}

		/// <summary>
		/// Gets a value indicating the mask is empty with no characters filled out.
		/// </summary>
		public virtual bool IsEmpty
		{
			get { return handler.IsEmpty; }
		}

		/// <summary>
		/// Gets a value indicating that all available edit positions in the mask have been filled out.
		/// </summary>
		/// <value><c>true</c> if mask the mask is full; otherwise, <c>false</c>.</value>
		public virtual bool MaskFull
		{
			get { return handler.MaskFull; }
		}

		/// <summary>
		/// Handler interface for implementations of the <see cref="FixedMaskedTextProvider"/>.
		/// </summary>
		public interface IHandler : IMaskedTextProvider
		{
			/// <summary>
			/// Called when a new instance of the <see cref="FixedMaskedTextProvider"/> is created
			/// </summary>
			/// <param name="mask">Mask for the input. See <see cref="Mask"/> for mask format.</param>
			/// <param name="culture">Culture to format date/time separators and numeric placeholders.</param>
			/// <param name="allowPromptAsInput">If set to <c>true</c>, then allow the <see cref="PromptChar"/> as valid input.</param>
			/// <param name="restrictToAscii">If set to <c>true</c>, restrict input characters to ASCII only (a-z or A-Z).</param>
			void Create(string mask, CultureInfo culture, bool allowPromptAsInput, bool restrictToAscii);

			/// <summary>
			/// Gets the culture for the mask, as specified in the constructor.
			/// </summary>
			/// <value>The culture for the mask.</value>
			CultureInfo Culture { get; }

			/// <summary>
			/// Gets the mask for this provider, as specified in the constructor.
			/// </summary>
			string Mask { get; }

			/// <summary>
			/// Gets a value indicating whether the <see cref="PromptChar"/> can be a valid input character by the user.
			/// </summary>
			/// <value><c>true</c> to allow prompt as input; otherwise, <c>false</c>.</value>
			bool AllowPromptAsInput { get; }

			/// <summary>
			/// Gets a value indicating whether the input only accepts ascii characters A-Z or a-z.
			/// </summary>
			/// <value><c>true</c> if only ASCII characters are accepted; otherwise, <c>false</c>.</value>
			bool AsciiOnly { get; }

			/// <summary>
			/// Gets or sets a value indicating whether the mask should be password protected.
			/// </summary>
			/// <value><c>true</c> if this instance is password; otherwise, <c>false</c>.</value>
			bool IsPassword { get; set; }

			/// <summary>
			/// Gets or sets the character to show for each unfilled edit position in the mask.
			/// </summary>
			/// <value>The prompt character.</value>
			char PromptChar { get; set; }

			/// <summary>
			/// In password mode, gets or sets the character to show for filled edit characters in the mask.
			/// </summary>
			/// <value>The password character.</value>
			char PasswordChar { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that the <see cref="Text"/> property includes literals in the mask.
			/// </summary>
			/// <value><c>true</c> to include literals; otherwise, <c>false</c>.</value>
			bool IncludeLiterals { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that the <see cref="Text"/> property includes prompt characters for each
			/// edit position in the mask.
			/// </summary>
			/// <value><c>true</c> to include prompt; otherwise, <c>false</c>.</value>
			bool IncludePrompt { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that the user can type literals to skip them in the mask.
			/// </summary>
			/// <value><c>true</c> to allow the user to enter literals to skip them; otherwise, <c>false</c>.</value>
			bool SkipLiterals { get; set; }

			/// <summary>
			/// Gets or sets a value indicating that the cursor position will advance past literals to the next available 
			/// edit position automatically.
			/// </summary>
			/// <value><c>true</c> to auto advance past literals; otherwise, <c>false</c>.</value>
			bool AutoAdvance { get; set; }

			/// <summary>
			/// Gets a value indicating that all available edit positions in the mask have been filled out.
			/// </summary>
			/// <value><c>true</c> if mask the mask is full; otherwise, <c>false</c>.</value>
			bool MaskFull { get; }
		}
	}
	
}
