using System;
using System.Collections.Generic;
#if XAML
using System.Windows.Markup;
#endif
using System.Collections.Specialized;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ITreeItem<T>
	{
		bool Expanded { get; set; }
		
		bool Expandable { get; }
		
		T Parent { get; set; }
	}
	
	public interface ITreeGridItem : IGridItem, ITreeItem<ITreeGridItem>
	{
	}

	public interface ITreeGridItem<T> : ITreeGridItem, ITreeGridStore<T>
		where T: ITreeGridItem
	{
	}
	
	public class TreeGridItemCollection : DataStoreCollection<ITreeGridItem>, ITreeGridStore<ITreeGridItem>
	{
		public TreeGridItemCollection ()
		{
		}

		public TreeGridItemCollection (IEnumerable<ITreeGridItem> items)
			: base(items)
		{
		}
	}
	
#if XAML
	[ContentProperty("Children")]
#endif
	public class TreeGridItem : GridItem, ITreeGridItem, ITreeGridStore<ITreeGridItem>
	{
		TreeGridItemCollection children;

		public TreeGridItemCollection Children
		{
			get { 
				if (children != null)
					return children;
				children = new TreeGridItemCollection ();
				children.CollectionChanged += (sender, e) => {
					if (e.Action == NotifyCollectionChangedAction.Add) {
						foreach (ITreeGridItem item in e.NewItems) {
							item.Parent = this;
						}
					}
				};
				return children; 
			}
		}
		
		public ITreeGridItem Parent { get; set; }
		
		public virtual bool Expandable { get { return this.Count > 0; } }
		
		public virtual bool Expanded { get; set; }
		
		public virtual ITreeGridItem this [int index] {
			get { return children [index]; }
		}

		public virtual int Count {
			get { return (children != null) ? children.Count : 0; }
		}
		
		public TreeGridItem ()
		{
		}
		
		public TreeGridItem (params object[] values)
			: base (values)
		{
		}

		public TreeGridItem (IEnumerable<ITreeGridItem> children, params object[] values)
			: base (values)
		{
			this.Children.AddRange (children);
		}
	}
}

