using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Collections;

namespace Eto.Forms
{
	public interface IMenuItem : IMenu
	{
	}
	
	public class MenuItem : Menu
	{
		public MenuItem(Generator g, Type type) : base(g, type)
		{
		}
	}


	public class MenuItemCollection : BaseList<MenuItem>
	{
		Menu parent;
		internal protected MenuItemCollection(Menu parent)
		{
			this.parent = parent;
		}

		protected override void OnAdded (ListEventArgs<MenuItem> e)
		{
			base.OnAdded (e);
			parent.AddMenu(IndexOf(e.Item), e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<MenuItem> e)
		{
			base.OnRemoved (e);
			parent.RemoveMenu(e.Item);
		}
		
		public override void Clear()
		{
			base.Clear ();
			parent.Clear();
		}

		public MenuItem Add(string text)
		{
			var item = new ImageMenuItem(parent.Generator);
			item.Text = text;
			Add(item);
			return item;
		}
	}
}
