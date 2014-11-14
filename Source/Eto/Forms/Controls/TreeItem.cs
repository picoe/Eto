using System.Collections.Generic;
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

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TreeItem"/> is expanded.
		/// </summary>
		/// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
		public virtual bool Expanded { get; set; }

		/// <summary>
		/// Gets the child <see cref="Eto.Forms.ITreeItem"/> at the specified index.
		/// </summary>
		/// <param name="index">Index to get the child</param>
		public virtual ITreeItem this [int index]
		{
			get { return children[index]; }
		}

		/// <summary>
		/// Gets the count of children of this node
		/// </summary>
		/// <value>The child count</value>
		public virtual int Count
		{
			get { return (children != null) ? children.Count : 0; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeItem"/> class.
		/// </summary>
		public TreeItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeItem"/> class with the specified children
		/// </summary>
		/// <param name="children">Children to populate this node with</param>
		public TreeItem(IEnumerable<ITreeItem> children)
		{
			this.Children.AddRange(children);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeItem"/> class with the specified children
		/// </summary>
		/// <param name="children">Children to populate this node with</param>
		public TreeItem(params ITreeItem[] children)
		{
			this.Children.AddRange(children);
		}
	}
}

