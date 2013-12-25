#if DESKTOP

namespace Eto.Forms
{
	public partial interface ITreeView : IHasContextMenu
	{
	}
	
	public partial class TreeView
	{
		public ContextMenu ContextMenu {
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}
	}
}
#endif