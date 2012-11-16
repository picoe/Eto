using System;
using Eto.Forms;

namespace Eto.Interface
{
    public interface IKeyboardInputSource
    {
        event EventHandler<KeyPressEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyDown;
    }
}
