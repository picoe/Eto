using System;
using Eto.Forms;
using Eto.Drawing;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android
{
	internal class ButtonMenuItemHandler : MenuItemHandler<av.IMenuItem, ButtonMenuItem, ButtonMenuItem.ICallback>, ButtonMenuItem.IHandler
	{
		public ButtonMenuItemHandler()
		{
		}

		public override void CreateControl(av.IMenu androidMenu, Int32 index)
		{
			Control = androidMenu.Add(av.Menu.None, index, index, new Java.Lang.String(Text));
		}

		public Image Image
		{
			get;
			set;
		}

		public void AddMenu(System.Int32 index, MenuItem item)
		{
			//throw new System.NotImplementedException();
		}

		public void RemoveMenu(MenuItem item)
		{
			//throw new System.NotImplementedException();
		}

		public void Clear()
		{
			//throw new System.NotImplementedException();
		}

		public override void PerformClick()
		{
			Callback?.OnClick(Widget, EventArgs.Empty);
		}
	}
}
