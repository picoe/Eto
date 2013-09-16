using System;
using MonoTouch;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class GridHandler<T, W> : iosControl<T, W>, IGrid, IiosViewController
		where T: UITableView
		where W: Grid
	{
		RotatableTableViewController tableViewController;

		public override UIViewController Controller
		{
			get { return tableViewController; }
		}

		public override T CreateControl ()
		{
			tableViewController = new RotatableTableViewController { Control = this.Widget };
			return (T)tableViewController.TableView;
		}

		protected virtual UITableViewDelegate CreateDelegate()
		{
			return new GridHandlerTableDelegate(this);
		}

		protected override void Initialize ()
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
			get 
			{
				var i = Control.IndexPathsForSelectedRows;
				if (i != null)
					foreach (var s in i)
						yield return s.Row;
			}
		}
	}

	public class GridHandlerTableDelegate : UITableViewDelegate
	{
		public IGrid Handler { get; private set; }

		public Grid Widget { get { return Handler.Widget as Grid; } }

		public GridHandlerTableDelegate(IGrid handler)
		{
			this.Handler = handler;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			Widget.OnSelectionChanged(EventArgs.Empty);
		}
	}

	internal class RotatableTableViewController : UITableViewController
	{
		public object Control { get; set; }

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		protected override void Dispose(bool disposing) // TODO: Is this needed? RotatableViewController implements Dispose but RotatableNavigationController does not.
		{
			var c = Control as IDisposable;
			if (c != null)
			{
				c.Dispose();
				c = null;
			}
			base.Dispose(disposing);
		}
	}
}

