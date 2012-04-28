using System;
using System.Collections.Generic;
using Eto.Collections;
#if DESKTOP
using System.Windows.Markup;
#endif
using System.Collections.Specialized;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ITreeGridItem : IGridItem
	{
		bool Expanded { get; set; }
		
		bool Expandable { get; }
		
		ITreeGridItem Parent { get; set; }
	}

	public interface ITreeItem<T> : ITreeGridItem, ITreeGridStore<T>
		where T: ITreeGridItem
	{
	}
	
	public class TreeItemCollection : DataStoreCollection<ITreeGridItem>, ITreeGridStore<ITreeGridItem>
	{
		public TreeItemCollection ()
		{
		}

		public TreeItemCollection (IEnumerable<ITreeGridItem> items)
			: base(items)
		{
		}
	}
	
#if DESKTOP
	[ContentProperty("Children")]
#endif
	public class TreeItem : GridItem, ITreeGridItem, ITreeGridStore<ITreeGridItem>
	{
		TreeItemCollection children;

		public TreeItemCollection Children {
			get { 
				if (children != null)
					return children;
				children = new TreeItemCollection ();
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
		
		public TreeItem ()
		{
		}
		
		public TreeItem (params object[] values)
			: base (values)
		{
		}
		
		public TreeItem (IEnumerable<ITreeGridItem> children, params object[] values)
			: base (values)
		{
			this.Children.AddRange (children);
		}
	}
}

