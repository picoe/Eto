using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using UIKit;
using System.Linq;
using Eto.Mac.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class TabPageHandler : MacPanel<UIViewController, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		public TabPageHandler()
		{
			Control = new UIViewController();
			Controller = Control;
		}

		public string Text
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}

		Image image;

		public Image Image
		{
			get { return image ?? (image = Control.TabBarItem.Image.ToEto()); }
			set
			{ 
				image = value;
				Control.TabBarItem.Image = value.ToUI(32);
			}
		}

		public override UIView ContainerControl
		{
			get { return Control.View; }
		}

		public override void LayoutChildren()
		{
			if (Content != null && Widget.Parent != null && Control.EdgesForExtendedLayoutIsSupported())
			{
				var child = Content.Handler as ScrollableHandler;
				if (child != null)
				{
					// need to manually adjust content insets for scrollable children
					var tabs = Widget.Parent.Handler as TabControlHandler;
					var inset = new UIEdgeInsets(0, 0, tabs.Control.TabBar.Bounds.Height, 0);
					child.Control.ContentInset = inset;
					child.Control.ScrollIndicatorInsets = inset;
					Control.EdgesForExtendedLayout = UIRectEdge.All;
				}
				else
				{
					// otherwise, show everything
					Control.EdgesForExtendedLayout = UIRectEdge.None;
				}
			}
			base.LayoutChildren();
		}
	}
}
