using System;
using Eto.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Eto.Shared.Forms
{
	public class FixedMaskedTextProviderHandler : FixedMaskedTextProvider.IHandler
	{
		public MaskedTextProvider Provider { get; private set; }

		public void Create(string mask, CultureInfo culture, bool allowPromptAsInput, bool restrictToAscii)
		{
			Provider = new MaskedTextProvider(mask, culture, allowPromptAsInput, '_', (char)0, restrictToAscii);
		}

		public bool Insert(char character, ref int position)
		{
			int testPosition;
			MaskedTextResultHint resultHint;
			if (Provider.InsertAt(character, position, out testPosition, out resultHint))
			{
				if (testPosition >= 0)
				{
					position = Math.Min(Provider.Length, testPosition + 1);
					if (AutoAdvance)
						position = AdvanceToNextEditPosition(position);
				}
				return true;
			}
			return false;
		}

		int AdvanceToNextEditPosition(int position)
		{
			var newPos = Provider.FindEditPositionFrom(position, true);
			if (newPos >= 0)
				position = newPos;
			return position;
		}

		public bool Delete(ref int position, int length, bool forward)
		{
			if (!forward && position == 0)
				return false;
			var pos = position;
			if (!forward)
				pos--;
			var endPos = pos + length - 1;
			pos = Provider.FindEditPositionFrom(pos, forward);
			if (pos >= 0 && Provider.RemoveAt(pos, endPos))
			{
				position = pos;
				if (!SkipLiterals)
				{
					position = Provider.FindEditPositionFrom(pos, forward);
				}
				return true;
			}
			return false;
		}

		public bool Clear(ref int position, int length, bool forward)
		{
			var oldReset = Provider.ResetOnPrompt;
			Provider.ResetOnPrompt = true;
			var pos = position;
			if (!forward)
				pos--;
			int testPosition;
			MaskedTextResultHint resultHint;
			var endPos = pos + length - 1;
			pos = Provider.FindEditPositionFrom(pos, forward);
			var count = EditPositions.Count(r => r >= pos && r <= endPos);

			var success = Provider.Replace(new String(Provider.PromptChar, count), pos, endPos, out testPosition, out resultHint);
			if (success)
				position = pos;
			Provider.ResetOnPrompt = oldReset;
			return success;
		}

		public string DisplayText
		{
			get { return Provider.ToDisplayString(); }
		}

		public string Text
		{
			get
			{
				return Provider.ToString();
			}
			set
			{
				Provider.Set(value ?? string.Empty);
			}
		}

		public bool IsMaskComplete
		{
			get { return Provider.MaskCompleted; }
		}

		public bool IncludeLiterals
		{
			get { return Provider.IncludeLiterals; }
			set { Provider.IncludeLiterals = value; }
		}
		public bool IncludePrompt
		{
			get { return Provider.IncludePrompt; }
			set { Provider.IncludePrompt = value; }
		}

		public CultureInfo Culture
		{
			get { return Provider.Culture; }
		}

		public string Mask
		{
			get { return Provider.Mask; }
		}

		public bool AllowPromptAsInput
		{
			get { return Provider.AllowPromptAsInput; }
		}

		public bool AsciiOnly
		{
			get { return Provider.AsciiOnly; }
		}

		public bool IsPassword
		{
			get { return Provider.IsPassword; }
			set { Provider.IsPassword = value; }
		}

		public char PromptChar
		{
			get { return Provider.PromptChar; }
			set { Provider.PromptChar = value; }
		}

		public char PasswordChar
		{
			get { return Provider.PasswordChar; }
			set { Provider.PasswordChar = value; }
		}

		public bool MaskCompleted
		{
			get { return Provider.MaskCompleted; }
		}

		public bool SkipLiterals
		{
			get { return Provider.SkipLiterals; }
			set { Provider.SkipLiterals = value; }
		}

		public bool AutoAdvance { get; set; }

		public bool Replace(char character, ref int position)
		{
			int testPosition;
			MaskedTextResultHint resultHint;
			if (Provider.Replace(character, position, out testPosition, out resultHint))
			{
				if (testPosition >= 0)
				{
					position = Math.Min(Provider.Length, testPosition + 1);
					if (AutoAdvance)
						position = AdvanceToNextEditPosition(position);
				}
				return true;
			}
			return false;
		}

		public System.Collections.Generic.IEnumerable<int> EditPositions
		{
			get
			{
				var e = Provider.EditPositions;
				while (e.MoveNext())
				{
					yield return (int)e.Current;
				}
			}
		}

		public bool IsEmpty
		{
			get { return Provider.AssignedEditPositionCount == 0; }
		}

		public bool MaskFull
		{
			get { return Provider.MaskFull; }
		}
	}
}

