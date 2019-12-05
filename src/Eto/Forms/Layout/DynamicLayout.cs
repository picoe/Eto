using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Collections.ObjectModel;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Dynamic and extensible layout
	/// </summary>
	/// <remarks>
	/// The dynamic layout allows you to build a complex structure of controls easily. The core functionality allows 
	/// you to build a hierarchical set of tables with rows and columns of controls.
	/// 
	/// A vertical section begins a new table, whereas a horizontal section refers to a row in a table.
	/// The dynamic layout intrinsically starts with a vertical section.
	/// 
	/// You can define your layout verbosely or succinctly as you see fit.  The Begin.../End... methods allow you define
	/// the vertical/horizontal sections individually with separate commands, whereas you can also create a layout entirely 
	/// with a constructor or initializer pattern using the <see cref="Rows"/>, <see cref="DynamicTable.Rows"/>, and <see cref="DynamicRow.Items"/>
	/// properties.
	/// 
	/// To learn about how scaling works, see <see cref="TableLayout"/>
	/// </remarks>
	/// <example>
	/// This example uses the verbose methods creating sections in separate statements
	/// <code>
	/// 	var layout = new DynamicLayout();
	/// 	
	/// 	layout.BeginHorizontal();
	/// 	layout.Add(new Label { Text = "My Label" });
	/// 	layout.Add(new TextBox());
	/// 	layout.EndHorizontal();
	/// 
	/// 	layout.BeginHorizontal()
	/// 	layout.Add(new Label { Text = "Vertical controls:" });
	/// 	
	/// 	layout.BeginVertical(padding: new Padding(10));
	/// 	layout.Add(new TextBox());
	/// 	layout.Add(new Label { Text = "Some text below the text box" });
	/// 	layout.EndVertical();
	/// 
	/// 	layout.EndHorizontal();
	/// </code>
	/// 
	/// This example uses the constructor method:
	/// <code>
	/// 	var layout = new DynamicLayout(
	/// 		new DynamicRow(
	/// 			new Label { Text = "My Label" },
	/// 			new TextBox()
	/// 		),
	/// 		new DynamicRow(
	/// 			new Label { Text = "Vertical controls:" },
	/// 			new DynamicTable(
	/// 				new TextBox(),
	/// 				new Label { Text = "Some text below the text box" }
	/// 			) { Padding = new Padding(10) }
	/// 		)
	/// 	);
	/// </code>
	/// 
	/// And finally the initializer pattern, which allows you to set other properties of rows/tables cleaner, such as padding/spacing/scaling:
	/// <code>
	///		var layout = new DynamicLayout { Rows = {
	///			new DynamicRow { Items = {
	///				new Label { Text = "My Label" },
	///				new TextBox()
	///			} },
	///			new DynamicRow { Items = {
	///				new Label { Text = "Vertical controls:" },
	///				new DynamicTable { Padding = new Padding(10), Rows = {
	///					new TextBox(),
	///					new Label { Text = "Some text below the text box" }
	///				} }
	///			} }
	///		} };
	/// </code>
	/// </example>
	[ContentProperty("Rows")]
	public class DynamicLayout : Panel
	{
		readonly DynamicTable topTable;
		DynamicTable currentItem;
		Stack<bool?> yscales = new Stack<bool?>();

		/// <summary>
		/// Gets or sets the top level rows in the layout
		/// </summary>
		/// <value>The rows.</value>
		public Collection<DynamicRow> Rows
		{
			get { return topTable.Rows; }
		}

		/// <summary>
		/// Gets a value indicating whether the layout has been created
		/// </summary>
		/// <remarks>
		/// The layout automatically will be created during the <see cref="OnPreLoad"/> or <see cref="OnLoad"/>
		/// </remarks>
		/// <value><c>true</c> if the layout is created; otherwise, <c>false</c>.</value>
		public bool IsCreated { get; private set; }

		/// <summary>
		/// Gets or sets the padding around the entire content of the layout
		/// </summary>
		/// <value>The padding.</value>
		/// <seealso cref="DefaultPadding"/>
		public new Padding? Padding
		{
			get { return topTable.Padding; }
			set { topTable.Padding = value; }
		}

		/// <summary>
		/// Gets or sets the spacing between the first level of cells
		/// </summary>
		/// <seealso cref="DefaultSpacing"/>
		/// <value>The spacing.</value>
		public Size? Spacing
		{
			get { return topTable.Spacing; }
			set { topTable.Spacing = value; }
		}

		/// <summary>
		/// Gets or sets the default padding for all child <see cref="DynamicTable"/> instances (vertical sections)
		/// </summary>
		/// <value>The default padding for each vertical section.</value>
		public Padding? DefaultPadding { get; set; }

		/// <summary>
		/// Gets or sets the default spacing for all cells in the layout
		/// </summary>
		/// <value>The default spacing.</value>
		public Size? DefaultSpacing { get; set; }

		/// <summary>
		/// Gets an enumeration of controls that are directly contained by this container
		/// </summary>
		/// <value>The contained controls.</value>
		public override IEnumerable<Control> Controls
		{
			get { return topTable.Controls; }
		}

		/// <summary>
		/// Gets an enumeration of controls that are in the visual tree.
		/// </summary>
		/// <remarks>This is used to specify which controls are contained by this instance that are part of the visual tree.
		/// This should include all controls including non-logical Eto controls used for layout.</remarks>
		/// <value>The visual controls.</value>
		public override IEnumerable<Control> VisualControls
		{
			get
			{
				if (topTable.Table != null)
					yield return topTable.Table;
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.PreLoad"/> event, and creates the layout if it has not been created
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnPreLoad(EventArgs e)
		{
			if (!IsCreated)
				Create();

			base.OnPreLoad(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.Load"/> event, and creates the layout if it has not been created
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoad(EventArgs e)
		{
			if (!IsCreated)
				Create();

			base.OnLoad(e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicLayout"/> class.
		/// </summary>
		public DynamicLayout()
		{
			topTable = new DynamicTable();
			topTable.layout = this;
			currentItem = topTable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicLayout"/> class with the specified rows
		/// </summary>
		/// <param name="rows">Rows to populate the layout</param>
		public DynamicLayout(params DynamicRow[] rows)
			: this((IEnumerable<DynamicRow>)rows)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.DynamicLayout"/> class with the specified rows
		/// </summary>
		/// <param name="rows">Rows to populate the layout.</param>
		public DynamicLayout(IEnumerable<DynamicRow> rows)
		{
			topTable = new DynamicTable(rows);
			topTable.layout = this;
			currentItem = topTable;
		}

		/// <summary>
		/// Begins a new vertical section in the layout
		/// </summary>
		/// <remarks>
		/// After calling this method, each subsequent call to <see cref="Add"/> will add controls in a vertical orientation.
		/// 
		/// When finished adding controls to the vertical section, call either <see cref="EndVertical"/> or <see cref="EndBeginVertical"/>.
		/// </remarks>
		/// <returns>A new DynamicTable instance used for the vertical section</returns>
		/// <param name="padding">Padding to apply around the vertical section, or null to use the <see cref="DefaultPadding"/></param>
		/// <param name="spacing">Spacing to apply to cells in the vertical section, or null to use the <see cref="DefaultSpacing"/></param>
		/// <param name="xscale">Xscale of the vertical section</param>
		/// <param name="yscale">Yscale of the vertical section</param>
		public DynamicTable BeginVertical(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			var newItem = new DynamicTable
			{ 
				Padding = padding, 
				Spacing = spacing,
				XScale = xscale,
				YScale = yscale
			};
			currentItem.Add(newItem);
			currentItem = newItem;
			return newItem;
		}

		/// <summary>
		/// Ends the current vertical section
		/// </summary>
		/// <remarks>
		/// This should be balanced with every call to <see cref="BeginVertical(Padding?,Size?,bool?,bool?)"/>.
		/// Alternatively, you can call <see cref="EndBeginVertical"/> to end the current vertical section and start a new one.
		/// </remarks>
		public void EndVertical()
		{
			currentItem = currentItem.Parent ?? topTable;
		}

		/// <summary>
		/// Ends the current vertical section, then begins a new vertical section
		/// </summary>
		/// <remarks>
		/// When finished adding controls to the vertical section, call <see cref="EndVertical"/>
		/// </remarks>
		/// <returns>A new DynamicTable instance used for the vertical section</returns>
		/// <param name="padding">Padding to apply around the vertical section, or null to use the <see cref="DefaultPadding"/></param>
		/// <param name="spacing">Spacing to apply to cells in the vertical section, or null to use the <see cref="DefaultSpacing"/></param>
		/// <param name="xscale">Xscale of the vertical section</param>
		/// <param name="yscale">Yscale of the vertical section</param>
		public DynamicTable EndBeginVertical(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			EndVertical();
			return BeginVertical(padding, spacing, xscale, yscale);
		}

		/// <summary>
		/// Begins a new horizontal row section
		/// </summary>
		/// <remarks>
		/// After calling this method, each subsequent call to <see cref="Add"/> will add controls in a horizontal orientation.
		/// 
		/// When finished adding controls to the horizontal section, call <see cref="EndHorizontal"/>
		/// </remarks>
		/// <returns>A new row to hold the horizontal controls</returns>
		/// <param name="yscale">YScale of the horizontal section</param>
		public DynamicRow BeginHorizontal(bool? yscale = null)
		{
			currentItem.AddRow(currentItem.CurrentRow = new DynamicRow());
			yscales.Push(yscale);
			return currentItem.CurrentRow;
		}

		/// <summary>
		/// Ends the current horizontal section
		/// </summary>
		/// <remarks>
		/// This should be balanced with every call to <see cref="BeginHorizontal"/>.
		/// Alternatively, you can call <see cref="EndBeginHorizontal"/> to end the current horizontal section and start a new one
		/// with a new row.
		/// </remarks>
		public void EndHorizontal()
		{
			yscales.Pop();
			if (currentItem.CurrentRow == null)
				EndVertical();
			else
				currentItem.CurrentRow = null;
		}

		/// <summary>
		/// Ends the current horizontal section, then begins a new horizontal section with a new row
		/// </summary>
		/// <remarks>
		/// When finished adding controls to the vertical section, call <see cref="EndHorizontal"/>
		/// </remarks>
		/// <returns>A new DynamicRow instance used to hold the horizontal controls</returns>
		/// <param name="yscale">Yscale of the horizontal section</param>
		public DynamicRow EndBeginHorizontal(bool? yscale = null)
		{
			EndHorizontal();
			return BeginHorizontal(yscale);
		}

		/// <summary>
		/// Creates a new section where all controls will be centered together.
		/// </summary>
		/// <remarks>
		/// This is useful when you want to create a section that groups controls which align themselves together but are centered in the parent.
		/// When finished adding controls to the centered section, call <see cref="EndCentered"/>.
		/// To center vertically, set <paramref name="yscale"/> to <c>true</c>.
		/// </remarks>
		/// <param name="padding">Padding to apply around the controls, or null to use the <see cref="DefaultPadding"/></param>
		/// <param name="spacing">Spacing to apply to cells in the section, or null to use the <see cref="DefaultSpacing"/></param>
		/// <param name="xscale">Xscale of the vertical section</param>
		/// <param name="yscale">Yscale of the vertical section</param>
		public void BeginCentered(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			BeginVertical(Drawing.Padding.Empty, Size.Empty, xscale, yscale);
			if (yscale == true)
				Add(null);
			yscales.Push(yscale);
			BeginHorizontal();
			Add(null);
			BeginVertical(padding, spacing);
		}

		/// <summary>
		/// Ends the current centered section.
		/// </summary>
		/// <remarks>
		/// This should be balanced with every call to <see cref="BeginCentered"/>.
		/// </remarks>
		public void EndCentered()
		{
			EndVertical();
			Add(null);
			EndHorizontal();
			if (yscales.Pop() == true)
				Add(null);
			EndVertical();
		}

		/// <summary>
		/// Begins a the group section in the dynamic layout with a title.
		/// </summary>
		/// <remarks>
		/// Should be balanced with a call to <see cref="EndGroup"/>.
		/// </remarks>
		/// <returns>The group instance.</returns>
		/// <param name="title">Title for the group, or null to have no title.</param>
		/// <param name="padding">Padding around the children of the group.</param>
		/// <param name="spacing">Spacing between the children of the group.</param>
		/// <param name="xscale">Xscale of the group itself.</param>
		/// <param name="yscale">Yscale of the group itself.</param>
		public DynamicGroup BeginGroup(string title, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			var newItem = new DynamicGroup
			{
				Title = title,
				Padding = padding,
				Spacing = spacing,
				XScale = xscale,
				YScale = yscale
			};
			currentItem.Add(newItem);
			currentItem = newItem;
			return newItem;
		}

		/// <summary>
		/// Ends a group.
		/// </summary>
		/// <remarks>
		/// Should be balanced with a previous call to <see cref="BeginGroup"/>.
		/// </remarks>
		public void EndGroup() => EndVertical();

		/// <summary>
		/// Begins a the scrollable section in the dynamic layout with a specified border.
		/// </summary>
		/// <remarks>
		/// Should be balanced with a call to <see cref="EndScrollable"/>.
		/// </remarks>
		/// <returns>The scrollable instance.</returns>
		/// <param name="border">BorderType for the Scrollable.</param>
		/// <param name="padding">Padding around the children in the scrollable.</param>
		/// <param name="spacing">Spacing between the children in the scrollable.</param>
		/// <param name="xscale">Xscale of the scrollable itself.</param>
		/// <param name="yscale">Yscale of the scrollable itself.</param>
		public DynamicScrollable BeginScrollable(BorderType border = BorderType.Bezel, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null)
		{
			var newItem = new DynamicScrollable
			{
				Border = border,
				Padding = padding,
				Spacing = spacing,
				XScale = xscale,
				YScale = yscale
			};
			currentItem.Add(newItem);
			currentItem = newItem;
			return newItem;
		}

		/// <summary>
		/// Ends a scrollable section.
		/// </summary>
		/// <remarks>
		/// Should be balanced with a previous call to <see cref="BeginScrollable"/>.
		/// </remarks>
		public void EndScrollable() => EndVertical();


		/// <summary>
		/// Add the control with the optional scaling
		/// </summary>
		/// <remarks>
		/// This will add either horizontally or vertically depending on whether <see cref="BeginVertical(Padding?,Size?,bool?,bool?)"/> or
		/// <see cref="BeginHorizontal"/> was called last.
		/// 
		/// The x/y scaling specified applies either to the entire column or row in the parent table that the control
		/// was added to, not just this individual control.
		/// </remarks>
		/// <param name="control">Control to add, or null to add blank space</param>
		/// <param name="xscale">Xscale for this control and any in the same column</param>
		/// <param name="yscale">Yscale for this control and any in the same row</param>
		public DynamicControl Add(Control control, bool? xscale = null, bool? yscale = null)
		{
			if (xscale == null && currentItem.CurrentRow != null && control == null)
				xscale = true;
			yscale = yscale ?? (yscales.Count > 0 ? yscales.Peek() : null);
			if (yscale == null && currentItem.CurrentRow == null && control == null)
				yscale = true;
			var dynamicControl = new DynamicControl { Control = control, XScale = xscale, YScale = yscale };
			currentItem.Add(dynamicControl);
			return dynamicControl;
		}

		/// <summary>
		/// Adds a list of controls
		/// </summary>
		/// <remarks>
		/// This enumerates the collection and calls <see cref="Add"/> for each control.
		/// </remarks>
		/// <param name="controls">Controls to add.</param>
		public void AddRange(IEnumerable<Control> controls)
		{
			foreach (var control in controls)
				Add(control);
		}

		/// <summary>
		/// Adds a list of controls
		/// </summary>
		/// <remarks>
		/// This enumerates the collection and calls <see cref="Add"/> for each control.
		/// </remarks>
		/// <param name="controls">Controls to add.</param>
		public void AddRange(params Control[] controls)
		{
			foreach (var control in controls)
				Add(control);
		}

		/// <summary>
		/// Adds a separate horizontal row of items in a new vertical section
		/// </summary>
		/// <remarks>
		/// This performs the same as the following, but in a single line:
		/// <code>
		/// 	layout.BeginVertical();
		/// 	layout.BeginHorizontal();
		/// 	layout.Add(control1);
		/// 	layout.Add(control2);
		/// 	...
		/// 	layout.EndHorizontal();
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <returns>The separate row.</returns>
		/// <param name="controls">Controls.</param>
		public DynamicRow AddSeparateRow(params Control[] controls)
		{
			return AddSeparateRow(padding: null, controls: controls);
		}

		/// <summary>
		/// Adds a separate horizontal row of items in a new vertical section
		/// </summary>
		/// <remarks>
		/// This performs the same as the following, but in a single line:
		/// <code>
		/// 	layout.BeginVertical(padding, spacing, xscale, yscale);
		/// 	layout.BeginHorizontal();
		/// 	layout.Add(control1);
		/// 	layout.Add(control2);
		/// 	...
		/// 	layout.EndHorizontal();
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <returns>The row added to contain the items</returns>
		/// <param name="padding">Padding for the vertical section</param>
		/// <param name="spacing">Spacing between each cell in the row</param>
		/// <param name="xscale">Xscale for the vertical section</param>
		/// <param name="yscale">Yscale for each of the controls in the row</param>
		/// <param name="controls">Controls to add initially</param>
		public DynamicRow AddSeparateRow(Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null, IEnumerable<Control> controls = null)
		{
			BeginVertical(padding, spacing, xscale, yscale);
			var row = AddRow();
			if (controls != null)
				row.Add(controls);
			EndVertical();
			return row;
		}

		/// <summary>
		/// Adds a separate vertical column of items in a new vertical section
		/// </summary>
		/// <remarks>
		/// This performs the same as the following, but in a single line:
		/// <code>
		/// 	layout.BeginVertical();
		/// 	layout.Add(control1);
		/// 	layout.Add(control2);
		/// 	...
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <returns>The table added to contain the items</returns>
		/// <param name="controls">Controls to add initially</param>
		public DynamicTable AddSeparateColumn(params Control[] controls)
		{
			return AddSeparateColumn(padding: null, controls: controls);
		}

		/// <summary>
		/// Adds a separate vertical column of items in a new vertical section
		/// </summary>
		/// <remarks>
		/// This performs the same as the following, but in a single line:
		/// <code>
		/// 	layout.BeginVertical(padding, spacing, xscale, yscale);
		/// 	layout.Add(control1);
		/// 	layout.Add(control2);
		/// 	...
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <returns>The table added to contain the items</returns>
		/// <param name="padding">Padding for the vertical section</param>
		/// <param name="spacing">Spacing between each cell in the column</param>
		/// <param name="xscale">Xscale for the vertical section</param>
		/// <param name="yscale">Yscale for the vertical section</param>
		/// <param name="controls">Controls to add initially</param>
		public DynamicTable AddSeparateColumn(Padding? padding = null, int? spacing = null, bool? xscale = null, bool? yscale = null, IEnumerable<Control> controls = null)
		{
			var spacingSize = spacing != null ? (Size?)new Size(0, spacing.Value) : null;
			var table = BeginVertical(padding, spacingSize, xscale, yscale);
			if (controls != null)
			{
				foreach (var control in controls)
					Add(control);
			}
			EndVertical();
			return table;
		}

		/// <summary>
		/// Adds a new row of controls to the current vertical section
		/// </summary>
		/// <returns>A new instance of the row that was added</returns>
		/// <param name="controls">Controls to add to the row</param>
		public DynamicRow AddRow(params Control[] controls)
		{
			if (controls == null)
				controls = new Control[] { null };
			
			var row = new DynamicRow(controls);
			currentItem.AddRow(row);
			currentItem.CurrentRow = null;
			return row;
		}

		/// <summary>
		/// Adds a control centered in a new vertical section
		/// </summary>
		/// <remarks>
		/// This adds scaled blank space around the control, and sizes the control to its preferred size.
		/// This is similar to doing the following:
		/// <code>
		/// 	layout.BeginVertical(padding, spacing, xscale, yscale);
		/// 	layout.Add(null); // spacing at top
		/// 
		/// 	layout.BeginHorizontal();
		/// 	layout.Add(null); // spacing to left
		/// 	layout.Add(control);
		/// 	layout.Add(null); // spacing to right
		/// 	layout.EndHorizontal();
		/// 
		/// 	layout.Add(null); // spacing at bottom
		/// 
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <seealso cref="AddAutoSized"/>
		/// <param name="control">Control to add</param>
		/// <param name="padding">Padding around the vertical section</param>
		/// <param name="spacing">Spacing between cells</param>
		/// <param name="xscale">Xscale for the vertical section</param>
		/// <param name="yscale">Yscale for the vertical section</param>
		/// <param name="horizontalCenter">If set to <c>true</c> horizontally center the control</param>
		/// <param name="verticalCenter">If set to <c>true</c> vertically center the control</param>
		public void AddCentered(Control control, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null, bool horizontalCenter = true, bool verticalCenter = true)
		{
			BeginVertical(padding ?? Drawing.Padding.Empty, spacing ?? Size.Empty, xscale, yscale);
			if (verticalCenter)
				Add(null, null, true);
			
			BeginHorizontal();
			if (horizontalCenter)
				Add(null, true, null);
			
			Add(control);
			
			if (horizontalCenter)
				Add(null, true, null);
			
			EndHorizontal();
			
			if (verticalCenter)
				Add(null, null, true);
			EndVertical();
			
		}

		/// <summary>
		/// Adds a control to the layout with its preferred size instead of taking the entire space of the cell
		/// </summary>
		/// <seealso cref="AddCentered(Control,Padding?,Size?,bool?,bool?,bool,bool)"/>
		/// <param name="control">Control to add</param>
		/// <param name="padding">Padding around the vertical section</param>
		/// <param name="spacing">Spacing between cells</param>
		/// <param name="xscale">Xscale for the vertical section</param>
		/// <param name="yscale">Yscale for the vertical section</param>
		/// <param name="centered">If set to <c>true</c> center the control.</param>
		public void AddAutoSized(Control control, Padding? padding = null, Size? spacing = null, bool? xscale = null, bool? yscale = null, bool centered = false)
		{
			BeginVertical(padding ?? Eto.Drawing.Padding.Empty, spacing ?? Size.Empty, xscale, yscale);
			if (centered)
			{
				Add(null);
				AddRow(null, control, null);
			}
			else
				AddRow(control, null);
			Add(null);
			EndVertical();
		}

		/// <summary>
		/// Adds a column of controls in a new vertical section
		/// </summary>
		/// <remarks>
		/// This allows you to add columns of controls.
		/// 
		/// If you are in a horizontal section, you can call this method repeatedly to add columns of controls that are
		/// sized independently from eachother.
		/// 
		/// This is a shortcut for the following:
		/// <code>
		/// 	layout.BeginVertical();
		/// 	layout.Add(control1);
		/// 	...
		/// 	layout.EndVertical();
		/// </code>
		/// </remarks>
		/// <param name="controls">Controls to add</param>
		public void AddColumn(params Control[] controls)
		{
			BeginVertical();
			foreach (var control in controls)
				Add(control);
			EndVertical();
		}

		/// <summary>
		/// Adds an empty space.  Equivalent to calling Add(null);
		/// </summary>
		/// <returns>The item representing the space.</returns>
		/// <param name="xscale">Xscale for this control and any in the same column</param>
		/// <param name="yscale">Yscale for this control and any in the same row</param>
		public DynamicControl AddSpace(bool? xscale = null, bool? yscale = null)
		{
			return Add(null, xscale, yscale);
		}

		/// <summary>
		/// Creates the layout content
		/// </summary>
		/// <remarks>
		/// This is called automatically during the PreLoad or Load event, but can be called manually if changes are made
		/// after initially created
		/// </remarks>
		/// <seealso cref="IsCreated"/>
		public void Create()
		{
			Content = topTable.Create(this);
			IsCreated = true;
		}

		/// <summary>
		/// Clears the layout so it can be recreated
		/// </summary>
		/// <remarks>
		/// You must call <see cref="Create"/> when done updating the layout
		/// </remarks>
		public void Clear()
		{
			topTable.Rows.Clear();
			IsCreated = false;
		}

		internal void AddChild(Control child)
		{
			SetLogicalParent(child);
		}

		internal void RemoveChild(Control child)
		{
			RemoveLogicalParent(child);
		}
	}
}