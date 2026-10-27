using NUnit.Framework;
using Range = Eto.Forms.Range;

namespace Eto.Test.UnitTests.Forms.Controls
{
	/// <summary>
	/// Reproduces a ghost-completion (inline autocomplete) typing bug on Gtk.
	///
	/// A ghost-completion input, modelled by <see cref="MiniAutoComplete"/> below,
	/// handles TextChanged: it finds the best candidate for the typed prefix and
	/// writes that candidate's full text into the TextBox, keeping the caret after
	/// the typed prefix and selecting the not-yet-typed suffix (the "ghost").
	///
	/// On Gtk, typing a character while that suffix is selected fires the native
	/// Changed signal twice per keystroke (once after the selection is deleted, once
	/// after the character is inserted). Because the completion runs synchronously
	/// inside the first (post-delete) TextChanged and re-writes Text + re-selects the
	/// suffix, the second (insert) phase operates on scrambled state and the typed
	/// character is lost -- so the first character completes fine but the second
	/// appears to do nothing. On WPF/Mac the keystroke fires TextChanged once and it
	/// works.
	///
	/// This is a manual test because a programmatic Text/SelectedText/Selection write
	/// goes through Gtk's guarded Text setter (DisableTextChanged), which fires
	/// TextChanged exactly once and therefore cannot reproduce the native double-fire
	/// -- a real keystroke is required.
	/// </summary>
	[TestFixture]
	public class AutoCompleteGhostTests : TestBase
	{
		// Kept deterministic and prefix-disjoint after the first letter so the tester
		// always knows exactly what each keystroke should complete to.
		static readonly string[] Candidates = { "Line", "Loft", "Circle", "Curve" };

		/// <summary>
		/// Minimal, self-contained ghost-completion driver over a plain TextBox: on
		/// each TextChanged it completes the typed prefix to the best candidate and
		/// selects the suffix. Enough to exercise the behavior with no extra deps.
		/// </summary>
		class MiniAutoComplete
		{
			readonly TextBox _input;
			bool _skipUpdate; // guards against re-entering on our own programmatic write

			public MiniAutoComplete(TextBox input)
			{
				_input = input;
				_input.TextChanged += OnTextChanged;
			}

			void OnTextChanged(object sender, EventArgs e)
			{
				// Don't recurse into our own programmatic Text write below.
				if (_skipUpdate)
					return;
				Show();
			}

			void Show()
			{
				if (string.IsNullOrEmpty(_input.Text))
					return;

				// the completion prefix is the text up to the caret
				var prefix = _input.Text.Substring(0, _input.Selection.Start);
				if (string.IsNullOrEmpty(prefix))
					return;

				var best = Candidates.FirstOrDefault(
					c => c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

				if (best != null)
					SetInput(best);
			}

			// complete to value, keeping the typed prefix and selecting the ghost suffix
			void SetInput(string value)
			{
				var oldText = _input.Text;
				var selectionIndex = _input.Selection.Start;
				if (string.IsNullOrEmpty(value))
					return;
				if (oldText == value)
					return;
				_skipUpdate = true; // prevent our TextChanged from re-entering
				_input.Text = value;
				if (selectionIndex > value.Length)
					selectionIndex = value.Length;
				// select the ghost suffix, keeping the caret after the typed prefix
				_input.Selection = Range.FromLength(selectionIndex, _input.Text.Length - selectionIndex);
				_skipUpdate = false;
			}
		}

		[Test, ManualTest]
		public void TypingSecondCharacterWithGhostCompletionShouldWork()
		{
			ManualForm(
				"Click in the box and type 'L'.\n"
				+ "  -> it should complete to 'Line' with 'ine' selected (caret after 'L').\n"
				+ "Now type a SECOND character while 'ine' is selected:\n"
				+ "  - type 'i' -> should stay 'Line' with 'ne' selected (caret after 'Li').\n"
				+ "  - or (clear and retry) type 'o' -> should complete to 'Loft' with 'ft' selected.\n\n"
				+ "PASS if the second character is accepted and drives the completion.\n"
				+ "FAIL if typing the second character does nothing (text stays 'Line', or the\n"
				+ "typed character is dropped).",
				form =>
				{
					// The driver manages the selection itself and keeps it visible
					// without focus, as an inline-autocomplete input would.
					var textBox = new TextBox
					{
						AutoSelectMode = AutoSelectMode.Never,
						AlwaysShowSelection = true,
						Width = 240
					};

					_ = new MiniAutoComplete(textBox);

					form.Shown += (sender, e) => textBox.Focus();
					return textBox;
				});
		}
	}
}
