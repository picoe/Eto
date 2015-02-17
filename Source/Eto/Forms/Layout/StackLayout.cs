using System;
using System.Collections.ObjectModel;
using Eto.Drawing;
using System.ComponentModel;


namespace Eto.Forms
{
	/// <summary>
	/// Alignment for child controls of the <see cref="StackLayout"/>.
	/// </summary>
	public enum StackLayoutAlignment
	{
		/// <summary>
		/// Controls are aligned to the top in a horizontal layout, left in a vertical layout.
		/// </summary>
		TopOrLeft,
		/// <summary>
		/// Controls are centered.
		/// </summary>
		Center,
		/// <summary>
		/// Controls are aligned to the bottom in a horizontal layout, right in a vertical layout.
		/// </summary>
		BottomOrRight,
		/// <summary>
		/// Controls fill the entire height in a horizontal layout, or width in a vertical layout.
		/// </summary>
		Fill
	}

	/// <summary>
	/// Item for a single control in a <see cref="StackLayout"/>.
	/// </summary>
	public class StackLayoutItem
	{
		/// <summary>
		/// Gets or sets the control for this item.
		/// </summary>
		/// <value>The item's control.</value>
		public Control Control { get; set; }

		/// <summary>
		/// Gets or sets the alignment for the control, or null to use the default <see cref="StackLayout.ContentAlign"/>.
		/// </summary>
		/// <value>The alignment of the control.</value>
		public StackLayoutAlignment? Align { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether the control expands to available space in the direction of the layout.
		/// </summary>
		/// <value><c>true</c> to expand; otherwise, <c>false</c>.</value>
		public bool Expand { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.StackLayoutItem"/> class.
		/// </summary>
		public StackLayoutItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.StackLayoutItem"/> class.
		/// </summary>
		/// <param name="control">Control for the item.</param>
		/// <param name="align">Alignment for the control.</param>
		/// <param name="expand">Whether the control expands to fill space along the direction of the layout</param>
		public StackLayoutItem(Control control, StackLayoutAlignment? align = null, bool expand = false)
		{
			Control = control;
			Align = align;
			Expand = expand;
		}

		/// <summary>
		/// Converts a control to a StackLayoutItem implicitly.
		/// </summary>
		/// <param name="control">Control to convert.</param>
		public static implicit operator StackLayoutItem(Control control)
		{
			return new StackLayoutItem { Control = control };
		}

		/// <summary>
		/// Converts a string to a StackLayoutItem with a label control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add labels to your layout through code, without having to create <see cref="Label"/> instances.
		/// </remarks>
		/// <param name="labelText">Text to convert to a Label control.</param>
		public static implicit operator StackLayoutItem(string labelText)
		{
			return new StackLayoutItem { Control = new Label { Text = labelText } };
		}
	}

	/// <summary>
	/// Layout to stack controls horizontally or vertically, with the ability for each child to be aligned to a side
	/// of the layout.
	/// </summary>
	public class StackLayout : Panel
	{
		Orientation orientation = Orientation.Vertical;

		/// <summary>
		/// Gets or sets the orientation of the controls in the stack layout.
		/// </summary>
		/// <remarks>
		/// When the orientation is Horizontal, the <see cref="ContentAlign"/> specifies the default vertical alignment 
		/// for the child controls.
		/// When the orientation is Vertical, the ContentAlign specifies the default horizontal alignment for the child
		/// controls.
		/// </remarks>
		/// <value>The orientation of the controls.</value>
		[DefaultValue(Orientation.Vertical)]
		public Orientation Orientation
		{
			get { return orientation; }
			set
			{
				if (orientation != value)
				{
					orientation = value;
					CreateIfNeeded(true);
				}
			}
		}

		int spacing;

		/// <summary>
		/// Gets or sets the spacing between each of the controls in the stack.
		/// </summary>
		/// <value>The spacing between each control.</value>
		public int Spacing
		{
			get { return spacing; }
			set
			{
				if (spacing != value)
				{
					spacing = value;
					var table = Content as TableLayout;
					if (table != null)
						table.Spacing = new Size(spacing, spacing);
				}
			}
		}

		StackLayoutAlignment contentAlign;

		/// <summary>
		/// Gets or sets the default alignment of the child controls in the stack layout.
		/// </summary>
		/// <remarks>
		/// The alignment can also be specified on a per-child basis with the <see cref="StackLayoutItem.Align"/> property.
		/// </remarks>
		/// <value>The default child control alignment.</value>
		public StackLayoutAlignment ContentAlign
		{
			get { return contentAlign; }
			set
			{
				if (contentAlign != value)
				{
					contentAlign = value;
					CreateIfNeeded(true);
				}
			}
		}

		/// <summary>
		/// Gets or sets a delegate to update the item before being added to the underlying TableLayout.
		/// </summary>
		/// <remarks>
		/// This can be used to apply rules to each item. The default behaviour of this delegate will
		/// update all <see cref="Label"/> controls added directly to the layout to match its HorizontalAlign or
		/// VerticalAlign to match the alignment of the stack panel.
		/// </remarks>
		/// <value>The update item delegate.</value>
		public Action<StackLayoutItem> ItemAdding { get; set; }

		class StackLayoutItemCollection : Collection<StackLayoutItem>
		{
			public StackLayout Parent { get; set; }

			protected override void InsertItem(int index, StackLayoutItem item)
			{
				base.InsertItem(index, item);
				Parent.CreateIfNeeded(true);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				Parent.CreateIfNeeded(true);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				Parent.CreateIfNeeded(true);
			}

			protected override void SetItem(int index, StackLayoutItem item)
			{
				base.SetItem(index, item);
				Parent.CreateIfNeeded(true);
			}
		}

		readonly StackLayoutItemCollection items;

		/// <summary>
		/// Gets the collection of items in the stack layout.
		/// </summary>
		/// <value>The item collection.</value>
		public Collection<StackLayoutItem> Items { get { return items; } }

		bool isCreated;
		int suspended;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.StackLayout"/> class.
		/// </summary>
		public StackLayout()
		{
			items = new StackLayoutItemCollection { Parent = this };
			ItemAdding = UpdateLabelItem;
		}

		void UpdateLabelItem(StackLayoutItem item)
		{
			var label = item.Control as Label;
			if (label != null)
			{
				switch (Orientation)
				{
					case Orientation.Horizontal:
						switch (item.Align ?? ContentAlign)
						{
							case StackLayoutAlignment.TopOrLeft:
								label.VerticalAlign = VerticalAlign.Top;
								break;
							case StackLayoutAlignment.Center:
								label.VerticalAlign = VerticalAlign.Middle;
								break;
							case StackLayoutAlignment.BottomOrRight:
								label.VerticalAlign = VerticalAlign.Bottom;
								break;
							default:
								return;
						}
						break;
					case Orientation.Vertical:
						switch (item.Align ?? ContentAlign)
						{
							case StackLayoutAlignment.TopOrLeft:
								label.HorizontalAlign = HorizontalAlign.Left;
								break;
							case StackLayoutAlignment.Center:
								label.HorizontalAlign = HorizontalAlign.Center;
								break;
							case StackLayoutAlignment.BottomOrRight:
								label.HorizontalAlign = HorizontalAlign.Right;
								break;
							default:
								return;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				item.Align = StackLayoutAlignment.Fill;
			}
		}

		/// <summary>
		/// Raises the <see cref="Control.PreLoad"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnPreLoad(EventArgs e)
		{
			if (!isCreated && suspended <= 0)
				Create();
			base.OnPreLoad(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.Load"/> event, and recurses to this container's children
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected override void OnLoad(EventArgs e)
		{
			if (!isCreated && suspended <= 0)
				Create();
			base.OnLoad(e);
		}

		/// <summary>
		/// Suspends the layout of child controls
		/// </summary>
		public override void SuspendLayout()
		{
			base.SuspendLayout();
			suspended++;
		}

		/// <summary>
		/// Resumes the layout after it has been suspended, and performs a layout
		/// </summary>
		public override void ResumeLayout()
		{
			if (suspended == 0)
				throw new InvalidOperationException("Must balance ResumeLayout with SuspendLayout calls");
			suspended--;
			base.ResumeLayout();
			CreateIfNeeded();
		}

		void CreateIfNeeded(bool force = false)
		{
			if (suspended > 0 || !Loaded)
				return;
			if (!isCreated || force)
				Create();
		}

		void Create()
		{
			var table = new TableLayout();
			table.Spacing = new Size(Spacing, Spacing);

			bool filled = false;
			var itemAdding = ItemAdding;
			switch (Orientation)
			{
				case Orientation.Horizontal:
					var topRow = new TableRow();
					for (int i = 0; i < items.Count; i++)
					{
						var item = items[i];
						itemAdding(item);
						var control = item.Control;
						filled |= item.Expand;
						var cell = new TableCell { ScaleWidth = item.Expand };
						switch (item.Align ?? ContentAlign)
						{
							case StackLayoutAlignment.TopOrLeft:
								cell.Control = new TableLayout(control, null);
								break;
							case StackLayoutAlignment.Center:
								cell.Control = new TableLayout(null, control, null);
								break;
							case StackLayoutAlignment.BottomOrRight:
								cell.Control = new TableLayout(null, control);
								break;
							default:
								cell.Control = control;
								break;
						}
						topRow.Cells.Add(cell);
					}
					if (!filled)
						topRow.Cells.Add(null);
					table.Rows.Add(topRow);
					break;
				case Orientation.Vertical:
					for (int i = 0; i < items.Count; i++)
					{
						var item = items[i];
						itemAdding(item);
						var control = item.Control;
						filled |= item.Expand;
						var vrow = new TableRow { ScaleHeight = item.Expand };
						switch (item.Align ?? ContentAlign)
						{
							case StackLayoutAlignment.TopOrLeft:
								vrow.Cells.Add(TableLayout.Horizontal(control, null));
								break;
							case StackLayoutAlignment.Center:
								vrow.Cells.Add(TableLayout.Horizontal(null, control, null));
								break;
							case StackLayoutAlignment.BottomOrRight:
								vrow.Cells.Add(TableLayout.Horizontal(null, control));
								break;
							default:
								vrow.Cells.Add(control);
								break;
						}
						table.Rows.Add(vrow);
					}
					if (!filled)
						table.Rows.Add(null);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			Content = table;
			isCreated = true;
		}
	}
}

