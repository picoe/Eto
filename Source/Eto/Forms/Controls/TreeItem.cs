using System.Collections.Generic;

#if XAML
using System.Windows.Markup;

#endif
using System.Collections.Specialized;

namespace Eto.Forms
{
	public interface ITreeItem : IImageListItem, ITreeStore, ITreeItem<ITreeItem>
	{
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
			get
			{ 
				if (children != null)
					return children;
				children = new TreeItemCollection();
				children.CollectionChanged += (sender, e) => {
					switch (e.Action)
					{
						case NotifyCollectionChangedAction.Reset:
							foreach (var item in children)
							{
								item.Parent = this;
							}
							break;
						case NotifyCollectionChangedAction.Add:
						case NotifyCollectionChangedAction.Replace:
							foreach (ITreeItem item in e.NewItems)
							{
								item.Parent = this;
							}
							break;
					}
				};
				return children; 
			}
		}

		public ITreeItem Parent { get; set; }

		public virtual bool Expandable { get { return Count > 0; } }

		public virtual bool Expanded { get; set; }

		public virtual ITreeItem this [int index]
		{
			get { return children[index]; }
		}

		public virtual int Count
		{
			get { return (children != null) ? children.Count : 0; }
		}

		public TreeItem()
		{
		}

		public TreeItem(IEnumerable<ITreeItem> children)
		{
			this.Children.AddRange(children);
		}
	}
}

