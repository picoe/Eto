using System;
using System.Collections;
using System.Collections.ObjectModel;

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
		void AddButton(ToolBarItem button);
		void RemoveButton(ToolBarItem button);
		void Clear();
		ToolBarTextAlign TextAlign { get; set; }
		ToolBarDock Dock { get; set; }
	}
	
	/// <summary>
	/// Summary description for ToolBar.
	/// </summary>
	public class ToolBar : InstanceWidget
	{
		IToolBar inner;
		ToolbarItemCollection items;
	
		public class ToolbarItemCollection : Collection<ToolBarItem>
		{
			ToolBar toolBar;
			
			protected internal ToolbarItemCollection(ToolBar toolBar)
			{
				this.toolBar = toolBar;
			}
			
			protected override void InsertItem (int index, ToolBarItem item)
			{
				base.InsertItem (index, item);
				toolBar.inner.AddButton (item);
			}
			
			protected override void RemoveItem (int index)
			{
				var item = this[index];
				base.RemoveItem (index);
				toolBar.inner.RemoveButton (item);
			}
			
			protected override void ClearItems ()
			{
				base.ClearItems ();
				toolBar.inner.Clear ();
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
