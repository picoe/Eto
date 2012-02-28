using System;
using System.Linq;
using swf = System.Windows.Forms;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Windows.Forms.Controls
{
	public class ComboBoxCellHandler : CellHandler<swf.DataGridViewComboBoxCell, ComboBoxCell>, IComboBoxCell
	{
		CollectionHandler collection;

		public ComboBoxCellHandler ()
		{
			Control = new swf.DataGridViewComboBoxCell {
				ValueMember = "Key",
				DisplayMember = "Text"
			};
		}
		
		class CollectionHandler : CollectionChangedHandler<IListItem, IListStore>
		{
			public ComboBoxCellHandler Handler { get; set; }
			
			public override int IndexOf (IListItem item)
			{
				return Handler.Control.Items.IndexOf (item);
			}
			
			public override void AddRange (IEnumerable<IListItem> items)
			{
				Handler.Control.Items.AddRange (items.ToArray ());
			}
			
			public override void AddItem (IListItem item)
			{
				Handler.Control.Items.Add (item);
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.Control.Items.Insert (index, item);
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Items.RemoveAt (index);
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Items.Clear ();
			}
		}

		public IListStore DataStore {
			get { return collection != null ? collection.DataStore : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value); 
			}
		}
	}
}

