using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp
{
	public class TabControlHandler : GtkContainer<Gtk.Notebook, TabControl>, ITabControl
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
			get { return ContainerContentControl.Style.Base(Gtk.StateType.Normal).ToEto(); }
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
					handler.Widget.OnSelectedIndexChanged(EventArgs.Empty);
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
			while (Control.NPages > 0)
				Control.RemovePage(0);
		}

		public void RemoveTab(int index, TabPage page)
		{
			Control.RemovePage(index);
			if (Widget.Loaded && Control.NPages == 0)
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
		}
	}
}
