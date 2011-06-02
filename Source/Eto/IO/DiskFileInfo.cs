using System;
using System.IO;

namespace Eto.IO
{
	public class DiskFileInfo : EtoFileInfo
	{
		FileInfo info;

		public DiskFileInfo(FileInfo fileInfo)
		{
			this.info = fileInfo;
		}

		public DiskFileInfo(string fileName)
		{
			info = new FileInfo(fileName);
		}

		public override string FullName 
		{
			get { return info.FullName; }
		}

		public override Stream Open(FileMode fileMode)
		{
			return info.Open(fileMode);
		}
		public override Stream Open(FileMode fileMode, FileAccess fileAccess)
		{
			return info.Open(fileMode, fileAccess);
		}
		public override Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			return info.Open(fileMode, fileAccess, fileShare);
		}
		public override Stream OpenRead()
		{
			return info.OpenRead();
		}
		
		public override EtoDirectoryInfo Directory 
		{
			get 
			{
				return new DiskDirectoryInfo(info.Directory);
			}
		}

	
	}
}
