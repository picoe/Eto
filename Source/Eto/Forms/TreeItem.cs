using System;
using System.Collections.Generic;
using Eto.Collections;
using System.Windows.Markup;

namespace Eto.Forms
{
	public interface ITreeItem : IImageListItem, ITreeStore
	{
		bool Expanded { get; set; }
		
		bool Expandable { get; }
		
		ITreeItem Parent { get; set; }
	}
	
	[ContentProperty("Children")]
	public class TreeItem : ImageListItem, ITreeItem
	{
		BaseList<ITreeItem> children;
		
		public BaseList<ITreeItem> Children {
			get { 
				if (children != null)
					return children;
				children = new BaseList<ITreeItem> ();
				children.Added += (sender, e) => {
					e.Item.Parent = this;
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

