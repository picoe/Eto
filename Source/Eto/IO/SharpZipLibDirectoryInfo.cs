#if !MOBILE && !PCL
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace Eto.IO
{
	public class ZipDirectoryInfo : VirtualDirectoryInfo
	{
		public ZipDirectoryInfo(EtoFileInfo fileInfo)
			: base(fileInfo)
		{
		}

		private ZipDirectoryInfo(ZipDirectoryInfo parent, string path)
			: base(parent, path)
		{
		}

		public ZipDirectoryInfo(Stream stream)
			: base(stream)
		{
		}
		
		protected override VirtualDirectoryInfo CreateDirectory (VirtualDirectoryInfo parent, string path)
		{
			return new ZipDirectoryInfo((ZipDirectoryInfo)parent, path);
		}
		
		protected override IEnumerable<VirtualFileEntry> ReadEntries (Stream stream)
		{
			using (ZipInputStream zis = new ZipInputStream(stream)) {
				ZipEntry entry;
				while ((entry = zis.GetNextEntry()) != null)
				{
					yield return new VirtualFileEntry(entry.Name.TrimEnd(Path.DirectorySeparatorChar), entry.IsDirectory);
				}
			}
		}

		public override Stream OpenRead(string fileName)
		{
			MemoryStream ms = null;
			using (Stream stream = FileInfo.OpenRead())
			using (ZipInputStream zis = new ZipInputStream(stream))
			{
				ZipEntry entry;
				while ((entry = zis.GetNextEntry()) != null)
				{
					if (entry.IsFile && string.Equals(entry.Name, fileName, StringComparison.InvariantCultureIgnoreCase))
					{
						ms = new MemoryStream((int)entry.Size);
						zis.CopyTo(ms, (int)entry.Size);
						ms.Seek(0, SeekOrigin.Begin);
						break;
					}
				}
			}
			return ms;
		}

	}
}
#endif