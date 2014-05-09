using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace Eto.Forms
{
	public enum ToolBarTextAlign
	{
		Right,
		Underneath
	}

	public enum ToolBarDock
	{
		Top,
		Bottom
	}

	[Handler(typeof(ToolBar.IHandler))]
	public class ToolBar : Widget
	{
		internal new IHandler Handler { get { return (IHandler)base.Handler; } }

		ToolItemCollection items;

		public ToolBar()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ToolBar(Generator generator) : base(generator, typeof(IHandler))
		{
		}

		public ToolBarDock Dock
		{
			get { return Handler.Dock; }
			set { Handler.Dock = value; }
		}

		public ToolItemCollection Items { get { return items ?? (items = new ToolItemCollection(this)); } }

		public ToolBarTextAlign TextAlign
		{
			get { return Handler.TextAlign; }
			set { Handler.TextAlign = value; }
		}

		internal protected virtual void OnLoad(EventArgs e)
		{
			foreach (var item in Items)
				item.OnLoad(e);
		}

		internal protected virtual void OnUnLoad(EventArgs e)
		{
			foreach (var item in Items)
				item.OnLoad(e);
		}

		public interface IHandler : Widget.IHandler
		{
			void AddButton(ToolItem button, int index);

			void RemoveButton(ToolItem button);

			void Clear();

			ToolBarTextAlign TextAlign { get; set; }

			ToolBarDock Dock { get; set; }
		}

	}
}
