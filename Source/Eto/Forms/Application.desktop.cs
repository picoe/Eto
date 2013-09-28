#if DESKTOP
using System;

namespace Eto.Forms
{
	public partial interface IApplication
	{
		void Restart ();
		void RunIteration ();
	}
	
	public partial class Application
	{
		public void RunIteration ()
		{
			Handler.RunIteration ();
		}

		public void Restart ()
		{
			Handler.Restart ();
		}
	}
}
#endif