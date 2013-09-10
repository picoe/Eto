#if !IOS
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Collections.Generic;
//using NUnrar;
using SharpCompress.Archive.Rar;
using SharpCompress.Reader;

namespace Eto.IO
{
	public class RarDirectoryInfo : VirtualDirectoryInfo
	{
		public RarDirectoryInfo(EtoFileInfo fileInfo)
			: base(fileInfo)
		{
		}

		private RarDirectoryInfo(RarDirectoryInfo parent, string path)
			: base(parent, path)
		{
		}

		public RarDirectoryInfo(Stream stream)
			: base(stream)
		{
		}
		
		protected override VirtualDirectoryInfo CreateDirectory (VirtualDirectoryInfo parent, string path)
		{
			return new RarDirectoryInfo((RarDirectoryInfo)parent, path);
		}
		
		protected override IEnumerable<VirtualFileEntry> ReadEntries (Stream stream)
		{
			var foundDirs = new Dictionary<string, bool>();
			var archive = RarArchive.Open(stream);
			foreach (var entry in archive.Entries)
			{
				var file = new VirtualFileEntry(entry.FilePath, entry.IsDirectory);
				
				if (!string.IsNullOrEmpty(file.Path) && !foundDirs.ContainsKey(file.Path))
				{
					foundDirs.Add(file.Path, true);
					yield return new VirtualFileEntry(file.Path, true);
				}
				
				yield return file;
			}
		}
		
		public override Stream OpenRead (string fileName)
		{
			MemoryStream ms = null;
			var archive = RarArchive.Open(FileInfo.OpenRead());
			foreach (var entry in archive.Entries)
			{
				if (!entry.IsDirectory && string.Equals(entry.FilePath, fileName, StringComparison.InvariantCultureIgnoreCase))
				{
					ms = new MemoryStream((int)entry.Size);
					entry.WriteTo(ms);
					ms.Seek(0, SeekOrigin.Begin);
					break;
				}
			}
			return ms;
		}
	}
}
#endif