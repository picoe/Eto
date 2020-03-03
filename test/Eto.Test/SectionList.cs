using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

namespace Eto.Test
{
	public interface ISectionName
	{
		string Text { get; }
	}

	public interface ISection : ISectionName
	{
		Control CreateContent();
	}

	/// <summary>
	/// Sections can nest. Each section item can also host
	/// a control that is displayed in the details view when 
	/// the section is selected.
	/// 
	/// Sections do not have any particular visual representation,
	/// and can be wrapped within a tree item (SectionTreeItem) or
	/// any other visual representation.
	/// </summary>
	public class Section : List<Section>, ISectionName
	{
		public string Text { get; set; }

		public Section()
		{
		}

		public Section(string text, IEnumerable<Section> sections)
			: base(sections.OrderBy(r => r.Text, StringComparer.CurrentCultureIgnoreCase).ToArray())
		{
			this.Text = text;
		}
	}

	/// <summary>
	/// A tree item representation of a section.
	/// </summary>
	public class SectionTreeItem : List<SectionTreeItem>, ITreeGridItem<SectionTreeItem>
	{
		public Section Section { get; private set; }
		public string Text { get { return Section.Text; } }
		public bool Expanded { get; set; }
		public bool Expandable { get { return Count > 0; } }
		public ITreeGridItem Parent { get; set; }

		public SectionTreeItem(Section section)
		{
			this.Section = section;
			this.Expanded = true;
			foreach (var child in section)
			{
				var temp = new SectionTreeItem(child);
				temp.Parent = this;
				this.Add(temp); // recursive
			}
		}
	}
	public class SectionItem : Section, ISection
	{
		public Func<Control> Creator { get; set; }

		public Control CreateContent()
		{
			return Creator();
		}
	}

	/// <summary>
	/// The base class for views that display the set of tests.
	/// </summary>
	public abstract class SectionList
	{
		public abstract Control Control { get; }
		public abstract ISection SelectedItem { get; set; }
		public event EventHandler SelectedItemChanged;

		public string SectionTitle => SelectedItem?.Text;

		protected void OnSelectedItemChanged(EventArgs e)
		{
			SelectedItemChanged?.Invoke(this, e);
		}

		public abstract void Focus();
	}

	public class SectionListTreeGridView : SectionList
	{
		TreeGridView treeView;

		public override Control Control { get { return this.treeView; } }

		public override void Focus() { Control.Focus(); }

		public override ISection SelectedItem
		{
			get
			{
				var sectionTreeItem = treeView.SelectedItem as SectionTreeItem;
				return sectionTreeItem?.Section as ISection;
			}
			set
			{
				treeView.SelectedItem = FindItem(treeView.DataStore as SectionTreeItem, value);
			}
		}

		ITreeGridItem FindItem(SectionTreeItem node, ISection section)
		{
			foreach (var item in node)
			{
				if (ReferenceEquals(item.Section, section))
					return item;
				if (item.Count > 0)
				{
					var child = FindItem(item, section);
					if (child != null)
						return child;
				}
			}
			return null;
		}

		public SectionListTreeGridView(IEnumerable<Section> topNodes)
		{
			this.treeView = new TreeGridView();
			treeView.Style = "sectionList";
			treeView.ShowHeader = false;
			treeView.AllowEmptySelection = false;
			treeView.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = new DelegateBinding<SectionTreeItem, string>(r => r.Text) } });
			treeView.SelectedItemChanged += (sender, e) => OnSelectedItemChanged(e);
			treeView.DataStore = new SectionTreeItem(new Section("Top", topNodes));
		}
	}

	public class SectionListGridView : SectionList
	{
		FilterCollection<MyItem> items;
		class MyItem
		{
			public string Name { get; set; }
			public string SectionName { get; set; }
			public Section Section { get; set; }
		}

		TableLayout layout;
		GridView gridView;
		SearchBox filterText;

		public override Control Control { get { return layout; } }
		public override ISection SelectedItem
		{
			get
			{
				var item = gridView.SelectedItem as MyItem;
				return item != null ? item.Section as ISection : null;
			}
			set
			{
				var item = items.FirstOrDefault(r => ReferenceEquals(r.Section, value));
				gridView.SelectedRow = items.IndexOf(item);
			}
		}

		public override void Focus() { filterText.Focus(); }

		public SectionListGridView(IEnumerable<Section> topNodes)
		{
			gridView = new GridView { GridLines = GridLines.None };
			gridView.Columns.Add(new GridColumn { HeaderText = "Name", Width = 100, AutoSize = false, DataCell = new TextBoxCell { Binding = new DelegateBinding<MyItem, string>(r => r.Name) }, Sortable = true });
			gridView.Columns.Add(new GridColumn { HeaderText = "Section", DataCell = new TextBoxCell { Binding = new DelegateBinding<MyItem, string>(r => r.SectionName) }, Sortable = true });
			items = new FilterCollection<MyItem>();
			foreach (var section in topNodes)
			{
				foreach (var test in section)
				{
					items.Add(new MyItem
					{
						Name = test.Text,
						SectionName = section.Text,
						Section = test,
					});
				}
			}
			gridView.DataStore = items;
			gridView.SelectionChanged += (sender, e) => OnSelectedItemChanged(e);

			layout = new TableLayout(
				filterText = new SearchBox { PlaceholderText = "Filter" },
				gridView
			);

			// Filter
			filterText.TextChanged += (s, e) =>
			{
				var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				if (filterItems.Length == 0)
					items.Filter = null;
				else
					items.Filter = i =>
					{
						// Every item in the split filter string should be within the Text property
						foreach (var filterItem in filterItems)
							if (i.Name.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1 &&
								i.SectionName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
							{
								return false;
							}

						return true;
					};
			};
		}
	}
}

