using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms.Menu
{
	public class CheckMenuItemHandler : MenuActionItemHandler<Gtk.CheckMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
		string text;
		string tooltip;
		Keys shortcut;
		readonly Gtk.AccelLabel label;

		public CheckMenuItemHandler()
		{
			Control = new Gtk.CheckMenuItem();
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;
			label.AccelWidget = Control;
			Control.Add(label);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Toggled += Connector.HandleToggled;
		}

		protected new CheckMenuItemConnector Connector { get { return (CheckMenuItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CheckMenuItemConnector();
		}

		protected class CheckMenuItemConnector : WeakConnector
		{
			public new CheckMenuItemHandler Handler { get { return (CheckMenuItemHandler)base.Handler; } }

			public void HandleToggled(object sender, EventArgs e)
			{
				var handler = Handler;
				if (!handler.isBeingChecked)
				{
					handler.isBeingChecked = true;
					handler.Control.Active = !handler.Control.Active; // don't let Gtk turn it on/off
					handler.isBeingChecked = false;
					handler.Callback.OnClick(handler.Widget, e);
				}
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				string val = text;
				label.Text = GtkControl<Gtk.Widget, Control, Control.ICallback>.StringToMnuemonic(val);
				label.UseUnderline = true;
			}
		}

		public string ToolTip
		{
			get { return tooltip; }
			set
			{
				tooltip = value;
				//label.TooltipText = value;
			}
		}

		public Keys Shortcut
		{
			get { return shortcut; }
			set
			{ 
				shortcut = value;
				SetAccelerator();
			}
		}

		protected override Keys GetShortcut()
		{
			return Shortcut;
		}

		bool isBeingChecked;

		public bool Checked
		{
			get { return Control.Active; }
			set
			{
				isBeingChecked = true;
				Control.Active = value;
				isBeingChecked = false;
			}
		}

		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}
	}
}
