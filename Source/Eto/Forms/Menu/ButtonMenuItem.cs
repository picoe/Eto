using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Menu item for a button / submenu
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(ButtonMenuItem.IHandler))]
	public class ButtonMenuItem : MenuItem, ISubmenu
	{
		MenuItemCollection items;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets the collection of menu items.
		/// </summary>
		/// <value>The items.</value>
		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler)); } }

		public bool Trim { get; set; }

		public ButtonMenuItem()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ButtonMenuItem(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		public ButtonMenuItem(Command command)
			: base(command)
		{
			Image = command.Image;
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use constructor without generator instead")]
		public ButtonMenuItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IHandler))
		{
			Image = command.Image;
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ButtonMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public Image Image
		{
			get { return Handler.Image; }
			set { Handler.Image = value; }
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

		public new interface IHandler : MenuItem.IHandler, ISubmenuHandler
		{
			Image Image { get; set; }
		}
	}
}