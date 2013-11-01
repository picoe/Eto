using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Test
{
	public interface ISection
	{
		string Text { get; }

		Control CreateContent();
	}

	public class Section : List<Section>, ITreeGridItem<Section>
	{
		public string Text { get; set; }

		public bool Expanded { get; set; }

		public bool Expandable { get { return Count > 0; } }

		public ITreeGridItem Parent { get; set; }

		public new ITreeGridItem this [int index]
		{
			get { return null; }
		}

		public Section()
		{
		}

		public Section(string text, IEnumerable<Section> sections)
			: base (sections.OrderBy (r => r.Text, StringComparer.CurrentCultureIgnoreCase).ToArray())
		{
			this.Text = text;
			this.Expanded = true;
			this.ForEach(r => r.Parent = this);
		}
	}

	public abstract class SectionBase : Section, ISection
	{
		public abstract Control CreateContent();
	}

	public class Section<T> : SectionBase
		where T: Control, new()
	{
		public override Control CreateContent()
		{
			return new T();
		}
	}

	/// <summary>
	/// Tests for dialogs and forms use this.
	/// </summary>
	public class WindowSectionMethod : Section, ISection
	{
		Func<Window> Func { get; set; }

		public WindowSectionMethod(string text = null)
		{
			Text = text;
		}

		public WindowSectionMethod(string text, Func<Window> f)
		{
			Func = f;
			Text = text;
		}

		protected virtual Window GetWindow()
		{
			return null;
		}

		public Control CreateContent()
		{
			var button = new Button { Text = string.Format("Show the {0} test", Text) };
			var layout = new DynamicLayout();
			layout.AddCentered(button);
			button.Click += (sender, e) => {

				try
				{
					var window = Func != null ? Func() : null ?? GetWindow();

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
					Log.Write(this, "Error loading section: {0}", ex.GetBaseException());
				}
			};
			return layout;
		}
	}

	public class SectionList : TreeGridView
	{
		public new ISection SelectedItem
		{
			get { return base.SelectedItem as ISection; }
		}


		public SectionList(IEnumerable<Section> topNodes)
		{
			this.Style = "sectionList";
			this.ShowHeader = false;

			Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding ("Text") } });

			this.DataStore = new Section("Top", topNodes);
		}

		public string SectionTitle
		{
			get
			{
				var section = SelectedItem as Section;
				return section != null ? section.Text : null;
			}
		}
	}
}

