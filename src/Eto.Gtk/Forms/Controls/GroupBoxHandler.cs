using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class GroupBoxHandler : GtkPanel<Gtk.Frame, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		public GroupBoxHandler ()
		{
			Control = new Gtk.Frame ();
		}

		protected override Gtk.Widget FontControl
		{
			get { return Control.LabelWidget; }
		}

		public override string Text {
			get { return Control.Label; }
			set { Control.Label = value; }
		}

		public override Size ClientSize {
			get {
				if (Control.Visible && Control.Child != null)
					return Control.Child.Allocation.Size.ToEto ();
				else {
					var label = Control.LabelWidget;
					var size = Size;
					size.Height -= label.Allocation.Height + 10;
					size.Width -= 10;
					return size;
				}
			}
			set {
				var label = Control.LabelWidget;
				var size = value;
				size.Height += label.Allocation.Height + 10;
				size.Width += 10;
				Size = size;
			}
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.Add(content);

			/*if (clientSize != null) {
				var label = Control.LabelWidget;
				Control.SetSizeRequest(clientSize.Value.Width + 10, clientSize.Value.Height + label.Allocation.Height + 10);
				clientSize = null;
			}*/
		}

		public Color TextColor
		{
			get { return Control.LabelWidget.GetForeground(); }
			set { Control.LabelWidget.SetForeground(value); }
		}
	}
}
