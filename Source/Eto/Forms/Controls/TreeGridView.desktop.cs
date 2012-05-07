using System;

namespace Eto.Forms
{
	public partial interface ITreeGridView
	{
		ContextMenu ContextMenu { get; set; }
	}
	
	public partial class TreeGridView
	{
		public ContextMenu ContextMenu {
			get { return inner.ContextMenu; }
			set { inner.ContextMenu = value; }
		}
	}
}

