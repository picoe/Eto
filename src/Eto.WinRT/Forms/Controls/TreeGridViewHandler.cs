#if TODO_XAML
using System;
using swc = Windows.UI.Xaml.Controls;
using sw = Windows.UI.Xaml;
using swd = Windows.UI.Xaml.Data;
using swm = Windows.UI.Xaml.Media;
using Eto.Forms;
using Eto.Platform.CustomControls;
using Eto.WinRT.CustomControls.TreeGridView;

namespace Eto.WinRT.Forms.Controls
{
	public class TreeGridViewHandler : GridHandler<swc.DataGrid, TreeGridView>, ITreeGridView, ITreeHandler
	{
		TreeController controller;
		ITreeGridItem lastSelected;

		protected override object GetItemAtRow (int row)
		{
			return controller[row];
		}

		protected override void Initialize ()
		{
			base.Initialize ();
			controller = new TreeController { Handler = this };
			Control.Background = sw.SystemColors.WindowBrush;
			Control.GridLinesVisibility = swc.DataGridGridLinesVisibility.None;
			Control.KeyDown += (sender, e) => {
				if (e.Key == sw.Input.Key.Enter) {
					if (SelectedItem != null)
						Widget.OnActivated (new TreeGridViewItemEventArgs(SelectedItem));
				}
			};
			Control.MouseDoubleClick += delegate {
				if (SelectedItem != null) {
					Widget.OnActivated (new TreeGridViewItemEventArgs(SelectedItem));
				}
			};
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case TreeGridView.ExpandingEvent:
				controller.Expanding += (sender, e) => Widget.OnExpanding(e);
				break;
			case TreeGridView.ExpandedEvent:
				controller.Expanded += (sender, e) => Widget.OnExpanded(e);
				break;
			case TreeGridView.CollapsingEvent:
				controller.Collapsing += (sender, e) => Widget.OnCollapsing(e);
				break;
			case TreeGridView.CollapsedEvent:
				controller.Collapsed += (sender, e) => Widget.OnCollapsed(e);
				break;
			case TreeGridView.SelectedItemChangedEvent:
				Control.SelectedCellsChanged += (sender, e) => {
					var item = SelectedItem;
					if (!SkipSelectionChanged && !object.ReferenceEquals(lastSelected, item))
					{
						Widget.OnSelectedItemChanged (EventArgs.Empty);
						lastSelected = item;
					}
				};
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public ITreeGridStore<ITreeGridItem> DataStore
		{
			get { return controller.Store; }
			set
			{
				controller.InitializeItems (value);
				Control.ItemsSource = controller;
			}
		}

		public ITreeGridItem SelectedItem
		{
			get { return Control.SelectedItem as ITreeGridItem; }
			set {
				if (controller != null && value != null) {
					controller.ExpandToItem (value);
					Control.SelectedItem = value;
					Control.ScrollIntoView (value);
				}
				else
					Control.UnselectAll ();
			}
		}

		public override sw.FrameworkElement SetupCell (IGridColumnHandler column, sw.FrameworkElement defaultContent)
		{
			if (object.ReferenceEquals(column, Columns.Collection[0].Handler))
				return TreeToggleButton.Create(defaultContent, controller);
			return defaultContent;
		}

		void ITreeHandler.PreResetTree ()
		{
			SkipSelectionChanged = true;
			SaveFocus ();
		}

		void ITreeHandler.PostResetTree ()
		{
			RestoreFocus ();
			SkipSelectionChanged = false;
		}
	}
}
#endif