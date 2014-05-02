using System;
using MonoTouch;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public abstract class GridHandler<T, W> : IosControl<T, W>, IGrid
		where T: UITableView
		where W: Grid
	{
		public new UITableViewController Controller
		{
			get { return (UITableViewController)base.Controller; }
			set { base.Controller = value; }
		}

		public override T CreateControl ()
		{
			Controller = new RotatableTableViewController { Control = this.Widget };
			return (T)Controller.TableView;
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
		WeakReference handler;
		public IGrid Handler { get { return (IGrid)handler.Target; } set { handler = new WeakReference(value); } }

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
		WeakReference handler;
		public object Control { get { return (object)handler.Target; } set { handler = new WeakReference(value); } }

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

