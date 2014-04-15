#if !PCL
using System.Globalization;

namespace Eto.IO
{
	public class VirtualFileEntry
	{
		
		public VirtualFileEntry(string name, bool isDirectory)
		{
			this.Name = System.IO.Path.GetFileName(name);
			this.Path = System.IO.Path.GetDirectoryName(name);
			this.IsDirectory = isDirectory;
		}

		public string Name
		{
			get; private set;
		}

		public string Path
		{
			get; private set;
		}
		
		public string FullPath
		{
			get { return System.IO.Path.Combine(Path, Name); }
		}

		public bool IsDirectory
		{
			get; private set;
		}

		public override string ToString ()
		{
			return string.Format (CultureInfo.InvariantCulture, "[VirtualFileEntry: Name={0}, Path={1}, IsDirectory={2}]", Name, Path, IsDirectory);
		}
	}
}
#endif