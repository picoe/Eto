#if DESKTOP
using System;

namespace Eto.Forms
{
	public partial interface ITreeGridView
	{
		ContextMenu ContextMenu { get; set; }
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