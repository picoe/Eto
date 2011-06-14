using System;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public interface ISectionGenerator
	{
		Control GenerateControl();
	}
	
	public class Section<T> : ISectionGenerator
		where T: Control, new()
	{
		public string Text { get; set; }
		
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
			
			Items.Add (new Section<LabelSection> { Text = "Label Control" });
			Items.Add (new Section<ButtonSection> { Text = "Button Control" });
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

