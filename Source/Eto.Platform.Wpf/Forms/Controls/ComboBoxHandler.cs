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
using System.Collections;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class ComboBoxHandler : WpfControl<swc.ComboBox, ComboBox>, IComboBox
	{
		IListStore store;

		public class EtoComboBox : swc.ComboBox
		{
			int? _selected;

			public EtoComboBox ()
			{
				Loaded += ComboBoxEx_Loaded;
			}

			public override void OnApplyTemplate ()
			{
				base.OnApplyTemplate ();

				_selected = SelectedIndex;
				SelectedIndex = -1;
			}

			protected override void OnSelectionChanged (swc.SelectionChangedEventArgs e)
			{
				if (_selected == null)
					base.OnSelectionChanged (e);
			}

			protected override void OnItemsChanged (System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				base.OnItemsChanged (e);
				if (this.IsLoaded) {
					this.InvalidateMeasure ();
				}
			}

			void ComboBoxEx_Loaded (object sender, sw.RoutedEventArgs e)
			{
				if (_selected != null) {
					SelectedIndex = _selected.Value;
					_selected = null;
				}
			}

			protected override sw.Size MeasureOverride (sw.Size constraint)
			{
				var size = base.MeasureOverride (constraint);

				var popup = GetTemplateChild ("PART_Popup") as swc.Primitives.Popup;
				var content = popup.Child as sw.FrameworkElement;
				content.Measure (constraint);
				size.Width = Math.Min(constraint.Width, Math.Max (content.DesiredSize.Width, size.Width));
				return size;
			}

		}

		public ComboBoxHandler ()
		{
			Control = new EtoComboBox ();
			var template = new sw.DataTemplate (typeof (IListItem));
			template.VisualTree = WpfListItemHelper.TextBlock ();
			Control.ItemTemplate = template;
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			Control.SelectionChanged += delegate {
				Widget.OnSelectedIndexChanged (EventArgs.Empty);
			};
		}


		public IListStore DataStore
		{
			get { return store; }
			set
			{
				store = value;
				Control.ItemsSource = store as IEnumerable ?? store.AsEnumerable ();
			}
		}

		public int SelectedIndex
		{
			get { return Control.SelectedIndex; }
			set { Control.SelectedIndex = value; }
		}
	}
}
