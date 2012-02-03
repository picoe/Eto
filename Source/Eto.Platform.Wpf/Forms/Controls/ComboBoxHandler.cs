using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swc = System.Windows.Controls;
using sw = System.Windows;
using swd = System.Windows.Data;
using swa = System.Windows.Automation;
using swm = System.Windows.Media;
using Eto.Forms;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ComboBoxHandler : WpfControl<swc.ComboBox, ComboBox>, IComboBox
	{

		public class EtoComboBox : swc.ComboBox
		{
			private int _selected;

			public override void OnApplyTemplate ()
			{
				base.OnApplyTemplate ();

				_selected = SelectedIndex;
				SelectedIndex = -1;

				Loaded += ComboBoxEx_Loaded;
			}

			void ComboBoxEx_Loaded (object sender, sw.RoutedEventArgs e)
			{
				var popup = GetTemplateChild ("PART_Popup") as swc.Primitives.Popup;
				var content = popup.Child as sw.FrameworkElement;
				content.Measure (new sw.Size (double.PositiveInfinity, double.PositiveInfinity));
				MinWidth = content.DesiredSize.Width;
				SelectedIndex = _selected;
			}

		}

		public ComboBoxHandler ()
		{
			Control = new EtoComboBox ();
			Control.SelectionChanged += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
			var template = new sw.DataTemplate (typeof (IListItem));
			template.VisualTree = WpfListItemHelper.TextBlock ();
			Control.ItemTemplate = template;
		}


		public void AddRange (IEnumerable<IListItem> collection)
		{
			foreach (var item in collection)
				Control.Items.Add (item);
		}

		public void AddItem (IListItem item)
		{
			Control.Items.Add (item);
		}

		public void RemoveItem (IListItem item)
		{
			Control.Items.Remove (item);
		}

		public void RemoveAll ()
		{
			Control.Items.Clear ();
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}
	}
}
