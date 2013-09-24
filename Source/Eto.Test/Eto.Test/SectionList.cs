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

		void Show(Navigation navigation, Panel contentContainer, string sectionTitle);
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
			: base (sections.OrderBy (r => r.Text, StringComparer.CurrentCultureIgnoreCase).ToArray())
		{
			this.Text = text;
			this.Expanded = true;
			this.ForEach (r => r.Parent = this);
		}
	}

	public abstract class SectionBase : Section, ISectionGenerator
	{
		public abstract Control GenerateControl();

		public void Show(Navigation navigation, Panel contentContainer, string sectionTitle)
		{
			Control sectionControl = null;

			try
			{
				sectionControl = this.GenerateControl();
			}
			catch (Exception ex)
			{
				Log.Write(this, "Error loading section: {0}", ex.InnerException != null ? ex.InnerException : ex);
			}
		
			if (navigation != null)
			{
				if (sectionControl != null)
					navigation.Push(sectionControl, sectionTitle);
			}
			else
			{
				contentContainer.SuspendLayout();
				contentContainer.Content = sectionControl;
				contentContainer.ResumeLayout();
			}
		}
	}

	public class Section<T> : SectionBase
		where T: Control, new()
	{
		public override Control GenerateControl ()
		{
			return new T();
		}
	}

	/// <summary>
	/// Tests for dialogs and forms use this.
	/// </summary>
	public class WindowSectionMethod : Section, ISectionGenerator
	{
		private Func<Window> Func { get; set; }

		public WindowSectionMethod(string text, Func<Window> f) { Func = f; Text = text; }

		protected WindowSectionMethod(string text = null)
		{
		}

		protected virtual Window GetWindow()
		{
			return null;
		}

		public void Show(Navigation navigation, Panel contentContainer, string sectionTitle)
		{
			try
			{
				var window = Func != null ? Func() : null; // First try the delegate method
				if (window == null)
					window = GetWindow(); // then the virtual method

				if (window != null)
				{
					var dialog = window as Dialog;
					if (dialog != null)
					{
						dialog.ShowDialog(null);
						return;
					}
					var form = window as Form;
					if (form != null)
					{
						form.Show();
						return;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Write(this, "Error loading section: {0}", ex.InnerException != null ? ex.InnerException : ex);
			}
		}
	}
		
	public class SectionList : TreeGridView
	{
		public SectionList(IEnumerable<Section> topNodes)
		{
			this.Style = "sectionList";
			this.ShowHeader = false;

			Columns.Add (new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding ("Text") } });

			this.DataStore = new Section ("Top", topNodes);
			HandleEvent (SelectionChangedEvent);
		}

		public string SectionTitle {
			get {
				var section = this.SelectedItem as Section;
				if (section != null)
					return section.Text;
				return null;
			}
		}
		
		public void Show(Navigation navigation, Panel contentContainer)
		{
			var sectionGenerator = this.SelectedItem as ISectionGenerator;
			if (sectionGenerator != null)
				sectionGenerator.Show(navigation, contentContainer, this.SectionTitle);
		}
	}
}

