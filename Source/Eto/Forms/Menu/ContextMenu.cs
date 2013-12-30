using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface IContextMenu : ISubMenu
	{
		void Show(Control relativeTo);
	}

	public interface IContextMenuHost
	{
		ContextMenu ContextMenu { get; set; }
	}

	public class ContextMenu : Menu, ISubMenuWidget
	{
		new IContextMenu Handler { get { return (IContextMenu)base.Handler; } }

		public MenuItemCollection Items { get; private set; }

		[Obsolete("Use Items instead")]
		public MenuItemCollection MenuItems { get { return Items; } } 

		public ContextMenu()
			: this((Generator)null)
		{
		}

		public ContextMenu(Generator generator) : this(generator, typeof(IContextMenu))
		{
		}

		protected ContextMenu(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Items = new MenuItemCollection(Handler);
		}

		public ContextMenu(Generator g, IEnumerable<MenuItem> items) : this(g)
		{
			Items.AddRange(items);
		}

		public void Show(Control relativeTo)
		{
			Handler.Show(relativeTo);
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