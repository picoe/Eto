using System;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Forms;
using Eto.CustomControls;
using Eto.Wpf.CustomControls.TreeGridView;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Wpf.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<TreeGridView, TreeGridView.ICallback>, TreeGridView.IHandler, ITreeHandler
	{
		TreeController controller;
		ITreeGridItem lastSelected;

		protected override object GetItemAtRow(int row)
		{
			return controller[row];
		}

		protected override void Initialize()
		{
			base.Initialize();
			controller = new TreeController { Handler = this };
			Control.Background = sw.SystemColors.WindowBrush;
			Control.KeyDown += (sender, e) =>
			{
				if (e.Key == sw.Input.Key.Enter)
				{
					if (SelectedItem != null)
						Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
				}
			};
			Control.MouseDoubleClick += delegate
			{
				if (SelectedItem != null)
				{
					Callback.OnActivated(Widget, new TreeGridViewItemEventArgs(SelectedItem));
				}
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TreeGridView.ExpandingEvent:
					controller.Expanding += (sender, e) => Callback.OnExpanding(Widget, e);
					break;
				case TreeGridView.ExpandedEvent:
					controller.Expanded += (sender, e) => Callback.OnExpanded(Widget, e);
					break;
				case TreeGridView.CollapsingEvent:
					controller.Collapsing += (sender, e) => Callback.OnCollapsing(Widget, e);
					break;
				case TreeGridView.CollapsedEvent:
					controller.Collapsed += (sender, e) => Callback.OnCollapsed(Widget, e);
					break;
				case TreeGridView.SelectedItemChangedEvent:
					Control.SelectedCellsChanged += (sender, e) =>
					{
						var item = SelectedItem;
						if (!SkipSelectionChanged && !object.ReferenceEquals(lastSelected, item))
						{
							Callback.OnSelectedItemChanged(Widget, EventArgs.Empty);
							lastSelected = item;
						}
					};
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return controller.Store; }
			set
			{
				controller.InitializeItems(value);
				Control.ItemsSource = controller;
			}
		}

		public ITreeGridItem SelectedItem
		{
			get { return Control.SelectedItem as ITreeGridItem; }
			set
			{
				if (controller != null && value != null)
				{
					controller.ExpandToItem(value);
					Control.SelectedItem = value;
					Control.ScrollIntoView(value);
				}
				else
					Control.UnselectAll();
			}
		}

		public IEnumerable<object> SelectedItems => Control.SelectedItems.OfType<object>();

		public override sw.FrameworkElement SetupCell(IGridColumnHandler column, sw.FrameworkElement defaultContent)
		{
			if (object.ReferenceEquals(column, Columns.Collection[0].Handler))
				return TreeToggleButton.Create(defaultContent, controller);
			return defaultContent;
		}

		void ITreeHandler.PreResetTree()
		{
			SkipSelectionChanged = true;
			SaveFocus();
		}

		void ITreeHandler.PostResetTree()
		{
			RestoreFocus();
			SkipSelectionChanged = false;
		}

		public void ReloadData()
		{
			controller.ReloadData();
		}

		public void ReloadItem(ITreeGridItem item)
		{
			controller.ReloadData();
		}

		public ITreeGridItem GetCellAt(PointF location, out int column)
		{
			var hitTestResult = swm.VisualTreeHelper.HitTest(Control, location.ToWpf())?.VisualHit;
			if (hitTestResult == null)
			{
				column = -1;
				return null;
			}
			var dataGridCell = hitTestResult.GetVisualParent<swc.DataGridCell>();
			column = dataGridCell?.Column != null ? Control.Columns.IndexOf(dataGridCell.Column) : -1;

			var dataGridRow = hitTestResult.GetVisualParent<swc.DataGridRow>();
			if (dataGridRow != null)
			{
				int row = dataGridRow.GetIndex();
				return GetItemAtRow(row) as ITreeGridItem;
			}
			return null;
		}
	}
}
