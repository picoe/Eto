using System;
using MonoTouch;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class GridHandler<T, W> : iosControl<T, W>, IGrid
		where T: UITableView
		where W: Grid
	{
		public class TableDelegate : UITableViewDelegate
		{
			public virtual GridHandler<T, W> Handler { get; set; }

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Handler.Widget.OnSelectionChanged (EventArgs.Empty);
			}
		}

		public override T CreateControl ()
		{
			return (T)new UITableView ();
		}

		protected virtual UITableViewDelegate CreateDelegate()
		{
			return new TableDelegate { Handler = this };
		}

		public override void Initialize ()
		{
			base.Initialize ();

			Control.Delegate = CreateDelegate ();
		}

		public void SelectRow (int row)
		{
		}

		public void UnselectRow (int row)
		{
		}

		public void SelectAll ()
		{
		}

		public void UnselectAll ()
		{
		}

		public bool ShowHeader {
			get;
			set;
		}

		public int RowHeight {
			get;
			set;
		}

		public bool AllowColumnReordering {
			get;
			set;
		}

		public bool AllowMultipleSelection {
			get;
			set;
		}

		public IEnumerable<int> SelectedRows {
			get { return null; }
		}
	}
}

