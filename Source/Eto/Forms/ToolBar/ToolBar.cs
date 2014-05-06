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

	public interface IToolBar : IWidget
	{
		void AddButton(ToolItem button, int index);

		void RemoveButton(ToolItem button);

		void Clear();

		ToolBarTextAlign TextAlign { get; set; }

		ToolBarDock Dock { get; set; }
	}

	[Handler(typeof(IToolBar))]
	public class ToolBar : Widget
	{
		internal new IToolBar Handler { get { return (IToolBar)base.Handler; } }

		ToolItemCollection items;

		public ToolBar()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ToolBar(Generator generator) : base(generator, typeof(IToolBar))
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
	}
}
