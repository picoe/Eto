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

	[Handler(typeof(IButtonMenuItem))]
	public class ButtonMenuItem : MenuItem, ISubMenuWidget
	{
		MenuItemCollection items;

		new IButtonMenuItem Handler { get { return (IButtonMenuItem)base.Handler; } }

		public MenuItemCollection Items { get { return items ?? (items = new MenuItemCollection(Handler)); } }

		public bool Trim { get; set; }

		public ButtonMenuItem()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ButtonMenuItem(Generator generator)
			: this(generator, typeof(IButtonMenuItem))
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
			: base(command, generator, typeof(IButtonMenuItem))
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
	}
}