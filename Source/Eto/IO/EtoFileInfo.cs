using System;
using System.IO;
using Eto.Drawing;

namespace Eto.IO
{
	public abstract class EtoFileInfo : EtoSystemObjectInfo, IComparable<EtoFileInfo>
	{
		public EtoFileInfo()
		{
		}

		public static EtoFileInfo GetFile(string fileName)
		{
			return null;
		}
		
		public abstract bool ReadOnly
		{
			get;
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

		public abstract void Delete();

		//public abstract string Name { get; set; }

		public override bool Equals (object obj)
		{
			var dir = obj as EtoFileInfo;
			if (dir == null) return false;
			return this.FullName.Equals (dir.FullName, StringComparison.InvariantCultureIgnoreCase);
		}
		
		public override int GetHashCode ()
		{
			return this.FullName.GetHashCode ();
		}
		
		public static bool operator == (EtoFileInfo file1, EtoFileInfo file2)
		{
			if (ReferenceEquals (file1, null)) return ReferenceEquals (file2, null);
			return file1.Equals (file2);
		}

		public static bool operator != (EtoFileInfo file1, EtoFileInfo file2)
		{
			return !(file1 == file2);
		}
		
		#region IComparable[EtoFileInfo] implementation
		public virtual int CompareTo (EtoFileInfo other)
		{
			return this.FullName.CompareTo(other.FullName);
		}
		#endregion
	}
}
