
namespace Eto.Forms
{
	public partial interface ITreeGridView : IContextMenuHost
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