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
			inner.RunIteration ();
		}

		public void Restart ()
		{
			inner.Restart ();
		}
	}
}

