using System;
using System.Collections.Generic;
#if DESKTOP
using System.Windows.Markup;
#endif
using System.Collections.Specialized;

namespace Eto.Forms
{
	public interface ITreeItem : IImageListItem, ITreeStore, ITreeItem<ITreeItem>
	{
	}

	public class TreeItemCollection : DataStoreCollection<ITreeItem>
	{
	}
	
#if DESKTOP
	[ContentProperty("Children")]
#endif
	public class TreeItem : ImageListItem, ITreeItem
	{
		TreeItemCollection children;

		public TreeItemCollection Children
		{
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
		
		public virtual ITreeItem this[int index]
		{
			get { return children [index]; }
		}

		public virtual int Count {
			get { return (children != null) ? children.Count : 0; }
		}
		
		public TreeItem ()
		{
		}
		
		public TreeItem (IEnumerable<ITreeItem> children)
		{
			this.Children.AddRange (children);
		}
	}
}

