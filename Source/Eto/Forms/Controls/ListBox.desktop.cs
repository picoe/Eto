using System;

namespace Eto.Forms
{
	public partial interface IListBox : IListControl
	{
		ContextMenu ContextMenu { get; set; }
	}

	public partial class ListBox
	{
		public ContextMenu ContextMenu {
			get { return inner.ContextMenu; }
			set { inner.ContextMenu = value; }
		}
	}
}

