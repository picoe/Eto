using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public interface ISectionGenerator
	{
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
			
			// add some items
			Items.Add (new Section<LabelSection> { Text = "Label Control" });
			Items.Add (new Section<ButtonSection> { Text = "Button Control" });
			Items.Add (new Section<ScrollableSection> { Text = "Scrollable Control" });
			Items.Add (new Section<TextAreaSection> { Text = "Text Area" });
			Items.Add (new Section<WebViewSection> { Text = "Web View" });
			Items.Add (new Section<FileDialogSection> { Text = "File Dialog" });
			Items.Add (new Section<DrawableSection> { Text = "Drawable" });
			Items.Add (new Section<ListBoxSection> { Text = "List Box" });
			
			this.SelectedIndex = 5; // select the first item
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

