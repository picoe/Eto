using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Test.UnitTests.Handlers
{
	public abstract class TestContainerHandler : Controls.TestControlHandler, Eto.Forms.Container.IHandler
	{
		public virtual Size ClientSize
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool RecurseToChildren
		{
			get { return true; }
		}
	}
	
}
