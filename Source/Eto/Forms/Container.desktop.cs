using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;

namespace Eto.Forms
{
	public partial interface IContainer : IControl
	{
		Size? MinimumSize { get; set; }
	}
	
	public partial class Container : Control
	{
		public Size? MinimumSize {
			get { return handler.MinimumSize; }
			set { handler.MinimumSize = value; }
		}
	}
}
