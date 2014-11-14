using System.Collections.Generic;
using System.Collections.Specialized;

namespace Eto.Forms
{
	/// <summary>
	/// Base tree item interface
	/// </summary>
	public interface ITreeItem<T>
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ITreeItem{T}"/> is expanded.
		/// </summary>
		/// <remarks>
		/// When expanded, the children of the tree item will be shown.
		/// This will be set automatically by the <see cref="TreeGridView"/> when the item is expanded or collapsed
		/// and keep in in sync with the view.
		/// </remarks>
		/// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
		bool Expanded { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ITreeItem{T}"/> is expandable.
		/// </summary>
		/// <remarks>
		/// When <c>true</c>, this will typically show a glyph that can be clicked to expand the item to show its children.
		/// When <c>false</c>, the glyph is not shown and the node is not expandable by the user.
		/// </remarks>
		/// <value><c>true</c> if expandable; otherwise, <c>false</c>.</value>
		bool Expandable { get; }

		/// <summary>
		/// Gets or sets the parent of this item.
		/// </summary>
		/// <value>The parent of this item.</value>
		T Parent { get; set; }
	}

	/// <summary>
	/// Interface for an item in a <see cref="TreeGridView"/>.
	/// </summary>
	/// <remarks>
	/// This is the base interface for items in a tree grid.  Use this interface if you wish to
	/// use your own class for items in a tree. Otherwise, you can use the standard <see cref="TreeGridItem"/>.
	/// </remarks>
	public interface ITreeGridItem : ITreeItem<ITreeGridItem>
	{
	}

	/// <summary>
	/// Interface for an item in a <see cref="TreeGridView"/> that implements children
	/// </summary>
	public interface ITreeGridItem<T> : ITreeGridItem, ITreeGridStore<T>
		where T: ITreeGridItem
	{
	}

	/// <summary>
	/// Collection of <see cref="ITreeGridItem"/> objects for child nodes of a tree.
	/// </summary>
	public class TreeGridItemCollection : DataStoreCollection<ITreeGridItem>, ITreeGridStore<ITreeGridItem>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridItemCollection"/> class.
		/// </summary>
		public TreeGridItemCollection()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridItemCollection"/> class with an enumeration of existing values.
		/// </summary>
		/// <param name="items">Items to initialize the collection with.</param>
		public TreeGridItemCollection(IEnumerable<ITreeGridItem> items)
			: base(items)
		{
		}
	}

	/// <summary>
	/// Item for a <see cref="TreeGridView"/> for each node of the tree.
	/// </summary>
	/// <remarks>
	/// This is the standard implementation.  You can implement <see cref="ITreeGridItem"/> with your own
	/// class to use them as nodes in the tree instead.	
	/// </remarks>
	[ContentProperty("Children")]
	public class TreeGridItem : GridItem, ITreeGridItem, ITreeGridStore<ITreeGridItem>
	{
		TreeGridItemCollection children;

		/// <summary>
		/// Gets the collection of children for this tree grid item.
		/// </summary>
		/// <value>The children of this item.</value>
		public TreeGridItemCollection Children
		{
			get
			{ 
				if (children != null)
					return children;
				children = new TreeGridItemCollection();
				children.CollectionChanged += (sender, e) =>
				{
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
							foreach (ITreeGridItem item in e.NewItems)
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
		/// Gets or sets the parent of this item.
		/// </summary>
		/// <value>The parent of this item.</value>
		public ITreeGridItem Parent { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.TreeGridItem"/> is expandable.
		/// </summary>
		/// <value><c>true</c> if expandable; otherwise, <c>false</c>.</value>
		public virtual bool Expandable { get { return Count > 0; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.TreeGridItem"/> is expanded.
		/// </summary>
		/// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
		public virtual bool Expanded { get; set; }

		/// <summary>
		/// Gets the child item at the specified index.
		/// </summary>
		/// <param name="index">Index of the item to get.</param>
		public virtual ITreeGridItem this [int index]
		{
			get { return children[index]; }
		}

		/// <summary>
		/// Gets the number of child items.
		/// </summary>
		/// <value>The count of child items.</value>
		public virtual int Count
		{
			get { return (children != null) ? children.Count : 0; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridItem"/> class.
		/// </summary>
		public TreeGridItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridItem"/> class with the specified <paramref name="values"/>.
		/// </summary>
		/// <param name="values">Values for this node.</param>
		public TreeGridItem(params object[] values)
			: base(values)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.TreeGridItem"/> class with the specified <paramref name="children"/> and <paramref name="values"/>.
		/// </summary>
		/// <param name="children">Children to initialize the item with.</param>
		/// <param name="values">Values for this node in the tree.</param>
		public TreeGridItem(IEnumerable<ITreeGridItem> children, params object[] values)
			: base(values)
		{
			this.Children.AddRange(children);
		}
	}
}

