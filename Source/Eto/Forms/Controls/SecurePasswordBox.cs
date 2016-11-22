using System;
using System.Security;

namespace Eto.Forms
{
    /// <summary>
    /// Implementation of a password box that uses a <see cref="SecureString"/> as backing store.
    /// </summary>
    /// <seealso>
    ///   <cref>Eto.Forms.TextBox</cref>
    /// </seealso>
    public class SecurePasswordBox : TextBox
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public SecureString Password { get; set; } = new SecureString();

        
        private bool IsChar(Keys k)
        {
            bool isAlphaNumeric = (Keys.A <= k && Keys.Z >= k) || (Keys.D0 <= k && Keys.D9 >= k);
            if (isAlphaNumeric) return true;

            bool isOtherwiseChar = k == Keys.Plus || k == Keys.Minus || k == Keys.Grave ||  k == Keys.Divide || k == Keys.Decimal;
            if (isOtherwiseChar) return true;

            isOtherwiseChar = k == Keys.Space || (k >= Keys.Backslash && k <= Keys.LeftBracket);
            return isOtherwiseChar;
        }

        /// <summary>
        /// Gets or sets the password masking character.
        /// </summary>
        /// <value>
        /// The password character.
        /// </value>
        public char PasswordChar { get; set; } = '*';

        /// <summary>
        /// Occurs when password has changed.
        /// </summary>
        public event EventHandler<EventArgs> PasswordChanged = delegate { };

        /// <summary>
        /// Gets the password binding.
        /// </summary>
        /// <value>
        /// The password binding.
        /// </value>
        public BindableBinding<SecurePasswordBox, SecureString> PasswordBinding
        {
            get
            {
                return new BindableBinding<SecurePasswordBox, SecureString>(
                            this, 
                            c => c.Password, 
                            (c, v) => c.Password = v, 
                            (c, h) => c.PasswordChanged += h,
                            (c, h) => c.PasswordChanged -= h
                       );
            }

        }

        /// <summary>
        /// Called when the <see cref="E:Eto.Forms.Control.KeyDown" /> event is fired.
        /// </summary>
        /// <param name="e">Key event arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // this can be called twice, once with e.KeyChar == char.MaxValue
            // and, for character-printing keys, another time with e.KeyChar the actual
            // key that was pressed.
            // I understand that as the first time being a "preview" of the actual key-press.

            // store the caret index - changing the Text will 
            // set it to zero and we want to preserve its location
            int caretIndex = CaretIndex;

            // when there is a selection - remove the selection from the textbox
            if (Selection.Length() > 0 && e.KeyChar == char.MaxValue)
            {
                // when a character-printing key will be pressed remove the 
                // selection from the password and set the masking text
                if (Keys.Delete == e.Key || Keys.Backspace == e.Key || IsChar(e.Key))
                {
                    int start = Selection.Start, end = Selection.End;
                    for (int i = start; i <= end; ++i)
                    {
                        Password.RemoveAt(start); // keep removing at the start index
                    }


                    Text = new string(PasswordChar, Password.Length);
                    Selection = new Range<int>(0, -1);

                    // restore the caret index
                    CaretIndex = Math.Min(Text.Length, caretIndex);
                }

                // if delete or backspace were pressed, don't handle that event again.
                e.Handled = Keys.Delete == e.Key || Keys.Backspace == e.Key;

                // raise the event and return
                PasswordChanged?.Invoke(this, new EventArgs());
                
                return;
            }

            if (e.IsChar)
            {
                // add a masking character
                Text += PasswordChar; 
                if (caretIndex == Text.Length)
                {
                    // and append the character
                    Password.AppendChar(e.KeyChar); 
                }
                else
                {
                    // or insert it
                    Password.InsertAt(caretIndex, e.KeyChar);
                }
                ++caretIndex;
            }
            else if (e.Key == Keys.Backspace)
            {
                if (caretIndex > 0)
                {
                    // remove at the location left of the caret index
                    Password.RemoveAt(caretIndex - 1);
                }
                e.Handled = true;
            }
            else if (e.Key == Keys.Delete)
            {
                if (caretIndex < Text.Length)
                {
                    // remove at the location right of the caret index
                    Password.RemoveAt(caretIndex);
                }
                e.Handled = true;
            }
            // remove any existing selection
            Selection = new Range<int>(0, -1);

            // restore the caret index
            CaretIndex = caretIndex;
            
            // only set the event to handled if not in "preview" mode
            e.Handled = e.KeyChar != char.MaxValue;

            // invoke the event
            PasswordChanged?.Invoke(this, new EventArgs());
        }
    }
}
