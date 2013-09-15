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
		public SectionList(Func<IEnumerable<Section>> topNodes)
		{
			this.TopNodes = topNodes;
			this.Style = "sectionList";
			this.ShowHeader = false;

			Columns.Add (new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding ("Text") } });

			this.DataStore = new Section ("Top", TopNodes ());
			HandleEvent (SelectionChangedEvent);
		}

		private Func<IEnumerable<Section>> TopNodes { get; set; }

		public Control SectionControl { get; private set; }

		public string SectionTitle {
			get {
				var section = this.SelectedItem as Section;
				if (section != null)
					return section.Text;
				return null;
			}
		}
		
		public override void OnSelectionChanged (EventArgs e)
		{
			var sectionGenerator = this.SelectedItem as ISectionGenerator;
			
			if (sectionGenerator != null) {
				SectionControl = sectionGenerator.GenerateControl ();
			} else 
				SectionControl = null;

			base.OnSelectionChanged (e);
		}
	}
}

