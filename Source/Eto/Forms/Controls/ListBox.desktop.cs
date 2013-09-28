#if DESKTOP
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
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}
	}
}
#endif