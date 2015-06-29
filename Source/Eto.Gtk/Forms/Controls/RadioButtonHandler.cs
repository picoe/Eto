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
			Control.Add(label); //control.AddMnemonicLabel(label);
			Control.Toggled += Connector.HandleCheckedChanged;
			box = new Gtk.EventBox();
			box.Child = Control;
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
		}

		public override string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set { label.TextWithMnemonic = value.ToPlatformMnemonic(); }
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
