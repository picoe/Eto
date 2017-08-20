using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class TextBoxHandler : TextBoxBaseHandler<Gtk.Widget, TextBox, TextBox.ICallback>
    {
        public TextBoxHandler()
        {
            Handle = GtkWrapper.gtk_entry_new();
        }
    }
}
