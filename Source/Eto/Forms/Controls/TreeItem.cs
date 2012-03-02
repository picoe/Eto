using System;
using System.Collections.Generic;
using Eto.Collections;
using System.Windows.Markup;
using System.Collections.Specialized;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ITreeItem : IGridItem
	{
		bool Expanded { get; set; }
		
		bool Expandable { get; }
		
		ITreeItem Parent { get; set; }
	}

	public interface ITreeItem<T> : ITreeItem, ITreeStore<T>
		where T: ITreeItem
	{
	}
	

	public class TreeItemCollection : DataStoreCollection<ITreeItem>, ITreeStore<ITreeItem>
	{
		public TreeItemCollection ()
		{
		}

		public TreeItemCollection (IEnumerable<ITreeItem> items)
			: base(items)
		{
		}
	}
	
	[ContentProperty("Children")]
	public class TreeItem : GridItem, ITreeItem, ITreeStore<ITreeItem>
	{
		TreeItemCollection children;

		public TreeItemCollection Children {
			get { 
				if (children != null)
					return children;
				children = new TreeItemCollection ();
				children.CollectionChanged += (sender, e) => {
					if (e.Action == NotifyCollectionChangedAction.Add) {
						foreach (ITreeItem item in e.NewItems) {
							item.Parent = this;
						}
					}
				};
				return children; 
			}
		}
		
		public ITreeItem Parent { get; set; }
		
		public virtual bool Expandable { get { return this.Count > 0; } }
		
		public virtual bool Expanded { get; set; }
		
		public virtual ITreeItem this [int index] {
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
		
		public TreeItem (IEnumerable<ITreeItem> children, params object[] values)
			: base (values)
		{
			this.Children.AddRange (children);
		}
	}
}

