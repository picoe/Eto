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
	[aa.Activity]
	public class EtoNavigationActivity : aa.Activity
	{
		INavigationItem content;

		protected override void OnCreate(ao.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var key = Intent.GetStringExtra("item");

			content = NavigationHandler.GetItem(key);
			if (content != null && content.Content != null)
			{
				SetContentView(content.Content.GetContainerView());
			}
		}
	}

	public class NavigationHandler : AndroidContainer<aw.FrameLayout, Navigation, Navigation.ICallback>, Navigation.IHandler
	{
		static Dictionary<string, INavigationItem> itemsLookup = new Dictionary<string, INavigationItem>();

		public static INavigationItem GetItem(string key)
		{
			INavigationItem item;
			return itemsLookup.TryGetValue(key, out item) ? item : null;
		}

		readonly Stack<INavigationItem> items = new Stack<INavigationItem>();

		public override av.View ContainerControl { get { return Control; } }

		public NavigationHandler()
		{
			Control = new aw.FrameLayout(aa.Application.Context);
		}

		public void Push(INavigationItem item)
		{
			if (items.Count > 0)
			{
				var intent = new ac.Intent(Control.Context, typeof(EtoNavigationActivity));
				var key = Guid.NewGuid().ToString();
				itemsLookup.Add(key, item);
				intent.PutExtra("item", key);
				intent.SetFlags(ac.ActivityFlags.NewTask);
				aa.Application.Context.StartActivity(intent);
			}
			else
			{
				SetContent(item.Content);
			}
			items.Push(item);
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