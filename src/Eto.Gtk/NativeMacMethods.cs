using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Eto.GtkSharp
{
	static class NativeMacMethods
	{
		public const string CoreGraphicsLibrary = "/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/CoreGraphics.framework/CoreGraphics";
		public const string CoreTextLibrary = "/System/Library/Frameworks/ApplicationServices.framework/Frameworks/CoreText.framework/CoreText";
		public const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
		public const string SystemLibrary = "/usr/lib/libSystem.dylib";

		public struct CFRange
		{
			IntPtr loc; // defined as 'long' in native code
			IntPtr len; // defined as 'long' in native code

			public int Location
			{
				get { return loc.ToInt32(); }
			}

			public int Length
			{
				get { return len.ToInt32(); }
			}

			public long LongLocation
			{
				get { return loc.ToInt64(); }
			}

			public long LongLength
			{
				get { return len.ToInt64(); }
			}

			public CFRange(int loc, int len)
				: this((long)loc, (long)len)
			{
			}

			public CFRange(long l, long len)
			{
				this.loc = new IntPtr(l);
				this.len = new IntPtr(len);
			}

			public override string ToString()
			{
				return string.Format("CFRange [Location: {0} Length: {1}]", loc, len);
			}
		}

		[DllImport(CoreGraphicsLibrary)]
		public extern static IntPtr CGDataProviderCreateWithFilename(string filename);

		[DllImport(CoreGraphicsLibrary)]
		public extern static IntPtr CGDataProviderCreateWithData(IntPtr info, IntPtr data, IntPtr size, IntPtr releaseData);

		[DllImport(CoreGraphicsLibrary)]
		public extern static IntPtr CGFontCreateWithDataProvider(IntPtr provider);

		[DllImport(CoreTextLibrary)]
		public static extern bool CTFontManagerRegisterGraphicsFont(IntPtr cgfont, out IntPtr error);

		[DllImport(CoreGraphicsLibrary)]
		public extern static IntPtr CGFontCopyFullName(IntPtr font);

		[DllImport(CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		public extern static int CFStringGetLength(IntPtr handle);

		[DllImport(CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		public extern static IntPtr CFStringGetCharactersPtr(IntPtr handle);

		[DllImport(CoreFoundationLibrary, CharSet = CharSet.Unicode)]
		public extern static IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);

		public static string CFStringToString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;

			string str;

			int l = CFStringGetLength(handle);
			IntPtr u = CFStringGetCharactersPtr(handle);
			IntPtr buffer = IntPtr.Zero;
			if (u == IntPtr.Zero)
			{
				CFRange r = new CFRange(0, l);
				buffer = Marshal.AllocCoTaskMem(l * 2);
				CFStringGetCharacters(handle, r, buffer);
				u = buffer;
			}
			unsafe
			{
				str = new string((char*)u, 0, l);
			}

			if (buffer != IntPtr.Zero)
				Marshal.FreeCoTaskMem(buffer);

			return str;
		}

		[DllImport(SystemLibrary)]
		public static extern IntPtr dlopen(string path, int mode);

		[DllImport(SystemLibrary)]
		public static extern IntPtr dlsym(IntPtr handle, string symbol);

		static IntPtr? _CoreTextLibraryPtr;
		static IntPtr CoreTextLibraryPtr => _CoreTextLibraryPtr ?? (_CoreTextLibraryPtr = dlopen(CoreTextLibrary, 0)) ?? IntPtr.Zero;

		static IntPtr? _CTFontNameKeyFamily;
		public static IntPtr CTFontNameKeyFamily => _CTFontNameKeyFamily ?? (_CTFontNameKeyFamily = GetStringConstant(CoreTextLibraryPtr, "kCTFontFamilyNameKey")) ?? IntPtr.Zero;

		public static IntPtr GetStringConstant(IntPtr library, string name)
		{
			var indirect = dlsym(library, name);
			if (indirect == IntPtr.Zero)
				return IntPtr.Zero;
			return Marshal.ReadIntPtr(indirect);
		}


		[DllImport(CoreTextLibrary)]
		public static extern IntPtr CTFontCreateWithGraphicsFont(IntPtr cgfontRef, double size, IntPtr affine, IntPtr attrs);

		[DllImport(CoreTextLibrary)]
		public static extern IntPtr CTFontCopyName(IntPtr font, IntPtr nameKey);

		[DllImport(CoreGraphicsLibrary)]
		public extern static void CGDataProviderRelease(IntPtr handle);


		[DllImport(CoreFoundationLibrary)]
		internal extern static void CFRelease(IntPtr obj);


	}
}