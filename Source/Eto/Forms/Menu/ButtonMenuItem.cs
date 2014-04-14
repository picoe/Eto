using System;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	public interface IButtonMenuItem : IMenuItem, ISubMenu
	{
		Image Image { get; set; }
	}

	public class ButtonMenuItem : MenuItem, ISubMenuWidget
	{
		new IButtonMenuItem Handler { get { return (IButtonMenuItem)base.Handler; } }

		public MenuItemCollection Items { get; private set; }

		public bool Trim { get; set; }

		public ButtonMenuItem()
			: this((Generator)null)
		{
		}

		public ButtonMenuItem(Generator generator)
			: this(generator, typeof(IButtonMenuItem))
		{
		}

		public ButtonMenuItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IButtonMenuItem))
		{
			Items = new MenuItemCollection(Handler);
			Image = command.Image;
		}

		protected ButtonMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
			Items = new MenuItemCollection(Handler);
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
	}
}