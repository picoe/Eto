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
		new IToolBar Handler { get { return (IToolBar)base.Handler; } }
		readonly ToolbarItemCollection items;
	
		public class ToolbarItemCollection : Collection<ToolBarItem>
		{
			readonly ToolBar toolBar;
			
			protected internal ToolbarItemCollection(ToolBar toolBar)
			{
				this.toolBar = toolBar;
			}
			
			protected override void InsertItem (int index, ToolBarItem item)
			{
				base.InsertItem (index, item);
				toolBar.Handler.AddButton (item);
			}
			
			protected override void RemoveItem (int index)
			{
				var item = this[index];
				base.RemoveItem (index);
				toolBar.Handler.RemoveButton (item);
			}
			
			protected override void ClearItems ()
			{
				base.ClearItems ();
				toolBar.Handler.Clear ();
			}
		}
		
		public ToolBar() : this(Generator.Current)
		{
			
		}
		
		public ToolBar(Generator g) : base(g, typeof(IToolBar))
		{
			items = new ToolbarItemCollection(this);
		}
		
		public ToolBarDock Dock
		{
			get { return Handler.Dock; }
			set { Handler.Dock = value; }
		}

		public ToolbarItemCollection Items
		{
			get { return items; }
		}

		public ToolBarTextAlign TextAlign
		{
			get { return Handler.TextAlign; }
			set { Handler.TextAlign = value; }
		}

	}
}
