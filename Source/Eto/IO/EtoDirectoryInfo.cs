using System;
using System.IO;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eto.IO
{
	public abstract class EtoDirectoryInfo : EtoSystemObjectInfo
	{
		// TODO: should be a concurrent dictionary, but not supported on MonoTouch..
		static Dictionary<string, VirtualDirectoryType> virtualDirectoryTypes = new Dictionary<string, VirtualDirectoryType>(StringComparer.InvariantCultureIgnoreCase);

		public static void AddVirtualDirectoryType(VirtualDirectoryType type)
		{
			virtualDirectoryTypes.Add(type.Extension, type);
		}
		
		public static void AddVirtualDirectoryType<T>(string extension)
		{
			AddVirtualDirectoryType(new VirtualDirectoryType(typeof(T), extension));
		}

		public static VirtualDirectoryType FindVirtualDirectoryType(string extension)
		{
			VirtualDirectoryType type;
			if (virtualDirectoryTypes.TryGetValue(extension, out type))
			{
				return type;
			}
			return null;
		}
		
		public static EtoDirectoryInfo CreateVirtualDirectory(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			var type = FindVirtualDirectoryType(extension);
			if (type != null) return type.Create(new DiskFileInfo(fileName));
			else return null;
		}

		public static EtoDirectoryInfo CreateVirtualDirectory(string fileName, Stream stream)
		{
			string extension = Path.GetExtension(fileName);
			var type = FindVirtualDirectoryType(extension);
			if (type != null) return type.Create(stream);
			else return null;
		}
		
		public static IEnumerable<VirtualDirectoryType> VirtualDirectoryTypes
		{
			get { return virtualDirectoryTypes.Values; }
		}
		
		static EtoDirectoryInfo()
		{
/*#if !MOBILE
			AddVirtualDirectoryType<RarDirectoryInfo>(".rar");
			AddVirtualDirectoryType<ZipDirectoryInfo>(".zip");
#endif*/
			AddVirtualDirectoryType<SharpCompressDirectoryInfo>(".zip");
			AddVirtualDirectoryType<SharpCompressDirectoryInfo>(".rar");
			/* doesn't actually work...
			AddVirtualDirectoryType<SharpCompressDirectoryInfo>(".gz");
			AddVirtualDirectoryType<SharpCompressDirectoryInfo>(".tar");
			AddVirtualDirectoryType<SharpCompressDirectoryInfo>(".bz2");
			*/
		}
		
		public EtoDirectoryInfo()
		{
		}

		public static EtoDirectoryInfo GetDirectory(string path)
		{
			string pathRoot = string.Empty;
			if (!Path.IsPathRooted(path)) path = Path.GetFullPath(path);
			pathRoot = Path.GetPathRoot(path);
			path = path.Substring(pathRoot.Length);
			string[] paths = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			EtoDirectoryInfo di = new DiskDirectoryInfo(pathRoot);
			foreach (string dir in paths)
			{
				di = di.GetSubDirectory(dir);
				if (di == null) break;
			}
			return di;
		}
		
		public virtual EtoDirectoryInfo GetSubDirectory(string subDirectory)
		{
			return GetDirectories().FirstOrDefault(r => r.Name == subDirectory);
		}
		
		public IEnumerable<EtoDirectoryInfo> GetDirectories()
		{
			return GetPathDirectories().Union(GetVirtualDirectories());
		}
		
		public virtual IEnumerable<EtoDirectoryInfo> GetVirtualDirectories()
		{
			foreach (var type in EtoDirectoryInfo.VirtualDirectoryTypes)
			{
				foreach (var fileInfo in this.GetFiles(type.FileMask))
				{
					yield return type.Create(fileInfo);
				}
			}
		}

		protected abstract IEnumerable<EtoDirectoryInfo> GetPathDirectories();

		public abstract IEnumerable<EtoFileInfo> GetFiles();
		
		public virtual IEnumerable<EtoFileInfo> GetFiles(string searchPattern)
		{
			// convert search pattern to regular expression!
			string filter = searchPattern;
			filter = filter.Replace(".", "\\.");
			filter = filter.Replace("*", ".+");
			filter = filter.Replace(";", "|");

			Regex reg = new Regex(filter, RegexOptions.IgnoreCase
#if !MOBILE
				| RegexOptions.Compiled
#endif
				);

			foreach (var file in GetFiles())
			{
				if (reg.IsMatch(file.Name))
				{
					yield return file;
				}
			}
		}

		public abstract EtoDirectoryInfo Parent { get; }

		public virtual Icon GetOpenIcon(SystemIcons icons, IconSize iconSize)
		{
			return icons.GetStaticIcon(StaticIconType.OpenDirectory, iconSize);
		}

		public override Icon GetIcon(SystemIcons icons, IconSize iconSize)
		{
			return icons.GetStaticIcon(StaticIconType.CloseDirectory, iconSize);
		}
	}
}
