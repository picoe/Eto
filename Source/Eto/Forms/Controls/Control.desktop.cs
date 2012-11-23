#if !MOBILE
using System;

namespace Eto.Forms
{
	public partial interface IControl
	{
		string ToolTip { get; set; }

		Cursor Cursor { get; set; }
	}
	
	public partial class Control
	{
		public virtual Cursor Cursor {
			get { return Handler.Cursor; }
			set { Handler.Cursor = value; }
		}
		
		public virtual string ToolTip {
			get { return Handler.ToolTip; }
			set { Handler.ToolTip = value; }
		}
		
	}
}

#endif
