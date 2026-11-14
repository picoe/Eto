using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using IStream = System.Runtime.InteropServices.ComTypes.IStream;

namespace Eto.Windows.Build
{
	static class WindowsPackageHelper
	{
		// namespace for the GUIDs derived from names, so the same app always gets the same ones
		static readonly Guid EtoNamespace = new Guid("2f1c5e9a-7b3d-4e8f-9a61-0c5d8b7e2f43");

		/// <summary>Windows Installer versions are major.minor.build, with major and minor up to 255 and build up to 65535.</summary>
		public static string ProductVersion(string version, TaskLoggingHelper log)
		{
			var match = Regex.Match(version ?? string.Empty, @"^\s*(\d+)(?:\.(\d+))?(?:\.(\d+))?");
			if (!match.Success)
			{
				log.LogWarning("'{0}' isn't a version number, so the installer uses 1.0.0.", version);
				return "1.0.0";
			}
			int Part(int index) => match.Groups[index].Success ? int.Parse(match.Groups[index].Value) : 0;
			int major = Part(1), minor = Part(2), build = Part(3);
			if (major > 255 || minor > 255 || build > 65535)
				log.LogError("Version {0} can't be used by Windows Installer, which allows at most 255.255.65535. Set WindowsPackageVersion.", version);
			return $"{major}.{minor}.{build}";
		}

		public static Guid NameGuid(string name)
		{
			// RFC 4122 version 5, with the GUID fields in network byte order
			var ns = EtoNamespace.ToByteArray();
			SwapByteOrder(ns);
			var bytes = ns.Concat(Encoding.UTF8.GetBytes(name)).ToArray();
			byte[] hash;
			using (var sha = SHA1.Create())
				hash = sha.ComputeHash(bytes);
			var guid = new byte[16];
			Array.Copy(hash, guid, 16);
			guid[6] = (byte)((guid[6] & 0x0F) | 0x50);
			guid[8] = (byte)((guid[8] & 0x3F) | 0x80);
			SwapByteOrder(guid);
			return new Guid(guid);
		}

		static void SwapByteOrder(byte[] guid)
		{
			void Swap(int a, int b)
			{
				var t = guid[a];
				guid[a] = guid[b];
				guid[b] = t;
			}
			Swap(0, 3);
			Swap(1, 2);
			Swap(4, 5);
			Swap(6, 7);
		}

		public static string Format(Guid guid) => guid.ToString("B").ToUpperInvariant();
	}

	/// <summary>
	/// Converts license files to RTF, which is all an msi can show. The results are plain ASCII, so they fit any codepage.
	/// </summary>
	static class Rtf
	{
		const string Header = @"{\rtf1\ansi\ansicpg1252\deff0\uc1{\fonttbl{\f0\fswiss\fcharset0 Segoe UI;}{\f1\fmodern\fcharset0 Consolas;}}{\colortbl;\red160\green160\blue160;}\viewkind4\f0\fs18" + "\n";
		// the installer's text box doesn't draw paragraph borders, so rules are a line of characters
		static readonly string Rule = @"\pard\plain\sa120\f0\fs18\cf1 " + string.Concat(Enumerable.Repeat(@"\u9472?", 50)) + "\\par\n";
		const string Paragraph = @"\pard\plain\sa120\f0\fs18 ";

		static string Escape(string text)
		{
			var sb = new StringBuilder();
			foreach (var c in text)
			{
				if (c == '\\' || c == '{' || c == '}')
					sb.Append('\\').Append(c);
				else if (c == '\t')
					sb.Append(@"\tab ");
				else if (c < 0x80)
					sb.Append(c);
				else
					sb.Append(@"\u").Append((short)c).Append('?');
			}
			return sb.ToString();
		}

		/// <summary>Keeps the file as it is, apart from escaping characters outside ASCII.</summary>
		public static string FromRtfFile(string path)
		{
			var bytes = File.ReadAllBytes(path);
			var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
			var sb = new StringBuilder(bytes.Length);
			for (int i = start; i < bytes.Length; i++)
			{
				var b = bytes[i];
				if (b < 0x80)
					sb.Append((char)b);
				else
					sb.Append(@"\'").Append(b.ToString("x2"));
			}
			return sb.ToString();
		}

		public static string FromText(string text)
		{
			var sb = new StringBuilder(Header);
			sb.Append(@"\pard\f0\fs18 ");
			foreach (var line in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
				sb.Append(Escape(line)).Append("\\par\n");
			sb.Append('}');
			return sb.ToString();
		}

		static readonly Regex InlinePattern = new Regex(
			@"\\(?<escaped>[\\`*_{}\[\]()#+\-.!>])"
			+ @"|`(?<code>[^`]+)`"
			+ @"|\*\*(?<bold>.+?)\*\*|__(?<bold>.+?)__"
			+ @"|\*(?!\s)(?<italic>.+?)(?<!\s)\*|(?<![A-Za-z0-9])_(?!\s)(?<italic>.+?)(?<!\s)_(?![A-Za-z0-9])"
			+ @"|!?\[(?<text>[^\]]*)\]\((?<url>[^)\s]*)(?:\s+""[^""]*"")?\)"
			+ @"|<(?<autolink>(?:https?://|mailto:)[^>\s]+)>");

		static string Inline(string text)
		{
			var sb = new StringBuilder();
			var index = 0;
			foreach (Match match in InlinePattern.Matches(text))
			{
				sb.Append(Escape(text.Substring(index, match.Index - index)));
				index = match.Index + match.Length;
				if (match.Groups["escaped"].Success)
					sb.Append(Escape(match.Groups["escaped"].Value));
				else if (match.Groups["code"].Success)
					sb.Append(@"{\f1 ").Append(Escape(match.Groups["code"].Value)).Append('}');
				else if (match.Groups["bold"].Success)
					sb.Append(@"{\b ").Append(Inline(match.Groups["bold"].Value)).Append('}');
				else if (match.Groups["italic"].Success)
					sb.Append(@"{\i ").Append(Inline(match.Groups["italic"].Value)).Append('}');
				else if (match.Groups["autolink"].Success)
					sb.Append(Escape(match.Groups["autolink"].Value));
				else
				{
					// links can't be clicked in the installer, so show where they go
					var linkText = match.Groups["text"].Value;
					var url = match.Groups["url"].Value;
					sb.Append(Inline(linkText));
					if (url.Length > 0 && url != linkText && !url.StartsWith("#"))
						sb.Append(" (").Append(Escape(url)).Append(')');
				}
			}
			sb.Append(Escape(text.Substring(index)));
			return sb.ToString();
		}

		/// <summary>Shows headings, paragraphs, lists, quotes, code, rules, bold, italic and links.</summary>
		public static string FromMarkdown(string markdown)
		{
			var sb = new StringBuilder(Header);
			var lines = new List<string>();
			string prefix = null; // how the paragraph being collected starts, e.g. its bullet
			var indent = 0;
			var quote = false;
			var inCode = false;

			void Flush()
			{
				if (lines.Count == 0)
					return;
				sb.Append(Paragraph);
				var left = indent + (quote ? 360 : 0);
				if (prefix != null)
					sb.Append($@"\sa60\fi-300\li{left + 300}\tx{left + 300} ").Append(prefix).Append(@"\tab ");
				else if (left > 0)
					sb.Append($@"\li{left} ");
				if (quote)
					sb.Append(@"\i ");
				// a line ending in two spaces or a backslash continues on the next line
				for (int i = 0; i < lines.Count; i++)
				{
					var line = lines[i];
					var hardBreak = line.EndsWith("  ") || line.EndsWith("\\");
					sb.Append(Inline(line.TrimEnd(' ', '\\').Trim()));
					if (i < lines.Count - 1)
						sb.Append(hardBreak ? @"\line " : " ");
				}
				sb.Append("\\par\n");
				lines.Clear();
				prefix = null;
				indent = 0;
				quote = false;
			}

			foreach (var raw in markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
			{
				var line = raw.Replace("\t", "    ");
				if (inCode)
				{
					if (Regex.IsMatch(line, @"^\s*(```|~~~)"))
					{
						inCode = false;
						// space after the block, like after a paragraph
						sb.Append(@"\pard\plain\fs12\par").Append('\n');
					}
					else
						sb.Append(@"\pard\plain\li360\f1\fs16 ").Append(Escape(line)).Append("\\par\n");
					continue;
				}
				if (Regex.IsMatch(line, @"^\s*(```|~~~)"))
				{
					Flush();
					inCode = true;
					continue;
				}
				if (line.Trim().Length == 0)
				{
					Flush();
					continue;
				}
				Match match;
				// a line of = or - under a paragraph makes it a heading
				if (lines.Count > 0 && prefix == null && !quote && (match = Regex.Match(line, @"^\s{0,3}(=+|-+)\s*$")).Success)
				{
					var text = string.Join(" ", lines.Select(r => r.Trim()));
					lines.Clear();
					AppendHeading(sb, match.Groups[1].Value[0] == '=' ? 1 : 2, text);
					continue;
				}
				if ((match = Regex.Match(line, @"^\s{0,3}(#{1,6})\s+(.*?)(\s+#+)?\s*$")).Success)
				{
					Flush();
					AppendHeading(sb, match.Groups[1].Length, match.Groups[2].Value);
					continue;
				}
				if (Regex.IsMatch(line, @"^\s{0,3}([-*_])(\s*\1){2,}\s*$"))
				{
					Flush();
					sb.Append(Rule);
					continue;
				}
				if ((match = Regex.Match(line, @"^(\s*)([-*+]|\d{1,9}[.)])\s+(.*)$")).Success)
				{
					Flush();
					var marker = match.Groups[2].Value;
					prefix = char.IsDigit(marker[0]) ? Escape(marker.TrimEnd(')', '.') + ".") : @"\bullet";
					indent = 360 * (match.Groups[1].Value.Length / 2);
					lines.Add(match.Groups[3].Value);
					continue;
				}
				if ((match = Regex.Match(line, @"^\s{0,3}>\s?(.*)$")).Success)
				{
					if (!quote)
						Flush();
					quote = true;
					if (match.Groups[1].Value.Trim().Length == 0)
						Flush();
					else
						lines.Add(match.Groups[1].Value);
					continue;
				}
				lines.Add(line);
			}
			Flush();
			sb.Append('}');
			return sb.ToString();
		}

		static void AppendHeading(StringBuilder sb, int level, string text)
		{
			var size = level == 1 ? 28 : level == 2 ? 24 : level == 3 ? 20 : 18;
			sb.Append($@"\pard\plain\keepn\sb120\sa60\f0\fs{size}\b ").Append(Inline(text.Trim())).Append("\\b0\\par\n");
		}
	}

	/// <summary>
	/// Reads .ico and .png images for the wizard, which can only show icons.
	/// </summary>
	static class IconImage
	{
		static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

		/// <summary>The largest width in the image, or 0 when it isn't an .ico or .png.</summary>
		public static int GetLargestSize(string path)
		{
			var data = File.ReadAllBytes(path);
			if (IsPng(data))
				return data.Length >= 24 ? ReadBigEndian(data, 16) : 0;
			if (data.Length < 6 || BitConverter.ToUInt16(data, 0) != 0 || BitConverter.ToUInt16(data, 2) != 1)
				return 0;
			var largest = 0;
			var count = BitConverter.ToUInt16(data, 4);
			for (int i = 0; i < count && 6 + i * 16 + 16 <= data.Length; i++)
			{
				// 0 means 256
				var width = data[6 + i * 16];
				largest = Math.Max(largest, width == 0 ? 256 : width);
			}
			return largest;
		}

		/// <summary>Writes an .ico to folder with only the largest image, from an .ico or a .png.</summary>
		/// <remarks>The wizard picks the image closest to the system icon size (e.g. 32 pixels) and stretches it, whatever size it's shown at.</remarks>
		public static string ToIcon(string path, string folder)
		{
			var data = File.ReadAllBytes(path);
			byte[] entry, image;
			if (IsPng(data))
			{
				var width = ReadBigEndian(data, 16);
				var height = ReadBigEndian(data, 20);
				entry = new byte[16];
				entry[0] = (byte)(width >= 256 ? 0 : width);
				entry[1] = (byte)(height >= 256 ? 0 : height);
				entry[4] = 1;
				entry[6] = 32;
				image = data;
			}
			else
			{
				var count = BitConverter.ToUInt16(data, 4);
				var largest = Enumerable.Range(0, count)
					.Where(i => 6 + i * 16 + 16 <= data.Length)
					.OrderByDescending(i => data[6 + i * 16] == 0 ? 256 : data[6 + i * 16])
					.ThenByDescending(i => BitConverter.ToUInt16(data, 6 + i * 16 + 6))
					.First();
				entry = data.Skip(6 + largest * 16).Take(16).ToArray();
				var size = BitConverter.ToInt32(entry, 8);
				var offset = BitConverter.ToInt32(entry, 12);
				image = data.Skip(offset).Take(size).ToArray();
			}
			var icon = Path.Combine(folder, "WizardImage.ico");
			using (var writer = new BinaryWriter(File.Create(icon)))
			{
				writer.Write((ushort)0);
				writer.Write((ushort)1);
				writer.Write((ushort)1);
				writer.Write(entry, 0, 8);
				writer.Write(image.Length);
				writer.Write(22);
				writer.Write(image);
			}
			return icon;
		}

		/// <summary>The largest image in an .ico or .png as a .png, or null when there's none that can be read.</summary>
		public static byte[] ToPng(string path)
		{
			var data = File.ReadAllBytes(path);
			if (IsPng(data))
				return data;
			if (data.Length < 6 || BitConverter.ToUInt16(data, 0) != 0 || BitConverter.ToUInt16(data, 2) != 1)
				return null;
			var count = BitConverter.ToUInt16(data, 4);
			var frames = Enumerable.Range(0, count)
				.Where(i => 6 + i * 16 + 16 <= data.Length)
				.OrderByDescending(i => data[6 + i * 16] == 0 ? 256 : data[6 + i * 16])
				.ThenByDescending(i => BitConverter.ToUInt16(data, 6 + i * 16 + 6));
			foreach (var i in frames)
			{
				var size = BitConverter.ToInt32(data, 6 + i * 16 + 8);
				var offset = BitConverter.ToInt32(data, 6 + i * 16 + 12);
				if (size <= 0 || offset < 0 || offset + size > data.Length)
					continue;
				var image = data.Skip(offset).Take(size).ToArray();
				var png = IsPng(image) ? image : DibToPng(image);
				if (png != null)
					return png;
			}
			return null;
		}

		/// <summary>A .png of a single color, for when there's no image to use.</summary>
		public static byte[] SolidPng(int size, byte r, byte g, byte b)
		{
			var rgba = new byte[size * size * 4];
			for (int i = 0; i < rgba.Length; i += 4)
			{
				rgba[i] = r;
				rgba[i + 1] = g;
				rgba[i + 2] = b;
				rgba[i + 3] = 255;
			}
			return EncodePng(size, size, rgba);
		}

		// icons store their images as 24 or 32 bit bottom-up bitmaps followed by a transparency mask
		static byte[] DibToPng(byte[] dib)
		{
			if (dib.Length < 40)
				return null;
			var headerSize = BitConverter.ToInt32(dib, 0);
			var width = BitConverter.ToInt32(dib, 4);
			var height = BitConverter.ToInt32(dib, 8) / 2;
			var bits = BitConverter.ToUInt16(dib, 14);
			var compression = BitConverter.ToInt32(dib, 16);
			if ((bits != 32 && bits != 24) || compression != 0 || width <= 0 || height <= 0)
				return null;
			var stride = (width * bits / 8 + 3) & ~3;
			var maskStride = (width + 31) / 32 * 4;
			var maskOffset = headerSize + stride * height;
			if (maskOffset > dib.Length)
				return null;
			var hasMask = maskOffset + maskStride * height <= dib.Length;
			var rgba = new byte[width * height * 4];
			var hasAlpha = false;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var src = headerSize + (height - 1 - y) * stride + x * bits / 8;
					var dst = (y * width + x) * 4;
					rgba[dst] = dib[src + 2];
					rgba[dst + 1] = dib[src + 1];
					rgba[dst + 2] = dib[src];
					rgba[dst + 3] = bits == 32 ? dib[src + 3] : (byte)255;
					hasAlpha |= bits == 32 && dib[src + 3] != 0;
				}
			}
			// older icons leave the alpha empty and use only the mask
			if (!hasAlpha)
			{
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						var transparent = hasMask && (dib[maskOffset + (height - 1 - y) * maskStride + x / 8] >> (7 - x % 8) & 1) != 0;
						rgba[(y * width + x) * 4 + 3] = transparent ? (byte)0 : (byte)255;
					}
				}
			}
			return EncodePng(width, height, rgba);
		}

		static byte[] EncodePng(int width, int height, byte[] rgba)
		{
			// zlib stream: header, deflated rows each starting with filter type 0, and the Adler-32 of the rows
			var zlib = new MemoryStream();
			zlib.WriteByte(0x78);
			zlib.WriteByte(0x9C);
			uint a = 1, b = 0;
			using (var deflate = new System.IO.Compression.DeflateStream(zlib, System.IO.Compression.CompressionLevel.Optimal, true))
			{
				var row = new byte[width * 4 + 1];
				for (int y = 0; y < height; y++)
				{
					Array.Copy(rgba, y * width * 4, row, 1, width * 4);
					deflate.Write(row, 0, row.Length);
					foreach (var value in row)
					{
						a = (a + value) % 65521;
						b = (b + a) % 65521;
					}
				}
			}
			WriteBigEndian(zlib, b << 16 | a);

			var png = new MemoryStream();
			png.Write(PngSignature, 0, PngSignature.Length);
			var header = new MemoryStream();
			WriteBigEndian(header, (uint)width);
			WriteBigEndian(header, (uint)height);
			// 8 bits per channel, RGBA, default compression, filtering and no interlacing
			header.Write(new byte[] { 8, 6, 0, 0, 0 }, 0, 5);
			WriteChunk(png, "IHDR", header.ToArray());
			WriteChunk(png, "IDAT", zlib.ToArray());
			WriteChunk(png, "IEND", new byte[0]);
			return png.ToArray();
		}

		static void WriteChunk(Stream stream, string type, byte[] data)
		{
			var typeBytes = Encoding.ASCII.GetBytes(type);
			WriteBigEndian(stream, (uint)data.Length);
			stream.Write(typeBytes, 0, 4);
			stream.Write(data, 0, data.Length);
			var crc = 0xFFFFFFFFu;
			foreach (var value in typeBytes.Concat(data))
			{
				crc ^= value;
				for (int k = 0; k < 8; k++)
					crc = (crc & 1) != 0 ? 0xEDB88320 ^ (crc >> 1) : crc >> 1;
			}
			WriteBigEndian(stream, ~crc);
		}

		static void WriteBigEndian(Stream stream, uint value)
		{
			stream.WriteByte((byte)(value >> 24));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)value);
		}

		static bool IsPng(byte[] data) => data.Length >= PngSignature.Length && PngSignature.SequenceEqual(data.Take(PngSignature.Length));

		static int ReadBigEndian(byte[] data, int offset) => data[offset] << 24 | data[offset + 1] << 16 | data[offset + 2] << 8 | data[offset + 3];
	}

	class InstallerLanguage
	{
		public string Culture;
		public int Lcid;
		public int Codepage;
		public readonly Dictionary<string, string> Strings = new Dictionary<string, string>(StringComparer.Ordinal);
	}

	/// <summary>
	/// Reads the installers' text in each language from WindowsPackage.strings.xml and any files that change or add to it.
	/// </summary>
	static class InstallerStrings
	{
		const string Fallback = "en-US";

		/// <summary>The languages to include, with the one shown when Windows' language isn't included first.</summary>
		public static List<InstallerLanguage> Load(string file, ITaskItem[] extraFiles, string cultures, TaskLoggingHelper log)
		{
			var all = new List<InstallerLanguage>();
			InstallerLanguage Find(string culture) => all.FirstOrDefault(r => string.Equals(r.Culture, culture, StringComparison.OrdinalIgnoreCase));

			void Read(string path)
			{
				XDocument document;
				try
				{
					document = XDocument.Load(path);
				}
				catch (Exception ex)
				{
					log.LogError("Reading {0} failed: {1}", path, ex.Message);
					return;
				}
				foreach (var element in document.Root.Elements("Language"))
				{
					var culture = (string)element.Attribute("Culture");
					if (string.IsNullOrEmpty(culture))
					{
						log.LogError("{0} has a Language without a Culture.", path);
						continue;
					}
					var language = Find(culture);
					if (language == null)
						all.Add(language = new InstallerLanguage { Culture = culture });
					if (int.TryParse((string)element.Attribute("Lcid"), out var lcid))
						language.Lcid = lcid;
					if (int.TryParse((string)element.Attribute("Codepage"), out var codepage))
						language.Codepage = codepage;
					foreach (var text in element.Elements("String"))
					{
						var id = (string)text.Attribute("Id");
						if (!string.IsNullOrEmpty(id))
							language.Strings[id] = text.Value;
					}
				}
			}

			Read(file);
			foreach (var item in extraFiles ?? new ITaskItem[0])
				Read(item.GetMetadata("FullPath"));

			var fallback = Find(Fallback);
			foreach (var language in all)
			{
				if (language.Lcid == 0 || language.Codepage == 0)
				{
					try
					{
						var info = new CultureInfo(language.Culture);
						if (language.Lcid == 0)
							language.Lcid = info.LCID;
						if (language.Codepage == 0)
							language.Codepage = info.TextInfo.ANSICodePage;
					}
					catch (CultureNotFoundException)
					{
					}
					if (language.Lcid == 0 || language.Lcid == 4096 || language.Codepage == 0)
						log.LogError("Windows doesn't know the culture {0}, so give its Lcid and Codepage.", language.Culture);
				}
				// strings a language doesn't have are shown in English
				if (fallback != null && language != fallback)
				{
					foreach (var text in fallback.Strings)
					{
						if (!language.Strings.ContainsKey(text.Key))
							language.Strings[text.Key] = text.Value;
					}
				}
			}

			var selected = new List<InstallerLanguage>();
			var names = (cultures ?? string.Empty).Split(';').Select(r => r.Trim()).Where(r => r.Length > 0).ToList();
			if (names.Count == 0)
			{
				if (fallback != null)
					selected.Add(fallback);
				selected.AddRange(all.Where(r => r != fallback));
			}
			foreach (var name in names)
			{
				var language = Find(name);
				if (language == null)
					log.LogError("There's no installer text for the language {0}. Add a WindowsPackageStrings file with it.", name);
				else if (!selected.Contains(language))
					selected.Add(language);
			}
			foreach (var duplicate in selected.GroupBy(r => r.Lcid).Where(r => r.Count() > 1))
				log.LogError("The languages {0} have the same Lcid, {1}, so the installer can't tell them apart.", string.Join(", ", duplicate.Select(r => r.Culture)), duplicate.Key);
			if (selected.Count == 0)
				log.LogError("The installer has no languages to show.");
			return selected;
		}
	}

	/// <summary>
	/// Reads the shared frameworks a framework-dependent app needs from its runtimeconfig.json.
	/// </summary>
	public class GetRuntimeFrameworks : Task
	{
		[Required]
		public string RuntimeConfigFile { get; set; }

		/// <summary>Framework names, with the minimum version in the Version metadata.</summary>
		[Output]
		public ITaskItem[] Frameworks { get; set; }

		public override bool Execute()
		{
			var frameworks = new List<ITaskItem>();
			if (File.Exists(RuntimeConfigFile))
			{
				var json = File.ReadAllText(RuntimeConfigFile);
				// includedFrameworks lists what a self-contained app carries itself, so only these are needed from the system
				var section = Regex.Match(json, "\"frameworks?\"\\s*:\\s*(\\[[^\\]]*\\]|\\{[^}]*\\})");
				if (section.Success)
				{
					foreach (Match framework in Regex.Matches(section.Groups[1].Value, "\\{[^{}]*\\}"))
					{
						var name = Regex.Match(framework.Value, "\"name\"\\s*:\\s*\"([^\"]+)\"");
						var version = Regex.Match(framework.Value, "\"version\"\\s*:\\s*\"([^\"]+)\"");
						if (name.Success && version.Success)
						{
							var item = new TaskItem(name.Groups[1].Value);
							item.SetMetadata("Version", version.Groups[1].Value);
							frameworks.Add(item);
						}
					}
				}
			}
			// the desktop runtime's installer includes the .NET runtime it needs
			if (frameworks.Any(r => r.ItemSpec == "Microsoft.WindowsDesktop.App"))
				frameworks.RemoveAll(r => r.ItemSpec == "Microsoft.NETCore.App");
			Frameworks = frameworks.ToArray();
			return true;
		}
	}

	/// <summary>
	/// Writes the generated part of the setup bootstrapper's source: its version info and what it installs.
	/// </summary>
	public class WriteSetupInfo : Task
	{
		[Required]
		public string OutputFile { get; set; }
		[Required]
		public string Title { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		[Required]
		public string Version { get; set; }
		public string InformationalVersion { get; set; }
		[Required]
		public string Architecture { get; set; }
		[Required]
		public string MsiName { get; set; }
		public ITaskItem[] Frameworks { get; set; }
		[Required]
		public string StringsFile { get; set; }
		public ITaskItem[] ExtraStrings { get; set; }
		/// <summary>Cultures the msi has (from WriteMsiPackage), the first used when Windows' language isn't one of them.</summary>
		[Required]
		public string Languages { get; set; }

		static string Literal(string value) => "\"" + (value ?? string.Empty)
			.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "\"";

		public override bool Execute()
		{
			var version = WindowsPackageHelper.ProductVersion(Version, Log) + ".0";
			var frameworks = (Frameworks ?? new ITaskItem[0]).Select(r => Literal(r.ItemSpec + "|" + r.GetMetadata("Version")));
			var source = new StringBuilder();
			source.AppendLine("using System.Reflection;");
			source.AppendLine($"[assembly: AssemblyTitle({Literal(Title + " Setup")})]");
			source.AppendLine($"[assembly: AssemblyProduct({Literal(Title)})]");
			source.AppendLine($"[assembly: AssemblyCompany({Literal(Manufacturer)})]");
			source.AppendLine($"[assembly: AssemblyDescription({Literal(Description)})]");
			source.AppendLine($"[assembly: AssemblyVersion({Literal(version)})]");
			source.AppendLine($"[assembly: AssemblyFileVersion({Literal(version)})]");
			source.AppendLine($"[assembly: AssemblyInformationalVersion({Literal(string.IsNullOrEmpty(InformationalVersion) ? Version : InformationalVersion)})]");
			source.AppendLine("namespace Eto.Setup");
			source.AppendLine("{");
			source.AppendLine("\tstatic class SetupInfo");
			source.AppendLine("\t{");
			source.AppendLine($"\t\tpublic const string Title = {Literal(Title)};");
			source.AppendLine($"\t\tpublic const string Architecture = {Literal(Architecture.ToLowerInvariant())};");
			source.AppendLine($"\t\tpublic const string MsiName = {Literal(MsiName)};");
			source.AppendLine($"\t\tpublic static readonly string[] Frameworks = {{ {string.Join(", ", frameworks)} }};");
			// each language's culture, then pairs of id and text
			source.AppendLine("\t\tpublic static readonly string[][] Languages =");
			source.AppendLine("\t\t{");
			foreach (var language in InstallerStrings.Load(StringsFile, ExtraStrings, Languages, Log))
			{
				var values = new List<string> { language.Culture };
				foreach (var text in language.Strings.Where(r => r.Key.StartsWith("Setup", StringComparison.Ordinal) || r.Key == "CancelButton"))
				{
					values.Add(text.Key);
					values.Add(text.Value);
				}
				source.AppendLine($"\t\t\tnew[] {{ {string.Join(", ", values.Select(Literal))} }},");
			}
			source.AppendLine("\t\t};");
			source.AppendLine("\t}");
			source.AppendLine("}");
			Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
			File.WriteAllText(OutputFile, source.ToString(), Encoding.UTF8);
			return !Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// Creates a Windows Installer package with msi.dll and makecab.exe, which come with Windows.
	/// </summary>
	public class WriteMsiPackage : Task
	{
		/// <summary>Files to install, with TargetPath relative to the install folder.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }
		/// <summary>Path of the app's executable relative to the install folder; the shortcuts start it.</summary>
		[Required]
		public string MainExecutable { get; set; }
		[Required]
		public string ProductName { get; set; }
		[Required]
		public string Version { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		public string Homepage { get; set; }
		/// <summary>Defaults to a GUID derived from AppId.</summary>
		public string UpgradeCode { get; set; }
		[Required]
		public string AppId { get; set; }
		public bool PerMachine { get; set; }
		/// <summary>Folder under Programs or Program Files, with \ for nested folders.</summary>
		[Required]
		public string InstallFolder { get; set; }
		/// <summary>x86, x64 or arm64.</summary>
		[Required]
		public string Architecture { get; set; }
		/// <summary>.ico for Installed apps; the executable's own icon is used without one.</summary>
		public string IconFile { get; set; }
		public bool DesktopShortcut { get; set; }
		/// <summary>.rtf, .md or .txt file, or http(s) link, to accept in a wizard before installing.</summary>
		public string License { get; set; }
		/// <summary>Square .ico or .png shown in the wizard; defaults to IconFile when it has large enough sizes.</summary>
		public string Image { get; set; }
		/// <summary>The wizard's text in each language (WindowsPackage.strings.xml).</summary>
		[Required]
		public string StringsFile { get; set; }
		/// <summary>Files in the same format that change the text or add languages.</summary>
		public ITaskItem[] ExtraStrings { get; set; }
		/// <summary>Cultures to include, the first used when Windows' language isn't one of them; all when empty.</summary>
		public string Languages { get; set; }
		/// <summary>Shared frameworks the app needs (from GetRuntimeFrameworks); installing stops when one is missing.</summary>
		public ITaskItem[] Frameworks { get; set; }
		[Required]
		public string OutputFile { get; set; }
		[Required]
		public string WorkDirectory { get; set; }

		[Output]
		public string ProductCode { get; set; }
		/// <summary>Cultures the package can show, with its own first; others are left out when their codepage can't hold the app's name or files.</summary>
		[Output]
		public string IncludedLanguages { get; set; }

		class TableDef
		{
			public string Name;
			public string Columns;
			public string Keys;
		}

		// the standard Windows Installer table schemas this package uses
		static readonly TableDef[] Tables =
		{
			new TableDef { Name = "Property", Columns = "`Property` CHAR(72) NOT NULL, `Value` LONGCHAR NOT NULL LOCALIZABLE", Keys = "`Property`" },
			new TableDef { Name = "Directory", Columns = "`Directory` CHAR(72) NOT NULL, `Directory_Parent` CHAR(72), `DefaultDir` CHAR(255) NOT NULL LOCALIZABLE", Keys = "`Directory`" },
			new TableDef { Name = "Component", Columns = "`Component` CHAR(72) NOT NULL, `ComponentId` CHAR(38), `Directory_` CHAR(72) NOT NULL, `Attributes` SHORT NOT NULL, `Condition` CHAR(255), `KeyPath` CHAR(72)", Keys = "`Component`" },
			new TableDef { Name = "Feature", Columns = "`Feature` CHAR(38) NOT NULL, `Feature_Parent` CHAR(38), `Title` CHAR(64) LOCALIZABLE, `Description` CHAR(255) LOCALIZABLE, `Display` SHORT, `Level` SHORT NOT NULL, `Directory_` CHAR(72), `Attributes` SHORT NOT NULL", Keys = "`Feature`" },
			new TableDef { Name = "FeatureComponents", Columns = "`Feature_` CHAR(38) NOT NULL, `Component_` CHAR(72) NOT NULL", Keys = "`Feature_`, `Component_`" },
			new TableDef { Name = "File", Columns = "`File` CHAR(72) NOT NULL, `Component_` CHAR(72) NOT NULL, `FileName` CHAR(255) NOT NULL LOCALIZABLE, `FileSize` LONG NOT NULL, `Version` CHAR(72), `Language` CHAR(20), `Attributes` SHORT, `Sequence` LONG NOT NULL", Keys = "`File`" },
			new TableDef { Name = "Media", Columns = "`DiskId` SHORT NOT NULL, `LastSequence` LONG NOT NULL, `DiskPrompt` CHAR(64) LOCALIZABLE, `Cabinet` CHAR(255), `VolumeLabel` CHAR(32), `Source` CHAR(72)", Keys = "`DiskId`" },
			new TableDef { Name = "Shortcut", Columns = "`Shortcut` CHAR(72) NOT NULL, `Directory_` CHAR(72) NOT NULL, `Name` CHAR(128) NOT NULL LOCALIZABLE, `Component_` CHAR(72) NOT NULL, `Target` CHAR(72) NOT NULL, `Arguments` CHAR(255), `Description` CHAR(255) LOCALIZABLE, `Hotkey` SHORT, `Icon_` CHAR(72), `IconIndex` SHORT, `ShowCmd` SHORT, `WkDir` CHAR(72), `DisplayResourceDLL` CHAR(255), `DisplayResourceId` SHORT, `DescriptionResourceDLL` CHAR(255), `DescriptionResourceId` SHORT", Keys = "`Shortcut`" },
			new TableDef { Name = "Icon", Columns = "`Name` CHAR(72) NOT NULL, `Data` OBJECT NOT NULL", Keys = "`Name`" },
			new TableDef { Name = "Binary", Columns = "`Name` CHAR(72) NOT NULL, `Data` OBJECT NOT NULL", Keys = "`Name`" },
			new TableDef { Name = "Upgrade", Columns = "`UpgradeCode` CHAR(38) NOT NULL, `VersionMin` CHAR(20), `VersionMax` CHAR(20), `Language` CHAR(255), `Attributes` LONG NOT NULL, `Remove` CHAR(255), `ActionProperty` CHAR(72) NOT NULL", Keys = "`UpgradeCode`, `VersionMin`, `VersionMax`, `Language`, `Attributes`" },
			new TableDef { Name = "LaunchCondition", Columns = "`Condition` CHAR(255) NOT NULL, `Description` CHAR(255) NOT NULL LOCALIZABLE", Keys = "`Condition`" },
			new TableDef { Name = "AppSearch", Columns = "`Property` CHAR(72) NOT NULL, `Signature_` CHAR(72) NOT NULL", Keys = "`Property`, `Signature_`" },
			new TableDef { Name = "RegLocator", Columns = "`Signature_` CHAR(72) NOT NULL, `Root` SHORT NOT NULL, `Key` CHAR(255) NOT NULL, `Name` CHAR(255), `Type` SHORT", Keys = "`Signature_`" },
			new TableDef { Name = "DrLocator",Columns = "`Signature_` CHAR(72) NOT NULL, `Parent` CHAR(72), `Path` CHAR(255), `Depth` SHORT", Keys = "`Signature_`, `Parent`, `Path`" },
			new TableDef { Name = "Signature", Columns = "`Signature` CHAR(72) NOT NULL, `FileName` CHAR(255) NOT NULL, `MinVersion` CHAR(20), `MaxVersion` CHAR(20), `MinSize` LONG, `MaxSize` LONG, `MinDate` LONG, `MaxDate` LONG, `Languages` CHAR(255)", Keys = "`Signature`" },
			new TableDef { Name = "CustomAction", Columns = "`Action` CHAR(72) NOT NULL, `Type` SHORT NOT NULL, `Source` CHAR(72), `Target` CHAR(255), `ExtendedType` LONG", Keys = "`Action`" },
			new TableDef { Name = "InstallExecuteSequence", Columns = "`Action` CHAR(72) NOT NULL, `Condition` CHAR(255), `Sequence` SHORT", Keys = "`Action`" },
			new TableDef { Name = "InstallUISequence", Columns = "`Action` CHAR(72) NOT NULL, `Condition` CHAR(255), `Sequence` SHORT", Keys = "`Action`" },
			new TableDef { Name = "TextStyle", Columns = "`TextStyle` CHAR(72) NOT NULL, `FaceName` CHAR(32) NOT NULL, `Size` SHORT NOT NULL, `Color` LONG, `StyleBits` SHORT", Keys = "`TextStyle`" },
			new TableDef { Name = "Dialog", Columns = "`Dialog` CHAR(72) NOT NULL, `HCentering` SHORT NOT NULL, `VCentering` SHORT NOT NULL, `Width` SHORT NOT NULL, `Height` SHORT NOT NULL, `Attributes` LONG, `Title` CHAR(128) LOCALIZABLE, `Control_First` CHAR(50) NOT NULL, `Control_Default` CHAR(50), `Control_Cancel` CHAR(50)", Keys = "`Dialog`" },
			new TableDef { Name = "Control", Columns = "`Dialog_` CHAR(72) NOT NULL, `Control` CHAR(50) NOT NULL, `Type` CHAR(20) NOT NULL, `X` SHORT NOT NULL, `Y` SHORT NOT NULL, `Width` SHORT NOT NULL, `Height` SHORT NOT NULL, `Attributes` LONG, `Property` CHAR(72), `Text` LONGCHAR LOCALIZABLE, `Control_Next` CHAR(50), `Help` CHAR(255) LOCALIZABLE", Keys = "`Dialog_`, `Control`" },
			new TableDef { Name = "ControlEvent", Columns = "`Dialog_` CHAR(72) NOT NULL, `Control_` CHAR(50) NOT NULL, `Event` CHAR(50) NOT NULL, `Argument` CHAR(255) NOT NULL, `Condition` CHAR(255), `Ordering` SHORT", Keys = "`Dialog_`, `Control_`, `Event`, `Argument`, `Condition`" },
			new TableDef { Name = "ControlCondition", Columns = "`Dialog_` CHAR(72) NOT NULL, `Control_` CHAR(50) NOT NULL, `Action` CHAR(50) NOT NULL, `Condition` CHAR(255) NOT NULL", Keys = "`Dialog_`, `Control_`, `Action`, `Condition`" },
			new TableDef { Name = "EventMapping", Columns = "`Dialog_` CHAR(72) NOT NULL, `Control_` CHAR(50) NOT NULL, `Event` CHAR(50) NOT NULL, `Attribute` CHAR(50) NOT NULL", Keys = "`Dialog_`, `Control_`, `Event`" },
			new TableDef { Name = "CheckBox", Columns = "`Property` CHAR(72) NOT NULL, `Value` CHAR(64)", Keys = "`Property`" },
		};

		// the file each framework always has, which AppSearch looks for to tell whether it's installed
		static readonly Dictionary<string, string> FrameworkProbeFiles = new Dictionary<string, string>
		{
			{ "Microsoft.NETCore.App", "System.Private.CoreLib.dll" },
			{ "Microsoft.WindowsDesktop.App", "PresentationFramework.dll" },
			{ "Microsoft.AspNetCore.App", "Microsoft.AspNetCore.dll" },
		};

		static readonly Dictionary<string, string> FrameworkNames = new Dictionary<string, string>
		{
			{ "Microsoft.NETCore.App", ".NET Runtime" },
			{ "Microsoft.WindowsDesktop.App", ".NET Desktop Runtime" },
			{ "Microsoft.AspNetCore.App", "ASP.NET Core Runtime" },
		};

		class StreamValue
		{
			public string Path;
		}

		class InstallFile
		{
			public string Source;
			public string TargetPath;
			public string FileKey;
			public string ComponentKey;
			public string DirectoryKey;
		}

		readonly List<string> strings = new List<string>();
		readonly Dictionary<string, List<object[]>> rows = new Dictionary<string, List<object[]>>();
		readonly Dictionary<string, HashSet<string>> shortNames = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		void Add(string table, params object[] values)
		{
			if (!rows.TryGetValue(table, out var list))
				rows[table] = list = new List<object[]>();
			list.Add(values);
			strings.AddRange(values.OfType<string>());
		}

		public override bool Execute()
		{
			try
			{
				Write();
			}
			catch (Exception ex)
			{
				Log.LogError("Creating {0} failed: {1}", OutputFile, ex.Message);
			}
			if (Log.HasLoggedErrors && File.Exists(OutputFile))
				File.Delete(OutputFile);
			return !Log.HasLoggedErrors;
		}

		// the same for each language's database
		string version, architecture, platform, manufacturer, summary;
		// the language the rows are being built in
		InstallerLanguage language;
		// .ico shown in the wizard's header, and large on its welcome and finished pages
		string wizardImage;
		bool headerImage, sideImage;

		// pixels the image is shown at with 100% display scaling, which is 3/4 of its size in installer units
		const int HeaderImagePixels = 48, SideImagePixels = 128;

		bool LoadWizardImage()
		{
			var path = Image;
			if (!string.IsNullOrEmpty(path))
			{
				if (!File.Exists(path))
				{
					Log.LogError("The wizard image {0} doesn't exist.", path);
					return false;
				}
				var size = IconImage.GetLargestSize(path);
				if (size == 0)
				{
					Log.LogError("The wizard image {0} must be an .ico or .png file.", path);
					return false;
				}
				if (size > 256)
				{
					Log.LogError("The wizard image {0} is {1} pixels wide, but can be 256 at most.", path, size);
					return false;
				}
				if (size < HeaderImagePixels)
					Log.LogWarning("The wizard image {0} is {1} pixels wide, so it may look blurry. Use {2} or larger.", path, size, SideImagePixels);
				else if (size < SideImagePixels)
					Log.LogMessage(MessageImportance.High, "The wizard image {0} is {1} pixels wide, so it's only shown small. Use {2} or larger to show it large on the welcome and finished pages.", path, size, SideImagePixels);
				// too small to show large, so the welcome and finished pages show it small like the others
				headerImage = true;
				sideImage = size >= SideImagePixels;
			}
			else if (!string.IsNullOrEmpty(IconFile) && File.Exists(IconFile))
			{
				// only where the icon has a size that won't look blurry
				path = IconFile;
				var size = IconImage.GetLargestSize(path);
				headerImage = size >= HeaderImagePixels;
				sideImage = size >= SideImagePixels;
			}
			if (headerImage || sideImage)
				wizardImage = IconImage.ToIcon(path, WorkDirectory);
			return true;
		}

		string T(string id) => language.Strings[id];

		void Write()
		{
			version = WindowsPackageHelper.ProductVersion(Version, Log);
			var languages = InstallerStrings.Load(StringsFile, ExtraStrings, Languages, Log);
			if (Log.HasLoggedErrors)
				return;
			architecture = Architecture.ToLowerInvariant();
			platform = architecture == "x64" ? "x64" : architecture == "arm64" ? "Arm64" : "Intel";
			ProductCode = WindowsPackageHelper.Format(Guid.NewGuid());
			manufacturer = string.IsNullOrEmpty(Manufacturer) ? ProductName : Manufacturer;
			summary = Regex.Replace((Description ?? string.Empty).Trim(), @"(?s)[\r\n].*", "");
			Directory.CreateDirectory(WorkDirectory);
			if (!LoadWizardImage())
				return;

			language = languages[0];
			var sorted = BuildRows();
			if (sorted == null)
				return;

			var cabinet = CreateCabinet(sorted);
			if (cabinet == null)
				return;

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			if (File.Exists(OutputFile))
				File.Delete(OutputFile);
			var codepage = ChooseCodepage(language.Codepage);
			if (codepage == null)
			{
				Log.LogWarning("The product name, file names or other text use characters that no single Windows codepage has, so some may not show correctly.");
				codepage = 1252;
			}
			WriteDatabase(OutputFile, codepage.Value);

			// other languages are transforms in the package, which Windows Installer applies to match the user's language
			var included = new List<InstallerLanguage> { language };
			var transforms = new List<KeyValuePair<int, string>>();
			foreach (var other in languages.Skip(1))
			{
				language = other;
				strings.Clear();
				rows.Clear();
				shortNames.Clear();
				BuildRows();
				var languageCodepage = ChooseCodepage(language.Codepage, onlyPreferred: true);
				if (languageCodepage == null)
				{
					Log.LogWarning("The installer won't show {0}, as its text, or the app's name or files, use characters that codepage {1} doesn't have.", language.Culture, language.Codepage);
					continue;
				}
				var database = Path.Combine(WorkDirectory, language.Lcid + ".msi");
				var transform = Path.Combine(WorkDirectory, language.Lcid + ".mst");
				WriteDatabase(database, languageCodepage.Value);
				Database.GenerateTransform(database, OutputFile, transform);
				transforms.Add(new KeyValuePair<int, string>(language.Lcid, transform));
				included.Add(language);
			}

			using (var db = Database.Open(OutputFile))
			{
				db.Execute("INSERT INTO `_Streams` (`Name`, `Data`) VALUES (?, ?)", "app.cab", new StreamValue { Path = cabinet });
				foreach (var transform in transforms)
					db.Execute("INSERT INTO `_Storages` (`Name`, `Data`) VALUES (?, ?)", transform.Key.ToString(), new StreamValue { Path = transform.Value });
				db.SetSummary(new Dictionary<int, object> { { 7, platform + ";" + string.Join(",", included.Select(r => r.Lcid)) } });
				db.Commit();
			}
			IncludedLanguages = string.Join(";", included.Select(r => r.Culture));
		}

		/// <summary>Creates the database with the rows built so far, without the files.</summary>
		void WriteDatabase(string path, int codepage)
		{
			if (File.Exists(path))
				File.Delete(path);
			using (var db = Database.Create(path))
			{
				db.SetCodepage(codepage, WorkDirectory);
				foreach (var table in Tables)
				{
					db.Execute($"CREATE TABLE `{table.Name}` ({table.Columns} PRIMARY KEY {table.Keys})");
					if (!rows.TryGetValue(table.Name, out var tableRows))
						continue;
					var columns = Regex.Matches(table.Columns, @"`(\w+)` [A-Z]").Cast<Match>().Select(r => "`" + r.Groups[1].Value + "`").ToList();
					var query = $"INSERT INTO `{table.Name}` ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(r => "?"))})";
					foreach (var row in tableRows)
					{
						if (row.Length != columns.Count)
							throw new InvalidOperationException($"{table.Name} row has {row.Length} values instead of {columns.Count}.");
						db.Execute(query, row);
					}
				}

				var now = DateTime.Now;
				db.SetSummary(new Dictionary<int, object>
				{
					{ 1, (short)codepage },
					{ 2, "Installation Database" },
					{ 3, ProductName },
					{ 4, manufacturer },
					{ 5, "Installer" },
					{ 6, string.IsNullOrEmpty(summary) ? $"This installer database contains the logic and data required to install {ProductName}." : summary },
					{ 7, platform + ";" + language.Lcid },
					{ 9, WindowsPackageHelper.Format(Guid.NewGuid()) },
					{ 12, now },
					{ 13, now },
					// Windows Installer 5, which Arm64 packages need
					{ 14, 500 },
					// compressed with long file names, and per-user packages don't need admin rights
					{ 15, PerMachine ? 2 : 2 | 8 },
					{ 18, "Eto.Forms" },
					{ 19, 2 },
				});
				db.Commit();
			}
		}

		/// <summary>Builds every table's rows in the current language, returning the files to install, or null when they can't be.</summary>
		List<InstallFile> BuildRows()
		{
			var is64Bit = architecture != "x86";
			var upgradeCode = string.IsNullOrEmpty(UpgradeCode) ? WindowsPackageHelper.NameGuid("upgrade:" + AppId) : new Guid(UpgradeCode);
			var scope = PerMachine ? "perMachine" : "perUser";

			// directories
			Add("Directory", "TARGETDIR", null, "SourceDir");
			string parent;
			if (PerMachine)
			{
				parent = is64Bit ? "ProgramFiles64Folder" : "ProgramFilesFolder";
				Add("Directory", parent, "TARGETDIR", is64Bit ? "PFiles64" : "PFiles");
			}
			else
			{
				// where per-user apps go, like %ProgramFiles% is for everyone
				Add("Directory", "LocalAppDataFolder", "TARGETDIR", "LocalApp");
				Add("Directory", "UserProgramsFolder", "LocalAppDataFolder", "Programs");
				parent = "UserProgramsFolder";
			}
			var folders = InstallFolder.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (folders.Length == 0)
				folders = new[] { ProductName };
			for (int i = 0; i < folders.Length; i++)
			{
				var key = i == folders.Length - 1 ? "INSTALLFOLDER" : "InstallParent" + i;
				Add("Directory", key, parent, FileNameColumn(parent, folders[i]));
				parent = key;
			}
			Add("Directory", "ProgramMenuFolder", "TARGETDIR", "PMenu");
			if (DesktopShortcut)
				Add("Directory", "DesktopFolder", "TARGETDIR", "Desktop");

			// files, with one component each so each can be tracked on its own
			var files = new Dictionary<string, InstallFile>(StringComparer.OrdinalIgnoreCase);
			foreach (var item in Files)
			{
				var target = item.GetMetadata("TargetPath");
				if (string.IsNullOrEmpty(target))
					target = Path.GetFileName(item.ItemSpec);
				target = target.Replace('/', '\\').TrimStart('\\');
				// later items replace earlier ones, so extra content can override what was published
				files[target] = new InstallFile { Source = item.ItemSpec, TargetPath = target };
			}
			var mainExecutable = MainExecutable.Replace('/', '\\');
			if (!files.ContainsKey(mainExecutable))
			{
				Log.LogError("The app's executable {0} wasn't published, so there's nothing for the shortcuts to start. Windows packages need UseAppHost to be on.", mainExecutable);
				return null;
			}

			var directories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { string.Empty, "INSTALLFOLDER" } };
			string GetDirectory(string relative)
			{
				if (directories.TryGetValue(relative, out var key))
					return key;
				var parentKey = GetDirectory(Path.GetDirectoryName(relative) ?? string.Empty);
				key = "dir" + directories.Count;
				Add("Directory", key, parentKey, FileNameColumn(parentKey, Path.GetFileName(relative)));
				directories[relative] = key;
				return key;
			}

			var componentAttributes = is64Bit ? 256 : 0; // msidbComponentAttributes64bit
			var sorted = files.Values.OrderBy(r => r.TargetPath, StringComparer.OrdinalIgnoreCase).ToList();
			for (int i = 0; i < sorted.Count; i++)
			{
				var file = sorted[i];
				var sequence = i + 1;
				file.FileKey = string.Equals(file.TargetPath, mainExecutable, StringComparison.OrdinalIgnoreCase) ? "MainExecutable" : "file" + sequence;
				file.ComponentKey = "cmp" + sequence;
				file.DirectoryKey = GetDirectory(Path.GetDirectoryName(file.TargetPath) ?? string.Empty);
				// stable across builds, so Windows Installer knows it's the same file in each version
				var componentId = WindowsPackageHelper.NameGuid($"component:{upgradeCode}:{scope}:{architecture}:{file.TargetPath.ToLowerInvariant()}");
				var info = new FileInfo(file.Source);
				if (info.Length > int.MaxValue)
					throw new InvalidOperationException($"{file.Source} is too large for Windows Installer.");
				Add("Component", file.ComponentKey, WindowsPackageHelper.Format(componentId), file.DirectoryKey, componentAttributes, null, file.FileKey);
				Add("FeatureComponents", "ProductFeature", file.ComponentKey);
				Add("File", file.FileKey, file.ComponentKey, FileNameColumn(file.DirectoryKey, Path.GetFileName(file.TargetPath)), (int)info.Length, GetFileVersion(file.Source), null, 512 /* vital */, sequence);
			}
			Add("Feature", "ProductFeature", null, ProductName, null, 0, 1, "INSTALLFOLDER", 0);
			Add("Media", 1, sorted.Count, null, "#app.cab", null, null);

			// shortcuts to the app, with its own icon
			var main = sorted.First(r => r.FileKey == "MainExecutable");
			Add("Shortcut", "StartMenuShortcut", "ProgramMenuFolder", FileNameColumn("ProgramMenuFolder", ProductName), main.ComponentKey, "[#MainExecutable]", null, string.IsNullOrEmpty(summary) ? null : summary, null, null, null, null, main.DirectoryKey, null, null, null, null);
			if (DesktopShortcut)
				Add("Shortcut", "DesktopShortcut", "DesktopFolder", FileNameColumn("DesktopFolder", ProductName), main.ComponentKey, "[#MainExecutable]", null, string.IsNullOrEmpty(summary) ? null : summary, null, null, null, null, main.DirectoryKey, null, null, null, null);

			var iconFile = !string.IsNullOrEmpty(IconFile) && File.Exists(IconFile) ? IconFile : main.Source;
			var iconName = "AppIcon" + Path.GetExtension(iconFile).ToLowerInvariant();
			Add("Icon", iconName, new StreamValue { Path = iconFile });

			// properties
			Add("Property", "ProductCode", ProductCode);
			Add("Property", "ProductName", ProductName);
			Add("Property", "ProductVersion", version);
			Add("Property", "Manufacturer", manufacturer);
			Add("Property", "ProductLanguage", language.Lcid.ToString());
			Add("Property", "UpgradeCode", WindowsPackageHelper.Format(upgradeCode));
			Add("Property", "ARPPRODUCTICON", iconName);
			// there's no UI to change anything with
			Add("Property", "ARPNOMODIFY", "1");
			if (!string.IsNullOrEmpty(Homepage))
				Add("Property", "ARPURLINFOABOUT", Homepage);
			if (!string.IsNullOrEmpty(summary))
				Add("Property", "ARPCOMMENTS", summary);
			if (PerMachine)
				Add("Property", "ALLUSERS", "1");

			// a newer build replaces any older one (or another build of this version), but not a newer one
			Add("Upgrade", WindowsPackageHelper.Format(upgradeCode), null, version, null, 512 /* VersionMaxInclusive */, null, "OLDERVERSIONFOUND");
			Add("Upgrade", WindowsPackageHelper.Format(upgradeCode), version, null, null, 2 /* OnlyDetect */, null, "NEWERVERSIONFOUND");
			Add("LaunchCondition", "Installed OR NOT NEWERVERSIONFOUND", T("NewerVersion"));
			var secureProperties = new List<string> { "OLDERVERSIONFOUND", "NEWERVERSIONFOUND" };

			// stop when a runtime the app needs is missing, instead of installing an app that can't start
			var frameworkIndex = 0;
			foreach (var framework in Frameworks ?? new ITaskItem[0])
			{
				if (!FrameworkProbeFiles.TryGetValue(framework.ItemSpec, out var probeFile))
				{
					Log.LogMessage(MessageImportance.Normal, "Not checking for {0} when installing, as it can't be detected.", framework.ItemSpec);
					continue;
				}
				var frameworkVersion = System.Version.TryParse(Regex.Match(framework.GetMetadata("Version"), @"^\d+(\.\d+)*").Value, out var v) ? v : new System.Version(0, 0);
				var property = "DOTNETRUNTIME" + ++frameworkIndex;
				var programFiles = is64Bit ? "[ProgramFiles64Folder]" : "[ProgramFilesFolder]";
				// file versions start with the runtime's major.minor, and apps roll forward to any newer minor version
				void Search(string searchProperty, string path)
				{
					var signature = "Sig" + searchProperty;
					secureProperties.Add(searchProperty);
					Add("AppSearch", searchProperty, signature);
					Add("Signature", signature, FileNameColumn(signature, probeFile), $"{frameworkVersion.Major}.{Math.Max(0, frameworkVersion.Minor)}.0.0", $"{frameworkVersion.Major}.65535.65535.65535", null, null, null, null, null);
					Add("DrLocator", signature, null, path, 1);
				}
				Search(property, $"{programFiles}dotnet\\shared\\{framework.ItemSpec}");
				var condition = $"Installed OR {property}";
				if (architecture == "x64")
				{
					// on Arm64 Windows that folder has the Arm64 runtime, and x64 runtimes go in their own folder
					Search(property + "X64", $"{programFiles}dotnet\\x64\\shared\\{framework.ItemSpec}");
					if (!rows.ContainsKey("RegLocator"))
					{
						Add("AppSearch", "NATIVEARCHITECTURE", "SigNativeArchitecture");
						Add("RegLocator", "SigNativeArchitecture", 2 /* HKLM */, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE", 2 | 16 /* raw value, 64 bit */);
						secureProperties.Add("NATIVEARCHITECTURE");
					}
					condition = $"Installed OR {property}X64 OR ({property} AND NOT (NATIVEARCHITECTURE ~= \"ARM64\"))";
				}
				var name = FrameworkNames.TryGetValue(framework.ItemSpec, out var n) ? n : framework.ItemSpec;
				var runtimeVersion = $"{frameworkVersion.Major}.{Math.Max(0, frameworkVersion.Minor)}";
				Add("LaunchCondition", condition, string.Format(T("RuntimeMissing"),
					EscapeFormatted($"{name} {runtimeVersion} ({architecture})"),
					EscapeFormatted($"https://dotnet.microsoft.com/download/dotnet/{runtimeVersion}")));
			}
			Add("Property", "SecureCustomProperties", string.Join(";", secureProperties));

			// show where it's installed in Installed apps
			Add("CustomAction", "SetARPINSTALLLOCATION", 51, "ARPINSTALLLOCATION", "[INSTALLFOLDER]", null);

			void Execute(string action, int sequence, string condition = null) => Add("InstallExecuteSequence", action, condition, sequence);
			Execute("FindRelatedProducts", 25);
			Execute("AppSearch", 50);
			Execute("LaunchConditions", 100);
			Execute("ValidateProductID", 700);
			Execute("CostInitialize", 800);
			Execute("FileCost", 900);
			Execute("CostFinalize", 1000);
			Execute("SetARPINSTALLLOCATION", 1001, "NOT Installed");
			Execute("MigrateFeatureStates", 1200);
			Execute("InstallValidate", 1400);
			// removing the old version completely first means nothing depends on what changed between them
			Execute("RemoveExistingProducts", 1401);
			Execute("InstallInitialize", 1500);
			Execute("ProcessComponents", 1600);
			Execute("UnpublishFeatures", 1800);
			Execute("RemoveShortcuts", 3200);
			Execute("RemoveFiles", 3500);
			Execute("RemoveFolders", 3600);
			Execute("CreateFolders", 3700);
			Execute("InstallFiles", 4000);
			Execute("CreateShortcuts", 4500);
			Execute("RegisterUser", 6000);
			Execute("RegisterProduct", 6100);
			Execute("PublishFeatures", 6300);
			Execute("PublishProduct", 6400);
			Execute("InstallFinalize", 6600);

			void UI(string action, int sequence, string condition = null) => Add("InstallUISequence", action, condition, sequence);
			UI("FindRelatedProducts", 25);
			UI("AppSearch", 50);
			UI("LaunchConditions", 100);
			UI("ValidateProductID", 700);
			UI("CostInitialize", 800);
			UI("FileCost", 900);
			UI("CostFinalize", 1000);
			UI("MigrateFeatureStates", 1200);
			UI("ExecuteAction", 1300);

			if (!AddWizard(main.DirectoryKey))
				return null;
			UI("FirstDialog", 1250, "NOT Installed");
			UI("ProgressDialog", 1280);
			UI("ExitDialog", -1);
			UI("UserExitDialog", -2);
			UI("FatalErrorDialog", -3);
			return sorted;
		}

		/// <summary>Adds a wizard that welcomes or asks to accept the license, then shows progress and the result.</summary>
		bool AddWizard(string mainDirectory)
		{
			const int Visible = 1, Enabled = 2, Sunken = 4, NoPrefix = 0x20000, Progress95 = 0x10000, ElevationShield = 0x800000;
			const int Modal = 2, Minimize = 4;
			// text that isn't enabled is drawn greyed out
			const int Label = Visible | Enabled | NoPrefix, Button = Visible | Enabled;
			const int Width = 370, Height = 270;

			string licenseText = null, licenseUrl = null;
			if (!string.IsNullOrEmpty(License) && Regex.IsMatch(License, "^https?://", RegexOptions.IgnoreCase))
				licenseUrl = License;
			else if (!string.IsNullOrEmpty(License))
			{
				if (!File.Exists(License))
				{
					Log.LogError("The license file {0} doesn't exist.", License);
					return false;
				}
				var extension = Path.GetExtension(License).ToLowerInvariant();
				if (extension == ".rtf")
					licenseText = Rtf.FromRtfFile(License);
				else if (extension == ".md" || extension == ".markdown")
					licenseText = Rtf.FromMarkdown(File.ReadAllText(License));
				else
					licenseText = Rtf.FromText(File.ReadAllText(License));
			}

			Add("TextStyle", "EtoNormal", "Segoe UI", 9, null, 0);
			Add("TextStyle", "EtoTitle", "Segoe UI", 12, null, 1 /* bold */);
			Add("Property", "DefaultUIFont", "EtoNormal");
			Add("CheckBox", "LicenseAccepted", "1");
			Add("CheckBox", "LAUNCHAPP", "1");
			Add("Property", "LAUNCHAPP", "1");
			// started from the wizard's own process, so per-machine installs don't start it as administrator
			Add("CustomAction", "LaunchApp", 34 | 192 /* exe in a folder, don't wait */, mainDirectory, "\"[#MainExecutable]\"", null);

			void Dialog(string name, int attributes, string first, string defaultControl, string cancel, int width = Width, int height = Height)
				=> Add("Dialog", name, 50, 50, width, height, attributes, T("SetupTitle"), first, defaultControl, cancel);
			void Control(string dialog, string name, string type, int x, int y, int width, int height, int attributes, string text = null, string next = null, string property = null)
				=> Add("Control", dialog, name, type, x, y, width, height, attributes, property, text, next, null);
			void Event(string dialog, string control, string name, string argument, string condition = "1", int ordering = 1)
				=> Add("ControlEvent", dialog, control, name, argument, condition, ordering);
			if (wizardImage != null)
				Add("Binary", "WizardImage", new StreamValue { Path = wizardImage });
			// sized in installer units, so the icon picks the image closest to its size in pixels
			const int HeaderImageSize = HeaderImagePixels * 3 / 4, SideImageSize = SideImagePixels * 3 / 4;
			void Header(string dialog, string title, string description)
			{
				var textWidth = headerImage ? 340 - HeaderImageSize - 10 : 340;
				Control(dialog, "Title", "Text", 15, 10, textWidth, 20, Label, "{\\EtoTitle}" + title);
				if (description != null)
					Control(dialog, "Description", "Text", 15, 31, textWidth, 26, Label, description);
				if (headerImage)
					Control(dialog, "Image", "Icon", 355 - HeaderImageSize, (60 - HeaderImageSize) / 2, HeaderImageSize, HeaderImageSize, Visible, "WizardImage");
				Control(dialog, "HeaderLine", "Line", 0, 60, Width, 0, Visible);
				Control(dialog, "FooterLine", "Line", 0, 234, Width, 0, Visible);
			}
			// the large image on the left, with the text beside it and anything else below that
			const int SideX = 15 + SideImageSize + 15, SideBottom = 112;
			void Side(string dialog, string title, string description)
			{
				Control(dialog, "Image", "Icon", 15, 20, SideImageSize, SideImageSize, Visible, "WizardImage");
				Control(dialog, "Title", "Text", SideX, 20, 355 - SideX, 34, Label, "{\\EtoTitle}" + title);
				Control(dialog, "Description", "Text", SideX, 58, 355 - SideX, 50, Label, description);
				Control(dialog, "FooterLine", "Line", 0, 234, Width, 0, Visible);
			}

			// welcome, or the license to accept, before installing
			var hasLicense = licenseText != null || licenseUrl != null;
			var first = licenseText != null ? "License" : licenseUrl != null ? "Link" : "Install";
			Dialog("FirstDialog", Visible | Modal | Minimize, first, "Install", "Cancel");
			if (!hasLicense && sideImage)
				Side("FirstDialog", T("WelcomeTitle"), T("WelcomeText"));
			else if (!hasLicense)
				Header("FirstDialog", T("WelcomeTitle"), T("WelcomeText"));
			else
			{
				Header("FirstDialog", T("LicenseTitle"), T("LicenseText"));
				if (licenseText != null)
					Control("FirstDialog", "License", "ScrollableText", 15, 67, 340, 141, Visible | Enabled | Sunken, licenseText, "Accept");
				else
				{
					Control("FirstDialog", "LinkLabel", "Text", 15, 70, 340, 15, Label, T("LicenseLink"));
					Control("FirstDialog", "Link", "Hyperlink", 15, 88, 340, 15, Visible | Enabled, $"<a href=\"{licenseUrl}\">{licenseUrl}</a>", "Accept");
				}
				Control("FirstDialog", "Accept", "CheckBox", 15, 213, 340, 15, Button, T("LicenseAccept"), "Install", "LicenseAccepted");
				Add("ControlCondition", "FirstDialog", "Install", "Disable", "LicenseAccepted <> \"1\"");
				Add("ControlCondition", "FirstDialog", "Install", "Enable", "LicenseAccepted = \"1\"");
			}
			// per-machine installs ask to run as administrator after this
			Control("FirstDialog", "Install", "PushButton", 232, 243, 60, 17, Button | (PerMachine ? ElevationShield : 0), T("InstallButton"), "Cancel");
			Control("FirstDialog", "Cancel", "PushButton", 299, 243, 60, 17, Button, T("CancelButton"), first);
			Event("FirstDialog", "Install", "EndDialog", "Return", hasLicense ? "LicenseAccepted = \"1\"" : "1");
			Event("FirstDialog", "Cancel", "SpawnDialog", "CancelDialog");

			// shown while the files are installed, and closed by the installer when done
			Dialog("ProgressDialog", Visible | Minimize, "Cancel", null, "Cancel");
			Header("ProgressDialog", T("ProgressTitle"), T("ProgressText"));
			Control("ProgressDialog", "Action", "Text", 15, 70, 340, 15, Label);
			Control("ProgressDialog", "Progress", "ProgressBar", 15, 90, 340, 12, Visible | Progress95, "Progress done");
			Control("ProgressDialog", "Cancel", "PushButton", 299, 243, 60, 17, Button, T("CancelButton"));
			Event("ProgressDialog", "Cancel", "SpawnDialog", "CancelDialog");
			Add("EventMapping", "ProgressDialog", "Action", "ActionText", "Text");
			Add("EventMapping", "ProgressDialog", "Progress", "SetProgress", "Progress");

			Dialog("CancelDialog", Visible | Modal, "No", "No", "No", 260, 85);
			Control("CancelDialog", "Text", "Text", 15, 15, 230, 30, Label, T("CancelQuestion"));
			Control("CancelDialog", "Yes", "PushButton", 128, 57, 56, 17, Button, T("YesButton"), "No");
			Control("CancelDialog", "No", "PushButton", 192, 57, 56, 17, Button, T("NoButton"), "Yes");
			Event("CancelDialog", "Yes", "EndDialog", "Exit");
			Event("CancelDialog", "No", "EndDialog", "Return");

			void Finished(string dialog, string title, string description, string result, bool launch = false)
			{
				Dialog(dialog, Visible | Modal | Minimize, launch ? "Launch" : "Finish", "Finish", "Finish");
				int launchX = 15, launchY = 70;
				if (sideImage)
				{
					launchX = SideX;
					launchY = SideBottom;
					Side(dialog, title, description);
				}
				else
					Header(dialog, title, description);
				if (launch)
				{
					Control(dialog, "Launch", "CheckBox", launchX, launchY, 355 - launchX, 15, Button, T("LaunchApp"), "Finish", "LAUNCHAPP");
					Event(dialog, "Finish", "DoAction", "LaunchApp", "LAUNCHAPP = \"1\" AND NOT REMOVE ~= \"ALL\"", 1);
				}
				Control(dialog, "Finish", "PushButton", 299, 243, 60, 17, Button, T("FinishButton"), launch ? "Launch" : null);
				Event(dialog, "Finish", "EndDialog", result, "1", 2);
			}
			Finished("ExitDialog", T("ExitTitle"), T("ExitText"), "Return", true);
			Finished("UserExitDialog", T("UserExitTitle"), T("UserExitText"), "Exit");
			Finished("FatalErrorDialog", T("FatalErrorTitle"), T("FatalErrorText"), "Exit");
			return true;
		}

		string CreateCabinet(List<InstallFile> files)
		{
			var cabinet = Path.Combine(WorkDirectory, "app.cab");
			var ddf = Path.Combine(WorkDirectory, "app.ddf");
			if (File.Exists(cabinet))
				File.Delete(cabinet);
			var lines = new List<string>
			{
				".OPTION EXPLICIT",
				".Set CabinetNameTemplate=app.cab",
				$".Set DiskDirectoryTemplate=\"{WorkDirectory.TrimEnd('\\')}\"",
				".Set Cabinet=on",
				".Set Compress=on",
				".Set CompressionType=LZX",
				".Set CompressionMemory=21",
				// no limits, so everything goes in a single cabinet
				".Set MaxDiskSize=0",
				".Set MaxCabinetSize=0",
				".Set CabinetFileCountThreshold=0",
				".Set FolderFileCountThreshold=0",
				".Set FolderSizeThreshold=0",
				".Set RptFileName=NUL",
				".Set InfFileName=NUL",
			};
			// files are stored by their File key, in sequence order
			lines.AddRange(files.Select(r => $"\"{Path.GetFullPath(r.Source)}\" {r.FileKey}"));
			File.WriteAllLines(ddf, lines);

			var output = new StringBuilder();
			var start = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "makecab.exe"), $"/F \"{ddf}\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = WorkDirectory,
			};
			using (var process = new Process { StartInfo = start })
			{
				process.OutputDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
				process.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (process.ExitCode != 0 || !File.Exists(cabinet))
				{
					Log.LogError("makecab failed to compress the files: {0}", output.ToString().Trim());
					return null;
				}
			}
			return cabinet;
		}

		static string GetFileVersion(string path)
		{
			var info = FileVersionInfo.GetVersionInfo(path);
			if (info.FileMajorPart == 0 && info.FileMinorPart == 0 && info.FileBuildPart == 0 && info.FilePrivatePart == 0)
				return null;
			return $"{info.FileMajorPart}.{info.FileMinorPart}.{info.FileBuildPart}.{info.FilePrivatePart}";
		}

		// brackets have a meaning in formatted text, so show them as they are
		static string EscapeFormatted(string text) => Regex.Replace(text, @"[\[\]]", m => m.Value == "[" ? "[\\[]" : "[\\]]");

		/// <summary>Names are given as short|long, as Windows Installer needs an 8.3 name for each.</summary>
		string FileNameColumn(string directoryKey, string name)
		{
			if (!shortNames.TryGetValue(directoryKey, out var used))
				shortNames[directoryKey] = used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (Regex.IsMatch(name, @"^[A-Za-z0-9_\-]{1,8}(\.[A-Za-z0-9_\-]{1,3})?$") && used.Add(name))
				return name;
			var extension = Regex.Replace(Path.GetExtension(name).TrimStart('.'), @"[^A-Za-z0-9_\-]", "");
			if (extension.Length > 3)
				extension = extension.Substring(0, 3);
			if (extension.Length > 0)
				extension = "." + extension.ToUpperInvariant();
			string hash;
			using (var sha = SHA1.Create())
				hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(name.ToLowerInvariant()))).Replace("-", "");
			for (int i = 1; ; i++)
			{
				var suffix = "~" + i;
				var shortName = hash.Substring(0, 8 - suffix.Length) + suffix + extension;
				if (used.Add(shortName))
					return shortName + "|" + name;
			}
		}

		/// <summary>The preferred codepage, or another with every character the package uses; null when none has them all.</summary>
		int? ChooseCodepage(int preferred, bool onlyPreferred = false)
		{
			// strings are stored in the database's codepage, so pick one that has every character the package uses
			try
			{
				var provider = Type.GetType("System.Text.CodePagesEncodingProvider, System.Text.Encoding.CodePages")?.GetProperty("Instance")?.GetValue(null) as EncodingProvider;
				if (provider != null)
					Encoding.RegisterProvider(provider);
			}
			catch
			{
			}
			var text = string.Concat(strings.Where(r => r != null));
			var candidates = onlyPreferred ? new[] { preferred } : new[] { preferred, 1252, 1250, 1251, 1253, 1254, 1255, 1256, 1257, 1258, 874, 932, 936, 949, 950 };
			foreach (var codepage in candidates)
			{
				try
				{
					var encoding = Encoding.GetEncoding(codepage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
					encoding.GetBytes(text);
					return codepage;
				}
				catch
				{
				}
			}
			return null;
		}

		class Database : IDisposable
		{
			uint handle;

			public static Database Create(string path)
			{
				Check(MsiOpenDatabaseW(path, (IntPtr)3 /* MSIDBOPEN_CREATE */, out var handle), "creating the database");
				return new Database { handle = handle };
			}

			public static Database Open(string path, bool readOnly = false)
			{
				Check(MsiOpenDatabaseW(path, (IntPtr)(readOnly ? 0 /* MSIDBOPEN_READONLY */ : 1 /* MSIDBOPEN_TRANSACT */), out var handle), "opening the database");
				return new Database { handle = handle };
			}

			/// <summary>Writes the changes that turn reference into database, which may use another codepage.</summary>
			public static void GenerateTransform(string database, string reference, string transform)
			{
				if (File.Exists(transform))
					File.Delete(transform);
				using (var changed = Open(database, true))
				using (var original = Open(reference, true))
				{
					Check(MsiDatabaseGenerateTransformW(changed.handle, original.handle, transform, 0, 0), "creating the language transform");
					// a language transform changes the codepage, which is an error unless it's allowed
					Check(MsiCreateTransformSummaryInfoW(changed.handle, original.handle, transform, 0x20 /* MSITRANSFORM_ERROR_CHANGECODEPAGE */, 0), "creating the language transform");
				}
			}

			public void SetCodepage(int codepage, string folder)
			{
				var file = "codepage.idt";
				File.WriteAllText(Path.Combine(folder, file), $"\r\n\r\n{codepage}\t_ForceCodepage\r\n");
				Check(MsiDatabaseImportW(handle, folder, file), "setting the codepage");
			}

			public void Execute(string query, params object[] values)
			{
				Check(MsiDatabaseOpenViewW(handle, query, out var view), query);
				uint record = 0;
				try
				{
					if (values.Length > 0)
					{
						record = MsiCreateRecord((uint)values.Length);
						for (uint i = 0; i < values.Length; i++)
						{
							switch (values[i])
							{
								case null:
									break;
								case string s:
									Check(MsiRecordSetStringW(record, i + 1, s), query);
									break;
								case int n:
									Check(MsiRecordSetInteger(record, i + 1, n), query);
									break;
								case StreamValue stream:
									Check(MsiRecordSetStreamW(record, i + 1, stream.Path), query);
									break;
								default:
									throw new ArgumentException($"Unsupported value {values[i]}");
							}
						}
					}
					Check(MsiViewExecute(view, record), $"{query} ({string.Join(", ", values)})");
				}
				finally
				{
					if (record != 0)
						MsiCloseHandle(record);
					MsiViewClose(view);
					MsiCloseHandle(view);
				}
			}

			public void SetSummary(Dictionary<int, object> properties)
			{
				Check(MsiGetSummaryInformationW(handle, null, (uint)properties.Count, out var summary), "opening the summary information");
				try
				{
					foreach (var property in properties)
					{
						var time = new FILETIME();
						switch (property.Value)
						{
							case short n:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 2 /* VT_I2 */, n, ref time, null), "setting the summary information");
								break;
							case int n:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 3 /* VT_I4 */, n, ref time, null), "setting the summary information");
								break;
							case DateTime date:
								var value = date.ToFileTime();
								time.Low = (uint)value;
								time.High = (uint)(value >> 32);
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 64 /* VT_FILETIME */, 0, ref time, null), "setting the summary information");
								break;
							case string s:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 30 /* VT_LPSTR */, 0, ref time, s), "setting the summary information");
								break;
						}
					}
					Check(MsiSummaryInfoPersist(summary), "saving the summary information");
				}
				finally
				{
					MsiCloseHandle(summary);
				}
			}

			public void Commit() => Check(MsiDatabaseCommit(handle), "saving the database");

			public void Dispose()
			{
				if (handle != 0)
					MsiCloseHandle(handle);
				handle = 0;
			}

			static void Check(uint result, string context)
			{
				if (result == 0)
					return;
				var message = $"error {result}";
				var record = MsiGetLastErrorRecord();
				if (record != 0)
				{
					uint size = 1024;
					var buffer = new StringBuilder((int)size);
					if (MsiFormatRecordW(0, record, buffer, ref size) == 0)
						message += ": " + buffer;
					MsiCloseHandle(record);
				}
				throw new InvalidOperationException($"Windows Installer reported {message} while {context}");
			}

			[StructLayout(LayoutKind.Sequential)]
			struct FILETIME
			{
				public uint Low;
				public uint High;
			}

			// MSIHANDLE is 32 bits even in 64 bit processes
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiOpenDatabaseW(string szDatabasePath, IntPtr szPersist, out uint phDatabase);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiDatabaseOpenViewW(uint hDatabase, string szQuery, out uint phView);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiViewExecute(uint hView, uint hRecord);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiViewClose(uint hView);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiCloseHandle(uint hAny);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiCreateRecord(uint cParams);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiRecordSetStringW(uint hRecord, uint iField, string szValue);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiRecordSetInteger(uint hRecord, uint iField, int iValue);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiRecordSetStreamW(uint hRecord, uint iField, string szFilePath);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiDatabaseCommit(uint hDatabase);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiDatabaseImportW(uint hDatabase, string szFolderPath, string szFileName);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiDatabaseGenerateTransformW(uint hDatabase, uint hDatabaseReference, string szTransformFile, int iReserved1, int iReserved2);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiCreateTransformSummaryInfoW(uint hDatabase, uint hDatabaseReference, string szTransformFile, int iErrorConditions, int iValidation);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiGetSummaryInformationW(uint hDatabase, string szDatabasePath, uint uiUpdateCount, out uint phSummaryInfo);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiSummaryInfoSetPropertyW(uint hSummaryInfo, uint uiProperty, uint uiDataType, int iValue, ref FILETIME pftValue, string szValue);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiSummaryInfoPersist(uint hSummaryInfo);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiGetLastErrorRecord();
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiFormatRecordW(uint hInstall, uint hRecord, StringBuilder szResultBuf, ref uint pcchResultBuf);
		}
	}

	/// <summary>
	/// The MSIX packaging API in appxpackaging.dll, which comes with Windows.
	/// </summary>
	static class Appx
	{
		// lets Windows install the package without a signature, e.g. Add-AppxPackage -AllowUnsigned
		public const string UnsignedPublisherOid = "OID.2.25.311729368913984317654407730594956997722=1";

		/// <summary>MSIX versions are major.minor.build.revision, each up to 65535.</summary>
		public static string PackageVersion(string version, TaskLoggingHelper log)
		{
			var match = Regex.Match(version ?? string.Empty, @"^\s*(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:\.(\d+))?");
			if (!match.Success)
			{
				log.LogWarning("'{0}' isn't a version number, so the msix uses 1.0.0.0.", version);
				return "1.0.0.0";
			}
			var parts = Enumerable.Range(1, 4).Select(i => match.Groups[i].Success ? long.Parse(match.Groups[i].Value) : 0).ToArray();
			if (parts.Any(r => r > 65535))
				log.LogError("Version {0} can't be used by an msix, which allows at most 65535.65535.65535.65535. Set WindowsPackageVersion.", version);
			return string.Join(".", parts);
		}

		public static ulong PackedVersion(string version) => version.Split('.').Aggregate(0UL, (all, part) => all << 16 | ulong.Parse(part));

		/// <summary>Quotes a distinguished name's value when it has characters that would otherwise end it.</summary>
		public static string QuoteName(string value) => Regex.IsMatch(value, "[,+=\"<>#;]") || value.Trim() != value
			? "\"" + value.Replace("\"", "\"\"") + "\""
			: value;

		public static IStream OpenRead(string path) => SHCreateStreamOnFileEx(path, StgmRead | StgmShareDenyWrite, FileAttributeNormal, false, IntPtr.Zero);

		public static IStream Create(string path) => SHCreateStreamOnFileEx(path, StgmCreate | StgmWrite | StgmShareExclusive, FileAttributeNormal, true, IntPtr.Zero);

		public static IAppxFactory CreateFactory() => (IAppxFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("5842a140-ff9f-4166-8f5c-62f5b7b0c781")));

		public static IAppxBundleFactory CreateBundleFactory() => (IAppxBundleFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("378E0446-5384-43B7-8877-E7DBDD883446")));

		public static void Release(object com)
		{
			if (com != null)
				Marshal.FinalReleaseComObject(com);
		}

		public static string Describe(Exception ex) => ex is COMException
			? $"{ex.Message} (0x{ex.HResult:X8}). Event Viewer has the details, under Applications and Services Logs > Microsoft > Windows > AppxPackagingOM."
			: ex.Message;

		const uint StgmRead = 0, StgmWrite = 1, StgmShareExclusive = 0x10, StgmShareDenyWrite = 0x20, StgmCreate = 0x1000;
		const uint FileAttributeNormal = 0x80;

		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
		static extern IStream SHCreateStreamOnFileEx(string pszFile, uint grfMode, uint dwAttributes, bool fCreate, IntPtr pstmTemplate);

		[DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
		public static extern IntPtr CreateUri(string pwzURI, uint dwFlags, UIntPtr dwReserved);

		[StructLayout(LayoutKind.Sequential)]
		public struct PackageSettings
		{
			public int ForceZip32;
			public IntPtr HashMethod;
		}

		// only the methods used are declared; the rest come after them in each interface
		[ComImport, Guid("beb94909-e451-438b-b5a7-d79e767b75d8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IAppxFactory
		{
			IAppxPackageWriter CreatePackageWriter(IStream outputStream, ref PackageSettings settings);
		}

		[ComImport, Guid("9099e33b-246f-41e4-881a-008eb613f858"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IAppxPackageWriter
		{
			void AddPayloadFile([MarshalAs(UnmanagedType.LPWStr)] string fileName, [MarshalAs(UnmanagedType.LPWStr)] string contentType, int compressionOption, IStream inputStream);
			void Close(IStream manifest);
		}

		[ComImport, Guid("BBA65864-965F-4A5F-855F-F074BDBF3A7B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IAppxBundleFactory
		{
			IAppxBundleWriter CreateBundleWriter(IStream outputStream, ulong bundleVersion);
		}

		[ComImport, Guid("EC446FE8-BFEC-4C64-AB4F-49F038F0C6D2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IAppxBundleWriter
		{
			void AddPayloadPackage([MarshalAs(UnmanagedType.LPWStr)] string fileName, IStream packageStream);
			void Close();
		}
	}

	/// <summary>
	/// Creates an MSIX package with the packaging API that comes with Windows.
	/// </summary>
	public class WriteMsixPackage : Task
	{
		/// <summary>Files to install, with TargetPath relative to the package's root.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }
		/// <summary>Path of the app's executable relative to the package's root.</summary>
		[Required]
		public string MainExecutable { get; set; }
		/// <summary>Package name that identifies the app; letters, numbers, periods and dashes.</summary>
		[Required]
		public string IdentityName { get; set; }
		[Required]
		public string DisplayName { get; set; }
		public string PublisherDisplayName { get; set; }
		public string Description { get; set; }
		[Required]
		public string Version { get; set; }
		/// <summary>x86, x64, arm64 or neutral.</summary>
		[Required]
		public string Architecture { get; set; }
		/// <summary>.ico or .png logo, used before IconFile.</summary>
		public string Image { get; set; }
		public string IconFile { get; set; }
		/// <summary>Culture the package is in, e.g. en-US.</summary>
		public string Language { get; set; }
		/// <summary>Distinguished name that must match the signing certificate's subject; read from the certificate when empty.</summary>
		public string Publisher { get; set; }
		public string CertificateFile { get; set; }
		public string CertificatePassword { get; set; }
		public string CertificateThumbprint { get; set; }
		/// <summary>Whether the package will be signed; without a publisher or certificate it then can't be created.</summary>
		public bool Signed { get; set; }
		[Required]
		public string OutputFile { get; set; }
		[Required]
		public string WorkDirectory { get; set; }

		const string LogoFile = "PackageLogo.png";

		public override bool Execute()
		{
			var version = Appx.PackageVersion(Version, Log);
			if (!Regex.IsMatch(IdentityName ?? string.Empty, @"^[A-Za-z0-9.-]{3,50}$") || IdentityName.EndsWith(".", StringComparison.Ordinal))
				Log.LogError("'{0}' can't be used as the msix's name, which must be 3 to 50 letters, numbers, periods or dashes, not ending in a period. Set WindowsPackageIdentityName.", IdentityName);
			var publisher = GetPublisher();
			if (Log.HasLoggedErrors)
				return false;

			Directory.CreateDirectory(WorkDirectory);
			var logo = Path.Combine(WorkDirectory, LogoFile);
			File.WriteAllBytes(logo, LoadLogo());
			var manifest = Path.Combine(WorkDirectory, "AppxManifest.xml");
			WriteManifest(manifest, publisher, version);

			var files = Files.Select(r => new { Source = r.GetMetadata("FullPath"), Target = r.GetMetadata("TargetPath").Replace('/', '\\') }).ToList();
			if (files.Any(r => string.Equals(r.Target, LogoFile, StringComparison.OrdinalIgnoreCase)))
				Log.LogError("The app has a file named {0}, which the msix uses for its logo.", LogoFile);
			if (Log.HasLoggedErrors)
				return false;
			files.Add(new { Source = logo, Target = LogoFile });

			Log.LogMessage("Creating msix with publisher {0}", publisher);
			Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
			IStream output = null;
			Appx.IAppxPackageWriter writer = null;
			var hashMethod = IntPtr.Zero;
			var succeeded = false;
			try
			{
				hashMethod = Appx.CreateUri("http://www.w3.org/2001/04/xmlenc#sha256", 0, UIntPtr.Zero);
				var settings = new Appx.PackageSettings { HashMethod = hashMethod };
				output = Appx.Create(OutputFile);
				writer = Appx.CreateFactory().CreatePackageWriter(output, ref settings);
				foreach (var file in files)
				{
					var input = Appx.OpenRead(file.Source);
					try
					{
						writer.AddPayloadFile(file.Target, "application/octet-stream", 1, input);
					}
					finally
					{
						Appx.Release(input);
					}
				}
				var manifestStream = Appx.OpenRead(manifest);
				try
				{
					writer.Close(manifestStream);
				}
				finally
				{
					Appx.Release(manifestStream);
				}
				succeeded = true;
			}
			catch (Exception ex)
			{
				Log.LogError("The msix couldn't be created: {0}", Appx.Describe(ex));
			}
			finally
			{
				// the file stays open until both are released
				Appx.Release(writer);
				Appx.Release(output);
				if (hashMethod != IntPtr.Zero)
					Marshal.Release(hashMethod);
				if (!succeeded && File.Exists(OutputFile))
					File.Delete(OutputFile);
			}
			return !Log.HasLoggedErrors;
		}

		string GetPublisher()
		{
			if (!string.IsNullOrWhiteSpace(Publisher))
				return Publisher.Trim();
			try
			{
				if (!string.IsNullOrEmpty(CertificateFile))
				{
					using (var certificate = new X509Certificate2(CertificateFile, CertificatePassword))
						return certificate.Subject;
				}
				if (!string.IsNullOrEmpty(CertificateThumbprint))
				{
					var thumbprint = CertificateThumbprint.Replace(" ", "");
					foreach (var location in new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine })
					{
						using (var store = new X509Store(StoreName.My, location))
						{
							store.Open(OpenFlags.ReadOnly);
							var found = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
							if (found.Count > 0)
								return found[0].Subject;
						}
					}
					Log.LogError("The certificate with thumbprint {0} isn't in your certificate store, so the msix's publisher can't be read from it.", thumbprint);
					return null;
				}
			}
			catch (CryptographicException ex)
			{
				Log.LogError("The msix's publisher couldn't be read from the certificate: {0}", ex.Message);
				return null;
			}
			if (Signed)
			{
				Log.LogError("Set WindowsPackagePublisher to the subject of the certificate WindowsSignCommand signs with, e.g. CN=My Company, O=My Company, C=US, as the msix's publisher must match it.");
				return null;
			}
			var name = string.IsNullOrWhiteSpace(PublisherDisplayName) ? DisplayName : PublisherDisplayName.Trim();
			return $"CN={Appx.QuoteName(name)}, {Appx.UnsignedPublisherOid}";
		}

		byte[] LoadLogo()
		{
			foreach (var path in new[] { Image, IconFile })
			{
				if (string.IsNullOrEmpty(path) || !File.Exists(path))
					continue;
				var png = IconImage.ToPng(path);
				if (png != null)
					return png;
				Log.LogWarning("{0} has no image the msix can use as its logo. Use a .png, or an .ico with a 32 bit or .png image.", path);
			}
			Log.LogWarning("The msix has a plain logo. Set WindowsIcon or WindowsPackageImage to the app's icon.");
			return IconImage.SolidPng(256, 0x60, 0x70, 0x80);
		}

		void WriteManifest(string path, string publisher, string version)
		{
			XNamespace ns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
			XNamespace uap = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
			XNamespace rescap = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities";
			var summary = Regex.Replace((Description ?? string.Empty).Trim(), @"(?s)[\r\n].*", "");
			var publisherDisplayName = string.IsNullOrWhiteSpace(PublisherDisplayName) ? DisplayName : PublisherDisplayName.Trim();
			var language = (Language ?? string.Empty).Split(';').Select(r => r.Trim()).FirstOrDefault(r => r.Length > 0) ?? "en-US";
			var document = new XDocument(
				new XElement(ns + "Package",
					new XAttribute(XNamespace.Xmlns + "uap", uap),
					new XAttribute(XNamespace.Xmlns + "rescap", rescap),
					new XAttribute("IgnorableNamespaces", "uap rescap"),
					new XElement(ns + "Identity",
						new XAttribute("Name", IdentityName),
						new XAttribute("Publisher", publisher),
						new XAttribute("Version", version),
						new XAttribute("ProcessorArchitecture", Architecture.ToLowerInvariant())),
					new XElement(ns + "Properties",
						new XElement(ns + "DisplayName", DisplayName),
						new XElement(ns + "PublisherDisplayName", publisherDisplayName),
						summary.Length > 0 ? new XElement(ns + "Description", summary) : null,
						new XElement(ns + "Logo", LogoFile)),
					new XElement(ns + "Dependencies",
						// Windows 10 1809, the first to install msix
						new XElement(ns + "TargetDeviceFamily",
							new XAttribute("Name", "Windows.Desktop"),
							new XAttribute("MinVersion", "10.0.17763.0"),
							new XAttribute("MaxVersionTested", "10.0.26100.0"))),
					new XElement(ns + "Resources",
						new XElement(ns + "Resource", new XAttribute("Language", language))),
					new XElement(ns + "Applications",
						new XElement(ns + "Application",
							new XAttribute("Id", "App"),
							new XAttribute("Executable", MainExecutable.Replace('/', '\\')),
							new XAttribute("EntryPoint", "Windows.FullTrustApplication"),
							new XElement(uap + "VisualElements",
								new XAttribute("DisplayName", DisplayName),
								new XAttribute("Description", summary.Length > 0 ? summary : DisplayName),
								new XAttribute("BackgroundColor", "transparent"),
								new XAttribute("Square150x150Logo", LogoFile),
								new XAttribute("Square44x44Logo", LogoFile)))),
					new XElement(ns + "Capabilities",
						new XElement(rescap + "Capability", new XAttribute("Name", "runFullTrust")))));
			document.Save(path);
		}
	}

	/// <summary>
	/// Combines MSIX packages for different architectures into one .msixbundle that installs the right one.
	/// </summary>
	public class WriteMsixBundle : Task
	{
		[Required]
		public ITaskItem[] Packages { get; set; }
		[Required]
		public string Version { get; set; }
		[Required]
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			var version = Appx.PackedVersion(Appx.PackageVersion(Version, Log));
			if (Log.HasLoggedErrors)
				return false;
			Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
			IStream output = null;
			Appx.IAppxBundleWriter writer = null;
			var succeeded = false;
			try
			{
				output = Appx.Create(OutputFile);
				writer = Appx.CreateBundleFactory().CreateBundleWriter(output, version);
				foreach (var package in Packages)
				{
					var input = Appx.OpenRead(package.GetMetadata("FullPath"));
					try
					{
						writer.AddPayloadPackage(Path.GetFileName(package.ItemSpec), input);
					}
					finally
					{
						Appx.Release(input);
					}
				}
				writer.Close();
				succeeded = true;
			}
			catch (Exception ex)
			{
				Log.LogError("The msix bundle couldn't be created: {0}", Appx.Describe(ex));
			}
			finally
			{
				Appx.Release(writer);
				Appx.Release(output);
				if (!succeeded && File.Exists(OutputFile))
					File.Delete(OutputFile);
			}
			return !Log.HasLoggedErrors;
		}
	}
}
