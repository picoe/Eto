using System;
using System.Reflection;
using Eto.Forms;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Linq;
using Eto.iOS.Drawing;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class ListBoxHandler : IosView<UITableView, ListBox, ListBox.ICallback>, ListBox.IHandler
	{
		CollectionHandler collection;

		public override UIView ContainerControl { get { return Control; } }

		class DataSource : UITableViewDataSource
		{
			static readonly NSString kCellIdentifier = new NSString("cell");

			public ListBoxHandler Handler { get; set; }

			public override nint RowsInSection(UITableView tableView, nint section)
			{
				return Handler.collection != null ? Handler.collection.Collection.Count() : 0;
			}

			public override nint NumberOfSections(UITableView tableView)
			{
				return 1;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(kCellIdentifier);
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, kCellIdentifier);
				}
				var item = Handler.collection.ElementAt(indexPath.Row);
				cell.TextLabel.Text = Handler.Widget.ItemTextBinding.GetValue(item);
				var imageBinding = Handler.Widget.ItemImageBinding;
				if (imageBinding != null)
					cell.ImageView.Image = imageBinding.GetValue(item).ToUI();
				else
					cell.ImageView.Image = null;
				return cell;
			}
		}

		class Delegate : UITableViewDelegate
		{
			public ListBoxHandler Handler { get; set; }

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				Handler.Callback.OnSelectedIndexChanged(Handler.Widget, EventArgs.Empty);
			}
		}

		public ListBoxHandler()
		{
			Control = new UITableView();
			Control.DataSource = new DataSource{ Handler = this };
			Control.Delegate = new Delegate { Handler = this };
		}

		class CollectionHandler : EnumerableChangedHandler<object>
		{
			public ListBoxHandler Handler { get; set; }

			protected override void OnRegisterCollection(EventArgs e)
			{
				Handler.Control.ReloadData();
			}

			protected override void OnUnregisterCollection(EventArgs e)
			{
				Handler.Control.ReloadData();
			}

			public override void AddItem(object item)
			{
				Handler.Control.ReloadData();
			}

			public override void InsertItem(int index, object item)
			{
				Handler.Control.ReloadData();
			}

			public override void RemoveItem(int index)
			{
				Handler.Control.ReloadData();
			}

			public override void RemoveAllItems()
			{
				Handler.Control.ReloadData();
			}
		}

		public IEnumerable<object> DataStore
		{
			get
			{
				return collection != null ? collection.Collection : null;
			}
			set
			{
				if (collection != null)
				{
					var oldcollection = collection;
					collection = null;
					oldcollection.Unregister();
				}
				if (value != null)
				{
					collection = new CollectionHandler { Handler = this };
					collection.Register(value);
				}
			}
		}

		public int SelectedIndex
		{
			get	{ return Control.IndexPathForSelectedRow != null ? Control.IndexPathForSelectedRow.Row : -1; }
			set { Control.SelectRow(NSIndexPath.FromRowSection(value, 0), true, UITableViewScrollPosition.Middle); }
		}

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Color TextColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}
