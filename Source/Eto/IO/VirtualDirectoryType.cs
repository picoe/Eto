using System;
using System.IO;
using System.Globalization;

namespace Eto.IO
{
	public class VirtualDirectoryType
	{
		public Type Type { get; private set; }
		public string Extension { get; set; }
		
		public string FileMask
		{
			get { return string.Format(CultureInfo.InvariantCulture, "*{0}", Extension); }
		}
		
		public virtual VirtualDirectoryInfo Create(EtoFileInfo fileInfo)
		{
			return (VirtualDirectoryInfo)Activator.CreateInstance(Type, fileInfo);
		}

		public virtual VirtualDirectoryInfo Create(Stream stream)
		{
			return (VirtualDirectoryInfo)Activator.CreateInstance(Type, stream);
		}
		
		public VirtualDirectoryType(Type type, string extension)
		{
			this.Type = type;
			this.Extension = extension;
		}
	}
}

