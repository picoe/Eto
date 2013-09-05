using System;
using Eto.Forms;

namespace Eto.Forms
{
    public interface IKeyboardInputSource
    {
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyEventArgs> KeyDown;
    }
}
