﻿using System;
using System.Runtime.InteropServices;
using GLib;
using System.Reflection;

namespace Eto.GtkSharp
{
	static class NativeMethods
	{
		#if GTK2
		const string ver = "2.0";
		#elif GTK3
		const string ver = "3";
		#endif

		static class NativeMethodsWindows
		{
			#if GTK2
			const string plat = "win32-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = "-0.dll";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, IntPtr text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);
#if GTK3

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gtk_color_chooser_get_rgba(IntPtr chooser, out Gdk.RGBA color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);
#endif
		}

		static class NativeMethodsLinux
		{
			#if GTK2
			const string plat = "x11-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = ".so.0";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, IntPtr text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);
#if GTK3

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gtk_color_chooser_get_rgba(IntPtr chooser, out Gdk.RGBA color);

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);
#endif
        }

		static class NativeMethodsMac
		{
			#if GTK2
			const string plat = "quartz-";
			#elif GTK3
			const string plat = "";
			#endif

			const string ext = ".dylib";
			const string libgobject = "libgobject-" + ver + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;

			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public static extern void g_signal_stop_emission_by_name(IntPtr instance, IntPtr name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, IntPtr text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);
#if GTK3

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gtk_color_chooser_get_rgba(IntPtr chooser, out Gdk.RGBA color);

            [DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
            public static extern void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public static extern void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);
#endif
        }

#pragma warning disable 0649

		static class Impl
		{
			public static readonly Action<IntPtr, IntPtr> g_signal_stop_emission_by_name;

			public delegate bool ClipboardWaitForTargetsDelegate(IntPtr cp, out IntPtr atoms, out int number);

			public static readonly ClipboardWaitForTargetsDelegate gtk_clipboard_wait_for_targets;

            public static readonly Action<IntPtr, IntPtr> gtk_entry_set_placeholder_text;

			public delegate IntPtr PrintSettingsGetPageRanges(IntPtr handle, out int num_ranges);

			public static readonly PrintSettingsGetPageRanges gtk_print_settings_get_page_ranges;
#if GTK3

            public delegate IntPtr ColorChooserNew(string title, IntPtr parent);

            public static readonly ColorChooserNew gtk_color_chooser_dialog_new;

            public delegate void ColorChooserGet(IntPtr chooser, out Gdk.RGBA color);

            public static readonly ColorChooserGet gtk_color_chooser_get_rgba;

            public delegate void ColorChooserSet(IntPtr chooser, double[] color);

            public static readonly ColorChooserSet gtk_color_chooser_set_rgba;

			public delegate bool ColorChooserGetAlpha(IntPtr chooser);

			public static readonly ColorChooserGetAlpha gtk_color_chooser_get_use_alpha;

			public delegate void ColorChooserSetAlpha(IntPtr chooser, bool use_alpha);

			public static readonly ColorChooserSetAlpha gtk_color_chooser_set_use_alpha;

            public delegate IntPtr FontChooserNew(string title, IntPtr parent);

			public static readonly FontChooserNew gtk_font_chooser_dialog_new;

			public delegate string FontChooserGetFont(IntPtr fontchooser);

			public static readonly FontChooserGetFont gtk_font_chooser_get_font;

			public delegate void FontChooserSetFont(IntPtr fontchooser, string fontname);

			public static readonly FontChooserSetFont gtk_font_chooser_set_font;
#endif
		}

#pragma warning restore 0649

		static NativeMethods()
		{
			var fields = typeof(Impl).GetFields();
			Type platformType;
			if (EtoEnvironment.Platform.IsLinux)
				platformType = typeof(NativeMethodsLinux);
			else if (EtoEnvironment.Platform.IsMac)
				platformType = typeof(NativeMethodsMac);
			else
				platformType = typeof(NativeMethodsWindows);
			
			// instead of requiring an accompanying .config file.
			foreach (var field in fields)
			{
				var method = platformType.GetMethod(field.Name);
				if (method != null)
					field.SetValue(null, Delegate.CreateDelegate(field.FieldType, method));
			}
		}

		public static void StopEmissionByName(GLib.Object o, string signal)
		{
			if (Impl.g_signal_stop_emission_by_name == null)
				return;
			IntPtr intPtr = Marshaller.StringToPtrGStrdup(signal);
			Impl.g_signal_stop_emission_by_name(o.Handle, intPtr);
			Marshaller.Free(intPtr);
		}

		public static void gtk_entry_set_placeholder_text(Gtk.Entry entry, string text)
		{
			if (Impl.gtk_entry_set_placeholder_text == null)
				return;
			IntPtr textPtr = Marshaller.StringToPtrGStrdup(text);
			Impl.gtk_entry_set_placeholder_text(entry.Handle, textPtr);
			Marshaller.Free(textPtr);
		}

		public static Gtk.PageRange[] gtk_print_settings_get_page_ranges(Gtk.PrintSettings settings)
		{
			if (Impl.gtk_entry_set_placeholder_text == null)
				return null;
			int num_ranges;
			IntPtr intPtr = Impl.gtk_print_settings_get_page_ranges(settings.Handle, out num_ranges);
			Gtk.PageRange[] array = new Gtk.PageRange[num_ranges];
			for (int i = 0; i < num_ranges; i++)
			{
				array[i] = Gtk.PageRange.New(intPtr + i * IntPtr.Size);
			}
			Marshaller.Free(intPtr);
			return array;
		}
#if GTK3

        public static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parrent)
        {
            return Impl.gtk_color_chooser_dialog_new(title, parrent);
        }

        public static Gdk.RGBA gtk_color_chooser_get_rgba(IntPtr chooser)
        {
            Gdk.RGBA ret;
            Impl.gtk_color_chooser_get_rgba(chooser, out ret);
            return ret;
        }

		public static void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color)
		{
			Impl.gtk_color_chooser_set_rgba(chooser, color);
		}

		public static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha)
		{
			Impl.gtk_color_chooser_set_use_alpha(chooser, use_alpha);
		}

		public static bool gtk_color_chooser_get_use_alpha(IntPtr chooser)
		{
			return Impl.gtk_color_chooser_get_use_alpha(chooser);
		}

		public static IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parrent)
		{
			return Impl.gtk_font_chooser_dialog_new(title, parrent);
		}

		public static string gtk_font_chooser_get_font(IntPtr fontchooser)
		{
			return Impl.gtk_font_chooser_get_font(fontchooser);
		}

		public static void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname)
		{
			Impl.gtk_font_chooser_set_font(fontchooser, fontname);
		}

#endif

		public static bool ClipboardWaitForTargets(IntPtr cp, out Gdk.Atom[] atoms)
		{
			if (Impl.gtk_clipboard_wait_for_targets == null)
			{
				atoms = null;
				return false;
			}
			IntPtr atomPtrs;
			int count;
			var success = Impl.gtk_clipboard_wait_for_targets(cp, out atomPtrs, out count);
			if (!success || count <= 0)
			{
				atoms = null;
				return false;
			}

			atoms = new Gdk.Atom[count];
			unsafe
			{
				byte* p = (byte*)atomPtrs.ToPointer();
				for (int i = 0; i < count; i++)
				{
					atoms[i] = new Gdk.Atom(new IntPtr(*p));
					p += IntPtr.Size;
				}
			}
			return true;
		}
	}

}

