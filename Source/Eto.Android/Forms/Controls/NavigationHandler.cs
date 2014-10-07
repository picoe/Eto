using System;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Android.Forms.Controls
{
	public class NavigationHandler : AndroidContainer<aw.FrameLayout, Navigation, Navigation.ICallback>, Navigation.IHandler
	{
		readonly Stack<INavigationItem> items = new Stack<INavigationItem>();

		public override av.View ContainerControl { get { return Control; } }

		public NavigationHandler()
		{
			Control = new aw.FrameLayout(aa.Application.Context);
		}

		public void Push(INavigationItem item)
		{
			items.Push(item);
			SetContent(item.Content);
		}

		public void Pop()
		{
			var item = items.Pop();
			SetContent(item.Content);
		}

		void SetContent(Control content)
		{
			Control.RemoveAllViews();

			var view = content.GetContainerView();
			if (view != null)
			{
				view.LayoutParameters = new av.ViewGroup.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.MatchParent);
				Control.AddView(view);
			}
		}
	}
}