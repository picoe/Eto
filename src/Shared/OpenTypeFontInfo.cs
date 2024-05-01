namespace Eto.Shared.Drawing
{
	/// <summary>
	/// Gets the opentype typographic font name/sub family info from file, since there's no built-in APIs 
	/// to do that in win32 or gdi.
	/// Reference: https://docs.microsoft.com/en-ca/typography/opentype/spec/otff
	/// </summary>
	class OpenTypeFontInfo
	{
		public string FamilyName { get; private set; }
		public string SubFamilyName { get; private set; }
		public string TypographicFamilyName { get; private set; }
		public string TypographicSubFamilyName { get; private set; }
		public string[] VariationSubFamilyNames { get; private set; }
		// public string[] VariationPostscriptNames { get; private set; }

		public static IEnumerable<OpenTypeFontInfo> FromFile(string fontFilePath)
		{
			using (var stream = File.OpenRead(fontFilePath))
			{
				// enumerate everything so the file isn't closed while we are reading.
				return FromStream(stream).ToList();
			}
		}

		public static IEnumerable<OpenTypeFontInfo> FromStream(Stream stream)
		{
			if (OTTTCHeader.TryRead(stream, out var ttcHeader))
			{
				foreach (var tableDirectory in ttcHeader.ReadTableDirectories(stream))
				{
					yield return ReadFont(stream);
				}
			}
			else
				yield return ReadFont(stream);
		}

		static OpenTypeFontInfo ReadFont(Stream stream)
		{
			// https://docs.microsoft.com/en-ca/typography/opentype/spec/otff#table-directory
			var offsetTable = new OTTableDirectory(stream);

			// validate the versions we know about in the spec
			if (!(offsetTable.sfntVersion == 0x00010000
				|| offsetTable.sfntVersion == 0x4F54544F
				))
				return null;

			// find the name table
			var tableLookup = offsetTable.ReadRecords(stream).ToDictionary(r => r.TagName);

			// no name table.. bad font?
			if (!tableLookup.TryGetValue("name", out var nameTableRecord))
				return null;

			var info = new OpenTypeFontInfo();

			// get all the name table records to extract the font family name
			// https://docs.microsoft.com/en-ca/typography/opentype/spec/name
			var nameTableHeader = new OTNamingTableHeader(nameTableRecord, stream);
			var nameRecords = nameTableHeader.ReadRecords(stream)
				.Where(r => r.platformID == 0 || r.platformID == 3)
				.ToLookup(r => r.nameID);
				
			string GetNameValue(UInt16 nameID) => nameRecords[nameID].FirstOrDefault()?.GetString(stream);

			// https://docs.microsoft.com/en-ca/typography/opentype/spec/name#name-ids
			info.FamilyName = GetNameValue(1);
			info.SubFamilyName = GetNameValue(2);
			info.TypographicFamilyName = GetNameValue(16);
			info.TypographicSubFamilyName = GetNameValue(17);

			if (tableLookup.TryGetValue("fvar", out var fontVariationsTable))
			{
				// variable font, so get the variations
				var variableHeader = new OTFontVariationsHeader(fontVariationsTable, stream);
				var variations = variableHeader.ReadInstanceRecords(stream);
				info.VariationSubFamilyNames = variations.Select(r => GetNameValue(r.subfamilyNameID)).ToArray();
				// info.VariationPostscriptNames = variations.Where(r => r.postScriptNameID != 0xFFFF).Select(r => GetNameValue(r.postScriptNameID)).ToArray();
			}

			return info;
		}

		static byte[] bufferUInt16 = new byte[2];

		static UInt16 ReadUInt16(Stream stream)
		{
			stream.Read(bufferUInt16, 0, bufferUInt16.Length);
			Array.Reverse(bufferUInt16);
			return BitConverter.ToUInt16(bufferUInt16, 0);
		}

		static byte[] bufferUInt32 = new byte[4];
		static unsafe UInt32 ReadUInt32(Stream stream)
		{
			stream.Read(bufferUInt32, 0, bufferUInt32.Length);
			Array.Reverse(bufferUInt32);
			return BitConverter.ToUInt32(bufferUInt32, 0);
		}

		class OTTTCHeader
		{
			public byte[] ttcTag;
			public UInt16 majorVersion;
			public UInt16 minorVersion;
			public UInt32 numFonts;

			static byte[] ttcTagIdentifier = new[] { (byte)'t', (byte)'t', (byte)'c', (byte)'f' };

			public static bool TryRead(Stream stream, out OTTTCHeader header)
			{
				var pos = stream.Position;
				var ttcTag = new byte[4];
				stream.Read(ttcTag, 0, ttcTag.Length);
				if (!ttcTag.SequenceEqual(ttcTagIdentifier))
				{
					stream.Position = pos;
					header = null;
					return false;
				}

				header = new OTTTCHeader();
				header.ttcTag = ttcTag;
				header.majorVersion = ReadUInt16(stream);
				header.minorVersion = ReadUInt16(stream);
				header.numFonts = ReadUInt32(stream);
				return true;
			}

			public IEnumerable<OTTableDirectory> ReadTableDirectories(Stream stream)
			{
				var currentPos = stream.Position;
				for (int i = 0; i < numFonts; i++)
				{
					stream.Position = currentPos;
					var offset = ReadUInt32(stream);
					currentPos = stream.Position;
					stream.Position = offset;
					var tableDirectory = new OTTableDirectory(stream);
					yield return tableDirectory;
				}
			}
		}

		class OTTableDirectory
		{
			public UInt32 sfntVersion;
			public UInt16 numTables;
			public UInt16 searchRange;
			public UInt16 entrySelector;
			public UInt16 rangeShift;

			public OTTableDirectory(Stream stream)
			{
				sfntVersion = ReadUInt32(stream);
				numTables = ReadUInt16(stream);
				searchRange = ReadUInt16(stream);
				entrySelector = ReadUInt16(stream);
				rangeShift = ReadUInt16(stream);
			}

			public IEnumerable<OTTableRecord> ReadRecords(Stream stream)
			{
				var currentPos = stream.Position;
				for (int i = 0; i <= numTables; i++)
				{
					stream.Position = currentPos;
					var tableRecord = new OTTableRecord(stream);
					// remember position for next record so we can do other things
					currentPos = stream.Position;
					yield return tableRecord;
				}
			}
		}
		class OTTableRecord
		{
			public byte[] tableTag = new byte[4];
			public UInt32 checksum;
			public UInt32 offset;
			public UInt32 length;

			string _tagName;

			public string TagName => _tagName ?? (_tagName = Encoding.UTF8.GetString(tableTag));

			public OTTableRecord(Stream stream)
			{
				stream.Read(tableTag, 0, tableTag.Length);
				checksum = ReadUInt32(stream);
				offset = ReadUInt32(stream);
				length = ReadUInt32(stream);

			}
		}
		class OTNamingTableHeader
		{
			public UInt16 version;
			public UInt16 count;
			public UInt16 storageOffset;
			internal long absoluteOffset;

			internal OTNamingTableHeader(OTTableRecord nameTableRecord, Stream stream)
			{
				stream.Position = nameTableRecord.offset;
				version = ReadUInt16(stream);
				count = ReadUInt16(stream);
				storageOffset = ReadUInt16(stream);
				absoluteOffset = nameTableRecord.offset + storageOffset;
			}

			internal IEnumerable<OTNameRecord> ReadRecords(Stream stream)
			{
				var currentPos = stream.Position;
				for (int i = 0; i < count; i++)
				{
					stream.Position = currentPos;
					var nameRecord = new OTNameRecord(this, stream);
					// remember position for next record so we can do other things
					currentPos = stream.Position;
					yield return nameRecord;
				}
			}
		}

		class OTNameRecord
		{
			public UInt16 platformID;
			public UInt16 encodingID;
			public UInt16 languageID;
			public UInt16 nameID;
			public UInt16 length;
			public UInt16 stringOffset;
			long absoluteOffset;

			public OTNameRecord(OTNamingTableHeader nameTableHeader, Stream stream)
			{
				platformID = ReadUInt16(stream);
				encodingID = ReadUInt16(stream);
				languageID = ReadUInt16(stream);
				nameID = ReadUInt16(stream);
				length = ReadUInt16(stream);
				stringOffset = ReadUInt16(stream);

				absoluteOffset = nameTableHeader.absoluteOffset + stringOffset;
			}

			public string GetString(Stream stream)
			{
				stream.Position = absoluteOffset;

				var stringBuffer = new byte[length];
				stream.Read(stringBuffer, 0, length);

				var isBigEndian = platformID == 3 || (platformID == 0 && encodingID == 1 || encodingID == 3);
				var encoding = isBigEndian ? Encoding.BigEndianUnicode : Encoding.UTF8;

				return encoding.GetString(stringBuffer);
			}
		}

		class OTFontVariationsHeader
		{
			public UInt16 majorVersion;
			public UInt16 minorVersion;
			public UInt16 axesArrayOffset;
			public UInt16 reserved;
			public UInt16 axisCount;
			public UInt16 axisSize;
			public UInt16 instanceCount;
			public UInt16 instanceSize;

			long _recordsOffset;

			public OTFontVariationsHeader(OTTableRecord tableRecord, Stream stream)
			{
				stream.Position = tableRecord.offset;
				majorVersion = ReadUInt16(stream);
				minorVersion = ReadUInt16(stream);
				axesArrayOffset = ReadUInt16(stream);
				reserved = ReadUInt16(stream);
				axisCount = ReadUInt16(stream);
				axisSize = ReadUInt16(stream);
				instanceCount = ReadUInt16(stream);
				instanceSize = ReadUInt16(stream);
				_recordsOffset = stream.Position;
			}

			public IEnumerable<OTFontVariationsInstanceRecord> ReadInstanceRecords(Stream stream)
			{
				var pos = _recordsOffset + axisCount * axisSize;
				bool includePostscriptName = instanceSize >= axisCount * 32 + 6;

				for (int i = 0; i < instanceCount; i++)
				{
					stream.Position = pos + i * instanceSize;
					yield return new OTFontVariationsInstanceRecord(stream, axisCount, includePostscriptName);
				}
			}
		}

		class OTFontVariationsInstanceRecord
		{
			public UInt16 subfamilyNameID;
			public UInt16 flags;
			// public UInt32[] coordinates; // Fixed point 16.16
			public UInt16 postScriptNameID;
			
			public OTFontVariationsInstanceRecord(Stream stream, int axisCount, bool includePostscriptName)
			{
				subfamilyNameID = ReadUInt16(stream);
				flags = ReadUInt16(stream);

				// don't need to actually read the coordinates for now
				// coordinates = new UInt32[axisCount];
				// for (int i = 0; i < axisCount; i++)
				// {
				// 	coordinates[i] = ReadUInt32(stream);
				// }
				if (includePostscriptName)
				{
					stream.Position += 32 * axisCount;
					postScriptNameID = ReadUInt16(stream);
				}
				else
					postScriptNameID = 0xFFFF;
			}
		}
	}


}
