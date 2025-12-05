#nullable enable

namespace Eto.Mac;

static class MacVersion
{
	static int s_major;
	static int s_minor;
	static int s_patch;

	static Lazy<bool> isUsingGlass = new Lazy<bool>(() =>
	{
		// if built with or running on pre-Tahoe SDK, glass is not enabled
		if (s_major < 26) // || SdkVersion < new Version(26, 0))
			return false;

		// check if compatibility has been turned on
		var obj = NSBundle.MainBundle.ObjectForInfoDictionary("UIDesignRequiresCompatibility");
		if (obj is not NSNumber number || !number.BoolValue)
			return true;

		return false;
	});

	public static bool IsUsingGlass => isUsingGlass.Value;

	public static bool IsAtLeast(int major, int minor, int patch = 0)
	{
		// check major
		if (s_major < major)
			return false;
		if (s_major > major)
			return true;

		// major equals, check minor
		if (s_minor < minor)
			return false;
		if (s_minor > minor)
			return true;

		// minor equals, check patch
		return s_patch >= patch;
	}

	[DllImport("/System/Library/Frameworks/CoreServices.framework/CoreServices")]
	internal static extern short Gestalt(int selector, ref int response);

	static MacVersion()
	{
#if !MONOMAC
		// TODO: Can't use IsOperatingSystemAtLeastVersion in monomac yet, it's not mapped.
		if (NSProcessInfo.ProcessInfo.RespondsToSelector(new Selector("operatingSystemVersion")))
		{
			var operatingSystemVersion = NSProcessInfo.ProcessInfo.OperatingSystemVersion;
			s_major = (int)operatingSystemVersion.Major;
			s_minor = (int)operatingSystemVersion.Minor;
			s_patch = (int)operatingSystemVersion.PatchVersion;
		}
		else
#endif
		{
			const int gestaltSystemVersionMajor = 0x73797331;
			const int gestaltSystemVersionMinor = 0x73797332;
			const int gestaltSystemVersionPatch = 0x73797333;

			Gestalt(gestaltSystemVersionMajor, ref s_major);
			Gestalt(gestaltSystemVersionMinor, ref s_minor);
			Gestalt(gestaltSystemVersionPatch, ref s_patch);
		}
	}

	private const uint LC_BUILD_VERSION = 0x32;  // from <mach-o/loader.h>

	[StructLayout(LayoutKind.Sequential)]
	private struct mach_header_64
	{
		public uint magic;
		public int cputype;
		public int cpusubtype;
		public uint filetype;
		public uint ncmds;
		public uint sizeofcmds;
		public uint flags;
		public uint reserved;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct load_command
	{
		public uint cmd;
		public uint cmdsize;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct build_version_command
	{
		public uint cmd;
		public uint cmdsize;
		public uint platform;
		public uint minos;
		public uint sdk;
		public uint ntools;
		// followed by tool_build_version entries we don't care about
	}

	static Lazy<Version?> sdkVersion = new Lazy<Version?>(() => GetBuildSdkVersion(Process.GetCurrentProcess().MainModule?.FileName!));
	public static Version? SdkVersion => sdkVersion.Value;

	public static Version? GetBuildSdkVersion(string exePath)
	{
		using var stream = File.OpenRead(exePath);
		using var reader = new BinaryReader(stream);

		uint magic = reader.ReadUInt32();

		// FAT (Universal) binary — we must locate the correct slice
		const uint FAT_MAGIC = 0xCAFEBABE;
		const uint FAT_CIGAM = 0xBEBAFECA;

		if (magic == FAT_MAGIC || magic == FAT_CIGAM)
		{
			// Read FAT header
			uint nfatArch = SwapIfNeeded(reader.ReadUInt32(), magic == FAT_CIGAM);

			for (int i = 0; i < nfatArch; i++)
			{
				uint cputype = SwapIfNeeded(reader.ReadUInt32(), magic == FAT_CIGAM);
				_ = reader.ReadUInt32(); // cpusubtype
				uint offset = SwapIfNeeded(reader.ReadUInt32(), magic == FAT_CIGAM);
				_ = reader.ReadUInt32(); // size
				_ = reader.ReadUInt32(); // align

				// Choose the arm64 slice
				const uint CPU_TYPE_ARM64 = 0x0100000C;

				if (cputype == CPU_TYPE_ARM64)
				{
					return ReadMachOSdkVersion(stream, offset);
				}
			}

			return null; // no appropriate slice
		}

		// Not FAT → maybe it's directly a Mach-O header
		return ReadMachOSdkVersion(stream, 0);
	}

	private static Version? ReadMachOSdkVersion(FileStream stream, uint offset)
	{
		stream.Position = offset;
		using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

		const uint MH_MAGIC_64 = 0xFEEDFACF;

		var header = ReadStruct<mach_header_64>(reader);
		if (header.magic != MH_MAGIC_64)
			return null;

		for (int i = 0; i < header.ncmds; i++)
		{
			long cmdStart = stream.Position;
			var lc = ReadStruct<load_command>(reader);

			if (lc.cmd == LC_BUILD_VERSION)
			{
				stream.Position = cmdStart;
				var bv = ReadStruct<build_version_command>(reader);
				int major = (int)((bv.sdk >> 16) & 0xFFFF);
				int minor = (int)((bv.sdk >> 8) & 0xFF);
				int patch = (int)(bv.sdk & 0xFF);
				return new Version(major, minor, patch);
			}

			stream.Position = cmdStart + lc.cmdsize;
		}

		return null;
	}

	private static uint SwapIfNeeded(uint value, bool swap)
	{
		if (!swap) return value;
		return (value >> 24)
			 | ((value >> 8) & 0x0000FF00)
			 | ((value << 8) & 0x00FF0000)
			 | (value << 24);
	}

	private static T ReadStruct<T>(BinaryReader reader) where T : struct
	{
		int size = Marshal.SizeOf<T>();
		byte[] bytes = reader.ReadBytes(size);

		GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject())!;
		}
		finally
		{
			handle.Free();
		}
	}
}
