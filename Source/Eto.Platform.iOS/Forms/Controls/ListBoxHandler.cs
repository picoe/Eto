using System;
using System.Reflection;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Linq;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ListBoxHandler : iosView<UITableView, ListBox>, IListBox
	{
		List<IListItem> data = new List<IListItem> ();
		
		class DataSource : UITableViewDataSource
		{
			public ListBoxHandler Handler { get; set; }

			public override int RowsInSection (UITableView tableView, int section)
			{
				return Handler.data.Count;
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell("cell");
				if (cell == null) {
					cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
				}
				cell.TextLabel.Text = Handler.data[indexPath.Row].Text;
				return cell;
			}
		}

		class Delegate : UITableViewDelegate
		{
			public ListBoxHandler Handler { get; set; }

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Handler.Widget.OnSelectedIndexChanged (EventArgs.Empty);
			}

		}
		
		public ListBoxHandler ()
		{
			Control = new UITableView ();
			Control.DataSource = new DataSource{ Handler = this };
			Control.Delegate = new Delegate { Handler = this };
		}
		
		public void AddRange (IEnumerable<IListItem> collection)
		{
			data.AddRange(collection);
			Control.ReloadData();
		}

		public void AddItem (IListItem item)
		{
			data.Add (item);
			Control.ReloadData ();
		}

		public void RemoveItem (IListItem item)
		{
			data.Remove (item);
			Control.ReloadData ();
		}

		public int SelectedIndex {
			get	{ return Control.IndexPathForSelectedRow.Row; }
			set { Control.SelectRow (NSIndexPath.FromRowSection (value, 0), true, UITableViewScrollPosition.Middle); }
		}

		public void RemoveAll ()
		{
			data.Clear ();
			Control.ReloadData ();
		}

	}
}
