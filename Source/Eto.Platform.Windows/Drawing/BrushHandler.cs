using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sd = System.Drawing;

namespace Eto.Platform.Windows.Drawing
{
	/// <summary>
	/// Base handler for <see cref="IBrush"/>, to get the platform-specific brush
	/// </summary>
	/// <copyright>(c) 2012 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class BrushHandler
	{
		public abstract sd.Brush GetBrush (Brush brush);
	}
}
