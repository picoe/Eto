using System;
using System.Collections.Generic;
using System.Linq;

namespace Eto.Forms
{
	[Obsolete("Use ISubmenu instead")]
	public interface ISubMenuWidget
	{
		MenuItemCollection Items { get; }
	}

	#pragma warning disable 612,618

	public interface ISubmenu : ISubMenuWidget
	{
	}

	#pragma warning restore 612,618
}

