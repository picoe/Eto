using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

#if XAML
using System.Windows.Markup;

#endif
namespace Eto.Forms
{
	[ContentProperty("Rows")]
	public class DynamicLayout : Panel
	{
		DynamicTable topTable;
		DynamicTable currentItem;
		bool? yscale;

		public List<DynamicRow> Rows
		{
			get { return topTable.Rows; }
		}

		[Obsolete("Use DynamicLayout directly as a control")]
		public Container Container
		{
			get { return this.Parent; }
		}

		public bool Generated { get; private set; }

		public Padding? Padding
		{
			get { return topTable.Padding; }
			set { topTable.Padding = value; }
		}

		public Size? Spacing
		{
			get { return topTable.Spacing; }
			set { topTable.Spacing = value; }
		}

		public Padding? DefaultPadding { get; set; }

		public Size? DefaultSpacing { get; set; }

		public override IEnumerable<Control> Controls
		{
			get
			{
				if (topTable.Table != null)
					yield return topTable.Table;
			}
		}

		#region Exceptions
		[Serializable]
		public class AlreadyGeneratedException : Exception
		{
			public AlreadyGeneratedException()
			{
			}

			public AlreadyGeneratedException(string message) : base (message)
			{
			}

			public AlreadyGeneratedException(string message, Exception inner) : base (message, inner)
			{
			}

			protected AlreadyGeneratedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
			{
			}
		}
		#endregion
		public override void OnPreLoad(EventArgs e)
		{
			if (!Generated)
				this.Generate();

			base.OnPreLoad(e);
		}

		public override void OnLoad(EventArgs e)
		{
			if (!Generated)
				this.Generate();

			base.OnLoad(e);
		}

		public DynamicLayout()
			: this((Padding?)null, (Size?)null, (Generator)null)
		{
		}

		public DynamicLayout(Padding? padding, Size? spacing = null, Generator generator = null)
			: base(generator)
		{
			topTable = new DynamicTable
			{ 
				Padding = padding, 
				Spacing = spacing
			};
			currentItem = topTable;
		}

		[Obsolete("Add the DynamicLayout to the container using its DockContainer.Content property")]
		public DynamicLayout(DockContainer container, Size? spacing)
			: this (container, null, spacing)
		{
		}

		[Obsolete("Add the DynamicLayout to the container using its DockContainer.Content property")]
		public DynamicLayout(DockContainer container, Padding? padding = null, Size? spacing = null)
			: this (padding, spacing, container != null ? container.Generator : null)
		{
			if (container != null)
				container.Content = this;
		}

		public DynamicTable  BeginVertical(bool xscale, bool? yscale = null)
		{
			return BeginVertical(null, null, xscale, yscale);
		}

		public DynamicTable BeginVertical(Size spacing, bool? xscale = null, bool? yscale = null)
		{
			return BeginVertical(null, spacing, xscale, yscale);
		}

		public DynamicTable BeginVertical(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			var newItem = new DynamicTable
			{ 
				Parent = currentItem ?? topTable, 
				Padding = padding, 
				Spacing = spacing,
				XScale = xscale,
				YScale = yscale
			};
			currentItem.Add(newItem);
			currentItem = newItem;
			return newItem;
		}

		public void EndVertical()
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			currentItem = currentItem.Parent ?? topTable;
		}

		public DynamicTable EndBeginVertical(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			EndVertical();
			return BeginVertical(padding, spacing, xscale, yscale);
		}

		public DynamicRow EndBeginHorizontal(bool? yscale = null)
		{
			EndHorizontal();
			return BeginHorizontal(yscale);
		}

		public DynamicRow BeginHorizontal(bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			currentItem.AddRow(currentItem.CurrentRow = new DynamicRow());
			this.yscale = yscale;
			return currentItem.CurrentRow;
		}

		public void EndHorizontal()
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			if (currentItem.CurrentRow == null)
				EndVertical();
			else 
				currentItem.CurrentRow = null;
		}

		public DynamicControl Add(Control control, bool? xscale = null, bool? yscale = null)
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			if (xscale == null && currentItem.CurrentRow != null && control == null)
				xscale = true;
			yscale = yscale ?? this.yscale;
			if (yscale == null && currentItem.CurrentRow == null && control == null)
				yscale = true;
			var dynamicControl = new DynamicControl { Control = control, XScale = xscale, YScale = yscale };
			currentItem.Add(dynamicControl);
			return dynamicControl;
		}

		public DynamicRow AddSeparateRow(params Control[] controls)
		{
			var row = AddSeparateRow();
			row.Add(controls);
			return row;
		}

		public DynamicRow AddSeparateRow(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			this.BeginVertical(padding, spacing, xscale, yscale);
			var row = this.AddRow();
			this.EndVertical();
			return row;
		}

		public DynamicRow AddRow(params Control[] controls)
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			if (controls == null)
				controls = new Control[] { null };
			
			var row = new DynamicRow(controls);
			currentItem.AddRow(row);
			currentItem.CurrentRow = null;
			return row;
		}

		public void AddCentered(Control control, bool? xscale, bool? yscale = null)
		{
			AddCentered(control, Drawing.Padding.Empty, Size.Empty, xscale, yscale, true, true);
		}

		public void AddCentered(Control control, Size spacing, bool? xscale = null, bool? yscale = null)
		{
			AddCentered(control, Drawing.Padding.Empty, spacing, xscale, yscale, true, true);
		}

		public void AddCentered(Control control, Padding padding, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			AddCentered(control, padding, spacing, xscale, yscale, true, true);
		}

		public void AddCentered(Control control, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null, bool horizontalCenter = true, bool verticalCenter = true)
		{
			this.BeginVertical(padding ?? Drawing.Padding.Empty, spacing ?? Size.Empty, xscale, yscale);
			if (verticalCenter)
				this.Add(null, null, true);
			
			this.BeginHorizontal();
			if (horizontalCenter)
				this.Add(null, true, null);
			
			this.Add(control);
			
			if (horizontalCenter)
				this.Add(null, true, null);
			
			this.EndHorizontal();
			
			if (verticalCenter)
				this.Add(null, null, true);
			this.EndVertical();
			
		}

		public void AddAutoSized(Control control, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null, bool centered = false)
		{
			this.BeginVertical(padding ?? Eto.Drawing.Padding.Empty, spacing ?? Size.Empty, xscale, yscale);
			if (centered)
			{
				this.Add(null);
				this.AddRow(null, control, null);
			}
			else
				this.AddRow(control, null);
			this.Add(null);
			this.EndVertical();
		}

		public void AddColumn(params Control[] controls)
		{
			BeginVertical();
			foreach (var control in controls)
				Add(control);
			EndVertical();
		}

		/// <summary>
		/// Generates the layout for the container
		/// </summary>
		/// <remarks>
		/// This is called automatically on the Container's LoadCompleted event, but can be called manually if needed.
		/// </remarks>
		/// <exception cref="AlreadyGeneratedException">specifies that the control was already generated</exception>
		public void Generate()
		{
			if (Generated)
				throw new AlreadyGeneratedException();
			this.Content = topTable.Generate(this);
			Generated = true;
		}
	}
}