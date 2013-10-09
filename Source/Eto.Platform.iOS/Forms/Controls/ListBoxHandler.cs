using System;
using System.Reflection;
using Eto.Forms;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Linq;
using Eto.Platform.iOS.Drawing;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class ListBoxHandler : iosView<UITableView, ListBox>, IListBox
	{
		CollectionChangedHandler<IListItem, IListStore> collection;

		public override UIView ContainerControl { get { return Control; } }
		
		class DataSource : UITableViewDataSource
		{
			 static NSString kCellIdentifier = new NSString ("cell");

			public ListBoxHandler Handler { get; set; }

			public override int RowsInSection (UITableView tableView, int section)
			{
				return Handler.collection != null ? Handler.collection.Collection.Count : 0;
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(kCellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
				}
				var item = Handler.collection.Collection[indexPath.Row];
				cell.TextLabel.Text = item.Text;
				var imageItem = item as IImageListItem;
				if (imageItem != null)
					cell.ImageView.Image = imageItem.Image.ToUI ();
				else
					cell.ImageView.Image = null;
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
		
		class CollectionHandler : CollectionChangedHandler<IListItem, IListStore>
		{
			public ListBoxHandler Handler { get; set; }
			
			protected override void OnRegisterCollection (EventArgs e)
			{
				Handler.Control.ReloadData ();
			}
			protected override void OnUnregisterCollection (EventArgs e)
			{
				Handler.Control.ReloadData ();
			}
			public override void AddItem (IListItem item)
			{
				Handler.Control.ReloadData ();
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.Control.ReloadData ();
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.ReloadData ();
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.ReloadData ();
			}

			protected override int InternalIndexOf (IListItem item)
			{
				return -1; // TODO
			}
		}
		
		public IListStore DataStore {
			get {
				return collection != null ? collection.Collection : null;
			}
			set {
				if (collection != null) {
					var oldcollection = collection;
					collection = null;
					oldcollection.Unregister ();
				}
				if (value != null) {
					collection = new CollectionHandler { Handler = this };
					collection.Register (value);
				}
			}
		}
		
		public int SelectedIndex {
			get	{ return Control.IndexPathForSelectedRow != null ? Control.IndexPathForSelectedRow.Row : -1; }
			set { Control.SelectRow (NSIndexPath.FromRowSection (value, 0), true, UITableViewScrollPosition.Middle); }
		}
	}
}
