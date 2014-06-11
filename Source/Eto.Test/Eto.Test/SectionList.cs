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
			: base (sections.OrderBy (r => r.Text, StringComparer.CurrentCultureIgnoreCase).ToArray())
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
	/// Tests for dialogs and forms use this.
	/// </summary>
	public class WindowSection : Section, ISection
	{
		Func<Window> Func { get; set; }

		public WindowSection(string text = null)
		{
			Text = text;
		}

		public WindowSection(string text, Func<Window> f)
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
							dialog.ShowModal(null);
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

	/// <summary>
	/// The base class for views that display the set of tests.
	/// </summary>
	public abstract class SectionList
	{
		public abstract Control Control { get; }
		public abstract ISection SelectedItem { get; }
		public event EventHandler SelectedItemChanged;

		public string SectionTitle
		{
			get
			{
				var section = SelectedItem as Section;
				return section != null ? section.Text : null;
			}
		}

		protected void OnSelectedItemChanged(object sender, EventArgs e)
		{
			if (this.SelectedItemChanged != null)
				this.SelectedItemChanged(sender, e);
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
				return sectionTreeItem.Section as ISection;
			}
		}

		public SectionListTreeGridView(IEnumerable<Section> topNodes)
		{
			this.treeView = new TreeGridView();
			treeView.Style = "sectionList";
			treeView.ShowHeader = false;
			treeView.Columns.Add(new GridColumn { DataCell = new TextBoxCell { Binding = new PropertyBinding<string>("Text") } });
			treeView.DataStore = new SectionTreeItem(new Section("Top", topNodes));
			treeView.SelectedItemChanged += OnSelectedItemChanged;
		}
	}

	public class SectionListTreeView : SectionList
	{
		TreeView treeView;

		public override Control Control { get { return this.treeView; } }

		public override void Focus() { Control.Focus(); }

		public override ISection SelectedItem
		{
			get
			{
				var treeItem = treeView.SelectedItem as TreeItem;
				return treeItem.Tag as ISection;
			}
		}

		public SectionListTreeView(IEnumerable<Section> topNodes)
		{
			this.treeView = new TreeView();
			treeView.Style = "sectionList";
			var top = new TreeItem(Enumerate(topNodes)) { Text = "Top", Tag = topNodes };
			treeView.DataStore = top;
			treeView.SelectionChanged += OnSelectedItemChanged;
		}

		private IEnumerable<ITreeItem> Enumerate(IEnumerable<Section> topNodes)
		{
			foreach (var section in topNodes)
			{
				var item = new TreeItem(Enumerate(section)) { Text = section.Text, Tag = section };
				yield return item;
			}
		}
	}

	public class SectionListGridView : SectionList
	{
		class MyItem
		{
			public string Name { get; set; }
			public string SectionName { get; set; }
			public Section Section { get; set; }
		}

		DynamicLayout layout;
		GridView gridView;
		SearchBox filterText;

		public override Control Control { get { return this.layout; } }
		public override ISection SelectedItem
		{
			get
			{
				var item = gridView.SelectedItem as MyItem;
				return item != null ? item.Section as ISection: null;
			}
		}

		public override void Focus() { filterText.Focus(); }

		public SectionListGridView(IEnumerable<Section> topNodes)
		{
			gridView = new GridView { ShowCellBorders = false };
			gridView.Columns.Add(new GridColumn { HeaderText = "Name", Width = 100, AutoSize = false, DataCell = new TextBoxCell("Name"), Sortable = true });
			gridView.Columns.Add(new GridColumn { HeaderText = "Section", DataCell = new TextBoxCell("SectionName"), Sortable = true });
			var items = new DataStoreCollection();
			foreach (var section in topNodes)
			{				
				foreach (var test in section)
				{
					items.Add(new MyItem { 
						Name = (test as ISectionName).Text,
						SectionName = (section as ISectionName).Text, 
						Section = test,
					});
				}
			}
			gridView.DataStore = items;
			gridView.SelectionChanged += OnSelectedItemChanged;

			layout = new DynamicLayout();
			layout.Add(filterText = new SearchBox { PlaceholderText = "Filter" });
			layout.Add(gridView);

			// Filter
			filterText.TextChanged += (s, e) => {
				var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				// Set the filter delegate on the GridView
				gridView.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o => {
					var i = o as MyItem;
					var matches = true;

					// Every item in the split filter string should be within the Text property
					foreach (var filterItem in filterItems)
						if (i.Name.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1 &&
							i.SectionName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
						{
							matches = false;
							break;
						}

					return matches;
				};
			};			
		}
	}
}

