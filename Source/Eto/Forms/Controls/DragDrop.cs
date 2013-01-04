using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
    [Flags]
    public enum DragDropEffects
    {
        Scroll = -2147483648,
        All = -2147483645,
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4,
    }

    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(
            IDataObject data,
            int x,
            int y,
            DragDropEffects allowedEffect,
            DragDropEffects effect)
        {
            this.Data = data;
            this.X = x;
            this.Y = y;
            this.AllowedEffect = allowedEffect;
            this.Effect = effect;
        }

        public DragDropEffects AllowedEffect { get; private set; }
        public IDataObject Data { get; private set; }
        public DragDropEffects Effect { get; set; }
        public int KeyState { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }


    public enum DragAction
    {
        Continue = 0,
        Drop = 1,
        Cancel = 2,
    }

    public class QueryContinueDragEventArgs : EventArgs
    {
        public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action)
        {
            this.KeyState = keyState;
            this.EscapePressed = escapePressed;
            this.Action = action;
        }

        public DragAction Action { get; set; }
        public bool EscapePressed { get; private set; }
        public int KeyState { get; private set; }
    }

    public class GiveFeedbackEventArgs : EventArgs
    {
        public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
        {
            this.Effect = effect;
            this.UseDefaultCursors = useDefaultCursors;
        }

        public DragDropEffects Effect { get; private set; }

        public bool UseDefaultCursors { get; set; }
    }
}
