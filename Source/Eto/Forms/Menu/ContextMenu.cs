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

	[Handler(typeof(IContextMenu))]
	public class ContextMenu : Menu, ISubMenuWidget
	{
		MenuItemCollection items;

		new IContextMenu Handler { get { return (IContextMenu)base.Handler; } }

		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler)); } }

		public ContextMenu()
		{
		}

		public ContextMenu(IEnumerable<MenuItem> items)
		{
			Items.AddRange(items);
		}

		public ContextMenu(params MenuItem[] items)
		{
			Items.AddRange(items);
		}

		[Obsolete("Use default constructor instead")]
		public ContextMenu(Generator generator) : this(generator, typeof(IContextMenu))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ContextMenu(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		[Obsolete("Use constructor without generator instead")]
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