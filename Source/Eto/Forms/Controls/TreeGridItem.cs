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
	
	public interface ITreeGridItem : ITreeItem<ITreeGridItem>
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
	
	[ContentProperty("Children")]
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
		
		public ITreeGridItem Parent { get; set; }
		
		public virtual bool Expandable { get { return Count > 0; } }
		
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

