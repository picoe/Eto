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

	public interface IToolBar : IInstanceWidget
	{
		void AddButton(ToolItem button, int index);

		void RemoveButton(ToolItem button);

		void Clear();

		ToolBarTextAlign TextAlign { get; set; }

		ToolBarDock Dock { get; set; }
	}

	public class ToolBar : InstanceWidget
	{
		internal new IToolBar Handler { get { return (IToolBar)base.Handler; } }

		readonly ToolItemCollection items;

		public ToolBar()
			: this((Generator)null)
		{
		}

		public ToolBar(Generator generator) : base(generator, typeof(IToolBar))
		{
			items = new ToolItemCollection(this);
		}

		public ToolBarDock Dock
		{
			get { return Handler.Dock; }
			set { Handler.Dock = value; }
		}

		public ToolItemCollection Items
		{
			get { return items; }
		}

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
	}
}
