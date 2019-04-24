using System;
using System.Collections.Generic;
using System.Text;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms.ThemedControls
{
	/// <summary>
	/// Themed <see cref="MenuSegmentedItem"/> handler
	/// </summary>
	public class ThemedMenuSegmentedItemHandler : ThemedSegmentedItemHandler<MenuSegmentedItem, MenuSegmentedItem.ICallback>, MenuSegmentedItem.IHandler
	{
		UITimer timer;
		bool menuWasShown;
		string text;
		string menuIndicator = "▼";

		/// <summary>
		/// Delay for the menu to show when the mouse is down and <see cref="CanSelect"/> is <c>true</c>.
		/// </summary>
		public TimeSpan MenuDelay { get; set; } = TimeSpan.FromSeconds(0.5);

		/// <summary>
		/// Gets or sets the indicator to show to the right of the text.
		/// </summary>
		/// <value>The menu indicator.</value>
		public string MenuIndicator
		{
			get => menuIndicator;
			set
			{
				menuIndicator = value;
				SetText();
			}
		}

		/// <inheritdoc/>
		public ContextMenu Menu { get; set; }

		/// <inheritdoc/>
		public bool CanSelect { get; set; }

		/// <inheritdoc/>
		public override string Text
		{
			get => text;
			set
			{
				text = value;
				SetText();
			}
		}

		void SetText()
		{
			if (!string.IsNullOrEmpty(text))
				base.Text = text + " " + MenuIndicator;
			else
				base.Text = MenuIndicator;
		}

		/// <inheritdoc/>
		protected override void Initialize()
		{
			base.Initialize();
			Control.MouseDown += Control_MouseDown;
			Control.MouseUp += Control_MouseUp;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			e.Handled |= menuWasShown;
			menuWasShown = false;
			timer?.Stop();
		}

		private void Timer_Elapsed(object sender, EventArgs e)
		{
			menuWasShown = true;
			Menu?.Show(Control, new PointF(0, Control.Height));
			timer.Stop();
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (CanSelect)
			{
				menuWasShown = false;
				if (timer == null)
				{
					timer = new UITimer { Interval = MenuDelay.TotalSeconds };
					timer.Elapsed += Timer_Elapsed;
				}
				timer.Start();
			}
			else
			{
				Menu?.Show(Control, new PointF(0, Control.Height));
				menuWasShown = true;
				e.Handled = true;
			}
		}
	}

	/// <summary>
	/// Themed <see cref="ButtonSegmentedItem"/> handler.
	/// </summary>
	public class ThemedButtonSegmentedItemHandler : ThemedSegmentedItemHandler<ButtonSegmentedItem, ButtonSegmentedItem.ICallback>, ButtonSegmentedItem.IHandler
	{

	}

	/// <summary>
	/// Themed <see cref="SegmentedItem"/> handler which uses a <see cref="ToggleButton"/> for its display.
	/// </summary>
	public abstract class ThemedSegmentedItemHandler<TWidget, TCallback> : WidgetHandler<ToggleButton, TWidget, TCallback>, SegmentedItem.IHandler
		where TWidget : SegmentedItem
		where TCallback : SegmentedItem.ICallback
	{
		/// <summary>
		/// Gets the parent handler.
		/// </summary>
		/// <value>The parent handler.</value>
		protected ThemedSegmentedButtonHandler ParentHandler => Widget.Parent?.Handler as ThemedSegmentedButtonHandler;

		/// <inheritdoc/>
		public bool Enabled { get => Control.Enabled; set => Control.Enabled = value; }
		/// <inheritdoc/>
		public bool Visible { get => Control.Visible; set => Control.Visible = value; }
		/// <inheritdoc/>
		public string ToolTip { get => Control.ToolTip; set => Control.ToolTip = value; }
		/// <inheritdoc/>
		public int Width { get => Control.Width; set => Control.Width = value; }
		/// <inheritdoc/>
		public virtual string Text
		{
			get => Control.Text;
			set
			{
				Control.Text = value;
				Control.ImagePosition = string.IsNullOrEmpty(value) ? ButtonImagePosition.Overlay : ButtonImagePosition.Left;
			}
		}
		/// <inheritdoc/>
		public Image Image { get => Control.Image; set => Control.Image = value; }
		/// <inheritdoc/>
		public bool Selected
		{
			get => Control.Checked;
			set
			{
				if (!value || ParentHandler?.SelectionMode != SegmentedSelectionMode.None)
				{
					Control.Checked = value;
				}
			}
		}

		/// <inheritdoc/>
		protected override ToggleButton CreateControl() => new ToggleButton { ImagePosition = ButtonImagePosition.Overlay };

		/// <inheritdoc/>
		protected override void Initialize()
		{
			base.Initialize();

			Control.Click += Control_Click;
			Control.CheckedChanged += Control_CheckedChanged;
			Control.MouseUp += Control_MouseUp;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			var mode = ParentHandler?.SelectionMode ?? SegmentedSelectionMode.None;
			if (mode == SegmentedSelectionMode.None // no selection
				|| (mode == SegmentedSelectionMode.Single && Control.Checked) // can't "unselect"
				)
			{
				e.Handled = true;
				TriggerClick();
			}
		}

		private void Control_CheckedChanged(object sender, EventArgs e)
		{
			Callback.OnSelectedChanged(Widget, EventArgs.Empty);
			ParentHandler?.TriggerSelectionChanged(Widget);
			if (Control.Checked)
				ParentHandler?.EnsureSingleSelected(Widget, false);
		}

		/// <inheritdoc/>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SegmentedItem.ClickEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_Click(object sender, EventArgs e) => TriggerClick();

		void TriggerClick()
		{
			Callback.OnClick(Widget, EventArgs.Empty);
			ParentHandler?.TriggerItemClick(Widget);
		}
	}

	/// <summary>
	/// Themed <see cref="SegmentedButton"/> handler which uses a series of <see cref="ToggleButton"/> controls in a table.
	/// </summary>
	public class ThemedSegmentedButtonHandler : ThemedControlHandler<Panel, SegmentedButton, SegmentedButton.ICallback>, SegmentedButton.IHandler
	{
		int suppressSelectionChanged;
		SegmentedSelectionMode selectionMode;

		/// <inheritdoc/>
		protected override Panel CreateControl() => new Panel();

		/// <inheritdoc/>
		public SegmentedSelectionMode SelectionMode
		{
			get => selectionMode;
			set
			{
				selectionMode = value;

				switch (value)
				{
					case SegmentedSelectionMode.None:
						ClearSelection();
						break;
					case SegmentedSelectionMode.Single:
						EnsureSingleSelected();
						break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the spacing between the buttons
		/// </summary>
		/// <value>The spacing between the buttons.</value>
		public int Spacing { get; set; }

		/// <inheritdoc/>
		public int SelectedIndex
		{
			get
			{
				for (int i = 0; i < Widget.Items.Count; i++)
				{
					if (Widget.Items[i].Selected)
						return i;
				}
				return -1;
			}
			set
			{
				if (selectionMode == SegmentedSelectionMode.None)
					return;
				if (value >= 0)
					Widget.Items[value].Selected = true;
				else
					ClearSelection();
			}
		}

		/// <inheritdoc/>
		public IEnumerable<int> SelectedIndexes
		{
			get
			{
				for (int i = 0; i < Widget.Items.Count; i++)
				{
					if (Widget.Items[i].Selected)
						yield return i;
				}
			}
			set
			{
				if (selectionMode == SegmentedSelectionMode.None)
					return;
				var indexes = new HashSet<int>(value);
				suppressSelectionChanged++;
				var newSelection = false;
				for (int i = 0; i < Widget.Items.Count; i++)
				{
					var item = Widget.Items[i];
					var newSelected = indexes.Contains(i);
					if (item.Selected != newSelected)
					{
						newSelected = true;
						item.Selected = newSelected;
						if (newSelected && selectionMode != SegmentedSelectionMode.Multiple)
							break;
					}
				}
				suppressSelectionChanged--;
				if (newSelection)
					TriggerSelectionChanged();
			}
		}

		/// <inheritdoc/>
		public void ClearItems()
		{
			var hasSelected = SelectedIndex != -1;
			CreateTable(false);
			if (hasSelected)
				TriggerSelectionChanged();
		}

		Control GetControl(SegmentedItem item) => item.ControlObject as Control;

		TableCell GetCell(SegmentedItem item) => new TableCell(GetControl(item));

		/// <inheritdoc/>
		public void ClearSelection()
		{
			suppressSelectionChanged++;
			var wasSelected = false;
			for (int i = 0; i < Widget.Items.Count; i++)
			{
				var item = Widget.Items[i];
				wasSelected |= item.Selected;
				item.Selected = false;
			}
			suppressSelectionChanged--;
			if (wasSelected)
				TriggerSelectionChanged();
		}

		/// <inheritdoc/>
		public void InsertItem(int index, SegmentedItem item)
		{
			var isSelected = item.Selected;
			CreateTable(false);
			if (isSelected)
			{
				suppressSelectionChanged++;
				if (SelectionMode == SegmentedSelectionMode.Single)
					EnsureSingleSelected(item, false);
				suppressSelectionChanged--;

				TriggerSelectionChanged();
			}
		}

		/// <inheritdoc/>
		public void RemoveItem(int index, SegmentedItem item)
		{
			var wasSelected = item.Selected;
			CreateTable(false);
			if (wasSelected)
				TriggerSelectionChanged();
		}

		/// <inheritdoc/>
		public void SelectAll()
		{
			suppressSelectionChanged++;
			bool wasChanged = false;
			foreach (var item in Widget.Items)
			{
				wasChanged |= !item.Selected;
				item.Selected = true;
			}
			suppressSelectionChanged--;
			if (wasChanged)
				TriggerSelectionChanged();
		}

		/// <inheritdoc/>
		public void SetItem(int index, SegmentedItem item)
		{
			var wasSelected = Widget.Items[index].Selected;
			var isSelected = item.Selected;
			CreateTable(false);
			if (wasSelected || isSelected)
				TriggerSelectionChanged();
		}

		internal void TriggerItemClick(SegmentedItem item)
		{
			var index = Widget.Items.IndexOf(item);
			Callback.OnItemClicked(Widget, new SegmentedItemClickEventArgs(item, index));
		}

		internal void TriggerSelectionChanged(SegmentedItem item)
		{
			if (suppressSelectionChanged > 0)
				return;
			if (selectionMode != SegmentedSelectionMode.Multiple && item.Selected)
			{
				suppressSelectionChanged++;
				for (int i = 0; i < Widget.Items.Count; i++)
				{
					var current = Widget.Items[i];
					if (!ReferenceEquals(item, current))
						current.Selected = false;
				}
				suppressSelectionChanged--;
			}
			TriggerSelectionChanged();
		}

		internal void TriggerSelectionChanged()
		{
			if (suppressSelectionChanged > 0)
				return;
			Callback.OnSelectedIndexesChanged(Widget, EventArgs.Empty);
		}

		internal void EnsureSingleSelected()
		{
			var selectedIndex = SelectedIndex;
			if (selectedIndex >= 0)
				EnsureSingleSelected(Widget.Items[selectedIndex], true);
		}

		internal void EnsureSingleSelected(SegmentedItem item, bool force)
		{
			if ((!force && selectionMode == SegmentedSelectionMode.Multiple) || item == null)
				return;

			var items = Widget.Items;
			suppressSelectionChanged++;
			var wasSelected = false;
			for (int i = 0; i < items.Count; i++)
			{
				var currentItem = items[i];
				if (ReferenceEquals(currentItem, item))
					continue;
				wasSelected |= currentItem.Selected;
				currentItem.Selected = false;
				
			}
			suppressSelectionChanged--;
			if (wasSelected)
				TriggerSelectionChanged();
		}

		/// <inheritdoc/>
		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case SegmentedButton.ItemClickEvent:
				case SegmentedButton.SelectedIndexesChangedEvent:
					// handled intrinsically
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		/// <inheritdoc/>
		public override void OnPreLoad(EventArgs e)
		{
			if (Control.Content == null)
				CreateTable(true);

			base.OnPreLoad(e);
		}

		/// <inheritdoc/>
		public override void OnLoad(EventArgs e)
		{
			if (Control.Content == null)
				CreateTable(true);

			base.OnLoad(e);
		}

		void CreateTable(bool force)
		{
			if (force || Widget.Loaded)
			{
				if (Widget.Items.Count > 0)
				{
					var buttonTable = new TableLayout
					{
						Style = "buttons",
						Spacing = new Size(Spacing, 0),
						Rows = { new TableRow(Widget.Items.Select(GetCell)) }
					};
					Control.Content = TableLayout.AutoSized(buttonTable, centered: true);
				}
				else
					Control.Content = null;
			}
		}
	}
}
