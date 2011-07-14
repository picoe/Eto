using System;
using SI = System.IO;
using Eto.Drawing;

namespace Eto.IO
{
	public abstract class EtoSystemObjectInfo
	{
		public EtoSystemObjectInfo()
		{
		}

		public abstract string FullName { get; }
		
		public abstract string Name { get; }

		public abstract Icon GetIcon(SystemIcons icons, IconSize iconSize);
	}
}
