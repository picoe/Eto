using System;
using Eto.Forms;
using Eto.GtkSharp.Forms.Controls;

namespace Eto.GtkSharp.Forms.Menu
{
	public class CheckMenuItemHandler : MenuActionItemHandler<Gtk.CheckMenuItem, CheckMenuItem, CheckMenuItem.ICallback>, CheckMenuItem.IHandler
	{
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
			Control.ShowAll();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Activated += Connector.HandleActivated;
		}

		protected new CheckMenuItemConnector Connector { get { return (CheckMenuItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new CheckMenuItemConnector();
		}

		protected class CheckMenuItemConnector : WeakConnector
		{
			public new CheckMenuItemHandler Handler { get { return (CheckMenuItemHandler)base.Handler; } }

			public void HandleActivated(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler.SuppressClick == 0)
					handler.Callback.OnClick(handler.Widget, e);
			}

			public void HandleToggled(object sender, EventArgs e)
			{
				var handler = Handler;
				handler.Callback.OnCheckedChanged(handler.Widget, e);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case CheckMenuItem.CheckedChangedEvent:
					Control.Toggled += Connector.HandleToggled;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string Text
		{
			get { return label.Text; }
			set
			{
				label.Text = value.ToPlatformMnemonic();
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

		static readonly object SuppressClick_Key = new object();

		int SuppressClick
		{
			get { return Widget.Properties.Get<int>(SuppressClick_Key); }
			set { Widget.Properties.Set(SuppressClick_Key, value); }
		}

		public bool Checked
		{
			get { return Control.Active; }
			set
			{
				SuppressClick++;
				Control.Active = value;
				SuppressClick--;
			}
		}

		public bool Enabled
		{
			get { return Control.Sensitive; }
			set { Control.Sensitive = value; }
		}
	}
}
