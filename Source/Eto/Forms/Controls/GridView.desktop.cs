#if DESKTOP

namespace Eto.Forms
{
	public partial interface IGridView : IHasContextMenu
	{
	}
	
	public partial class GridView
	{
		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

	}
}
#endif