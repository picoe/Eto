using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoTouch.UIKit;
using System.Linq;
using Eto.Mac.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class TabPageHandler : MacPanel<UIViewController, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		protected override UIViewController CreateController()
		{
			return Control;
		}

		public TabPageHandler()
		{
			Control = new UIViewController();
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
	}
}
