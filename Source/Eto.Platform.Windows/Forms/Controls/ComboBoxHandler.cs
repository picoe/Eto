using System;
using System.Reflection;
using SWF = System.Windows.Forms;
using SD = System.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

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
				DisplayMember = "Text",
				AutoSize = true,
				Size = SD.Size.Empty
			};
			Control.SelectedIndexChanged += delegate {
				Widget.OnSelectedIndexChanged(EventArgs.Empty);
			};

		}

		public override Size DesiredSize
		{
			get
			{
				var size = new Size(16, 20);
				var font = Control.Font;
				
				using (var graphics = Control.CreateGraphics ()) {
					foreach (object item in Control.Items) {
						var text = Control.GetItemText (item);
						var itemSize = Generator.ConvertF (graphics.MeasureString (text, font));
						// for drop down glyph and border
						size = Size.Max (size, itemSize);
					}
				}
				size += new Size (18, 8);
				return size;
			}
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
				Handler.UpdateSizes ();
			}
			
			public override void AddItem (IListItem item)
			{
				Handler.Control.Items.Add (item);
				Handler.UpdateSizes ();
			}

			public override void InsertItem (int index, IListItem item)
			{
				Handler.Control.Items.Insert (index, item);
				Handler.UpdateSizes ();
			}

			public override void RemoveItem (int index)
			{
				Handler.Control.Items.RemoveAt (index);
				Handler.UpdateSizes ();
			}

			public override void RemoveAllItems ()
			{
				Handler.Control.Items.Clear ();
				Handler.UpdateSizes ();
			}
		}

		protected void UpdateSizes ()
		{
			CalculateMinimumSize ();
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
