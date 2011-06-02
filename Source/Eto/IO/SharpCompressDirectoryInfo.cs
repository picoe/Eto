using System;
using SharpCompress.Reader;
using SharpCompress.Archive;
using System.IO;
using System.Collections.Generic;

namespace Eto.IO
{
	public class SharpCompressDirectoryInfo : VirtualDirectoryInfo
	{
		public SharpCompressDirectoryInfo(EtoFileInfo fileInfo)
			: base(fileInfo)
		{
		}

		private SharpCompressDirectoryInfo(SharpCompressDirectoryInfo parent, string path)
			: base(parent, path)
		{
		}

		public SharpCompressDirectoryInfo(Stream stream)
			: base(stream)
		{
		}
		
		protected override VirtualDirectoryInfo CreateDirectory (VirtualDirectoryInfo parent, string path)
		{
			return new SharpCompressDirectoryInfo((SharpCompressDirectoryInfo)parent, path);
		}
		
		protected override IEnumerable<VirtualFileEntry> ReadEntries (Stream stream)
		{
			/* reader
			var reader = ReaderFactory.OpenReader(stream, SharpCompress.Common.Options.GiveDirectoryEntries);
			
			while (reader.MoveToNextEntry())
			{
				var entry = reader.Entry;
				yield return new VirtualFileEntry(entry.FilePath.TrimEnd(Path.DirectorySeparatorChar), entry.IsDirectory);
			}
			*/
			using (var archive = ArchiveFactory.Open (stream, SharpCompress.Common.Options.GiveDirectoryEntries)) {
				foreach (var entry in archive.Entries)
				{
					yield return new VirtualFileEntry(entry.FilePath.TrimEnd(Path.DirectorySeparatorChar), entry.IsDirectory);
				}
			}
			
		}
		
		public override Stream OpenRead (string fileName)
		{
			MemoryStream ms = null;
			/* use reader
			using (var stream = FileInfo.OpenRead ())
			using (var reader = ReaderFactory.OpenReader(stream, SharpCompress.Common.Options.GiveDirectoryEntries)) {
				
				while (reader.MoveToNextEntry()) {
					if (!entry.IsDirectory && string.Equals(entry.FilePath, fileName, StringComparison.InvariantCultureIgnoreCase))
					{
						var entry = reader.Entry;
						ms = new MemoryStream((int)entry.Size);
						entry.WriteTo (ms);
						ms.Seek(0, SeekOrigin.Begin);
					}
				}
			}
			*/
			
			// use direct access..
			using (var stream = FileInfo.OpenRead ())
			using (var archive = ArchiveFactory.Open (stream, SharpCompress.Common.Options.None)) {
				foreach (var entry in archive.Entries)
				{
					if (!entry.IsDirectory && string.Equals(entry.FilePath, fileName, StringComparison.InvariantCultureIgnoreCase))
					{
						ms = new MemoryStream((int)entry.Size);
						entry.WriteTo (ms);
						ms.Seek(0, SeekOrigin.Begin);
						break;
					}
				}
			}
			return ms;
		}
	}
}
