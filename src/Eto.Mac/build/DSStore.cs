using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Eto.Mac.Build
{
	/// <summary>
	/// Writes the .DS_Store at the root of a mounted dmg volume. That single file is all Finder uses to
	/// decide how to present the disk image, so writing it directly lets us lay out the dmg without
	/// scripting Finder (which needs a GUI session and is prone to failing intermittently).
	/// </summary>
	/// <remarks>
	/// The file is a "Bud1" buddy-allocator container holding a B-tree of records keyed by file name.
	/// We only ever need a handful of records in a single leaf node:
	///	  "."	bwsp  window size and chrome, as a binary plist
	///	  "."	icvl  view style ("icnv" = icon view)
	///	  "."	icvp  icon size, arrangement and background picture, as a binary plist
	///	  name	Iloc  icon position of each item in the window
	/// The background picture is referenced by an alias record, which is the fiddliest part - see
	/// <see cref="BuildAlias"/>.
	/// </remarks>
	public class WriteDsStore : Task
	{
		/// <summary>Mount point of the dmg volume to write the .DS_Store into.</summary>
		[Required]
		public string VolumePath { get; set; }

		/// <summary>Name of the volume, as it appears under /Volumes.</summary>
		[Required]
		public string VolumeName { get; set; }

		/// <summary>File name of the app bundle on the volume, including the .app extension.</summary>
		[Required]
		public string AppName { get; set; }

		/// <summary>Path of the background image relative to the volume root, if any.</summary>
		public string BackgroundImage { get; set; }

		/// <summary>Name of the shortcut to /Applications, or empty to omit it.</summary>
		public string ApplicationsName { get; set; }

		/// <summary>Bounds of the Finder window as "left, top, right, bottom".</summary>
		public string WindowBounds { get; set; }

		/// <summary>Position of the app icon as "x, y".</summary>
		public string AppLocation { get; set; }

		/// <summary>Position of the Applications shortcut as "x, y".</summary>
		public string ApplicationsLocation { get; set; }

		public int IconSize { get; set; }

		public override bool Execute()
		{
			try
			{
				var records = new List<Record>();
				AddWindowRecords(records);
				AddIconLocation(records, AppName, AppLocation);
				if (!string.IsNullOrEmpty(ApplicationsName))
					AddIconLocation(records, ApplicationsName, ApplicationsLocation);

				var path = Path.Combine(VolumePath, ".DS_Store");
				File.WriteAllBytes(path, BuildStore(records));
				Log.LogMessage(MessageImportance.Normal, "Wrote {0} ({1} records)", path, records.Count);
				return true;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				return false;
			}
		}

		void AddWindowRecords(List<Record> records)
		{
			int left, top, right, bottom;
			ParseInts(WindowBounds, 4, new[] { 400, 100, 900, 430 }, out left, out top, out right, out bottom);

			var bwsp = new PlistDict();
			bwsp["WindowBounds"] = string.Format("{{{{{0}, {1}}}, {{{2}, {3}}}}}", left, top, right - left, bottom - top);
			bwsp["ShowStatusBar"] = false;
			bwsp["ShowToolbar"] = false;
			bwsp["ShowPathbar"] = false;
			bwsp["ShowSidebar"] = false;
			records.Add(Record.Blob(".", "bwsp", Plist.Write(bwsp)));

			records.Add(Record.FourCharCode(".", "icvl", "icnv"));

			var icvp = new PlistDict();
			icvp["viewOptionsVersion"] = 1;
			icvp["iconSize"] = (double)(IconSize > 0 ? IconSize : 72);
			icvp["arrangeBy"] = "none";
			icvp["gridSpacing"] = 100.0;
			icvp["gridOffsetX"] = 0.0;
			icvp["gridOffsetY"] = 0.0;
			icvp["textSize"] = 16.0;
			icvp["labelOnBottom"] = true;
			icvp["showItemInfo"] = false;
			icvp["showIconPreview"] = false;
			icvp["scrollPositionX"] = 0.0;
			icvp["scrollPositionY"] = 0.0;

			var alias = BuildBackgroundAlias();
			if (alias != null)
			{
				// Finder ignores the picture altogether unless the background colour is here as well, and it then
				// paints that colour rather than its own, so the window can't follow the light/dark appearance.
				icvp["backgroundType"] = 2;
				icvp["backgroundImageAlias"] = alias;
				icvp["backgroundColorRed"] = 1.0;
				icvp["backgroundColorGreen"] = 1.0;
				icvp["backgroundColorBlue"] = 1.0;
			}

			records.Add(Record.Blob(".", "icvp", Plist.Write(icvp)));
			records.Add(Record.Long(".", "vSrn", 1));
		}

		void AddIconLocation(List<Record> records, string name, string location)
		{
			int x, y;
			ParseInts(location, 2, new[] { 120, 150 }, out x, out y);
			var blob = new byte[16];
			Buddy.WriteUInt32(blob, 0, (uint)x);
			Buddy.WriteUInt32(blob, 4, (uint)y);
			for (int i = 8; i < 14; i++)
				blob[i] = 0xff;
			records.Add(Record.Blob(name, "Iloc", blob));
		}

		byte[] BuildBackgroundAlias()
		{
			if (string.IsNullOrEmpty(BackgroundImage))
				return null;
			var relative = BackgroundImage.Replace('\\', '/').TrimStart('/');
			var full = Path.Combine(VolumePath, relative.Replace('/', Path.DirectorySeparatorChar));
			if (!File.Exists(full))
			{
				Log.LogWarning("Background image '{0}' not found on the volume, omitting it", full);
				return null;
			}
			return BuildAlias(VolumeName, new DirectoryInfo(VolumePath).CreationTimeUtc, relative, new FileInfo(full).CreationTimeUtc);
		}

		/// <summary>
		/// Builds a version 2 alias record pointing at a file on the mounted volume. Finder resolves the
		/// background picture through this. The catalogue node ids are left zero - they would only serve as a
		/// fast path, and resolution falls back to the paths in tags 18/19 which are what actually matter here.
		/// </summary>
		static byte[] BuildAlias(string volumeName, DateTime volumeCreated, string relativePath, DateTime fileCreated)
		{
			var parts = relativePath.Split('/');
			var fileName = parts[parts.Length - 1];
			var parentName = parts.Length > 1 ? parts[parts.Length - 2] : volumeName;

			var body = new MemoryStream();
			var w = new BigEndianWriter(body);
			w.UInt32(0);								  // application info
			w.UInt16(0);								  // record size, filled in below
			w.UInt16(2);								  // version
			w.UInt16(0);								  // kind: file
			w.PascalString(volumeName, 27);
			w.UInt32(ToHfsDate(volumeCreated));
			w.UInt16(0x482b);							  // filesystem signature, "H+"
			w.UInt16(0);								  // disk type: fixed
			w.UInt32(0);								  // parent directory id
			w.PascalString(fileName, 63);
			w.UInt32(0);								  // file id
			w.UInt32(ToHfsDate(fileCreated));
			w.UInt32(0);								  // file type
			w.UInt32(0);								  // file creator
			w.UInt16(0xffff);							  // levels from
			w.UInt16(0xffff);							  // levels to
			w.UInt32(0);								  // volume attributes
			w.UInt16(0);								  // volume filesystem id
			w.Bytes(new byte[10]);						  // reserved

			w.Tag(0, Encoding.UTF8.GetBytes(parentName));
			w.Tag(1, BigEndianWriter.UInt32Bytes(0));
			w.Tag(2, Encoding.UTF8.GetBytes(volumeName + ":" + relativePath.Replace('/', ':')));
			w.Tag(14, UnicodeName(fileName));
			w.Tag(15, UnicodeName(volumeName));
			w.Tag(16, HighResDate(volumeCreated));
			w.Tag(17, HighResDate(fileCreated));
			w.Tag(18, Encoding.UTF8.GetBytes("/" + relativePath));
			w.Tag(19, Encoding.UTF8.GetBytes("/Volumes/" + volumeName));
			w.UInt16(0xffff);							  // end of tagged data
			w.UInt16(0);

			var bytes = body.ToArray();
			Buddy.WriteUInt16(bytes, 4, (ushort)bytes.Length);
			return bytes;
		}

		static byte[] UnicodeName(string value)
		{
			var chars = Encoding.BigEndianUnicode.GetBytes(value);
			var result = new byte[chars.Length + 2];
			Buddy.WriteUInt16(result, 0, (ushort)value.Length);
			Buffer.BlockCopy(chars, 0, result, 2, chars.Length);
			return result;
		}

		/// <summary>Seconds since 1904 in the upper 48 bits, fraction in the lower 16.</summary>
		static byte[] HighResDate(DateTime value)
		{
			var result = new byte[8];
			var ticks = (ulong)ToHfsDate(value) << 16;
			for (int i = 0; i < 8; i++)
				result[i] = (byte)(ticks >> ((7 - i) * 8));
			return result;
		}

		static readonly DateTime HfsEpoch = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		static uint ToHfsDate(DateTime value)
		{
			var seconds = (value.ToUniversalTime() - HfsEpoch).TotalSeconds;
			return seconds < 0 || seconds > uint.MaxValue ? 0 : (uint)seconds;
		}

		static void ParseInts(string value, int count, int[] fallback, out int a, out int b)
		{
			var v = ParseInts(value, count, fallback);
			a = v[0];
			b = v[1];
		}

		static void ParseInts(string value, int count, int[] fallback, out int a, out int b, out int c, out int d)
		{
			var v = ParseInts(value, count, fallback);
			a = v[0];
			b = v[1];
			c = v[2];
			d = v[3];
		}

		static int[] ParseInts(string value, int count, int[] fallback)
		{
			if (!string.IsNullOrEmpty(value))
			{
				var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == count)
				{
					var result = new int[count];
					for (int i = 0; i < count; i++)
					{
						if (!int.TryParse(parts[i].Trim(), out result[i]))
							return fallback;
					}
					return result;
				}
			}
			return fallback;
		}

		static byte[] BuildStore(List<Record> records)
		{
			records.Sort(Record.Compare);

			var nodeStream = new MemoryStream();
			var node = new BigEndianWriter(nodeStream);
			node.UInt32(0);								  // no child nodes, this is the only (leaf) node
			node.UInt32((uint)records.Count);
			foreach (var record in records)
				record.Write(node);
			var nodeData = nodeStream.ToArray();

			const int RootOffset = 0x800, RootSize = 0x800, MasterOffset = 0x20, MasterSize = 0x20;
			var nodeSize = Math.Max(0x1000, Buddy.RoundUpToPowerOfTwo(nodeData.Length));
			var nodeOffset = Math.Max(0x2000, nodeSize);

			var file = new byte[4 + nodeOffset + nodeSize];
			var header = new BigEndianWriter(new MemoryStream(file, 0, 32));
			header.UInt32(1);
			header.Bytes(Encoding.ASCII.GetBytes("Bud1"));
			header.UInt32(RootOffset);
			header.UInt32(RootSize);
			header.UInt32(RootOffset);

			// block addresses pack the offset and the log2 of the block size into one word
			var addresses = new[]
			{
				(uint)(RootOffset | 11),
				(uint)(MasterOffset | 5),
				(uint)(nodeOffset | Buddy.Log2(nodeSize))
			};

			var root = new BigEndianWriter(new MemoryStream(file, 4 + RootOffset, RootSize));
			root.UInt32((uint)addresses.Length);
			root.UInt32(0);
			foreach (var address in addresses)
				root.UInt32(address);
			root.Bytes(new byte[(256 - addresses.Length) * 4]);
			root.UInt32(1);								  // one directory, the store itself
			root.PascalString("DSDB", 4);
			root.UInt32(1);								  // held in block 1
			root.Bytes(new byte[32 * 4]);				  // free lists, empty - nothing ever allocates from this

			var master = new BigEndianWriter(new MemoryStream(file, 4 + MasterOffset, MasterSize));
			master.UInt32(2);							  // root node is block 2
			master.UInt32(0);							  // no internal levels
			master.UInt32((uint)records.Count);
			master.UInt32(1);							  // one node
			master.UInt32(0x1000);

			Buffer.BlockCopy(nodeData, 0, file, 4 + nodeOffset, nodeData.Length);
			return file;
		}

		class Record
		{
			public string Name;
			public string Id;
			public string Type;
			public byte[] Data;

			public static Record Blob(string name, string id, byte[] data)
			{
				var payload = new byte[data.Length + 4];
				Buddy.WriteUInt32(payload, 0, (uint)data.Length);
				Buffer.BlockCopy(data, 0, payload, 4, data.Length);
				return new Record { Name = name, Id = id, Type = "blob", Data = payload };
			}

			public static Record FourCharCode(string name, string id, string code)
			{
				return new Record { Name = name, Id = id, Type = "type", Data = Encoding.ASCII.GetBytes(code) };
			}

			public static Record Long(string name, string id, uint value)
			{
				return new Record { Name = name, Id = id, Type = "long", Data = BigEndianWriter.UInt32Bytes(value) };
			}

			public void Write(BigEndianWriter w)
			{
				w.UInt32((uint)Name.Length);
				w.Bytes(Encoding.BigEndianUnicode.GetBytes(Name));
				w.Bytes(Encoding.ASCII.GetBytes(Id));
				w.Bytes(Encoding.ASCII.GetBytes(Type));
				w.Bytes(Data);
			}

			public static int Compare(Record x, Record y)
			{
				var result = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
				return result != 0 ? result : string.CompareOrdinal(x.Id, y.Id);
			}
		}
	}

	/// <summary>Helpers for the block addresses in the buddy allocator, which pack an offset and a size.</summary>
	static class Buddy
	{
		public static void WriteUInt32(byte[] buffer, int offset, uint value)
		{
			buffer[offset] = (byte)(value >> 24);
			buffer[offset + 1] = (byte)(value >> 16);
			buffer[offset + 2] = (byte)(value >> 8);
			buffer[offset + 3] = (byte)value;
		}

		public static void WriteUInt16(byte[] buffer, int offset, ushort value)
		{
			buffer[offset] = (byte)(value >> 8);
			buffer[offset + 1] = (byte)value;
		}

		public static int RoundUpToPowerOfTwo(int value)
		{
			var result = 1;
			while (result < value)
				result <<= 1;
			return result;
		}

		public static int Log2(int value)
		{
			var result = 0;
			while ((1 << result) < value)
				result++;
			return result;
		}
	}

	class BigEndianWriter
	{
		readonly Stream stream;

		public BigEndianWriter(Stream stream)
		{
			this.stream = stream;
		}

		public void Bytes(byte[] value)
		{
			stream.Write(value, 0, value.Length);
		}

		public void UInt32(uint value)
		{
			Bytes(UInt32Bytes(value));
		}

		public void UInt16(ushort value)
		{
			var buffer = new byte[2];
			Buddy.WriteUInt16(buffer, 0, value);
			Bytes(buffer);
		}

		public static byte[] UInt32Bytes(uint value)
		{
			var buffer = new byte[4];
			Buddy.WriteUInt32(buffer, 0, value);
			return buffer;
		}

		/// <summary>Writes a length-prefixed string padded out to a fixed field width.</summary>
		public void PascalString(string value, int size)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			if (bytes.Length > size)
				Array.Resize(ref bytes, size);
			var buffer = new byte[size + 1];
			buffer[0] = (byte)bytes.Length;
			Buffer.BlockCopy(bytes, 0, buffer, 1, bytes.Length);
			Bytes(buffer);
		}

		/// <summary>Writes one entry of an alias record's tagged variable-length section.</summary>
		public void Tag(ushort tag, byte[] data)
		{
			UInt16(tag);
			UInt16((ushort)data.Length);
			Bytes(data);
			if (data.Length % 2 != 0)
				stream.WriteByte(0);
		}
	}

	/// <summary>A flat property list. Finder's window settings never nest, so nesting isn't supported.</summary>
	class PlistDict
	{
		readonly Dictionary<string, object> values = new Dictionary<string, object>();

		public object this[string key]
		{
			get { return values[key]; }
			set { values[key] = value; }
		}

		public IEnumerable<string> Keys
		{
			get { return values.Keys.OrderBy(k => k, StringComparer.Ordinal); }
		}
	}

	/// <summary>Writes the small subset of the binary plist format that the Finder records need.</summary>
	static class Plist
	{
		public static byte[] Write(PlistDict dict)
		{
			var objects = new List<object> { dict };
			var interned = new Dictionary<string, int>();
			var keys = dict.Keys.ToList();
			var refs = new List<int>();
			foreach (var key in keys)
				refs.Add(Intern(objects, interned, key));
			foreach (var key in keys)
				refs.Add(Intern(objects, interned, dict[key]));

			var refSize = objects.Count <= byte.MaxValue ? 1 : 2;
			var stream = new MemoryStream();
			stream.Write(Encoding.ASCII.GetBytes("bplist00"), 0, 8);

			var offsets = new int[objects.Count];
			for (int i = 0; i < objects.Count; i++)
			{
				offsets[i] = (int)stream.Position;
				if (i == 0)
				{
					WriteMarker(stream, 0xd0, keys.Count);
					foreach (var value in refs)
						WriteSized(stream, value, refSize);
				}
				else
					WriteValue(stream, objects[i]);
			}

			var tableOffset = (int)stream.Position;
			var offsetSize = tableOffset <= byte.MaxValue ? 1 : tableOffset <= ushort.MaxValue ? 2 : 4;
			foreach (var offset in offsets)
				WriteSized(stream, offset, offsetSize);

			stream.Write(new byte[6], 0, 6);
			stream.WriteByte((byte)offsetSize);
			stream.WriteByte((byte)refSize);
			WriteSized(stream, objects.Count, 8);
			WriteSized(stream, 0, 8);
			WriteSized(stream, tableOffset, 8);
			return stream.ToArray();
		}

		static int Intern(List<object> objects, Dictionary<string, int> interned, object value)
		{
			var key = value is byte[] ? "d" + objects.Count : value.GetType().Name + ":" + Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
			int index;
			if (interned.TryGetValue(key, out index))
				return index;
			objects.Add(value);
			index = objects.Count - 1;
			interned[key] = index;
			return index;
		}

		static void WriteValue(Stream stream, object value)
		{
			if (value is bool)
				stream.WriteByte((byte)((bool)value ? 0x09 : 0x08));
			else if (value is int)
			{
				var number = (int)value;
				var size = number <= byte.MaxValue ? 1 : number <= ushort.MaxValue ? 2 : 4;
				stream.WriteByte((byte)(0x10 | Buddy.Log2(size)));
				WriteSized(stream, number, size);
			}
			else if (value is double)
			{
				stream.WriteByte(0x23);
				var bits = (ulong)BitConverter.DoubleToInt64Bits((double)value);
				for (int i = 7; i >= 0; i--)
					stream.WriteByte((byte)(bits >> (i * 8)));
			}
			else if (value is byte[])
			{
				var data = (byte[])value;
				WriteMarker(stream, 0x40, data.Length);
				stream.Write(data, 0, data.Length);
			}
			else
			{
				var text = Encoding.ASCII.GetBytes(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
				WriteMarker(stream, 0x50, text.Length);
				stream.Write(text, 0, text.Length);
			}
		}

		/// <summary>Writes a type marker, spilling the count into a following integer when it doesn't fit.</summary>
		static void WriteMarker(Stream stream, int type, int count)
		{
			if (count < 15)
				stream.WriteByte((byte)(type | count));
			else
			{
				stream.WriteByte((byte)(type | 0x0f));
				var size = count <= byte.MaxValue ? 1 : count <= ushort.MaxValue ? 2 : 4;
				stream.WriteByte((byte)(0x10 | Buddy.Log2(size)));
				WriteSized(stream, count, size);
			}
		}

		static void WriteSized(Stream stream, int value, int size)
		{
			// widened to 64 bits so the 8 byte trailer fields don't wrap - C# masks 32 bit shift counts to 5 bits
			var number = (ulong)(uint)value;
			for (int i = size - 1; i >= 0; i--)
				stream.WriteByte((byte)(number >> (i * 8)));
		}
	}
}
