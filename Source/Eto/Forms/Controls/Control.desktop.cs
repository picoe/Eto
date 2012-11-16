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
			get { return inner.Cursor; }
			set { inner.Cursor = value; }
		}
		
		public virtual string ToolTip {
			get { return inner.ToolTip; }
			set { inner.ToolTip = value; }
		}
		
	}
}

#endif