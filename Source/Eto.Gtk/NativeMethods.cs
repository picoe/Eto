﻿
using System;
using System.Runtime.InteropServices;
using GLib;
using Gdk;

namespace Eto.GtkSharp
{
	static class NativeMethods
	{
#if GTK2
		const string ver = "2.0";
#elif GTK3
		const string ver = "3";
#endif

		static class NMWindows
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
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, string text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_get_rgba(IntPtr chooser, out RGBA color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr g_file_new_for_path(string path);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_app_chooser_dialog_new(IntPtr parrent, int flags, IntPtr file);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_application_prefers_app_menu(IntPtr application);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_header_bar_new();

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_window_set_titlebar(IntPtr window, IntPtr widget);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_start(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_end(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_set_show_close_button(IntPtr bar, bool setting);
		}

		static class NMLinux
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
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, string text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_get_rgba(IntPtr chooser, out RGBA color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr g_file_new_for_path(string path);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_app_chooser_dialog_new(IntPtr parrent, int flags, IntPtr file);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_application_prefers_app_menu(IntPtr application);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_header_bar_new();

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_window_set_titlebar(IntPtr window, IntPtr widget);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_start(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_end(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_set_show_close_button(IntPtr bar, bool setting);
		}

		static class NMMac
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
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_entry_set_placeholder_text(IntPtr entry, string text);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_get_rgba(IntPtr chooser, out RGBA color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static string gtk_font_chooser_get_font(IntPtr fontchooser);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr g_file_new_for_path(string path);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_app_chooser_dialog_new(IntPtr parrent, int flags, IntPtr file);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_application_prefers_app_menu(IntPtr application);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_header_bar_new();

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_window_set_titlebar(IntPtr window, IntPtr widget);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_start(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_pack_end(IntPtr bar, IntPtr child);

			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_header_bar_set_show_close_button(IntPtr bar, bool setting);
		}

		public static void g_signal_stop_emission_by_name(IntPtr instance, string name)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.g_signal_stop_emission_by_name(instance, name);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.g_signal_stop_emission_by_name(instance, name);
			else
				NMWindows.g_signal_stop_emission_by_name(instance, name);
		}

		public static bool gtk_clipboard_wait_for_targets(IntPtr cp, out IntPtr atoms, out int number)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_clipboard_wait_for_targets(cp, out atoms, out number);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_clipboard_wait_for_targets(cp, out atoms, out number);
			else
				return NMWindows.gtk_clipboard_wait_for_targets(cp, out atoms, out number);
		}

		public static void gtk_entry_set_placeholder_text(IntPtr entry, string text)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_entry_set_placeholder_text(entry, text);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_entry_set_placeholder_text(entry, text);
			else
				NMWindows.gtk_entry_set_placeholder_text(entry, text);
		}

		public static IntPtr gtk_print_settings_get_page_ranges(IntPtr handle, out int num_ranges)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_print_settings_get_page_ranges(handle, out num_ranges);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_print_settings_get_page_ranges(handle, out num_ranges);
			else
				return NMWindows.gtk_print_settings_get_page_ranges(handle, out num_ranges);
		}

		public static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_color_chooser_dialog_new(title, parent);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_color_chooser_dialog_new(title, parent);
			else
				return NMWindows.gtk_color_chooser_dialog_new(title, parent);
		}

		public static void gtk_color_chooser_get_rgba(IntPtr chooser, out RGBA color)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_color_chooser_get_rgba(chooser, out color);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_color_chooser_get_rgba(chooser, out color);
			else
				NMWindows.gtk_color_chooser_get_rgba(chooser, out color);
		}

		public static void gtk_color_chooser_set_rgba(IntPtr chooser, double[] color)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_color_chooser_set_rgba(chooser, color);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_color_chooser_set_rgba(chooser, color);
			else
				NMWindows.gtk_color_chooser_set_rgba(chooser, color);
		}

		public static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_color_chooser_set_use_alpha(chooser, use_alpha);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_color_chooser_set_use_alpha(chooser, use_alpha);
			else
				NMWindows.gtk_color_chooser_set_use_alpha(chooser, use_alpha);
		}

		public static bool gtk_color_chooser_get_use_alpha(IntPtr chooser)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_color_chooser_get_use_alpha(chooser);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_color_chooser_get_use_alpha(chooser);
			else
				return NMWindows.gtk_color_chooser_get_use_alpha(chooser);
		}

		public static IntPtr gtk_font_chooser_dialog_new(string title, IntPtr parent)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_font_chooser_dialog_new(title, parent);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_font_chooser_dialog_new(title, parent);
			else
				return NMWindows.gtk_font_chooser_dialog_new(title, parent);
		}

		public static string gtk_font_chooser_get_font(IntPtr fontchooser)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_font_chooser_get_font(fontchooser);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_font_chooser_get_font(fontchooser);
			else
				return NMWindows.gtk_font_chooser_get_font(fontchooser);
		}

		public static void gtk_font_chooser_set_font(IntPtr fontchooser, string fontname)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_font_chooser_set_font(fontchooser, fontname);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_font_chooser_set_font(fontchooser, fontname);
			else
				NMWindows.gtk_font_chooser_set_font(fontchooser, fontname);
		}

		public static IntPtr g_file_new_for_path(string path)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.g_file_new_for_path(path);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.g_file_new_for_path(path);
			else
				return NMWindows.g_file_new_for_path(path);
		}

		public static IntPtr gtk_app_chooser_dialog_new(IntPtr parrent, int flags, IntPtr file)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_app_chooser_dialog_new(parrent, flags, file);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_app_chooser_dialog_new(parrent, flags, file);
			else
				return NMWindows.gtk_app_chooser_dialog_new(parrent, flags, file);
		}

		public static bool gtk_application_prefers_app_menu(IntPtr application)
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_application_prefers_app_menu(application);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_application_prefers_app_menu(application);
			else
				return NMWindows.gtk_application_prefers_app_menu(application);
		}

		public static IntPtr gtk_header_bar_new()
		{
			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_header_bar_new();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_header_bar_new();
			else
				return NMWindows.gtk_header_bar_new();
		}

		public static void gtk_window_set_titlebar(IntPtr window, IntPtr widget)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_window_set_titlebar(window, widget);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_window_set_titlebar(window, widget);
			else
				NMWindows.gtk_window_set_titlebar(window, widget);
		}

		public static void gtk_header_bar_pack_start(IntPtr bar, IntPtr child)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_header_bar_pack_start(bar, child);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_header_bar_pack_start(bar, child);
			else
				NMWindows.gtk_header_bar_pack_start(bar, child);
		}

		public static void gtk_header_bar_pack_end(IntPtr bar, IntPtr child)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_header_bar_pack_end(bar, child);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_header_bar_pack_end(bar, child);
			else
				NMWindows.gtk_header_bar_pack_end(bar, child);
		}

		public static void gtk_header_bar_set_show_close_button(IntPtr bar, bool setting)
		{
			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_header_bar_set_show_close_button(bar, setting);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_header_bar_set_show_close_button(bar, setting);
			else
				NMWindows.gtk_header_bar_set_show_close_button(bar, setting);
		}
	}
}
