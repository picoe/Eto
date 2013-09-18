#if DESKTOP
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;

namespace Eto.Forms
{
	public abstract partial class BaseAction
	{
		public abstract MenuItem GenerateMenuItem(ActionItem actionItem, Generator generator);
	}
}
#endif