using System;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// State for a <see cref="Cell"/>
	/// </summary>
	[Flags]
	public enum CellStates
	{
		/// <summary>
		/// Normal state
		/// </summary>
		None = 0,

		/// <summary>
		/// Row is selected
		/// </summary>
		Selected = 1 << 0,

		/// <summary>
		/// Cell is in edit mode
		/// </summary>
		Editing = 1 << 1,
	}

	/// <summary>
	/// Event arguments for events that refer to a <see cref="Cell"/>.
	/// </summary>
	public class CellEventArgs : EventArgs, INotifyPropertyChanged
	{
		int row;
		object item;
		CellStates cellState;

		/// <summary>
		/// Gets the state of the cell.
		/// </summary>
		/// <value>The state of the cell.</value>
		public CellStates CellState
		{
			get { return cellState; }
			protected set
			{
				if (cellState != value)
				{
					var editChanged = cellState.HasFlag(CellStates.Editing) != value.HasFlag(CellStates.Editing);
					var selectedChanged = cellState.HasFlag(CellStates.Selected) != value.HasFlag(CellStates.Selected);
					cellState = value;
					OnPropertyChanged(new PropertyChangedEventArgs("CellState"));
					if (editChanged)
						OnPropertyChanged(new PropertyChangedEventArgs("IsEditing"));
					if (selectedChanged)
						OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
				}
			}
		}

		/// <summary>
		/// Gets or sets the item for the cell.
		/// </summary>
		/// <value>The cell's item.</value>
		public object Item
		{
			get { return item; }
			protected set
			{
				if (item != value)
				{
					item = value;
					OnPropertyChanged(new PropertyChangedEventArgs("Item"));
				}
			}
		}

		/// <summary>
		/// Gets or sets the row for the cell.
		/// </summary>
		/// <value>The cell's row.</value>
		public virtual int Row
		{
			get { return row; }
			protected set
			{
				if (row != value)
				{
					row = value;
					OnPropertyChanged(new PropertyChangedEventArgs("Row"));
				}
			}
		}

		/// <summary>
		/// Gets the column index for the cell.
		/// </summary>
		/// <value>The cell's column index.</value>
		public virtual int Column { get; }

		/// <summary>
		/// Gets the column for the cell.
		/// </summary>
		/// <value>The cell's column.</value>
		public GridColumn GridColumn => Column >= 0 ? Grid?.Columns[Column] : null;

		/// <summary>
		/// Gets the cell that triggered this event
		/// </summary>
		public Cell Cell { get; }

		/// <summary>
		/// Gets the grid that this event was triggered from
		/// </summary>
		public Grid Grid { get; }

		/// <summary>
		/// Gets the custom control associated with the cell (if any)
		/// </summary>
		/// <value>Instance of the control for the cell</value>
		public Control Control { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating that the default behavior should not be executed for the event, if supported.
		/// </summary>
		/// <remarks>
		/// Note that not all events can be handled.
		/// </remarks>
		/// <value>True if the event is user-handled, false to use system behavior.</value>
		public bool Handled { get; set; }

		Color cellTextColor = SystemColors.ControlText;
        
		/// <summary>
		/// Gets or sets the preferred color of the cell text given its state.
		/// </summary>
		/// <value>The preferred color of cell text.</value>
		public Color CellTextColor
		{
			get { return cellTextColor; }
			protected set
			{
				if (cellTextColor != value)
				{
					cellTextColor = value;
					OnPropertyChanged(new PropertyChangedEventArgs("CellTextColor"));
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CellEventArgs"/> class.
		/// </summary>
		/// <param name="row">Row for the cell.</param>
		/// <param name="item">Item the cell is displaying.</param>
		/// <param name="cellState">State of the cell.</param>
		[Obsolete("Use overload that passes the custom cell these arguments are for")]
		public CellEventArgs(int row, object item, CellStates cellState)
			: this(null, null, row, item, cellState)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CellEventArgs"/> class.
		/// </summary>
		/// <param name="grid">Grid the event is triggered for.</param>
		/// <param name="cell">Cell the event is triggered for.</param>
		/// <param name="row">Row for the cell.</param>
		/// <param name="item">Item the cell is displaying.</param>
		/// <param name="cellState">State of the cell.</param>
		[Obsolete("Use constructor with column number and control")]
		public CellEventArgs(Grid grid, Cell cell, int row, object item, CellStates cellState)
			: this(null, null, row, -1, item, cellState, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CellEventArgs"/> class.
		/// </summary>
		/// <param name="grid">Grid the event is triggered for.</param>
		/// <param name="cell">Cell the event is triggered for.</param>
		/// <param name="row">Row for the cell.</param>
		/// <param name="column">Column for the cell.</param>
		/// <param name="item">Item the cell is displaying.</param>
		/// <param name="cellState">State of the cell.</param>
		/// <param name="control">Control object for the cell (if any)</param>
		public CellEventArgs(Grid grid, Cell cell, int row, int column, object item, CellStates cellState, Control control)
		{
			Grid = grid;
			Cell = cell;
			Column = column;
			Row = row;
			Item = item;
			CellState = cellState;
			Control = control;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the cell is in edit mode.
		/// </summary>
		/// <value><c>true</c> if the cell is in edit mode; otherwise, <c>false</c>.</value>
		public bool IsEditing
		{
			get { return CellState.HasFlag(CellStates.Editing); }
			protected set
			{
				if (value)
					CellState |= CellStates.Editing;
				else
					CellState &= ~CellStates.Editing;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the cell is selected.
		/// </summary>
		/// <value><c>true</c> if the cell is selected; otherwise, <c>false</c>.</value>
		public bool IsSelected
		{
			get { return CellState.HasFlag(CellStates.Selected); }
			protected set
			{
				if (value)
					CellState |= CellStates.Selected;
				else
					CellState &= ~CellStates.Selected;
			}
		}

		/// <summary>
		/// Occurs when a property is changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}
	}

	/// <summary>
	/// Cell for <see cref="Grid"/> controls to create custom content for the cell.
	/// </summary>
	/// <remarks>
	/// The CustomCell is useful when you want to provide a fully custom implementation of a cell, including editable
	/// controls.
	/// 
	/// Note that some platforms bahave differently with a CustomCell, depending on the value of <see cref="CustomCell.SupportsControlView"/>.
	/// 
	/// When <c>false</c>, the grid will use the <see cref="CustomCell.Paint"/> event to paint the contents of each cell, and
	/// only use <see cref="CustomCell.CreateCell"/> and <see cref="CustomCell.ConfigureCell"/> when the cell is in edit mode.
	/// 
	/// When <c>true</c>, the <see cref="CustomCell.CreateCell"/> and <see cref="CustomCell.ConfigureCell"/> methods will be used
	/// for all cells, even when not in edit mode.
	/// 
	/// </remarks>
	[Handler(typeof(CustomCell.IHandler))]
	public class CustomCell : Cell
	{
		const float DefaultWidth = 80f;

		/// <summary>
		/// Gets a value indicating that the CustomCell on the current platform supports using a Control for its view mode.
		/// Gtk and WinForms only support custom controls when editing a cell.
		/// </summary>
		/// <remarks>
		/// When <c>false</c>, you must handle the <see cref="Paint"/> event (or override <see cref="OnPaint"/>) to provide
		/// a view mode for the cell contents.
		/// </remarks>
		/// <value><c>true</c> if supports control view; otherwise, <c>false</c>.</value>
		public static bool SupportsControlView
		{
			get { return Platform.Instance.SupportedFeatures.HasFlag(PlatformFeatures.CustomCellSupportsControlView); }
		}

		/// <summary>
		/// Creates a new CustomCell with instances of the specified <typeparamref name="TControl"/> type.
		/// </summary>
		/// <typeparam name="TControl">The type of control to instantiate for the cell, which must have a default constructor.</typeparam>
		public static CustomCell Create<TControl>()
			where TControl : Control, new()
		{
			return new CustomCell { CreateCell = args => new TControl() };
		}

		/// <summary>
		/// Gets or sets a delegate to create the contents of the cell.
		/// </summary>
		/// <remarks>
		/// You can also override the <see cref="OnCreateCell"/> method in subclasses.
		/// Note that you need to handle <see cref="ConfigureCell"/> to set up the cell for a particular row of data
		/// as the control may be reused when scrolling the view.
		/// </remarks>
		/// <value>The delegate to create the cell content.</value>
		public Func<CellEventArgs, Control> CreateCell { get; set; }

		/// <summary>
		/// Gets or sets a delegate to get the identifier of the cell based on its content.
		/// </summary>
		/// <remarks>
		/// When you have different controls on a per-row level, each variation must have an identifier string
		/// to allow the framework to cache the different types of cells to provide good performance.
		/// 
		/// This hooks into standard cell caching mechanisms in certain platforms, such as on the Mac.
		/// </remarks>
		/// <value>The delegate to get the identifier for the cell.</value>
		public Func<CellEventArgs, string> GetIdentifier { get; set; }

		/// <summary>
		/// Gets or sets a delegate to get the preferred width of the cell based on its content.
		/// </summary>
		/// <remarks>
		/// This may only be used when <see cref="SupportsControlView"/> is false.
		/// </remarks>
		/// <value>The delegate to get the preferred width of the cell.</value>
		public Func<CellEventArgs, float> GetPreferredWidth { get; set; }

		/// <summary>
		/// Gets or sets a delegate to configure an cell when it is reused for a different row or the data changes.
		/// </summary>
		/// <remarks>
		/// This should set up your control your cell content to be reused.  If null, the DataContext of your control will be set to the row model instance.
		/// 
		/// Typically if you use MVVM data binding, you do not need to override the standard behaviour.
		/// </remarks>
		/// <value>The delegate to configure an existing cell's control for a new row/model instance.</value>
		public Action<CellEventArgs, Control> ConfigureCell { get; set; }

		/// <summary>
		/// Creates an instance of the control for a cell.
		/// </summary>
		/// <remarks>
		/// This is called multiple times usually for the number of visible and/or cached cells that are displayed.
		/// 
		/// Note that you need to handle <see cref="OnConfigureCell"/> to set up the cell for a particular row of data
		/// as the control may be reused when scrolling the view.
		///
		/// If you intend on creating a different type of cell control based on the item, you should also override
		/// <see cref="GetIdentifier"/>
		/// </remarks>
		/// <param name="args">Arguments when creating the cell to get the row, item and state.</param>
		/// <returns>The control to display in the cell.</returns>
		protected virtual Control OnCreateCell(CellEventArgs args)
		{
			if (CreateCell != null)
				return CreateCell(args);
			return null;
		}

		/// <summary>
		/// Gets the identifier of the cell based on its content.
		/// </summary>
		/// <remarks>
		/// When you have different controls on a per-row level, each variation must have an identifier string
		/// to allow the framework to cache the different types of cells to provide good performance.
		/// 
		/// This hooks into standard cell caching mechanisms in certain platforms, such as on the Mac.
		/// </remarks>
		/// <seealso cref="GetIdentifier"/>
		/// <param name="args">Arguments for the cell</param>
		/// <value>The identifier for the cell.</value>
		protected virtual string OnGetIdentifier(CellEventArgs args)
		{
			if (GetIdentifier != null)
				return GetIdentifier(args);
			return null; 
		}

		/// <summary>
		/// Gets the preferred width of the cell based on its content.
		/// </summary>
		/// <remarks>
		/// This may only be used when <see cref="SupportsControlView"/> is false.
		/// </remarks>
		/// <param name="args">Arguments for the cell</param>
		/// <value>The preferred width of the cell.</value>
		protected virtual float OnGetPreferredWidth(CellEventArgs args)
		{
			if (GetPreferredWidth != null)
				return GetPreferredWidth(args);
			return DefaultWidth;
		}

		/// <summary>
		/// Configures an existing cell when it is reused for a different row or the data changes.
		/// </summary>
		/// <remarks>
		/// This should set up your control your cell content to be reused.  If null, the DataContext of your control will be set to the row model instance.
		/// 
		/// Typically if you use MVVM data binding, you do not need to override the standard behaviour.
		/// </remarks>
		/// <param name="args">Arguments for the cell</param>
		/// <param name="control">Existing control to configure for the new cell and/or data</param>
		protected virtual void OnConfigureCell(CellEventArgs args, Control control)
		{
			if (ConfigureCell != null)
				ConfigureCell(args, control);
			else if (control != null)
				control.DataContext = args.Item;
		}

		/// <summary>
		/// Event to handle when the cell should begin editing.
		/// </summary>
		public event EventHandler<CellEventArgs> BeginEdit;
		/// <summary>
		/// Event to handle when the cell should cancel editing.
		/// </summary>
		public event EventHandler<CellEventArgs> CancelEdit;
		/// <summary>
		/// Event to handle when the cell should commit editing.
		/// </summary>
		public event EventHandler<CellEventArgs> CommitEdit;

		/// <summary>
		/// Triggers the <see cref="BeginEdit"/> event.
		/// </summary>
		/// <param name="e">Cell event arguments</param>
		protected virtual void OnBeginEdit(CellEventArgs e) => BeginEdit?.Invoke(this, e);

		/// <summary>
		/// Triggers the <see cref="CancelEdit"/> event.
		/// </summary>
		/// <param name="e">Cell event arguments</param>
		protected virtual void OnCancelEdit(CellEventArgs e) => CancelEdit?.Invoke(this, e);

		/// <summary>
		/// Triggers the <see cref="CommitEdit"/> event.
		/// </summary>
		/// <param name="e">Cell event arguments</param>
		protected virtual void OnCommitEdit(CellEventArgs e) => CommitEdit?.Invoke(this, e);

		static readonly object PaintEvent = new object();

		/// <summary>
		/// Event to handle painting the content of the cell when <see cref="SupportsControlView"/> is false.
		/// </summary>
		public event EventHandler<CellPaintEventArgs> Paint
		{
			add { Properties.AddEvent(PaintEvent, value); }
			remove { Properties.RemoveEvent(PaintEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Paint"/> event.
		/// </summary>
		/// <param name="args">Cell paint arguments.</param>
		protected virtual void OnPaint(CellPaintEventArgs args)
		{
			Properties.TriggerEvent(PaintEvent, this, args);
		}

		/// <summary>
		/// Handler interface for the <see cref="CustomCell"/>.
		/// </summary>
		public new interface IHandler : Cell.IHandler
		{
		}

		/// <summary>
		/// Callback interface for the <see cref="CustomCell"/>
		/// </summary>
		public new interface ICallback : Cell.ICallback
		{
			/// <summary>
			/// Gets the preferred width of the cell based on its content.
			/// </summary>
			float OnGetPreferredWidth(CustomCell widget, CellEventArgs args);

			/// <summary>
			/// Raises the get identifier event.
			/// </summary>
			string OnGetIdentifier(CustomCell widget, CellEventArgs args);

			/// <summary>
			/// Raises the configure cell event.
			/// </summary>
			void OnConfigureCell(CustomCell widget, CellEventArgs args, Control control);

			/// <summary>
			/// Raises the create cell event.
			/// </summary>
			Control OnCreateCell(CustomCell widget, CellEventArgs args);

			/// <summary>
			/// Raises the paint event.
			/// </summary>
			void OnPaint(CustomCell widget, CellPaintEventArgs args);

			/// <summary>
			/// Raises the BeginEdit event.
			/// </summary>
			void OnBeginEdit(CustomCell widget, CellEventArgs args);

			/// <summary>
			/// Raises the CancelEdit event.
			/// </summary>
			void OnCancelEdit(CustomCell widget, CellEventArgs args);

			/// <summary>
			/// Raises the CommitEdit event.
			/// </summary>
			void OnCommitEdit(CustomCell widget, CellEventArgs args);
		}

		/// <summary>
		/// Callback implementation for the <see cref="CustomCell"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Gets the preferred width of the cell based on its content.
			/// </summary>
			public float OnGetPreferredWidth(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					return widget.OnGetPreferredWidth(args);
			}

			/// <summary>
			/// Raises the get identifier event.
			/// </summary>
			public string OnGetIdentifier(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					return widget.OnGetIdentifier(args);
			}

			/// <summary>
			/// Raises the configure cell event.
			/// </summary>
			public void OnConfigureCell(CustomCell widget, CellEventArgs args, Control control)
			{
				using (widget.Platform.Context)
					widget.OnConfigureCell(args, control);
			}

			/// <summary>
			/// Raises the create cell event.
			/// </summary>
			public Control OnCreateCell(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					return widget.OnCreateCell(args);
			}

			/// <summary>
			/// Raises the paint event.
			/// </summary>
			public void OnPaint(CustomCell widget, CellPaintEventArgs args)
			{
				using (widget.Platform.Context)
					widget.OnPaint(args);
			}

			/// <summary>
			/// Raises the BeginEdit event.
			/// </summary>
			public void OnBeginEdit(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					widget.OnBeginEdit(args);
			}

			/// <summary>
			/// Raises the CancelEdit event.
			/// </summary>
			public void OnCancelEdit(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					widget.OnCancelEdit(args);
			}

			/// <summary>
			/// Raises the CommitEdit event.
			/// </summary>
			public void OnCommitEdit(CustomCell widget, CellEventArgs args)
			{
				using (widget.Platform.Context)
					widget.OnCommitEdit(args);
			}
		}

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() => callback;
	}
}

