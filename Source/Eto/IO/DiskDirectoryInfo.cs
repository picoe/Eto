using System.IO;
using System.Collections.Generic;

namespace Eto.IO
{
	public class DiskDirectoryInfo : EtoDirectoryInfo
	{
		readonly DirectoryInfo info;
		EtoDirectoryInfo parent;
		
		public DiskDirectoryInfo(DirectoryInfo directoryInfo)
		{
			this.info = directoryInfo;
		}

		public DiskDirectoryInfo(string path)
		{
			info = new DirectoryInfo(path);
		}

		public override EtoDirectoryInfo Parent
		{
			get
			{
				if (parent == null) 
				{
					if (info.Parent != null)
						parent = new DiskDirectoryInfo(info.Parent);
					else
						parent = new EtoDriveInfo();
				}
				return parent;
			}
		}


		public override string FullName 
		{
			get { return info.FullName; }
		}
		
		public override string Name {
			get {
				return info.Name;
			}
		}

		public override EtoDirectoryInfo GetSubDirectory(string subDirectory)
		{
			string newDir = Path.Combine(info.FullName, subDirectory);
			if (File.Exists(newDir))
			{
				// 'tis an archive directory, most likely!
				return EtoDirectoryInfo.CreateVirtualDirectory(newDir);
			}
			return Directory.Exists(newDir) ? new DiskDirectoryInfo(newDir) : null;
		}

		protected override IEnumerable<EtoDirectoryInfo> GetPathDirectories()
		{
			var diskDirs = info.EnumerateDirectories();
			foreach (var diskDir in diskDirs)
			{
				if (
					((diskDir.Attributes & FileAttributes.Hidden) == 0)
					&& ((diskDir.Attributes & FileAttributes.System) == 0)
					&& (!diskDir.Name.StartsWith(".", System.StringComparison.Ordinal))
					)
				{
					yield return new DiskDirectoryInfo(diskDir);
				}
			}
		}

		public override IEnumerable<EtoFileInfo> GetFiles()
		{
			var diskFiles = info.EnumerateFiles();
			foreach (var file in diskFiles)
			{
				yield return new DiskFileInfo(file);
			}
		}

		public override IEnumerable<EtoFileInfo> GetFiles(IEnumerable<string> patterns)
		{
			foreach (string pattern in patterns)
			{
				var diskFiles = info.GetFiles (pattern); //.EnumerateFiles(pattern); // enumerate doesn't show readonly!!
				foreach (var diskFile in diskFiles)
				{
					yield return new DiskFileInfo(diskFile);
				}
			}
		}


	}
}
