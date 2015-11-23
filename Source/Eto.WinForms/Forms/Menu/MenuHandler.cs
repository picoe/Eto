using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms.Menu
{
	interface IMenuItemHandler
	{
		Keys Shortcut { get; }
		bool Enabled { get; }
		MenuItem Widget { get; }
		SWF.ToolStripMenuItem Control { get; }
		MenuItem.ICallback Callback { get; }
	}

	public abstract class MenuItemHandler<TControl, TWidget, TCallback> : MenuHandler<TControl, TWidget, TCallback>, Eto.Forms.MenuItem.IHandler, IMenuItemHandler
		where TControl: SWF.ToolStripMenuItem
		where TCallback: MenuItem.ICallback
		where TWidget: MenuItem
	{
		static readonly object CustomShortcutKey = new object();

		public string Text
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public string ToolTip
		{
			get { return Control.ToolTipText; }
			set { Control.ToolTipText = value; }
		}

		public bool Enabled
		{
			get { return Control.Enabled; }
			set { Control.Enabled = value; }
		}

		public Keys Shortcut
		{
			get { return Widget.Properties.Get<Keys>(CustomShortcutKey, Control.ShortcutKeys.ToEto()); }
			set
			{
				var key = value.ToSWF();
				if (!value.HasFlag(Keys.Application) && SWF.ToolStripManager.IsValidShortcut(key))
				{
					Control.ShortcutKeys = key;
					Widget.Properties.Set(CustomShortcutKey, default(Keys));
				}
				else
				{
					Control.ShortcutKeys = SWF.Keys.None;
					Widget.Properties.Set(CustomShortcutKey, value);
					if (value != Keys.None)
						Control.ShortcutKeyDisplayString = value.ToShortcutString();
				}
			}
		}


		MenuItem IMenuItemHandler.Widget
		{
			get { return Widget; }
		}

		SWF.ToolStripMenuItem IMenuItemHandler.Control
		{
			get { return Control; }
		}

		MenuItem.ICallback IMenuItemHandler.Callback
		{
			get { return Callback; }
		}
	}

	public abstract class MenuHandler<TControl, TWidget, TCallback> : WidgetHandler<TControl, TWidget, TCallback>, Eto.Forms.Menu.IHandler
		where TControl: SWF.ToolStripItem
		where TWidget: Widget
	{

		public override void AttachEvent (string id)
		{
			switch (id) {
			case MenuItem.ValidateEvent:
				// handled by parents
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public void CreateFromCommand(Command command)
		{
		}
	}
}
