using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Eto.Gtk.Build
{
	/// <summary>
	/// Minimal tar (ustar + GNU long names) writer, so packages can be built on any OS without native tools.
	/// </summary>
	class TarWriter
	{
		readonly Stream stream;
		readonly long mtime;
		readonly HashSet<string> directories = new HashSet<string>();

		public TarWriter(Stream stream, long mtime)
		{
			this.stream = stream;
			this.mtime = mtime;
		}

		public void AddDirectory(string path)
		{
			path = path.TrimEnd('/') + "/";
			if (!directories.Add(path))
				return;
			WriteHeader(path, 0x1ED /* 755 */, 0, (byte)'5');
		}

		/// <summary>Adds a file, creating its parent directories first. Paths are relative and use forward slashes.</summary>
		public void AddFile(string path, byte[] data, int mode)
		{
			var parts = path.Split('/');
			for (int i = 1; i < parts.Length; i++)
				AddDirectory(string.Join("/", parts, 0, i));
			WriteHeader(path, mode, data.Length, (byte)'0');
			stream.Write(data, 0, data.Length);
			Pad(data.Length);
		}

		public void Finish()
		{
			// two empty blocks mark the end of the archive
			stream.Write(new byte[1024], 0, 1024);
		}

		void Pad(long length)
		{
			var remainder = (int)(length % 512);
			if (remainder != 0)
				stream.Write(new byte[512 - remainder], 0, 512 - remainder);
		}

		void WriteHeader(string path, int mode, long size, byte type)
		{
			var nameBytes = Encoding.UTF8.GetBytes(path);
			if (nameBytes.Length > 100)
			{
				// GNU extension: the real name goes in a pseudo-file before the entry
				var longName = new byte[nameBytes.Length + 1];
				Array.Copy(nameBytes, longName, nameBytes.Length);
				WriteRawHeader(Encoding.ASCII.GetBytes("././@LongLink"), 0x1A4, longName.Length, (byte)'L');
				stream.Write(longName, 0, longName.Length);
				Pad(longName.Length);
				nameBytes = nameBytes.Take(100).ToArray();
			}
			WriteRawHeader(nameBytes, mode, size, type);
		}

		void WriteRawHeader(byte[] name, int mode, long size, byte type)
		{
			var header = new byte[512];
			Array.Copy(name, header, Math.Min(name.Length, 100));
			WriteOctal(header, 100, 8, mode);
			WriteOctal(header, 108, 8, 0); // uid root
			WriteOctal(header, 116, 8, 0); // gid root
			WriteOctal(header, 124, 12, size);
			WriteOctal(header, 136, 12, mtime);
			header[156] = type;
			Encoding.ASCII.GetBytes("ustar\0").CopyTo(header, 257);
			Encoding.ASCII.GetBytes("00").CopyTo(header, 263);
			Encoding.ASCII.GetBytes("root").CopyTo(header, 265);
			Encoding.ASCII.GetBytes("root").CopyTo(header, 297);

			// checksum is calculated with its own field filled with spaces
			for (int i = 148; i < 156; i++)
				header[i] = (byte)' ';
			var checksum = header.Sum(b => (long)b);
			var checksumText = Convert.ToString(checksum, 8).PadLeft(6, '0');
			Encoding.ASCII.GetBytes(checksumText).CopyTo(header, 148);
			header[154] = 0;
			header[155] = (byte)' ';

			stream.Write(header, 0, header.Length);
		}

		static void WriteOctal(byte[] buffer, int offset, int length, long value)
		{
			var text = Convert.ToString(value, 8).PadLeft(length - 1, '0');
			Encoding.ASCII.GetBytes(text).CopyTo(buffer, offset);
			buffer[offset + length - 1] = 0;
		}
	}

	static class PackageFiles
	{
		public static long GetModifiedTime()
		{
			// honour reproducible build conventions when set
			var sourceDateEpoch = Environment.GetEnvironmentVariable("SOURCE_DATE_EPOCH");
			if (long.TryParse(sourceDateEpoch, out var epoch))
				return epoch;
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}

		public static string GetTargetPath(ITaskItem item) => item.GetMetadata("TargetPath").Replace('\\', '/').TrimStart('/');

		public static int GetMode(ITaskItem item)
		{
			var mode = item.GetMetadata("Mode");
			return string.IsNullOrEmpty(mode) ? 0x1A4 /* 644 */ : Convert.ToInt32(mode, 8);
		}

		public static IEnumerable<ITaskItem> Sorted(ITaskItem[] files) => files.OrderBy(GetTargetPath, StringComparer.Ordinal);

		public static byte[] CreateTarGz(Action<TarWriter> addEntries, long mtime)
		{
			using (var output = new MemoryStream())
			{
				using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
				{
					var tar = new TarWriter(gzip, mtime);
					addEntries(tar);
					tar.Finish();
				}
				return output.ToArray();
			}
		}
	}

	/// <summary>
	/// Writes a Debian package: an ar archive holding debian-binary, control.tar.gz and data.tar.gz.
	/// </summary>
	public class WriteDebPackage : Task
	{
		/// <summary>Files to install, with TargetPath (e.g. usr/bin/app) and optional octal Mode metadata.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }

		[Required]
		public string Package { get; set; }

		[Required]
		public string Version { get; set; }

		[Required]
		public string Architecture { get; set; }

		[Required]
		public string Maintainer { get; set; }

		[Required]
		public string Description { get; set; }

		public string Homepage { get; set; }

		public string[] Depends { get; set; }

		[Required]
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var mtime = PackageFiles.GetModifiedTime();
			var files = PackageFiles.Sorted(Files).ToList();

			var md5sums = new StringBuilder();
			long installedSize = 0;
			var data = PackageFiles.CreateTarGz(tar =>
			{
				tar.AddDirectory(".");
				foreach (var file in files)
				{
					var targetPath = PackageFiles.GetTargetPath(file);
					var bytes = File.ReadAllBytes(file.ItemSpec);
					installedSize += bytes.Length;
					tar.AddFile("./" + targetPath, bytes, PackageFiles.GetMode(file));
					using (var md5 = MD5.Create())
						md5sums.Append(string.Concat(md5.ComputeHash(bytes).Select(b => b.ToString("x2")))).Append("  ").Append(targetPath).Append('\n');
				}
			}, mtime);

			var control = new StringBuilder();
			control.Append($"Package: {Package}\n");
			control.Append($"Version: {Version}\n");
			control.Append($"Architecture: {Architecture}\n");
			control.Append($"Maintainer: {Maintainer}\n");
			control.Append($"Installed-Size: {(installedSize + 1023) / 1024}\n");
			if (Depends != null && Depends.Length > 0)
				control.Append($"Depends: {string.Join(", ", Depends)}\n");
			control.Append("Section: misc\n");
			control.Append("Priority: optional\n");
			if (!string.IsNullOrEmpty(Homepage))
				control.Append($"Homepage: {Homepage}\n");
			// the first line is the synopsis; later lines are indented, with blank lines written as " ."
			var lines = Description.Replace("\r\n", "\n").Trim().Split('\n');
			control.Append($"Description: {lines[0].Trim()}\n");
			foreach (var line in lines.Skip(1))
				control.Append(string.IsNullOrWhiteSpace(line) ? " .\n" : $" {line.Trim()}\n");
			var controlTar = PackageFiles.CreateTarGz(tar =>
			{
				tar.AddDirectory(".");
				tar.AddFile("./control", Encoding.UTF8.GetBytes(control.ToString()), 0x1A4);
				tar.AddFile("./md5sums", Encoding.UTF8.GetBytes(md5sums.ToString()), 0x1A4);
			}, mtime);

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			using (var output = File.Create(OutputFile))
			{
				var signature = Encoding.ASCII.GetBytes("!<arch>\n");
				output.Write(signature, 0, signature.Length);
				WriteArMember(output, "debian-binary", Encoding.ASCII.GetBytes("2.0\n"), mtime);
				WriteArMember(output, "control.tar.gz", controlTar, mtime);
				WriteArMember(output, "data.tar.gz", data, mtime);
			}
			return !Log.HasLoggedErrors;
		}

		static void WriteArMember(Stream output, string name, byte[] data, long mtime)
		{
			var header = new StringBuilder();
			header.Append(name.PadRight(16));
			header.Append(mtime.ToString().PadRight(12));
			header.Append("0".PadRight(6)); // uid
			header.Append("0".PadRight(6)); // gid
			header.Append("100644".PadRight(8));
			header.Append(data.Length.ToString().PadRight(10));
			header.Append("`\n");
			var headerBytes = Encoding.ASCII.GetBytes(header.ToString());
			output.Write(headerBytes, 0, headerBytes.Length);
			output.Write(data, 0, data.Length);
			if (data.Length % 2 != 0)
				output.WriteByte((byte)'\n');
		}
	}

	/// <summary>
	/// Writes a .tar.gz with the specified files.
	/// </summary>
	public class WriteTarGzPackage : Task
	{
		/// <summary>Files to include, with TargetPath and optional octal Mode metadata.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }

		[Required]
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var data = PackageFiles.CreateTarGz(tar =>
			{
				foreach (var file in PackageFiles.Sorted(Files))
					tar.AddFile(PackageFiles.GetTargetPath(file), File.ReadAllBytes(file.ItemSpec), PackageFiles.GetMode(file));
			}, PackageFiles.GetModifiedTime());

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			File.WriteAllBytes(OutputFile, data);
			return !Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// Lays out the app icon in the hicolor theme: a .svg as scalable, a .png at its own size,
	/// and a Windows .ico as each PNG image it holds, so the app's ApplicationIcon can be reused.
	/// </summary>
	public class PrepareLinuxIcons : Task
	{
		[Required]
		public string IconFile { get; set; }

		/// <summary>Name the icons are installed as, which the desktop file refers to.</summary>
		[Required]
		public string IconName { get; set; }

		[Required]
		public string OutputDirectory { get; set; }

		/// <summary>Icon files, with TargetPath (e.g. usr/share/icons/hicolor/256x256/apps/name.png) and Size metadata.</summary>
		[Output]
		public ITaskItem[] Icons { get; set; }

		public override bool Execute()
		{
			var icons = new List<ITaskItem>();
			var extension = Path.GetExtension(IconFile).ToLowerInvariant();
			if (extension == ".svg")
				icons.Add(CreateIcon(IconFile, "scalable", ".svg", int.MaxValue));
			else if (extension == ".png")
				icons.Add(CreatePngIcon(IconFile, ReadPngSize(File.ReadAllBytes(IconFile), 0)));
			else if (extension == ".ico")
				icons.AddRange(ExtractIco());
			else
				Log.LogWarning($"Unsupported icon {IconFile}. Set LinuxIcon to a .png, .svg or .ico file.");
			Icons = icons.ToArray();
			return !Log.HasLoggedErrors;
		}

		TaskItem CreatePngIcon(string file, int size) => CreateIcon(file, $"{size}x{size}", ".png", size);

		TaskItem CreateIcon(string file, string folder, string extension, int size)
		{
			var item = new TaskItem(file);
			item.SetMetadata("TargetPath", $"usr/share/icons/hicolor/{folder}/apps/{IconName}{extension}");
			item.SetMetadata("Size", size.ToString());
			return item;
		}

		static int ReadPngSize(byte[] bytes, int offset)
		{
			// width is the first field of the IHDR chunk, big-endian
			return (bytes[offset + 16] << 24) | (bytes[offset + 17] << 16) | (bytes[offset + 18] << 8) | bytes[offset + 19];
		}

		IEnumerable<ITaskItem> ExtractIco()
		{
			var bytes = File.ReadAllBytes(IconFile);
			var count = BitConverter.ToUInt16(bytes, 4);
			var sizes = new HashSet<int>();
			Directory.CreateDirectory(OutputDirectory);
			for (int i = 0; i < count; i++)
			{
				var entry = 6 + i * 16;
				var length = BitConverter.ToInt32(bytes, entry + 8);
				var offset = BitConverter.ToInt32(bytes, entry + 12);

				// newer .ico files store large sizes as PNG; the rest are bitmaps that need converting
				var isPng = length > 24 && bytes[offset] == 0x89 && bytes[offset + 1] == (byte)'P' && bytes[offset + 2] == (byte)'N' && bytes[offset + 3] == (byte)'G';
				byte[] png;
				int size;
				if (isPng)
				{
					png = new byte[length];
					Array.Copy(bytes, offset, png, 0, length);
					size = ReadPngSize(png, 0);
				}
				else
				{
					png = IcoBitmapToPng(bytes, offset, out size);
					if (png == null)
						continue;
				}
				if (!sizes.Add(size))
					continue;

				var path = Path.Combine(OutputDirectory, $"{IconName}-{size}.png");
				File.WriteAllBytes(path, png);
				yield return CreatePngIcon(path, size);
			}

			if (sizes.Count == 0)
				Log.LogWarning($"{IconFile} has no images that can be used, so no icon will be installed. Set LinuxIcon to a .png or .svg file instead.");
		}

		/// <summary>Converts a bitmap image from an .ico (a DIB with an AND mask for transparency) to PNG.</summary>
		byte[] IcoBitmapToPng(byte[] bytes, int offset, out int size)
		{
			var width = BitConverter.ToInt32(bytes, offset + 4);
			// the height covers both the image and the AND mask below it
			var height = BitConverter.ToInt32(bytes, offset + 8) / 2;
			var bitCount = BitConverter.ToUInt16(bytes, offset + 14);
			var compression = BitConverter.ToInt32(bytes, offset + 16);
			var colorsUsed = BitConverter.ToInt32(bytes, offset + 32);
			size = width;
			if (width != height || compression != 0 || (bitCount != 1 && bitCount != 4 && bitCount != 8 && bitCount != 24 && bitCount != 32))
			{
				Log.LogMessage(MessageImportance.Low, $"Skipping a {width}x{height} {bitCount}-bit image in {IconFile}");
				return null;
			}

			var paletteOffset = offset + BitConverter.ToInt32(bytes, offset);
			var paletteSize = bitCount <= 8 ? (colorsUsed != 0 ? colorsUsed : 1 << bitCount) : 0;
			var pixelOffset = paletteOffset + paletteSize * 4;
			var stride = ((width * bitCount + 31) / 32) * 4;
			var maskOffset = pixelOffset + stride * height;
			var maskStride = ((width + 31) / 32) * 4;

			var rgba = new byte[width * height * 4];
			var hasAlpha = false;
			for (int y = 0; y < height; y++)
			{
				// rows are stored bottom up
				var row = pixelOffset + (height - 1 - y) * stride;
				for (int x = 0; x < width; x++)
				{
					int b, g, r, a = 255;
					if (bitCount == 32)
					{
						var p = row + x * 4;
						b = bytes[p]; g = bytes[p + 1]; r = bytes[p + 2]; a = bytes[p + 3];
						hasAlpha |= a != 0;
					}
					else if (bitCount == 24)
					{
						var p = row + x * 3;
						b = bytes[p]; g = bytes[p + 1]; r = bytes[p + 2];
					}
					else
					{
						var bit = x * bitCount;
						var index = (bytes[row + bit / 8] >> (8 - bitCount - bit % 8)) & ((1 << bitCount) - 1);
						var p = paletteOffset + index * 4;
						b = bytes[p]; g = bytes[p + 1]; r = bytes[p + 2];
					}
					var o = (y * width + x) * 4;
					rgba[o] = (byte)r; rgba[o + 1] = (byte)g; rgba[o + 2] = (byte)b; rgba[o + 3] = (byte)a;
				}
			}

			// without per-pixel alpha, transparency comes from the AND mask
			if (!hasAlpha)
			{
				for (int y = 0; y < height; y++)
				{
					var row = maskOffset + (height - 1 - y) * maskStride;
					for (int x = 0; x < width; x++)
					{
						var transparent = row < bytes.Length && (bytes[row + x / 8] & (0x80 >> (x % 8))) != 0;
						rgba[(y * width + x) * 4 + 3] = transparent ? (byte)0 : (byte)255;
					}
				}
			}
			return EncodePng(rgba, width, height);
		}

		static byte[] EncodePng(byte[] rgba, int width, int height)
		{
			var raw = new MemoryStream();
			for (int y = 0; y < height; y++)
			{
				raw.WriteByte(0); // no filter
				raw.Write(rgba, y * width * 4, width * 4);
			}
			var rawBytes = raw.ToArray();

			var zlib = new MemoryStream();
			zlib.WriteByte(0x78);
			zlib.WriteByte(0x9C);
			using (var deflate = new DeflateStream(zlib, CompressionLevel.Optimal, true))
				deflate.Write(rawBytes, 0, rawBytes.Length);
			uint a1 = 1, a2 = 0;
			foreach (var b in rawBytes)
			{
				a1 = (a1 + b) % 65521;
				a2 = (a2 + a1) % 65521;
			}
			WriteBigEndian(zlib, (a2 << 16) | a1);

			var png = new MemoryStream();
			png.Write(new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A }, 0, 8);
			var header = new MemoryStream();
			WriteBigEndian(header, (uint)width);
			WriteBigEndian(header, (uint)height);
			header.Write(new byte[] { 8, 6, 0, 0, 0 }, 0, 5); // 8-bit RGBA
			WriteChunk(png, "IHDR", header.ToArray());
			WriteChunk(png, "IDAT", zlib.ToArray());
			WriteChunk(png, "IEND", new byte[0]);
			return png.ToArray();
		}

		static void WriteChunk(Stream stream, string type, byte[] data)
		{
			WriteBigEndian(stream, (uint)data.Length);
			var typeAndData = Encoding.ASCII.GetBytes(type).Concat(data).ToArray();
			stream.Write(typeAndData, 0, typeAndData.Length);
			uint crc = 0xFFFFFFFF;
			foreach (var b in typeAndData)
			{
				crc ^= b;
				for (int k = 0; k < 8; k++)
					crc = (crc & 1) != 0 ? 0xEDB88320 ^ (crc >> 1) : crc >> 1;
			}
			WriteBigEndian(stream, crc ^ 0xFFFFFFFF);
		}

		static void WriteBigEndian(Stream stream, uint value)
		{
			stream.WriteByte((byte)(value >> 24));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)value);
		}
	}

	/// <summary>
	/// Writes a freedesktop.org .desktop file, which adds the app to the desktop's app menu and dock.
	/// </summary>
	public class WriteDesktopFile : Task
	{
		[Required]
		public string OutputFile { get; set; }

		[Required]
		public string Name { get; set; }

		public string Comment { get; set; }

		[Required]
		public string Exec { get; set; }

		public string Icon { get; set; }

		public string Categories { get; set; }

		/// <summary>Matches running windows to this file on X11; Wayland matches by the file name instead.</summary>
		public string StartupWMClass { get; set; }

		public override bool Execute()
		{
			var text = new StringBuilder();
			text.Append("[Desktop Entry]\n");
			text.Append("Type=Application\n");
			text.Append($"Name={Name}\n");
			// a comment that just repeats the name is flagged by desktop-file-validate
			if (!string.IsNullOrEmpty(Comment) && Comment != Name)
				text.Append($"Comment={Comment}\n");
			text.Append($"Exec={Exec}\n");
			if (!string.IsNullOrEmpty(Icon))
				text.Append($"Icon={Icon}\n");
			text.Append("Terminal=false\n");
			if (!string.IsNullOrEmpty(Categories))
				text.Append($"Categories={Categories.TrimEnd(';')};\n");
			if (!string.IsNullOrEmpty(StartupWMClass))
				text.Append($"StartupWMClass={StartupWMClass}\n");

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			File.WriteAllText(OutputFile, text.ToString());
			return true;
		}
	}

	/// <summary>
	/// Writes an rpm spec that packages files already laid out in a staging directory, for rpmbuild -bb.
	/// </summary>
	public class WriteRpmSpec : Task
	{
		/// <summary>Files to install, with TargetPath and optional octal Mode metadata.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }

		/// <summary>Directory holding the files at their TargetPath, copied into the package as-is.</summary>
		[Required]
		public string StagingDirectory { get; set; }

		/// <summary>Directories the package owns along with their subdirectories, e.g. usr/lib/app; shared ones like usr/share are left to the system.</summary>
		public string[] OwnedDirectories { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Version { get; set; }

		[Required]
		public string Summary { get; set; }

		public string Description { get; set; }

		[Required]
		public string License { get; set; }

		public string Url { get; set; }

		[Required]
		public string Architecture { get; set; }

		public string[] Requires { get; set; }

		[Required]
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var spec = new StringBuilder();
			spec.Append($"Name: {Name}\n");
			spec.Append($"Version: {Version}\n");
			spec.Append("Release: 1\n");
			spec.Append($"Summary: {Summary}\n");
			spec.Append($"License: {License}\n");
			if (!string.IsNullOrEmpty(Url))
				spec.Append($"URL: {Url}\n");
			spec.Append($"BuildArch: {Architecture}\n");
			foreach (var require in Requires ?? new string[0])
				spec.Append($"Requires: {require}\n");
			// the files are prebuilt .NET assemblies, so skip dependency scanning, stripping and debug info
			spec.Append("AutoReqProv: no\n");
			spec.Append("%define __os_install_post %{nil}\n");
			spec.Append("%define debug_package %{nil}\n");
			spec.Append("%define _build_id_links none\n");
			spec.Append("\n%description\n");
			spec.Append(string.IsNullOrEmpty(Description) ? Summary : Description).Append("\n");
			spec.Append("\n%install\n");
			spec.Append("mkdir -p %{buildroot}\n");
			spec.Append($"cp -a \"{StagingDirectory.TrimEnd('/', '\\')}/.\" %{{buildroot}}/\n");
			spec.Append("\n%files\n");
			foreach (var directory in GetOwnedDirectories().OrderBy(r => r, StringComparer.Ordinal))
				spec.Append($"%dir %attr(755,root,root) \"/{directory}\"\n");
			foreach (var file in PackageFiles.Sorted(Files))
			{
				var mode = Convert.ToString(PackageFiles.GetMode(file), 8);
				spec.Append($"%attr({mode},root,root) \"/{PackageFiles.GetTargetPath(file)}\"\n");
			}

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			File.WriteAllText(OutputFile, spec.ToString());
			return true;
		}

		IEnumerable<string> GetOwnedDirectories()
		{
			var directories = new HashSet<string>();
			foreach (var root in (OwnedDirectories ?? new string[0]).Select(r => r.Replace('\\', '/').Trim('/')))
			{
				directories.Add(root);
				foreach (var file in Files.Select(PackageFiles.GetTargetPath).Where(r => r.StartsWith(root + "/", StringComparison.Ordinal)))
				{
					var directory = Path.GetDirectoryName(file).Replace('\\', '/');
					while (directory.Length > root.Length && directories.Add(directory))
						directory = Path.GetDirectoryName(directory).Replace('\\', '/');
				}
			}
			return directories;
		}
	}
}
