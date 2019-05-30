using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class RadioButtonHandler : GtkControl<Gtk.RadioButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		Gtk.EventBox box;
		Gtk.AccelLabel label;

		protected override Gtk.Widget FontControl
		{
			get { return label; }
		}

		public override Gtk.Widget ContainerControl
		{
			get { return box; }
		}

		public void Create(RadioButton controller)
		{
			if (controller != null)
				Control = new Gtk.RadioButton(RadioButtonHandler.GetControl(controller));
			else
			{
				Control = new Gtk.RadioButton((Gtk.RadioButton)null);
				// make gtk work like others in that no radio button is initially selected
				var inactive = new Gtk.RadioButton(Control);
				inactive.Active = true;
			}
			label = new Gtk.AccelLabel("");
			Control.Add(label);
			Control.Realized += Connector.Control_Realized;

			Control.Toggled += Connector.HandleCheckedChanged;
			box = new Gtk.EventBox();
			box.Child = Control;
		}

		void UpdateLabel()
		{
			// if Text present show label, otherwise hide it.
			if (label.Text != null && label.Text != string.Empty){
				label.Visible = true;
			} else {
				label.Visible = false;
			}
		}

		void Control_Realized (object sender, EventArgs e)
		{
			UpdateLabel ();
		}

		protected new RadioButtonConnector Connector { get { return (RadioButtonConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new RadioButtonConnector();
		}

		protected class RadioButtonConnector : GtkControlConnector
		{
			public new RadioButtonHandler Handler { get { return (RadioButtonHandler)base.Handler; } }

			public void HandleCheckedChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnCheckedChanged(Handler.Widget, EventArgs.Empty);
			}

			internal void Control_Realized(object sender, EventArgs e) => Handler?.Control_Realized(sender, e);
		}

		public override string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set {
				label.TextWithMnemonic = value.ToPlatformMnemonic();
				UpdateLabel ();
			}
		}

		public bool Checked
		{
			get { return Control.Active; }
			set { Control.Active = value; }
		}

		public Color TextColor
		{
			get { return label.GetForeground(); }
			set
			{
				label.SetForeground(value, GtkStateFlags.Normal);
				label.SetForeground(value, GtkStateFlags.Active);
				label.SetForeground(value, GtkStateFlags.Prelight);
			}
		}
	}
}
