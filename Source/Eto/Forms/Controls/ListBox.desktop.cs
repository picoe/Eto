#if DESKTOP

namespace Eto.Forms
{
	public partial interface IListBox : IHasContextMenu
	{
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