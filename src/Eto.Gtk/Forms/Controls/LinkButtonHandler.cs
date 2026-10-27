namespace Eto.GtkSharp.Forms.Controls
{
	public class LinkButtonHandler : GtkControl<Gtk.LinkButton, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{

		Gtk.EventBox box;

		public override Gtk.Widget ContainerControl
		{
			get { return box; }
		}

		WrapMode wrap = WrapMode.Word;
		TextTrimming trimming;

		public LinkButtonHandler()
		{
			Control = new Gtk.LinkButton(string.Empty);
			Control.Xalign = 0f;
			Control.Yalign = .5f;
			Control.TooltipText = null;
			box = new Gtk.EventBox();
			box.Child = Control;
			SetWrap();
		}

		// A Gtk.LinkButton is a button wrapping a label, so the text settings go on that label.
		Gtk.Label TextLabel => Control.Child as Gtk.Label;

		public WrapMode Wrap
		{
			get => wrap;
			set
			{
				wrap = value;
				SetWrap();
			}
		}

		public TextTrimming Trimming
		{
			get => trimming;
			set
			{
				trimming = value;
				SetWrap();
			}
		}

		void SetWrap()
		{
			var label = TextLabel;
			if (label == null)
				return;

			switch (wrap)
			{
				case WrapMode.None:
					label.LineWrap = false;
					break;
				case WrapMode.Word:
					label.LineWrapMode = Pango.WrapMode.WordChar;
					label.LineWrap = true;
					break;
				case WrapMode.Character:
					label.LineWrapMode = Pango.WrapMode.Char;
					label.LineWrap = true;
					break;
				default:
					throw new NotSupportedException();
			}
			// Pango cannot wrap and ellipsize the same layout - wrapping wins, see Eto.Forms.TextTrimming.
			label.Ellipsize = wrap == WrapMode.None ? trimming.ToPango() : Pango.EllipsizeMode.None;
			Control.QueueResize();
		}

		public Color TextColor
		{
			get { return Control.GetForeground(); }
			set
			{
				Control.SetForeground(value);
				Control.SetTextColor(value);
				Control.Child.SetForeground(value);
				Control.Child.SetTextColor(value);
			}
		}

		public Color DisabledTextColor
		{
			get { return Control.GetForeground(GtkStateFlags.Insensitive); }
			set
			{
				Control.SetForeground(value, GtkStateFlags.Insensitive);
				Control.Child.SetForeground(value, GtkStateFlags.Insensitive);
			}
		}

		public override string Text
		{
			get { return Control.Label; }
			set { Control.Label = value; }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Control.Clicked += Connector.HandleClicked;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new LinkButtonConnector Connector { get { return (LinkButtonConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new LinkButtonConnector();
		}

		protected class LinkButtonConnector : GtkControlConnector
		{
			public new LinkButtonHandler Handler { get { return (LinkButtonHandler)base.Handler; } }

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler?.Callback.OnClick(Handler.Widget, EventArgs.Empty);
			}
		}

	}
}