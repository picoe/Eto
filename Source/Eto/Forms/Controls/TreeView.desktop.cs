#if DESKTOP
using System;

namespace Eto.Forms
{
	public partial interface ITreeView
	{
		ContextMenu ContextMenu { get; set; }
	}
	
	public partial class TreeView
	{
		public ContextMenu ContextMenu {
			get { return handler.ContextMenu; }
			set { handler.ContextMenu = value; }
		}
	}
}
#endif