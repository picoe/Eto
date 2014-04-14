using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IMenuBar : IMenu, ISubMenu
	{
	}

	public class MenuBar : Menu, ISubMenuWidget
	{
		new IMenuBar Handler { get { return (IMenuBar)base.Handler; } }

		public MenuItemCollection Items { get; private set; }

		public MenuBar()
			: this((Generator)null)
		{
		}

		public MenuBar(Generator generator) : this(generator, typeof(IMenuBar))
		{
		}

		protected MenuBar(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Items = new MenuItemCollection(Handler);
		}

		public MenuBar(Generator g, IEnumerable<MenuItem> items) : this(g)
		{
			Items.AddRange(items);
		}

		internal protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}

		internal protected override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			foreach (var item in Items)
				item.OnLoad(e);
		}
	}
}