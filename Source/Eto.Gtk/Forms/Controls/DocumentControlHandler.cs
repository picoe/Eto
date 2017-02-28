using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DocumentControlHandler : GtkContainer<Gtk.Notebook, DocumentControl, DocumentControl.ICallback>, DocumentControl.IHandler
	{
		List<DocumentPage> pages;

		public DocumentControlHandler()
		{
			pages = new List<DocumentPage>();

			Control = new Gtk.Notebook();
			Control.Scrollable = true;
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

		protected new DocumentControlConnector Connector { get { return (DocumentControlConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new DocumentControlConnector();
		}

		protected class DocumentControlConnector : GtkControlConnector
		{
			public new DocumentControlHandler Handler { get { return (DocumentControlHandler)base.Handler; } }

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

		public void InsertPage(int index, DocumentPage page)
		{
			pages.Add(page);

			var pageHandler = (DocumentPageHandler)page.Handler;

			if (Widget.Loaded)
			{
				pageHandler.ContainerControl.ShowAll();
				pageHandler.LabelControl.ShowAll();
			}

			if (index == -1)
				Control.AppendPage(pageHandler.ContainerControl, pageHandler.LabelControl);
			else
				Control.InsertPage(pageHandler.ContainerControl, pageHandler.LabelControl, index);

			pageHandler.closebutton1.Clicked += (o, args) => ClosePage(pageHandler.ContainerControl, page);
			pageHandler.LabelControl.ButtonPressEvent += (o, args) =>
			{
				if (args.Event.Button == 2 && page.Closable)
					ClosePage(pageHandler.ContainerControl, page);
			};

			Control.SetTabReorderable(pageHandler.ContainerControl, true);
			Control.ShowTabs = Control.NPages > 1;
		}

		private void ClosePage(Gtk.Widget control, DocumentPage page)
		{
			Control.RemovePage(Control.PageNum(control));
			Control.ShowTabs = Control.NPages > 1;

			var handler = Connector.Handler;
			if (handler != null && handler.Widget.Loaded)
				handler.Callback.OnPageClosed(handler.Widget, new DocumentPageEventArgs(page));
		}

		public void RemovePage(int index)
		{
			pages.Remove(GetPage(index));

			Control.RemovePage(index);
			if (Widget.Loaded && Control.NPages == 0)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);

			Control.ShowTabs = Control.NPages > 1;
		}

		public DocumentPage GetPage(int index)
		{
			var nativepage = Control.GetNthPage(index);
			return pages.Find((obj) => (obj.Handler as DocumentPageHandler).ContainerControl == nativepage);
		}

		public int GetPageCount()
		{
			return Control.NPages;
		}
	}
}
