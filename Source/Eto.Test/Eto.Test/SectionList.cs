using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Test.Sections.Controls;
using Eto.Test.Sections.Drawing;
using Eto.Test.Sections.Layouts;

namespace Eto.Test
{
	public interface ISectionGenerator
	{
		string Text { get; }

		Control GenerateControl ();
	}
	
	public class Section<T> : ISectionGenerator, ITreeItem
		where T: Control, new()
	{
		public string Text { get; set; }
		
		public Control GenerateControl ()
		{
			return new T ();
		}
		
		public object GetValue (int column)
		{
			if (column == 0)
				return Text;
			else
				return null;
		}
		
		public void SetValue (int column, object value)
		{
			// nothing!
		}
		
		public override string ToString ()
		{
			return Text;
		}

		#region ITreeItem implementation
		
		public ITreeItem this [int index] {
			get { return null; }
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
		
		public SectionList (Container contentContainer)
		{
			this.contentContainer = contentContainer;
			this.Style = "sectionList";
			this.ShowHeader = false;
			
			Columns.Add (new TreeColumn ());

			this.DataStore = Section ("Top", TopNodes ());
		}
		
		ITreeItem Section (string label, IEnumerable<ITreeItem> items)
		{
			var node = new TreeItem { Expanded = true, Values = new object[] { label } };
			var children = node.Children;
			children.AddRange (items);

			children.Sort ((x, y) => string.Compare (Convert.ToString (x.GetValue (0)), Convert.ToString (y.GetValue (0)), StringComparison.CurrentCultureIgnoreCase));
			return node;
		}
		
		IEnumerable<ITreeItem> TopNodes ()
		{
			yield return Section ("Behaviors", BehaviorsSection ());
			yield return Section ("Drawing", DrawingSection ());
			yield return Section ("Controls", ControlSection ());
			yield return Section ("Layouts", LayoutsSection ());
			yield return Section ("Dialogs", DialogsSection ());
		}
		
		IEnumerable<ITreeItem> ControlSection ()
		{
			yield return new Section<LabelSection> { Text = "Label" };
			yield return new Section<ButtonSection> { Text = "Button" };
			yield return new Section<CheckBoxSection> { Text = "Check Box" };
			yield return new Section<RadioButtonSection> { Text = "Radio Button" };
			yield return new Section<ScrollableSection> { Text = "Scrollable" };
			yield return new Section<TextBoxSection> { Text = "Text Box" };
			yield return new Section<TextAreaSection> { Text = "Text Area" };
			yield return new Section<WebViewSection> { Text = "Web View" };
			yield return new Section<DrawableSection> { Text = "Drawable" };
			yield return new Section<ListBoxSection> { Text = "List Box" };
			yield return new Section<TabControlSection> { Text = "Tab Control" };
			yield return new Section<TreeViewSection> { Text = "Tree View" };
			yield return new Section<NumericUpDownSection> { Text = "Numeric Up/Down" };
			yield return new Section<DateTimePickerSection> { Text = "Date / Time" };
			yield return new Section<ComboBoxSection> { Text = "Combo Box" };
			yield return new Section<GroupBoxSection> { Text = "Group Box" };
			yield return new Section<SliderSection> { Text = "Slider" };
			yield return new Section<XamlSection> { Text = "Xaml" };
			yield return new Section<GridViewSection> { Text = "Grid View" };
			yield return new Section<PasswordBoxSection> { Text = "Password Box" };
			yield return new Section<ProgressBarSection> { Text = "Progress Bar" };
			yield return new Section<KitchenSinkSection> { Text = "Kitchen Sink" };
		}

		IEnumerable<ITreeItem> DrawingSection ()
		{
			yield return new Section<BitmapSection> { Text = "Bitmap" };
			yield return new Section<IndexedBitmapSection> { Text = "Indexed Bitmap" };
			yield return new Section<PathSection> { Text = "Line Path" };
			yield return new Section<AntialiasSection> { Text = "Antialias" };
		}

		IEnumerable<ITreeItem> LayoutsSection ()
		{
			yield return Section ("Table Layout", TableLayoutsSection ());
		}

		IEnumerable<ITreeItem> TableLayoutsSection ()
		{
			yield return new Section<Sections.Layouts.TableLayoutSection.RuntimeSection> { Text = "Runtime Creation" };
			yield return new Section<Sections.Layouts.TableLayoutSection.SpacingSection> { Text = "Spacing & Scaling" };
		}

		IEnumerable<ITreeItem> DialogsSection ()
		{
			yield return new Section<Sections.Dialogs.ColorDialogSection> { Text = "Color Dialog" };
			yield return new Section<Sections.Dialogs.FileDialogSection> { Text = "File Dialog" };
			yield return new Section<Sections.Dialogs.SelectFolderSection> { Text = "Select Folder Dialog" };
		}

		IEnumerable<ITreeItem> BehaviorsSection ()
		{
			yield return new Section<Sections.Behaviors.FocusEventsSection> { Text = "Focus Events" };
			yield return new Section<Sections.Behaviors.MouseEventsSection> { Text = "Mouse Events" };
			yield return new Section<Sections.Behaviors.KeyEventsSection> { Text = "Key Events" };
			yield return new Section<Sections.Behaviors.ContextMenuSection> { Text = "Context Menu" };
		}
		
		public override void OnSelectionChanged (EventArgs e)
		{
			base.OnSelectionChanged (e);
			
			var sectionGenerator = this.SelectedItem as ISectionGenerator;
			
			if (sectionGenerator != null) {
				var control = sectionGenerator.GenerateControl ();
				contentContainer.AddDockedControl (control);
			} else 
				contentContainer.AddDockedControl (null);
		}
	}
}

