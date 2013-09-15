#if DESKTOP
using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms
{
	public partial interface IDockContainer
	{
		Size MinimumSize { get; set; }
	}
	
	public partial class DockContainer
	{
		public Size MinimumSize {
			get { return Handler.MinimumSize; }
			set { Handler.MinimumSize = value; }
		}
	}
}
#endif