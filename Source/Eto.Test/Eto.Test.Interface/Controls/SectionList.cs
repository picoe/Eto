using System;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Interface.Controls
{
	public interface ISectionGenerator
	{
		string Text { get; }
		Control GenerateControl();
	}
	
	public class Section<T> : ISectionGenerator, IListItem
		where T: Control, new()
	{
		public string Text { get; set; }
		
		public string Key
		{
			get { return Text; }
		}
		
		public Control GenerateControl()
		{
			return new T();
		}
		
		public override string ToString ()
		{
			return Text;
		}
	}

		
	public class SectionList : ListBox
	{
		Container contentContainer;
		
		public SectionList (Container contentContainer)
		{
			this.contentContainer = contentContainer;
			this.Style = "sectionList";
			
			// add some items
			var items = new List<ISectionGenerator>();
			
			items.Add (new Section<LabelSection> { Text = "Label Control" });
			items.Add (new Section<ButtonSection> { Text = "Button Control" });
			items.Add (new Section<ScrollableSection> { Text = "Scrollable Control" });
			items.Add (new Section<TextAreaSection> { Text = "Text Area" });
			items.Add (new Section<WebViewSection> { Text = "Web View" });
			items.Add (new Section<FileDialogSection> { Text = "File Dialog" });
			items.Add (new Section<DrawableSection> { Text = "Drawable" });
			items.Add (new Section<ListBoxSection> { Text = "List Box" });
			items.Add (new Section<TabControlSection> { Text = "Tab Control" });
			items.Add (new Section<TreeViewSection> { Text = "Tree View" });
			items.Add (new Section<NumericUpDownSection> { Text = "Numeric Up/Down" });
			items.Add (new Section<DateTimePickerSection> { Text = "Date / Time" });
			items.Sort ((x, y) => string.Compare (x.Text, y.Text, StringComparison.CurrentCultureIgnoreCase));
			Items.AddRange (items);
			
			this.SelectedIndex = 0;
		}
		
		public override void OnSelectedIndexChanged (EventArgs e)
		{
			base.OnSelectedIndexChanged (e);
			
			var sectionGenerator = this.SelectedValue as ISectionGenerator;
			
			if (sectionGenerator != null) {
				var control = sectionGenerator.GenerateControl();
				contentContainer.AddDockedControl (control);
			}
		}
	}
}

