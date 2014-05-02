using System.Collections.Generic;

#if XAML
using System.Windows.Markup;

#endif
using System.Collections.Specialized;

namespace Eto.Forms
{
	/// <summary>
	/// Interface for an item in a <see cref="TreeView"/>
	/// </summary>
	/// <remarks>
	/// This can be used instead of <see cref="TreeItem"/> when you want to use your own class as the item object
	/// of a tree.
	/// </remarks>
	public interface ITreeItem : IImageListItem, ITreeStore, ITreeItem<ITreeItem>
	{
	}

	/// <summary>
	/// Tree item collection.
	/// </summary>
	public class TreeItemCollection : DataStoreCollection<ITreeItem>, ITreeStore
	{
	}

	/// <summary>
	/// Item for a <see cref="TreeView"/>
	/// </summary>
	[ContentProperty("Children")]
	public class TreeItem : ImageListItem, ITreeItem, INotifyCollectionChanged
	{
		TreeItemCollection children;

		/// <summary>
		/// Occurs when the <see cref="Children"/> collection is changed.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add { Children.CollectionChanged += value; }
			remove { Children.CollectionChanged -= value; }
		}

		/// <summary>
		/// Gets the children collection
		/// </summary>
		/// <value>The children collection.</value>
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

		/// <summary>
		/// Gets or sets the parent tree item
		/// </summary>
		/// <value>The parent.</value>
		public ITreeItem Parent { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.TreeItem"/> is expandable.
		/// </summary>
		/// <value><c>true</c> if expandable; otherwise, <c>false</c>.</value>
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

