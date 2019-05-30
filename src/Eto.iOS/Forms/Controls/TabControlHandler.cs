using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using UIKit;
using System.Linq;
using Eto.Mac.Forms;
using System.Collections.Generic;
using sd = System.Drawing;
using System.Diagnostics;

namespace Eto.iOS.Forms.Controls
{
	public class TabControlHandler : MacContainer<UITabBarController, TabControl, TabControl.ICallback>, TabControl.IHandler
	{
		List<UIViewController> items = new List<UIViewController>();

		public override UIView ContainerControl { get { return Control.View; } }

		public TabControlHandler()
		{
			Control = new UITabBarController();
			Control.CustomizableViewControllers = null;
			Controller = Control;
		}

		public void InsertTab(int index, TabPage page)
		{
			items.Insert(index, TabPageHandler.GetControl(page));
			Control.ViewControllers = items.ToArray();
			Control.CustomizableViewControllers = null;
		}

		public void ClearTabs()
		{
			items.Clear();
			Control.ViewControllers = items.ToArray();
			Control.CustomizableViewControllers = null;
		}

		public void RemoveTab(int index, TabPage page)
		{
			items.RemoveAt(index);
			Control.ViewControllers = items.ToArray();
			Control.CustomizableViewControllers = null;
		}

		public int SelectedIndex
		{
			get { return (int)Control.SelectedIndex; }
			set
			{
				Control.SelectedIndex = value;
			}
		}

		public DockPosition TabPosition
		{
			get { return DockPosition.Bottom; }
			set
			{
				Debug.WriteLine("Warning: TabControl.TabPosition is not supported");
			}
		}
	}
}
