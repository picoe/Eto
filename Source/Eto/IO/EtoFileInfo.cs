using System;
using System.IO;
using Eto.Drawing;

namespace Eto.IO
{
	/// <summary>
	/// Summary description for FileInfo.
	/// </summary>
	public abstract class EtoFileInfo : EtoSystemObjectInfo
	{
		public EtoFileInfo()
		{
		}

		public static EtoFileInfo GetFile(string fileName)
		{
			return null;
		}

		public override string Name
		{
			get { return Path.GetFileName(FullName); }
		}

		public string Extension
		{
			get { return Path.GetExtension(FullName); }
		}

		public override Icon GetIcon(SystemIcons icons, IconSize iconSize)
		{
			return icons.GetFileIcon(Name, iconSize);
		}

		public virtual Stream Open(FileMode fileMode)
		{
			return Open(fileMode, FileAccess.ReadWrite, FileShare.Read);
		}

		public virtual Stream Open(FileMode fileMode, FileAccess fileAccess)
		{
			return Open(fileMode, fileAccess, FileShare.Read);
		}

		public abstract Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);

		public virtual Stream OpenRead()
		{
			return Open(FileMode.Open);
		}
		
		public abstract EtoDirectoryInfo Directory
		{
			get;
		}

		//public abstract bool Exists { get; }

		//public abstract void Delete();

		//public abstract string Name { get; set; }

	}
}
