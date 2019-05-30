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
	/// Interface for a masked text provider that can be used with <see cref="MaskedTextBox"/>.
	/// </summary>
	/// <remarks>
	/// This defines the interface that the <see cref="MaskedTextBox"/> uses when inserting, deleting, and clearing characters
	/// from a masked string.
	/// 
	/// This can be implemented by both variable and fixed masks (unlike the standard System.ComponentModel.MaskedTextProvider), 
	/// and provides a way to create your own completely custom masks.
	/// You can also use the <see cref="VariableMaskedTextProvider"/> as a base for custom masks to implement the default functionality.
	/// </remarks>
	public interface IMaskedTextProvider
	{
		/// <summary>
		/// Called to insert a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the insertion was successful, or <c>false</c> if it failed.</returns>
		bool Insert(char character, ref int position);

		/// <summary>
		/// Called to replace a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the replacement was successful, or <c>false</c> if it failed.</returns>
		bool Replace(char character, ref int position);

		/// <summary>
		/// Called to delete a range of characters at the specified position in the masked text.
		/// </summary>
		/// <param name="position">Position to delete at.</param>
		/// <param name="length">Length of text (in the mask) to delete</param> 
		/// <param name="forward"><c>true</c> to delete the text forward, or <c>false</c> to delete backward</param>
		/// <returns><c>true</c> when the deletion was successful, or <c>false</c> if it failed.</returns>
		bool Delete(ref int position, int length, bool forward);

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
		bool Clear(ref int position, int length, bool forward);

		/// <summary>
		/// Gets the display text, including prompt characters.
		/// </summary>
		/// <value>The display text.</value>
		string DisplayText { get; }

		/// <summary>
		/// Gets or sets the text, usually excluding prompt or literal characters depending on the mask provider.
		/// </summary>
		/// <value>The text.</value>
		string Text { get; set; }

		/// <summary>
		/// Gets a value indicating whether the mask has all required text to pass its validation.
		/// </summary>
		/// <value><c>true</c> if mask is completed; otherwise, <c>false</c>.</value>
		bool MaskCompleted { get; }

		/// <summary>
		/// Gets an enumeration of all valid edit positions in the mask.
		/// </summary>
		/// <value>The valid edit positions.</value>
		IEnumerable<int> EditPositions { get; }

		/// <summary>
		/// Gets a value indicating the mask is empty with no characters filled out.
		/// </summary>
		/// <value><c>true</c> if the mask value is empty; otherwise, <c>false</c>.</value>
		bool IsEmpty { get; }
	}

	/// <summary>
	/// Interface for a masked text provider that can be translated to a specific value.
	/// </summary>
	public interface IMaskedTextProvider<T> : IMaskedTextProvider
	{
		/// <summary>
		/// Gets or sets the translated value of the mask.
		/// </summary>
		/// <value>The value of the mask.</value>
		T Value { get; set; }
	}
}
