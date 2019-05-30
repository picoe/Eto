using System;
using System.Collections;
using System.Collections.ObjectModel;
using sc = System.ComponentModel;
using Eto.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	/// <summary>
	/// Horizontal alignment for controls
	/// </summary>
	public enum HorizontalAlignment
	{
		/// <summary>
		/// Controls are aligned to the left of the container.
		/// </summary>
		Left,
		/// <summary>
		/// Controls are centered.
		/// </summary>
		Center,
		/// <summary>
		/// Controls are aligned to the right of the container.
		/// </summary>
		Right,
		/// <summary>
		/// Controls stretch to fill the entire width of the container.
		/// </summary>
		Stretch
	}

	/// <summary>
	/// Horizontal alignment for controls
	/// </summary>
	public enum VerticalAlignment
	{
		/// <summary>
		/// Controls are aligned to the top of the container.
		/// </summary>
		Top,
		/// <summary>
		/// Controls are centered.
		/// </summary>
		Center,
		/// <summary>
		/// Controls are aligned to the bottom of the container.
		/// </summary>
		Bottom,
		/// <summary>
		/// Controls stretch to fill the entire height of the container.
		/// </summary>
		Stretch
	}

	/// <summary>
	/// Item for a single control in a <see cref="StackLayout"/>.
	/// </summary>
	[ContentProperty("Control")]
	[sc.TypeConverter(typeof(StackLayoutItemConverter))]
	public class StackLayoutItem
	{
		/// <summary>
		/// Gets or sets the control for this item.
		/// </summary>
		/// <value>The item's control.</value>
		public Control Control { get; set; }

		/// <summary>
		/// Gets or sets the horizontal alignment for the control for vertical stack layouts, or null to use <see cref="StackLayout.HorizontalContentAlignment"/>.
		/// </summary>
		/// <value>The horizontal alignment of the control.</value>
		public HorizontalAlignment? HorizontalAlignment { get; set; }

		/// <summary>
		/// Gets or sets the vertical alignment for the control for horizontal stack layouts, or null to use <see cref="StackLayout.VerticalContentAlignment"/>.
		/// </summary>
		/// <value>The vertical alignment of the control.</value>
		public VerticalAlignment? VerticalAlignment { get; set; }

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
		/// <param name="expand">Whether the control expands to fill space along the direction of the layout</param>
		public StackLayoutItem(Control control, bool expand = false)
		{
			Control = control;
			Expand = expand;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.StackLayoutItem"/> class when the StackLayout.Orientation is Vertical.
		/// </summary>
		/// <param name="control">Control for the item.</param>
		/// <param name="alignment">Horizontal alignment of the control for vertical layouts.</param>
		/// <param name="expand">Whether the control expands to fill space along the direction of the layout</param>
		public StackLayoutItem(Control control, HorizontalAlignment? alignment, bool expand = false)
		{
			Control = control;
			HorizontalAlignment = alignment;
			Expand = expand;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.StackLayoutItem"/> class when the StackLayout.Orientation is Horizontal.
		/// </summary>
		/// <param name="control">Control for the item.</param>
		/// <param name="alignment">Vertical alignment of the control for horizontal layouts.</param>
		/// <param name="expand">Whether the control expands to fill space along the direction of the layout</param>
		public StackLayoutItem(Control control, VerticalAlignment? alignment, bool expand = false)
		{
			Control = control;
			VerticalAlignment = alignment;
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

		/// <summary>
		/// Converts an <see cref="Image"/> to a StackLayoutItem with an <see cref="ImageView"/> control implicitly.
		/// </summary>
		/// <remarks>
		/// This provides an easy way to add images to your layout through code, without having to create <see cref="ImageView"/> instances manually.
		/// </remarks>
		/// <param name="image">Image to convert to a StackLayoutItem with a ImageView control.</param>
		public static implicit operator StackLayoutItem(Image image)
		{
			return new StackLayoutItem { Control = new ImageView { Image = image } };
		}
	}

	/// <summary>
	/// Layout to stack controls horizontally or vertically, with the ability for each child to be aligned to a side
	/// of the layout.
	/// </summary>
	[ContentProperty("Items")]
	public class StackLayout : Panel
	{
		Orientation orientation = Orientation.Vertical;

		/// <summary>
		/// Gets or sets the orientation of the controls in the stack layout.
		/// </summary>
		/// <remarks>
		/// When the orientation is Horizontal, the <see cref="VerticalContentAlignment"/> specifies the default
		/// vertical alignment for child controls.
		/// When the orientation is Vertical, the <see cref="HorizontalContentAlignment"/> specifies the default 
		/// horizontal alignment for child controls.
		/// </remarks>
		/// <value>The orientation of the controls.</value>
		[sc.DefaultValue(Orientation.Vertical)]
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

		HorizontalAlignment horizontalAlignment;

		/// <summary>
		/// Gets or sets the default horizontal alignment of the child controls in the stack layout when the <see cref="Orientation"/> is Vertical.
		/// </summary>
		/// <remarks>
		/// The alignment can also be specified on a per-child basis with the <see cref="StackLayoutItem.HorizontalAlignment"/> property.
		/// </remarks>
		/// <value>The default child control alignment.</value>
		public HorizontalAlignment HorizontalContentAlignment
		{
			get { return horizontalAlignment; }
			set
			{
				if (horizontalAlignment != value)
				{
					horizontalAlignment = value;
					CreateIfNeeded(true);
				}
			}
		}

		VerticalAlignment verticalAlignment;

		/// <summary>
		/// Gets or sets the default vertical alignment of the child controls in the stack layout when the <see cref="Orientation"/> is Horizontal.
		/// </summary>
		/// <remarks>
		/// The alignment can also be specified on a per-child basis with the <see cref="StackLayoutItem.VerticalAlignment"/> property.
		/// </remarks>
		/// <value>The default child control alignment.</value>
		public VerticalAlignment VerticalContentAlignment
		{
			get { return verticalAlignment; }
			set
			{
				if (verticalAlignment != value)
				{
					verticalAlignment = value;
					CreateIfNeeded(true);
				}
			}
		}

		bool alignLabels = true;

		/// <summary>
		/// Gets or sets a value indicating whether the Label's alignment will be changed to match the alignment of the StackLayout.
		/// </summary>
		/// <remarks>
		/// This is used so labels can be updated automatically to match the content alignment of the stack.
		/// For example, when <see cref="HorizontalContentAlignment"/> is Center, then all Labels will get their 
		/// <see cref="Label.TextAlignment"/> set to <see cref="TextAlignment.Center"/>.
		/// </remarks>
		/// <value><c>true</c> if to label alignment; otherwise, <c>false</c>.</value>
		[sc.DefaultValue(true)]
		public bool AlignLabels
		{
			get { return alignLabels; }
			set
			{
				if (alignLabels != value)
				{
					alignLabels = value;
					CreateIfNeeded(true);
				}
			}
		}

		class StackLayoutItemCollection : Collection<StackLayoutItem>, IList
		{
			public StackLayout Parent { get; set; }

			protected override void InsertItem(int index, StackLayoutItem item)
			{
				base.InsertItem(index, item);
				if (item != null)
					Parent.SetLogicalParent(item.Control);
				Parent.CreateIfNeeded(true);
			}

			protected override void RemoveItem(int index)
			{
				var item = this[index];
				if (item != null)
					Parent.RemoveLogicalParent(item.Control);
				base.RemoveItem(index);
				Parent.CreateIfNeeded(true);
			}

			protected override void ClearItems()
			{
				foreach (var item in this)
				{
					if (item != null)
						Parent.RemoveLogicalParent(item.Control);
				}
				base.ClearItems();
				Parent.CreateIfNeeded(true);
			}

			protected override void SetItem(int index, StackLayoutItem item)
			{
				var last = this[index];
				if (last != null)
					Parent.RemoveLogicalParent(last.Control);
				base.SetItem(index, item);
				if (item != null)
					Parent.SetLogicalParent(item.Control);
				Parent.CreateIfNeeded(true);
			}

			int IList.Add(object value)
			{
				// allow adding a control directly from xaml
				var control = value as Control;
				if (control != null)
					Add((StackLayoutItem)control);
				else
					Add((StackLayoutItem)value);
				return Count - 1;
			}
		}

		readonly StackLayoutItemCollection items;

		/// <summary>
		/// Gets the collection of items in the stack layout.
		/// </summary>
		/// <value>The item collection.</value>
		public Collection<StackLayoutItem> Items { get { return items; } }

		/// <summary>
		/// Gets the controls for the layout
		/// </summary>
		/// <remarks>
		/// This will return the list of controls in the stack layout when not created, and when it is,
		/// it will return the embedded TableLayout.
		/// </remarks>
		public override IEnumerable<Control> Controls
		{
			get
			{
				return Items.Where(r => r?.Control != null).Select(r => r.Control);
			}
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
				return base.Controls;
			}
		}

		bool isCreated;
		int suspended;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.StackLayout"/> class.
		/// </summary>
		public StackLayout()
		{
			items = new StackLayoutItemCollection { Parent = this };
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.Forms.StackLayout"/> class with the specified items.
		/// </summary>
		/// <param name="items">Initial items to add to the StackLayout.</param>
		public StackLayout(params StackLayoutItem[] items)
		{
			this.items = new StackLayoutItemCollection { Parent = this };
			foreach (var item in items)
			{
				this.items.Add(item);
			}
		}

		VerticalAlignment GetVerticalAlign(StackLayoutItem item)
		{
			var align = item.VerticalAlignment ?? VerticalContentAlignment;
			var label = item.Control as Label;
			if (!AlignLabels || label == null)
				return align;
			label.VerticalAlignment = align;
			return VerticalAlignment.Stretch;
		}

		HorizontalAlignment GetHorizontalAlign(StackLayoutItem item)
		{
			var align = item.HorizontalAlignment ?? HorizontalContentAlignment;
			var label = item.Control as Label;
			if (!AlignLabels || label == null)
				return align;
			switch (align)
			{
				case HorizontalAlignment.Left:
					label.TextAlignment = TextAlignment.Left;
					break;
				case HorizontalAlignment.Center:
					label.TextAlignment = TextAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					label.TextAlignment = TextAlignment.Right;
					break;
				default:
					return align;
			}
			return HorizontalAlignment.Stretch;
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
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Must balance ResumeLayout with SuspendLayout calls"));
			suspended--;
			base.ResumeLayout();
			CreateIfNeeded();
		}

		void CreateIfNeeded(bool force = false)
		{
			if (suspended > 0 || !Loaded)
			{
				if (force)
					isCreated = false;
				return;
			}
			if (!isCreated || force)
				Create();
		}

		void Create()
		{
			var table = new TableLayout { IsVisualControl = true };
			table.Spacing = new Size(Spacing, Spacing);

			bool filled = false;
			var expandItem = new StackLayoutItem { Expand = true };
			switch (Orientation)
			{
				case Orientation.Horizontal:
					var topRow = new TableRow();
					for (int i = 0; i < items.Count; i++)
					{
						var item = items[i] ?? expandItem;
						var align = GetVerticalAlign(item);
						var control = item.Control;
						filled |= item.Expand;
						var cell = new TableCell { ScaleWidth = item.Expand };
						switch (align)
						{
							case VerticalAlignment.Top:
								cell.Control = new TableLayout(control, null);
								break;
							case VerticalAlignment.Center:
								cell.Control = new TableLayout(null, control, null);
								break;
							case VerticalAlignment.Bottom:
								cell.Control = new TableLayout(null, control);
								break;
							case VerticalAlignment.Stretch:
								cell.Control = control;
								break;
							default:
								throw new ArgumentOutOfRangeException();
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
						var item = items[i] ?? expandItem;
						var align = GetHorizontalAlign(item);
						var control = item.Control;
						filled |= item.Expand;
						var vrow = new TableRow { ScaleHeight = item.Expand };
						switch (align)
						{
							case HorizontalAlignment.Left:
								vrow.Cells.Add(TableLayout.Horizontal(control, null));
								break;
							case HorizontalAlignment.Center:
								vrow.Cells.Add(TableLayout.Horizontal(null, control, null));
								break;
							case HorizontalAlignment.Right:
								vrow.Cells.Add(TableLayout.Horizontal(null, control));
								break;
							case HorizontalAlignment.Stretch:
								vrow.Cells.Add(control);
								break;
							default:
								throw new ArgumentOutOfRangeException();
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