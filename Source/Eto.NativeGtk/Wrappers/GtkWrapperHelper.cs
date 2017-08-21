using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

static partial class GtkWrapper
{
    public const int GTK_POS_LEFT = 0;
    public const int GTK_POS_RIGHT = 1;
    public const int GTK_POS_TOP = 2;
    public const int GTK_POS_BOTTOM = 3;

    public const int GTK_RESPONSE_NONE = -1;
    public const int GTK_RESPONSE_REJECT = -2;
    public const int GTK_RESPONSE_ACCEPT = -3;
    public const int GTK_RESPONSE_DELETE_EVENT = -4;
    public const int GTK_RESPONSE_OK = -5;
    public const int GTK_RESPONSE_CANCEL = -6;
    public const int GTK_RESPONSE_CLOSE = -7;
    public const int GTK_RESPONSE_YES = -8;
    public const int GTK_RESPONSE_NO = -9;
    public const int GTK_RESPONSE_APPLY = -10;
    public const int GTK_RESPONSE_HELP = -11;

    public const int PANGO_WRAP_WORD = 0;
    public const int PANGO_WRAP_CHAR = 1;
    public const int PANGO_WRAP_WORD_CHAR = 2;

    public const int GTK_ALIGN_FILL = 0;
    public const int GTK_ALIGN_START = 1;
    public const int GTK_ALIGN_END = 2;
    public const int GTK_ALIGN_CENTER = 3;
    public const int GTK_ALIGN_BASELINE = 4;

    public const int GTK_FILE_CHOOSER_ACTION_OPEN = 0;
    public const int GTK_FILE_CHOOSER_ACTION_SAVE = 1;
    public const int GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER = 2;
    public const int GTK_FILE_CHOOSER_ACTION_CREATE_FOLDER = 3;

    public const int GTK_DIALOG_MODAL = 1;
    public const int GTK_DIALOG_DESTROY_WITH_PARENT = 2;
    public const int GTK_DIALOG_USE_HEADER_BAR = 4;

    public struct Allocation
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public struct GSList
    {
        public IntPtr Data;
        public IntPtr Next;

        public bool MoveNext(out GSList list)
        {
            if (Next == IntPtr.Zero)
            {
                list = this;
                return false;
            }

            list = (GSList)Marshal.PtrToStructure(Next, typeof(GSList));
            return true;
        }

        public string GetData()
        {
            return WrapperHelper.GetString(Data);
        }
    }
}
