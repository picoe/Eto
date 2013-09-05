using System;
using System.Collections.Generic;
#if XAML
using System.Windows.Markup;
#endif
using System.Collections.Specialized;

namespace Eto.Forms
{
	public interface ITreeItem : IImageListItem, ITreeStore, ITreeItem<ITreeItem>
	{
        /// <summary>
        /// Used only by the back-ends, maps to a TreeNode
        /// or its equivalent
        /// </summary>
        object Handler { get; set; }

        object Tag { get; set; }

        object InternalTag { get; set; }

        ITreeItem Clone();
	}

	public class TreeItemCollection : DataStoreCollection<ITreeItem>, ITreeStore
	{
	}
	
	[ContentProperty("Children")]
	public class TreeItem : ImageListItem, ITreeItem, INotifyCollectionChanged
	{
		TreeItemCollection children;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { Children.CollectionChanged += value; }
			remove { Children.CollectionChanged -= value; }
		}

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

        /// <summary>
        /// Used internally to reference the UI item
        /// </summary>
        public object InternalTag { get; set; }

        public ITreeItem Clone()
        {
            throw new NotImplementedException();
        }

        public object Handler { get; set; }
    }
}

