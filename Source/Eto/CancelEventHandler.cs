using System;

namespace Eto
{
#if IOS
	public class CancelEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public CancelEventArgs ()
		{
		}
		
		public CancelEventArgs (bool cancel)
		{
			this.Cancel = cancel;
		}
	}	
	
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);
	
#endif
}

