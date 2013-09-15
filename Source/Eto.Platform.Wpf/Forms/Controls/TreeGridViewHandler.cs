using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swm = System.Windows.Media;
using Eto.Forms;
using System.Collections;
using Eto.Platform.Wpf.Drawing;
using Eto.Platform.Wpf.Forms.Menu;
using Eto.Platform.CustomControls;
using Eto.Platform.Wpf.CustomControls.TreeGridView;
using System.Collections.ObjectModel;

namespace Eto.Platform.Wpf.Forms.Controls
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
						Widget.OnActivated (new TreeGridViewItemEventArgs (this.SelectedItem));
				}
			};
			Control.MouseDoubleClick += delegate {
				if (SelectedItem != null) {
					Widget.OnActivated (new TreeGridViewItemEventArgs (this.SelectedItem));
				}
			};
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TreeGridView.ExpandingEvent:
				controller.Expanding += (sender, e) => {
					Widget.OnExpanding (e);
				};
				break;
			case TreeGridView.ExpandedEvent:
				controller.Expanded += (sender, e) => {
					Widget.OnExpanded (e);
				};
				break;
			case TreeGridView.CollapsingEvent:
				controller.Collapsing += (sender, e) => {
					Widget.OnCollapsing (e);
				};
				break;
			case TreeGridView.CollapsedEvent:
				controller.Collapsed += (sender, e) => {
					Widget.OnCollapsed (e);
				};
				break;
			case TreeGridView.SelectedItemChangedEvent:
				Control.SelectedCellsChanged += (sender, e) => {
					var item = this.SelectedItem;
					if (!SkipSelectionChanged && !object.ReferenceEquals(lastSelected, item))
					{
						Widget.OnSelectedItemChanged (EventArgs.Empty);
						lastSelected = item;
					}
				};
				break;
			default:
				base.AttachEvent (handler);
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
			if (object.ReferenceEquals (column, Columns.Collection[0].Handler))
				return TreeToggleButton.Create (defaultContent, controller);
			else
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
