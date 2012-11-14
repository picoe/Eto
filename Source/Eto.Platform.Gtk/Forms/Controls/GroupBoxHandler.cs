using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class GroupBoxHandler : GtkContainer<Gtk.Frame, GroupBox>, IGroupBox
	{
		public GroupBoxHandler ()
		{
			Control = new Gtk.Frame ();
		}

		protected override Gtk.Widget FontControl
		{
			get { return Control.LabelWidget; }
		}

		public override object ContainerObject {
			get { return Control; }
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
					var size = base.Size;
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
				base.Size = size;
			}
		}

		public override void SetLayout (Layout inner)
		{
			if (Control.Child != null)
				Control.Remove (Control.Child);
			IGtkLayout gtklayout = (IGtkLayout)inner.Handler;
			var widget = (Gtk.Widget)gtklayout.ContainerObject;
			Control.Add (widget);

			/*if (clientSize != null) {
				var label = Control.LabelWidget;
				Control.SetSizeRequest(clientSize.Value.Width + 10, clientSize.Value.Height + label.Allocation.Height + 10);
				clientSize = null;
			}*/
		}
	}
}
