using System;
using Eto.Forms;

namespace Eto.Interface
{
    public interface IKeyboardInputSource
    {
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyEventArgs> KeyDown;
		event EventHandler<TextInputEventArgs> TextInput;
    }
}
