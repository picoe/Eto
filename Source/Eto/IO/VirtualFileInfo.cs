#if !PCL
using System;
using System.IO;

namespace Eto.IO
{
	public class VirtualFileInfo : EtoFileInfo
	{
		readonly string file;
		readonly VirtualDirectoryInfo parent;

		public VirtualFileInfo(VirtualDirectoryInfo parent, string file)
		{
			this.parent = parent;
			this.file = file;
		}
		
		public override bool ReadOnly {
			get {
				return true;
			}
		}

		public override string FullName 
		{
			get { return Path.Combine(parent.FullName, file); }
		}

		public override Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			return parent.OpenRead(file);
		}
		
		public override void Delete ()
		{
			throw new NotSupportedException();
		}
		
		public override EtoDirectoryInfo Directory {
			get {
				return parent;
			}
		}
	}
}
#endif