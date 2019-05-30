using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TabControlHandler : GtkContainer<Gtk.Notebook, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		public TabControlHandler()
		{
			Control = new Gtk.Notebook();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.SwitchPage += Connector.HandleSwitchPage;
		}

		protected override bool IsTransparentControl
		{
			get { return false; }
		}

		protected override Color DefaultBackgroundColor
		{
			get { return ContainerContentControl.GetBase(); }
		}

		protected new TabControlConnector Connector { get { return (TabControlConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TabControlConnector();
		}

		protected class TabControlConnector : GtkControlConnector
		{
			public new TabControlHandler Handler { get { return (TabControlHandler)base.Handler; } }

			public void HandleSwitchPage(object o, Gtk.SwitchPageArgs args)
			{
				var handler = Handler;
				if (handler != null && handler.Widget.Loaded)
					handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
			}
		}

		public int SelectedIndex
		{
			get { return Control.CurrentPage; }
			set { Control.CurrentPage = value; }
		}

		public void InsertTab(int index, TabPage page)
		{
			var pageHandler = (TabPageHandler)page.Handler;

			if (Widget.Loaded)
			{
				pageHandler.ContainerControl.ShowAll();
				pageHandler.LabelControl.ShowAll();
			}
			
			if (index == -1)
				Control.AppendPage(pageHandler.ContainerControl, pageHandler.LabelControl);
			else
				Control.InsertPage(pageHandler.ContainerControl, pageHandler.LabelControl, index);
		}

		public void ClearTabs()
		{
			Control.CurrentPage = -1;
			while (Control.NPages > 0)
				Control.RemovePage(0);
		}

		public void RemoveTab(int index, TabPage page)
		{
			Control.RemovePage(index);
			if (Widget.Loaded && Control.NPages == 0)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);
		}

		public DockPosition TabPosition
		{
			get { return Control.TabPos.ToEto(); }
			set { Control.TabPos = value.ToGtk(); }
		}
	}
}
