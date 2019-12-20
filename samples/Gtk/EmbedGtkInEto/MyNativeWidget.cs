using System;
using Gtk;

namespace EmbedGtkInEto
{
	public class MyNativeWidget : Bin
	{
		private VBox vbox1;

		private Label label1;

		private Entry entry2;

		private ComboBox combobox1;

		public MyNativeWidget()
		{
			Name = "EmbedGtkInEto.MyNativeWidget";
			// Container child EmbedGtkInEto.MyNativeWidget.Gtk.Container+ContainerChild
			vbox1 = new VBox();
			vbox1.Name = "vbox1";
			vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			label1 = new global::Gtk.Label();
			label1.Name = "label1";
			label1.LabelProp = "A GtkSharp Control";
			vbox1.Add(label1);

			// Container child vbox1.Gtk.Box+BoxChild
			entry2 = new Entry();
			entry2.CanFocus = true;
			entry2.Name = "entry2";
			entry2.IsEditable = true;
			entry2.InvisibleChar = '●';
			vbox1.Add(entry2);

			// Container child vbox1.Gtk.Box+BoxChild
			combobox1 = ComboBox.NewWithEntry();
			combobox1.Name = "combobox1";
			vbox1.Add(combobox1);
			Add(vbox1);
			if ((Child != null))
			{
				Child.ShowAll();
			}
			Hide();
		}
	}
}

