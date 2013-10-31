#if DESKTOP

namespace Eto.Forms
{
	public partial interface IGridView
	{
		ContextMenu ContextMenu { get; set; }
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