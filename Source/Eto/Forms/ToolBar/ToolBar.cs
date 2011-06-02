using System;
using System.Collections;
using Eto.Collections;

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
	
	public interface IToolBar : IControl
	{
		void AddButton(ToolBarItem button);
		void RemoveButton(ToolBarItem button);
		void Clear();
		ToolBarTextAlign TextAlign { get; set; }
		ToolBarDock Dock { get; set; }
	}
	
	/// <summary>
	/// Summary description for ToolBar.
	/// </summary>
	public class ToolBar : Control
	{
		IToolBar inner;
		ToolbarItemCollection items;
	
		public class ToolbarItemCollection : BaseList<ToolBarItem>
		{
			ToolBar toolBar;
			protected internal ToolbarItemCollection(ToolBar toolBar)
			{
				this.toolBar = toolBar;
			}

			protected override void OnAdded (ListEventArgs<ToolBarItem> e)
			{
				base.OnAdded (e);
				((IToolBar)toolBar.Handler).AddButton(e.Item);
			}
			
			protected override void OnRemoved (ListEventArgs<ToolBarItem> e)
			{
				base.OnRemoved (e);
				((IToolBar)toolBar.Handler).RemoveButton(e.Item);
			}
			
			public override void Clear()
			{
				base.Clear ();
				((IToolBar)toolBar.Handler).Clear();
			}

		}
		
		public ToolBar() : this(Generator.Current)
		{
			
		}
		
		public ToolBar(Generator g) : base(g, typeof(IToolBar))
		{
			inner = (IToolBar)Handler;
			items = new ToolbarItemCollection(this);
		}
		
		public ToolBarDock Dock
		{
			get { return inner.Dock; }
			set { inner.Dock = value; }
		}

		public ToolbarItemCollection Items
		{
			get { return items; }
		}

		public ToolBarTextAlign TextAlign
		{
			get { return inner.TextAlign; }
			set { inner.TextAlign = value; }
		}

	}
}
