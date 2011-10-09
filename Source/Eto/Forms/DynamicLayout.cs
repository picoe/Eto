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

		public bool Generated { get; private set; }
		
		public Padding? DefaultPadding { get; set; }
		
		public Size? DefaultSpacing { get; set; }
		
		public Container Container { get; private set; }

		#region Exceptions
		
		[Serializable]
		public class AlreadyGeneratedException : Exception
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="T:AlreadyGeneratedException"/> class
			/// </summary>
			public AlreadyGeneratedException ()
			{
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="T:AlreadyGeneratedException"/> class
			/// </summary>
			/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
			public AlreadyGeneratedException (string message) : base (message)
			{
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="T:AlreadyGeneratedException"/> class
			/// </summary>
			/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
			/// <param name="inner">The exception that is the cause of the current exception. </param>
			public AlreadyGeneratedException (string message, Exception inner) : base (message, inner)
			{
			}
			
			/// <summary>
			/// Initializes a new instance of the <see cref="T:AlreadyGeneratedException"/> class
			/// </summary>
			/// <param name="context">The contextual information about the source or destination.</param>
			/// <param name="info">The object that holds the serialized object data.</param>
			protected AlreadyGeneratedException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
			{
			}
		}
		
		#endregion
		
		#region Dynamic Layout Items
		
		abstract class LayoutItem
		{
			public bool? XScale { get; set; }

			public bool? YScale { get; set; }

			public abstract Control Generate (DynamicLayout layout);
			
			public virtual void Generate (DynamicLayout layout, TableLayout parent, int x, int y)
			{
				var c = Generate (layout);
				if (c != null)
					parent.Add (c, x, y);
				if (XScale != null)
					parent.SetColumnScale (x, XScale.Value);
				if (YScale != null)
					parent.SetRowScale (y, YScale.Value);
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
			
			public void Add (LayoutItem item)
			{
				if (CurrentRow != null)
					CurrentRow.Add (item);
				else
					AddRow (item);
			}
			
			public void AddRow (LayoutItem item)
			{
				var row = new List<LayoutItem> ();
				row.Add (item);
				rows.Add (row);
			}
			
			public void AddRow (List<LayoutItem> row)
			{
				rows.Add (row);
			}
			
			public override Control Generate (DynamicLayout layout)
			{
				if (rows.Count == 0)
					return null;
				int cols = rows.Max (r => r.Count);
				if (Container == null)
					Container = new Panel ();
				var tableLayout = new TableLayout (Container, cols, rows.Count);
				var padding = this.Padding ?? layout.DefaultPadding;
				if (padding != null)
					tableLayout.Padding = padding.Value;
				
				var spacing = this.Spacing ?? layout.DefaultSpacing;
				if (spacing != null)
					tableLayout.Spacing = spacing.Value;
				
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
		
		#endregion
		
		public DynamicLayout (Container container, Padding? padding = null, Size? spacing = null)
		{
			this.Container = container;
			topTable = new TableItem{ 
				Container = container, 
				Padding = padding, 
				Spacing = spacing
			};
			currentItem = topTable;
			container.Load += HandleContainerLoad;
		}

		void HandleContainerLoad (object sender, EventArgs e)
		{
			if (!Generated)
				this.Generate ();
		}

		public void BeginVertical (bool xscale, bool? yscale = null)
		{
			BeginVertical (null, null, xscale, yscale);
		}
		
		public void BeginVertical (Size spacing, bool? xscale = null, bool? yscale = null)
		{
			BeginVertical (null, spacing, xscale, yscale);
		}
		
		public void BeginVertical (Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
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
			if (Generated)
				throw new AlreadyGeneratedException ();
			currentItem = currentItem.Parent ?? topTable;
		}
		
		public void BeginHorizontal (bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
			currentItem.AddRow (currentItem.CurrentRow = new List<LayoutItem> ());
			this.yscale = yscale;
		}
		
		public void EndHorizontal ()
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
			if (currentItem.CurrentRow == null)
				EndVertical ();
			else 
				currentItem.CurrentRow = null;
		}
		
		public void Add (Control control, bool? xscale = null, bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
			currentItem.Add (new ControlItem{ Control = control, XScale = xscale, YScale = yscale ?? this.yscale});
		}
		
		public void AddRow (params Control[] controls)
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
			var items = controls.Select (r => new ControlItem{ Control = r, YScale = yscale });
			var row = new List<LayoutItem> (items.Cast<LayoutItem>());
			currentItem.AddRow (row);
			currentItem.CurrentRow = null;
		}

		public void AddCentered (Control control, bool xscale, bool? yscale = null)
		{
			AddCentered (control, true, true, null, null, xscale, yscale);
		}

		public void AddCentered (Control control, Size spacing, bool? xscale = null, bool? yscale = null)
		{
			AddCentered (control, true, true, null, spacing, xscale, yscale);
		}
		
		public void AddCentered (Control control, Padding padding, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			AddCentered (control, true, true, padding, spacing, xscale, yscale);
		}

		public void AddCentered (Control control, bool horizontalCenter = true, bool verticalCenter = true, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			this.BeginVertical (padding, spacing, xscale, yscale);
			if (verticalCenter)
				this.Add (null, null, true);
			
			this.BeginHorizontal ();
			if (horizontalCenter)
				this.Add (null, true, null);
			
			this.Add (control);
			
			if (horizontalCenter)
				this.Add (null, true, null);
			
			this.EndHorizontal ();
			
			if (verticalCenter)
				this.Add (null, null, true);
			this.EndVertical ();
			
		}
		
		public void AddColumn (params Control[] controls)
		{
			BeginVertical ();
			foreach (var control in controls)
				Add (control);
			EndVertical ();
		}
		
		/// <summary>
		/// Generates the layout for the container
		/// </summary>
		/// <remarks>
		/// This is called automatically on the Container's LoadCompleted event, but can be called manually if needed.
		/// </remarks>
		/// <exception cref="AlreadyGeneratedException">specifies that the control was already generated</exception>
		public void Generate ()
		{
			if (Generated)
				throw new AlreadyGeneratedException ();
			topTable.Generate (this);
			Generated = true;
		}
	}
}