using System;
using System.Collections.Generic;
using Eto.Collections;

namespace Eto.Forms
{
	public interface ITreeItem : IImageListItem
	{
		int Count { get; }
		
		bool Expanded { get; set; }
		
		bool Expandable { get; }
		
		ITreeItem Parent { get; }
		
		ITreeItem GetChild (int index);
	}
	
	public class TreeItem : ImageListItem, ITreeItem
	{
		BaseList<ITreeItem> children;
		
		public IList<ITreeItem> Children {
			get { 
				if (children != null)
					return children;
				children = new BaseList<ITreeItem> ();
				children.Added += (sender, e) => {
					var item = e.Item as TreeItem;
					if (item != null) item.Parent = this;
				};
				return children; 
			}
		}
		
		public ITreeItem Parent {
			get; private set;
		}
		
		public virtual bool Expandable {
			get {
				return this.Count > 0;
			}
		}
		
		public virtual bool Expanded {
			get; set;
		}
		
		public virtual ITreeItem GetChild (int index)
		{
			return children [index];
		}

		public virtual int Count {
			get { return (children != null) ? children.Count : 0; }
		}
	}
}

