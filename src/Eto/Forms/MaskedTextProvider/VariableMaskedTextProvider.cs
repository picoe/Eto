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
	/// Base masked text provider that can be used for variable length masks.
	/// </summary>
	public abstract class VariableMaskedTextProvider : IMaskedTextProvider
	{
		readonly StringBuilder sb = new StringBuilder();

		/// <summary>
		/// Gets the underlying string builder for the current mask text value.
		/// </summary>
		/// <value>The string builder builder.</value>
		protected StringBuilder Builder { get { return sb; } }

		/// <summary>
		/// Called to insert a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the insertion was successful, or <c>false</c> if it failed.</returns>
		public virtual bool Insert(char character, ref int position)
		{
			sb.Insert(position, character);
			position++;
			return true;
		}

		/// <summary>
		/// Called to replace a character at the specified position in the masked text.
		/// </summary>
		/// <param name="character">Character to insert.</param>
		/// <param name="position">Position to insert at.</param>
		/// <returns><c>true</c> when the replacement was successful, or <c>false</c> if it failed.</returns>
		public virtual bool Replace(char character, ref int position)
		{
			if (position >= sb.Length)
				return Insert(character, ref position);
			sb[position] = character;
			position++;
			return true;
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
			if (sb.Length == 0)
				return false;
			if (forward)
			{
				length = Math.Min(length, sb.Length - position);
				sb.Remove(position, length);
			}
			else if (position >= length)
			{
				sb.Remove(position - length, length);
				position = Math.Max(0, position - length);
			}
			return true;
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
		public virtual bool Clear(ref int position, int length, bool forward)
		{
			return Delete(ref position, length, forward);
		}

		/// <summary>
		/// Gets a value indicating whether the mask has all required text to pass its validation.
		/// </summary>
		/// <value><c>true</c> if mask is completed; otherwise, <c>false</c>.</value>
		public virtual bool MaskCompleted
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the display text, including prompt characters.
		/// </summary>
		/// <value>The display text.</value>
		public virtual string DisplayText
		{
			get { return sb.ToString(); }
		}

		/// <summary>
		/// Gets or sets the text, usually excluding prompt or literal characters depending on the mask provider.
		/// </summary>
		/// <value>The text.</value>
		public virtual string Text
		{
			get { return sb.ToString(); }
			set
			{
				sb.Clear();
				if (value != null)
				{
					int pos = 0;
					foreach (char ch in value)
					{
						Insert(ch, ref pos);
					}
				}
			}
		}

		/// <summary>
		/// Gets an enumeration of all valid edit positions in the mask.
		/// </summary>
		/// <value>The valid edit positions.</value>
		public virtual IEnumerable<int> EditPositions
		{
			get
			{
				for (int i = 0; i <= sb.Length; i++)
				{
					yield return i;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the mask is empty with no characters filled out.
		/// </summary>
		/// <value><c>true</c> if the mask value is empty; otherwise, <c>false</c>.</value>
		public virtual bool IsEmpty
		{
			get { return sb.Length == 0; }
		}
	}
	
}
