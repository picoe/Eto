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
	
	public class Section : List<Section>, ITreeGridItem<Section>
	{
		public string Text { get; set; }
		
		public bool Expanded { get; set; }

		public bool Expandable { get { return Count > 0; } }

		public ITreeGridItem Parent { get; set; }
		
		public new ITreeGridItem this [int index] {
			get
			{
				return null;
			}
		}
		
		public Section ()
		{
		}
		
		public Section (string text, IEnumerable<Section> sections)
			: base (sections.OrderBy (r => r.Text, StringComparer.CurrentCultureIgnoreCase))
		{
			this.Text = text;
			this.Expanded = true;
			this.ForEach (r => r.Parent = this);
		}
	}
	
	public class Section<T> : Section, ISectionGenerator
		where T: Control, new()
	{
		public Control GenerateControl ()
		{
			try {
				return new T ();
			}
			catch (Exception ex) {
				Log.Write (this, "Error loading section: {0}", ex.InnerException != null ? ex.InnerException : ex);
				return null;
			}
		}
	}
	
		
	public class SectionList : TreeGridView
	{
		Container contentContainer;
		
		public SectionList (Container contentContainer)
		{
			this.contentContainer = contentContainer;
			this.Style = "sectionList";
			this.ShowHeader = false;
			
			Columns.Add (new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding ("Text") } });

			this.DataStore = new Section ("Top", TopNodes ());
			HandleEvent (SelectionChangedEvent);
		}

		IEnumerable<Section> TopNodes ()
		{
			yield return new Section ("Behaviors", BehaviorsSection ());
			yield return new Section ("Drawing", DrawingSection ());
			yield return new Section ("Controls", ControlSection ());
			yield return new Section ("Layouts", LayoutsSection ());
			yield return new Section ("Dialogs", DialogsSection ());
			yield return new Section ("Printing", PrintingSection ());
			yield return new Section ("Serialization", SerializationSection ());
		}
		
		IEnumerable<Section> ControlSection ()
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
			yield return new Section<TreeGridViewSection> { Text = "Tree Grid View" };
			yield return new Section<TreeViewSection> { Text = "Tree View" };
			yield return new Section<NumericUpDownSection> { Text = "Numeric Up/Down" };
			yield return new Section<DateTimePickerSection> { Text = "Date / Time" };
			yield return new Section<ComboBoxSection> { Text = "Combo Box" };
			yield return new Section<GroupBoxSection> { Text = "Group Box" };
			yield return new Section<SliderSection> { Text = "Slider" };
			yield return new Section<GridViewSection> { Text = "Grid View" };
			yield return new Section<GridCellFormattingSection> { Text = "Grid Cell Formatting" };
			yield return new Section<PasswordBoxSection> { Text = "Password Box" };
			yield return new Section<ProgressBarSection> { Text = "Progress Bar" };
			yield return new Section<KitchenSinkSection> { Text = "Kitchen Sink" };
		}

		IEnumerable<Section> DrawingSection ()
		{
			yield return new Section<BitmapSection> { Text = "Bitmap" };
			yield return new Section<IndexedBitmapSection> { Text = "Indexed Bitmap" };
			yield return new Section<PathSection> { Text = "Line Path" };
			yield return new Section<AntialiasSection> { Text = "Antialias" };
			yield return new Section<DrawTextSection> { Text = "Draw Text" };
			yield return new Section<FontsSection> { Text = "Control Fonts" };
			yield return new Section<InterpolationSection> { Text = "Image Interpolation" };
		}

		IEnumerable<Section> LayoutsSection ()
		{
			yield return new Section ("Table Layout", TableLayoutsSection ());
		}

		IEnumerable<Section> TableLayoutsSection ()
		{
			yield return new Section<Sections.Layouts.TableLayoutSection.RuntimeSection> { Text = "Runtime Creation" };
			yield return new Section<Sections.Layouts.TableLayoutSection.SpacingSection> { Text = "Spacing & Scaling" };
		}

		IEnumerable<Section> DialogsSection ()
		{
			yield return new Section<Sections.Dialogs.ColorDialogSection> { Text = "Color Dialog" };
			yield return new Section<Sections.Dialogs.FileDialogSection> { Text = "File Dialog" };
			yield return new Section<Sections.Dialogs.SelectFolderSection> { Text = "Select Folder Dialog" };
			yield return new Section<Sections.Dialogs.CustomDialogSection> { Text = "Custom Dialog" };
		}

		IEnumerable<Section> SerializationSection ()
		{
			yield return new Section<Sections.Serialization.JsonReadSection> { Text = "Json" };
#if XAML
			yield return new Section<Sections.Serialization.XamlReadSection> { Text = "Xaml" };
#endif
		}

		IEnumerable<Section> PrintingSection ()
		{
			yield return new Section<Sections.Printing.PrintDialogSection> { Text = "Print Dialog" };
		}

		IEnumerable<Section> BehaviorsSection ()
		{
			yield return new Section<Sections.Behaviors.FocusEventsSection> { Text = "Focus Events" };
			yield return new Section<Sections.Behaviors.MouseEventsSection> { Text = "Mouse Events" };
			yield return new Section<Sections.Behaviors.KeyEventsSection> { Text = "Key Events" };
			yield return new Section<Sections.Behaviors.BadgeLabelSection> { Text = "Badge Label" };
#if DESKTOP
			yield return new Section<Sections.Behaviors.ContextMenuSection> { Text = "Context Menu" };
#endif
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

