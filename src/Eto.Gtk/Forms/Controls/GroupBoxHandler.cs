namespace Eto.GtkSharp.Forms.Controls
{
	public class GroupBoxHandler : GtkPanel<Gtk.Frame, GroupBox, GroupBox.ICallback>, GroupBox.IHandler
	{
		static readonly object TextColor_Key = new object();

		public GroupBoxHandler ()
		{
			Control = new EtoFrame { Handler = this };
		}

		protected override Gtk.Widget FontControl => Control.LabelWidget ?? new Gtk.Label();

		public override string Text {
			get { return Control.Label; }
			set
			{
				// GTK replaces the label when the title changes, so reapply assigned styles.
				Control.Label = value;
				if (Control.LabelWidget is Gtk.Widget label)
				{
					if (Widget.Properties.ContainsKey(GtkControl.Font_Key))
						label.SetFont(Font.ToPango());
					if (Widget.Properties.Get<Color?>(TextColor_Key) is Color color)
						label.SetForeground(color);
				}
			}
		}

		public override Size ClientSize {
			get {
				if (Control.IsRealized && Control.Child != null)
					return Control.Child.Allocation.Size.ToEto ();
				else {
					var label = Control.LabelWidget;
					var size = Size;
					size.Height -= (label?.Allocation.Height ?? 0) + 10;
					size.Width -= 10;
					return size;
				}
			}
			set {
				var label = Control.LabelWidget;
				var size = value;
				size.Height += (label?.Allocation.Height ?? 0) + 10;
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
			get
			{
				if (Control.LabelWidget is Gtk.Widget label)
					return label.GetForeground();
				return Widget.Properties.Get<Color?>(TextColor_Key) ?? Control.GetForeground();
			}
			set
			{
				Widget.Properties.Set<Color?>(TextColor_Key, value);
				Control.LabelWidget?.SetForeground(value);
			}
		}
	}
}
