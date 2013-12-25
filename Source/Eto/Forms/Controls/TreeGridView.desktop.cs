#if DESKTOP

namespace Eto.Forms
{
	public partial interface ITreeGridView : IHasContextMenu
	{
	}
	
	public partial class TreeGridView
	{
		public ContextMenu ContextMenu {
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}
	}
}
#endif