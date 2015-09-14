using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using ObjCRuntime;
using UIKit;
using sd = System.Drawing;

namespace Eto.iOS.Forms.Toolbar
{
	public interface IToolBarBaseItemHandler
	{
		UIBarButtonItem Control { get; }
		bool Selectable { get; }
		void ControlAdded(ToolBarHandler toolbar);
	}

	public interface IToolBarItemHandler : IToolBarBaseItemHandler
	{
		void OnClick();
		bool Enabled { get; }
		UIBarButtonItem ButtonItem { get; }
	}

	public class ToolBarHandler : WidgetHandler<UIToolbar, ToolBar>, ToolBar.IHandler, IIosViewControllerSource
	{
		readonly List<IToolBarBaseItemHandler> items = new List<IToolBarBaseItemHandler>();

		public UIViewController Controller { get; set; }

		public Action UpdateContentSize { get; set; }

		public static float LandscapeToolbarHeight = 32f;

		public static float PortraitToolbarHeight = 44f;

		class ToolbarController : UIViewController
		{
			public ToolBarHandler Handler { get; set; }

			public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
			{
				// resize the toolbar based on orientation
				var frame = Handler.Control.Frame;
				if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
					frame.Height = LandscapeToolbarHeight;
				else
					frame.Height = PortraitToolbarHeight;

				var adjust = Handler.Control.Frame.Height - frame.Height;
				if (Handler.Dock == ToolBarDock.Bottom)
					frame.Y += adjust;

				Handler.Control.Frame = frame;
				// allow parent to update content sizes based on new size of toolbar
				if (Handler.UpdateContentSize != null)
					Handler.UpdateContentSize();
				base.WillRotate(toInterfaceOrientation, duration);
			}
		}

		public ToolBarHandler()
		{
			Control = new UIToolbar();
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			Control.BarStyle = UIBarStyle.Default;
			if (!Platform.IsIpad)
				Dock = ToolBarDock.Bottom;
			Controller = new ToolbarController { Handler = this, View = Control };
		}

		public void AddButton(ToolItem button, int index)
		{
			var handler = (IToolBarBaseItemHandler)button.Handler;
			items.Add(handler);
			var list = GetItems();
			list.Insert(index, (UIBarButtonItem)button.ControlObject);
			Control.Items = list.ToArray();
			if (handler != null)
				handler.ControlAdded(this);
		}

		private List<UIBarButtonItem> GetItems()
		{
			var list = Control.Items;
			return list != null ? list.ToList() : new List<UIBarButtonItem>();
		}

		public void RemoveButton(ToolItem item)
		{
			var handler = item.Handler as IToolBarBaseItemHandler;
			var index = items.IndexOf(handler);
			items.Remove(handler);
			var list = GetItems();
			list.RemoveAt(index);
			Control.Items = list.ToArray();
		}

		public void Clear()
		{
			Control.Items = null;
		}

		public ToolBarTextAlign TextAlign { get; set; } // TODO
		public ToolBarDock Dock { get; set; } // TODO
	}
}

