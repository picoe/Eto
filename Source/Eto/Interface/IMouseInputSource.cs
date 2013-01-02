using System;
using Eto.Drawing;

using MouseEventHandler = System.EventHandler<Eto.Forms.MouseEventArgs>;

namespace Eto.Interface
{
    public interface IMouseInputSource
    {
        event MouseEventHandler MouseHover;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseLeave;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseDoubleClick;
        event MouseEventHandler MouseWheel;
    }
}
