using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.Forms
{
	public class DynamicLayout
	{
		TableItem topTable;
		TableItem currentItem;
		bool? yscale;
		bool generated;
		
		public Padding? DefaultPadding { get; set; }
		
		public Size? DefaultSpacing { get; set; }
		
		public Container Container { get; private set; }
		
		abstract class LayoutItem
		{
			public bool? XScale { get; set; }

			public bool? YScale { get; set; }

			public abstract Control Generate (DynamicLayout layout);
			
			public virtual void Generate (DynamicLayout layout, TableLayout parent, int x, int y)
			{
				var c = Generate (layout);
				if (c != null) {
					if (XScale != null) parent.SetColumnScale (x, XScale.Value);
					if (YScale != null) parent.SetRowScale (y, YScale.Value);
					parent.Add (c, x, y);
				}
			}
		}
		
		class TableItem : LayoutItem
		{
			List<List<LayoutItem>> rows = new List<List<LayoutItem>> ();
			
			public TableItem Parent { get; set; }
			
			public Padding? Padding { get; set; }
			
			public Size? Spacing { get; set; }
			
			public List<LayoutItem> CurrentRow { get; set; }
			
			public Container Container { get; set; }
			
			public void Add(LayoutItem item)
			{
				if (CurrentRow != null)
					CurrentRow.Add (item);
				else
					AddRow (item);
			}
			
			public void AddRow (LayoutItem item)
			{
				var row = new List<LayoutItem>();
				row.Add (item);
				rows.Add (row);
			}
			
			public void AddRow (List<LayoutItem> row)
			{
				rows.Add (row);
			}
			
			public override Control Generate (DynamicLayout layout)
			{
				if (rows.Count == 0) return null;
				int cols = rows.Max (r => r.Count);
				if (Container == null)
					Container = new Panel ();
				var tableLayout = new TableLayout (Container, cols, rows.Count);
				var padding = this.Padding ?? layout.DefaultPadding;
				if (padding != null) tableLayout.Padding = padding.Value;
				
				var spacing = this.Spacing ?? layout.DefaultSpacing;
				if (spacing != null) tableLayout.Spacing = spacing.Value;
				
				for (int cy=0; cy<rows.Count; cy++) {
					var row = rows [cy];
					for (int cx=0; cx<row.Count; cx++) {
						var item = row [cx];
						item.Generate (layout, tableLayout, cx, cy);
					}
				}
				return Container;
			}
		}
		
		class ControlItem : LayoutItem
		{
			public override Control Generate (DynamicLayout layout)
			{
				return Control;
			}
			
			public Control Control { get; set; }
		}
		
		public DynamicLayout (Container container, Padding? padding = null, Size? spacing = null)
		{
			this.Container = container;
			topTable = new TableItem{ 
				Container = container, 
				Padding = padding, 
				Spacing = spacing
			};
			currentItem = topTable;
			container.LoadComplete += HandleContainerLoadComplete;
		}

		void HandleContainerLoadComplete (object sender, EventArgs e)
		{
			if (!generated)
				this.Generate ();
		}

		public void BeginVertical (Size spacing, bool? xscale = null, bool? yscale = null)
		{
			BeginVertical (null, spacing, xscale, yscale);
		}
		
		public void BeginVertical (Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			var newItem = new TableItem{ 
				Parent = currentItem ?? topTable, 
				Padding = padding, 
				Spacing = spacing,
				XScale = xscale,
				YScale = yscale
			};
			currentItem.Add (newItem);
			currentItem = newItem;
		}
		
		public void EndVertical ()
		{
			currentItem = currentItem.Parent ?? topTable;
		}
		
		public void BeginHorizontal (bool? yscale = null)
		{
			currentItem.AddRow (currentItem.CurrentRow = new List<LayoutItem> ());
			this.yscale = yscale;
		}
		
		public void EndHorizontal ()
		{
			if (currentItem.CurrentRow == null)
				EndVertical ();
			else 
				currentItem.CurrentRow = null;
		}
		
		public void Add (Control control, bool? xscale = null, bool? yscale = null)
		{
			currentItem.Add (new ControlItem{ Control = control, XScale = xscale, YScale = yscale ?? this.yscale});
		}
		
		public void AddRow (params Control[] controls)
		{
			var row = new List<LayoutItem>(controls.Select (r => new ControlItem{ Control = r, YScale = yscale }));
			currentItem.AddRow (row);
			currentItem.CurrentRow = null;
		}
		
		public void AddColumn (params Control[] controls)
		{
			BeginVertical ();
			foreach (var control in controls)
				Add (control);
			EndVertical ();
		}
		
		public Control Generate ()
		{
			var control = topTable.Generate (this);
			generated = true;
			return control;
		}
	}
}