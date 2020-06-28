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
		bool allowReorder;

		public DocumentControlHandler()
		{
			pages = new List<DocumentPage>();

			Control = new Gtk.Notebook();
			Control.Scrollable = true;
		}

		static readonly object HideTabsWithSinglePage_Key = new object();

		public bool HideTabsWithSinglePage
		{
			get => Widget.Properties.Get<bool>(HideTabsWithSinglePage_Key);
			set => Widget.Properties.Set(HideTabsWithSinglePage_Key, value);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.SwitchPage += Connector.HandleSwitchPage;
		}

		protected override bool IsTransparentControl => false;

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
			int oldIndex = -1;
			public new DocumentControlHandler Handler { get { return (DocumentControlHandler)base.Handler; } }

			public void HandleSwitchPage(object o, Gtk.SwitchPageArgs args)
			{
				var handler = Handler;
				if (handler != null && handler.Enabled)
				{
					handler.Callback.OnSelectedIndexChanged(handler.Widget, EventArgs.Empty);
					oldIndex = (int)args.PageNum;
				}
				else if (oldIndex >= 0)
				{
					handler.Control.Page = oldIndex;
				}
			}
			 
			public void HandlePageReordered(object o, Gtk.PageReorderedArgs args)
			{
				var handler = Handler;
				if (handler != null && handler.Enabled)
				{
					var newIndex = (int)(uint)args.Args[1];
					handler.Callback.OnPageReordered(handler.Widget, new DocumentPageReorderEventArgs(handler.GetPage(newIndex), oldIndex, newIndex));
					oldIndex = newIndex;
				}
			}
		}

		public int SelectedIndex
		{
			get { return Control.CurrentPage; }
			set { Control.CurrentPage = value; }
		}

		public bool AllowReordering
		{
			get { return allowReorder; }
			set
			{
				if (allowReorder != value)
				{
					allowReorder = value;
					UpdateReorder();
				}
			}
		}

		void UpdateReorder()
		{
			var enabled = Enabled;
			foreach (var page in pages)
			{
				var pageHandler = (DocumentPageHandler)page.Handler;
				Control.SetTabReorderable(pageHandler.ContainerControl, allowReorder && enabled);
				pageHandler.LabelControl.Sensitive = enabled;
			}
		}

		public void InsertPage(int index, DocumentPage page)
		{
			pages.Add(page);

			var pageHandler = (DocumentPageHandler)page.Handler;

			if (Widget.Loaded)
			{
				pageHandler.ContainerControl.ShowAll();
			}

			pageHandler.LabelControl.Sensitive = Enabled;

			if (index == -1)
				Control.AppendPage(pageHandler.ContainerControl, pageHandler.LabelControl);
			else
				Control.InsertPage(pageHandler.ContainerControl, pageHandler.LabelControl, index);


			Control.SetTabReorderable(pageHandler.ContainerControl, allowReorder && Enabled);
			SetShowTabs();
		}

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				if (Enabled != value)
				{
					base.Enabled = value;
					UpdateReorder();
				}
			}
		}

		void SetShowTabs()
		{
			if (HideTabsWithSinglePage)
			{
				Control.ShowTabs = Control.NPages > 1;
			}
		}

		internal void ClosePage(Gtk.Widget control, DocumentPage page)
		{
			Control.RemovePage(Control.PageNum(control));
			SetShowTabs();

			if (Widget.Loaded)
				Callback.OnPageClosed(Widget, new DocumentPageEventArgs(page));
		}

		public void RemovePage(int index)
		{
			pages.Remove(GetPage(index));

			Control.RemovePage(index);
			if (Widget.Loaded && Control.NPages == 0)
				Callback.OnSelectedIndexChanged(Widget, EventArgs.Empty);

			SetShowTabs();
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

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case DocumentControl.PageReorderedEvent:
					Control.PageReordered += Connector.HandlePageReordered;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
