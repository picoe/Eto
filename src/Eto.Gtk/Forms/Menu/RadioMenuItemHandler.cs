namespace Eto.GtkSharp.Forms.Menu
{
	/// <summary>
	/// Summary description for MenuBarHandler.
	/// </summary>
	public class RadioMenuItemHandler : MenuActionItemHandler<Gtk.RadioMenuItem, RadioMenuItem, RadioMenuItem.ICallback>, RadioMenuItem.IHandler
	{
		string text;
		Keys shortcut;
		Gtk.AccelLabel label;

		public RadioMenuItemHandler()
		{
			label = new Gtk.AccelLabel(string.Empty);
			label.Xalign = 0;
			label.UseUnderline = true;

			Control = new Gtk.RadioMenuItem(string.Empty);
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
			Control.Active = false;
			Control.Add(label);
			Control.ShowAll();

			label.AccelWidget = Control;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Activated += Connector.HandleActivated;
		}

		bool settingGroup;
		RadioMenuItem controller;

		public RadioMenuItem Controller
		{
			get => controller;
			set
			{
				controller = value;
				var controllerControl = (Gtk.RadioMenuItem)value?.ControlObject;
				if (controllerControl != null && Array.IndexOf(controllerControl.Group, Control) >= 0)
					return;

				var wasChecked = Control.Active;
				// joining a group toggles this item, which isn't a change the user should see
				settingGroup = true;
				SuppressClick++;
				try
				{
					Control.JoinGroup(controllerControl);
					Control.Active = wasChecked;
				}
				finally
				{
					SuppressClick--;
					settingGroup = false;
				}
			}
		}

		protected new RadioMenuItemConnector Connector { get { return (RadioMenuItemConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new RadioMenuItemConnector();
		}

		protected class RadioMenuItemConnector : WeakConnector
		{
			public new RadioMenuItemHandler Handler { get { return (RadioMenuItemHandler)base.Handler; } }

			public void HandleActivated(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler == null)
					return;
				if (handler.SuppressClick > 0 || !handler.Control.Active)
					return;
				handler.Callback.OnClick(handler.Widget, e);
			}

			public void HandleToggled(object sender, EventArgs e)
			{
				var handler = Handler;
				if (handler == null || handler.settingGroup)
					return;
				handler.Callback.OnCheckedChanged(handler.Widget, e);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case RadioMenuItem.CheckedChangedEvent:
					Control.Toggled += Connector.HandleToggled;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public string Text
		{
			get { return text; }
			set
			{
				if (text == value)
					return;
				text = value;
				label.TextWithMnemonic = value.ToPlatformMnemonic();
			}
		}

		public string ToolTip
		{
			get { return label.TooltipText; }
			set { label.TooltipText = value; }
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

		public void AddMenu(int index, MenuItem item)
		{
			if (Control.Submenu == null)
				Control.Submenu = new Gtk.Menu();
			((Gtk.Menu)Control.Submenu).Insert((Gtk.Widget)item.ControlObject, index);
		}

		public void RemoveMenu(MenuItem item)
		{
			if (Control.Submenu == null)
				return;
			var menu = (Gtk.Menu)Control.Submenu;
			menu.Remove((Gtk.Widget)item.ControlObject);
			if (menu.Children.Length == 0)
			{
				Control.Submenu = null;
			}
		}

		public void Clear()
		{
			foreach (Gtk.Widget w in Control.Children)
			{
				Control.Remove(w);
			}
		}
	}
}
