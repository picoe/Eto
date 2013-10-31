#if DESKTOP

namespace Eto.Forms
{
	public partial interface ITreeView
	{
		ContextMenu ContextMenu { get; set; }
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