using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Test.Interface.Sections.Controls;
using Eto.Test.Interface.Sections.Drawing;
using Eto.Test.Interface.Sections.Layouts;

namespace Eto.Test.Interface.Controls
{
	public interface ISectionGenerator
	{
		string Text { get; }

		Control GenerateControl ();
	}
	
	public class Section<T> : ISectionGenerator, IListItem, ITreeItem
		where T: Control, new()
	{
		public string Text { get; set; }
		
		public string Key {
			get { return Text; }
		}
		
		public Control GenerateControl ()
		{
			return new T ();
		}
		
		public override string ToString ()
		{
			return Text;
		}

		#region ITreeItem implementation
		
		public ITreeItem GetChild (int index)
		{
			return null;
		}

		public int Count { get { return 0; } }
		public bool Expanded { get; set; }
		public bool Expandable { get { return false; } }
		public ITreeItem Parent { get; set; }
		#endregion

		#region IImageListItem implementation
		public Eto.Drawing.Image Image { get { return null; } }

		#endregion


	}
		
	public class SectionList : TreeView
	{
		Container contentContainer;
		TextArea eventLog;
		
		public SectionList (Container contentContainer, TextArea eventLog)
		{
			this.contentContainer = contentContainer;
			this.Style = "sectionList";
			this.eventLog = eventLog;
			

			var top = new TreeItem();
			
			top.Children.Add (DrawingSection());
			top.Children.Add (ControlSection ());
			top.Children.Add (LayoutsSection ());
			top.Children.Add (DialogsSection ());

			this.TopNode = top;
			
			this.SelectedItem = top.Children.FirstOrDefault();
		}
		
		ITreeItem ControlSection()
		{
			var node = new TreeItem { Text = "Controls", Expanded = true };
			var items = node.Children;
			
			items.Add (new Section<LabelSection> { Text = "Label Control" });
			items.Add (new Section<ButtonSection> { Text = "Button Control" });
			items.Add (new Section<CheckBoxSection> { Text = "Check Box" });
			items.Add (new Section<RadioButtonSection> { Text = "Radio Button" });
			items.Add (new Section<ScrollableSection> { Text = "Scrollable Control" });
			items.Add (new Section<TextBoxSection> { Text = "Text Box" });
			items.Add (new Section<TextAreaSection> { Text = "Text Area" });
			items.Add (new Section<WebViewSection> { Text = "Web View" });
			items.Add (new Section<DrawableSection> { Text = "Drawable" });
			items.Add (new Section<ListBoxSection> { Text = "List Box" });
			items.Add (new Section<TabControlSection> { Text = "Tab Control" });
			items.Add (new Section<TreeViewSection> { Text = "Tree View" });
			items.Add (new Section<NumericUpDownSection> { Text = "Numeric Up/Down" });
			items.Add (new Section<DateTimePickerSection> { Text = "Date / Time" });
			items.Add (new Section<ComboBoxSection> { Text = "Combo Box" });
			items.Add (new Section<GroupBoxSection> { Text = "Group Box" });
			items.Add (new Section<SliderSection> { Text = "Slider" });
			items.Add (new Section<XamlSection> { Text = "Xaml" });
			
			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return node;
		}

		ITreeItem DrawingSection ()
		{
			var node = new TreeItem { Text = "Drawing", Expanded = true };
			var items = node.Children;

			items.Add (new Section<BitmapSection> { Text = "Bitmap" });
			items.Add (new Section<PathSection> { Text = "Line Path" });
			items.Add (new Section<AntialiasSection> { Text = "Antialias" });

			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return node;
		}

		ITreeItem LayoutsSection ()
		{
			var node = new TreeItem { Text = "Layouts", Expanded = true };
			var items = node.Children;

			items.Add (TableLayoutsSection ());

			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return node;
		}

		ITreeItem TableLayoutsSection ()
		{
			var node = new TreeItem { Text = "Table Layout", Expanded = true };
			var items = node.Children;

			items.Add (new Section<Sections.Layouts.TableLayoutSection.RuntimeSection> { Text = "Runtime Creation" });
			items.Add (new Section<Sections.Layouts.TableLayoutSection.SpacingSection> { Text = "Spacing & Scaling" });

			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return node;
		}

		ITreeItem DialogsSection ()
		{
			var node = new TreeItem { Text = "Common Dialogs", Expanded = true };
			var items = node.Children;

			items.Add (new Section<Sections.Dialogs.ColorDialogSection> { Text = "Color Picker" });
			items.Add (new Section<Sections.Dialogs.FileDialogSection> { Text = "File Dialog" });

			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			return node;
		}
		
		public override void OnSelectionChanged (EventArgs e)
		{
			base.OnSelectionChanged (e);
			
			var sectionGenerator = this.SelectedItem as ISectionGenerator;
			
			if (sectionGenerator != null) {
				var control = sectionGenerator.GenerateControl ();
				contentContainer.AddDockedControl (control);
			}
			else 
				contentContainer.AddDockedControl (null);
		}
	}
}

