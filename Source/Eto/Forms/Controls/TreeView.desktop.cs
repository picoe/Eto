namespace Eto.Forms
{
	public partial interface ITreeView : IContextMenuHost
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
