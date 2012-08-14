using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Platform.Windows
{
	public class ComboBoxHandler : WindowsControl<SWF.ComboBox, ComboBox>, IComboBox
	{
		CollectionHandler collection;
		
		public ComboBoxHandler()
		{
			Control = new SWF.ComboBox {
				DropDownStyle = SWF.ComboBoxStyle.DropDownList,
				ValueMember = "Key",
				DisplayMember = "Text"
			};
			Control.SelectedIndexChanged += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};

		}

		public int SelectedIndex
		{
			get	{ return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}

		class CollectionHandler : DataStoreChangedHandler<IListItem, IListStore>
		{
			public ComboBoxHandler Handler { get; set; }
			
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
			get { return collection != null ? collection.Collection : null; }
			set {
				if (collection != null)
					collection.Unregister ();
				collection = new CollectionHandler { Handler = this };
				collection.Register (value); 
			}
		}
	}
}
