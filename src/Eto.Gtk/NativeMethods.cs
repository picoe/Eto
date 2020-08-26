







using System;
using System.Runtime.InteropServices;
using System.Text;
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
			const string libgobject = "libgobject-2.0" + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;
			const string libgdk = "libgdk-" + plat + ver + ext;
			const string libpango = "libpango-" + plat + ver + ext;
			const string libwebkit = "libwebkit2gtk-4.0.so.37";


			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);



			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_new();


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_uri(IntPtr web_view, string uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_uri(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_html(IntPtr web_view, string content, string base_uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_title(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_reload(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_stop_loading(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_run_javascript(IntPtr web_view, string script, IntPtr cancellable, Delegate callback, IntPtr user_data);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_run_javascript_finish(IntPtr web_view, IntPtr result, IntPtr error);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_global_context(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_value(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr JSValueToStringCopy(IntPtr context, IntPtr value, IntPtr idk);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static int JSStringGetMaximumUTF8CStringSize(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringGetUTF8CString(IntPtr js_str_value, IntPtr str_value, int str_length);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringRelease(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_navigation_policy_decision_get_request(IntPtr decision);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_uri_request_get_uri(IntPtr request);



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
			public extern static IntPtr gtk_font_chooser_get_font(IntPtr fontchooser);


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


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_selection_data_get_uris(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_selection_data_set_uris(IntPtr raw, IntPtr[] uris);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_print_unix_dialog_get_embed_page_setup(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_print_unix_dialog_set_embed_page_setup(IntPtr raw, bool embed);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_grid_get_child_at(IntPtr raw, int left, int top);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_button_get_event_window(IntPtr button);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_major_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_minor_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_micro_version();



			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gdk_cairo_get_clip_rectangle(IntPtr context, IntPtr rect);


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_get_default_root_window();


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_pixbuf_get_from_window(IntPtr window, int x, int y, int width, int height);



			[DllImport(libpango, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool pango_font_has_char(IntPtr font, int wc);

		}


		static class NMLinux
		{
#if GTK2
			const string plat = "x11-";
#elif GTK3
			const string plat = "";
#endif
			const string ext = ".so.0";
			const string libgobject = "libgobject-2.0" + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;
			const string libgdk = "libgdk-" + plat + ver + ext;
			const string libpango = "libpango-" + plat + ver + ext;
			const string libwebkit = "libwebkit2gtk-4.0.so.37";


			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);



			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_new();


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_uri(IntPtr web_view, string uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_uri(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_html(IntPtr web_view, string content, string base_uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_title(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_reload(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_stop_loading(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_run_javascript(IntPtr web_view, string script, IntPtr cancellable, Delegate callback, IntPtr user_data);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_run_javascript_finish(IntPtr web_view, IntPtr result, IntPtr error);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_global_context(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_value(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr JSValueToStringCopy(IntPtr context, IntPtr value, IntPtr idk);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static int JSStringGetMaximumUTF8CStringSize(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringGetUTF8CString(IntPtr js_str_value, IntPtr str_value, int str_length);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringRelease(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_navigation_policy_decision_get_request(IntPtr decision);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_uri_request_get_uri(IntPtr request);



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
			public extern static IntPtr gtk_font_chooser_get_font(IntPtr fontchooser);


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


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_selection_data_get_uris(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_selection_data_set_uris(IntPtr raw, IntPtr[] uris);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_print_unix_dialog_get_embed_page_setup(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_print_unix_dialog_set_embed_page_setup(IntPtr raw, bool embed);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_grid_get_child_at(IntPtr raw, int left, int top);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_button_get_event_window(IntPtr button);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_major_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_minor_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_micro_version();



			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gdk_cairo_get_clip_rectangle(IntPtr context, IntPtr rect);


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_get_default_root_window();


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_pixbuf_get_from_window(IntPtr window, int x, int y, int width, int height);



			[DllImport(libpango, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool pango_font_has_char(IntPtr font, int wc);

		}


		static class NMMac
		{
#if GTK2
			const string plat = "quartz-";
#elif GTK3
			const string plat = "";
#endif
			const string ext = ".dylib";
			const string libgobject = "libgobject-2.0" + ext;
			const string libgtk = "libgtk-" + plat + ver + ext;
			const string libgdk = "libgdk-" + plat + ver + ext;
			const string libpango = "libpango-" + plat + ver + ext;
			const string libwebkit = "libwebkit2gtk-4.0.so.37";


			[DllImport(libgobject, CallingConvention = CallingConvention.Cdecl)]
			public extern static void g_signal_stop_emission_by_name(IntPtr instance, string name);



			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_new();


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_uri(IntPtr web_view, string uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_uri(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_load_html(IntPtr web_view, string content, string base_uri);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_get_title(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_reload(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_stop_loading(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_back(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool webkit_web_view_can_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_go_forward(IntPtr web_view);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void webkit_web_view_run_javascript(IntPtr web_view, string script, IntPtr cancellable, Delegate callback, IntPtr user_data);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_web_view_run_javascript_finish(IntPtr web_view, IntPtr result, IntPtr error);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_global_context(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_javascript_result_get_value(IntPtr js_result);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr JSValueToStringCopy(IntPtr context, IntPtr value, IntPtr idk);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static int JSStringGetMaximumUTF8CStringSize(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringGetUTF8CString(IntPtr js_str_value, IntPtr str_value, int str_length);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static void JSStringRelease(IntPtr js_str_value);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_navigation_policy_decision_get_request(IntPtr decision);


			[DllImport(libwebkit, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr webkit_uri_request_get_uri(IntPtr request);



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
			public extern static IntPtr gtk_font_chooser_get_font(IntPtr fontchooser);


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


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_selection_data_get_uris(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_selection_data_set_uris(IntPtr raw, IntPtr[] uris);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gtk_print_unix_dialog_get_embed_page_setup(IntPtr raw);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static void gtk_print_unix_dialog_set_embed_page_setup(IntPtr raw, bool embed);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_grid_get_child_at(IntPtr raw, int left, int top);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gtk_button_get_event_window(IntPtr button);


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_major_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_minor_version();


			[DllImport(libgtk, CallingConvention = CallingConvention.Cdecl)]
			public extern static uint gtk_get_micro_version();



			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool gdk_cairo_get_clip_rectangle(IntPtr context, IntPtr rect);


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_get_default_root_window();


			[DllImport(libgdk, CallingConvention = CallingConvention.Cdecl)]
			public extern static IntPtr gdk_pixbuf_get_from_window(IntPtr window, int x, int y, int width, int height);



			[DllImport(libpango, CallingConvention = CallingConvention.Cdecl)]
			public extern static bool pango_font_has_char(IntPtr font, int wc);

		}


		public static string GetString(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return "";

			int len = 0;
			while (Marshal.ReadByte(handle, len) != 0)
				len++;

			var bytes = new byte[len];
			Marshal.Copy(handle, bytes, 0, bytes.Length);
			return Encoding.UTF8.GetString(bytes);
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
				return GetString(NMLinux.gtk_font_chooser_get_font(fontchooser));
			else if (EtoEnvironment.Platform.IsMac)
				return GetString(NMMac.gtk_font_chooser_get_font(fontchooser));
			else
				return GetString(NMWindows.gtk_font_chooser_get_font(fontchooser));
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


		public static IntPtr gtk_selection_data_get_uris(IntPtr raw)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_selection_data_get_uris(raw);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_selection_data_get_uris(raw);
			else
				return NMWindows.gtk_selection_data_get_uris(raw);
		}


		public static bool gtk_selection_data_set_uris(IntPtr raw, IntPtr[] uris)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_selection_data_set_uris(raw, uris);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_selection_data_set_uris(raw, uris);
			else
				return NMWindows.gtk_selection_data_set_uris(raw, uris);
		}


		public static bool gtk_print_unix_dialog_get_embed_page_setup(IntPtr raw)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_print_unix_dialog_get_embed_page_setup(raw);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_print_unix_dialog_get_embed_page_setup(raw);
			else
				return NMWindows.gtk_print_unix_dialog_get_embed_page_setup(raw);
		}


		public static void gtk_print_unix_dialog_set_embed_page_setup(IntPtr raw, bool embed)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.gtk_print_unix_dialog_set_embed_page_setup(raw, embed);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.gtk_print_unix_dialog_set_embed_page_setup(raw, embed);
			else
				NMWindows.gtk_print_unix_dialog_set_embed_page_setup(raw, embed);
		}


		public static IntPtr gtk_grid_get_child_at(IntPtr raw, int left, int top)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_grid_get_child_at(raw, left, top);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_grid_get_child_at(raw, left, top);
			else
				return NMWindows.gtk_grid_get_child_at(raw, left, top);
		}


		public static IntPtr gtk_button_get_event_window(IntPtr button)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_button_get_event_window(button);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_button_get_event_window(button);
			else
				return NMWindows.gtk_button_get_event_window(button);
		}


		public static uint gtk_get_major_version()
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_get_major_version();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_get_major_version();
			else
				return NMWindows.gtk_get_major_version();
		}


		public static uint gtk_get_minor_version()
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_get_minor_version();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_get_minor_version();
			else
				return NMWindows.gtk_get_minor_version();
		}


		public static uint gtk_get_micro_version()
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gtk_get_micro_version();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gtk_get_micro_version();
			else
				return NMWindows.gtk_get_micro_version();
		}


		public static IntPtr webkit_web_view_new()
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_web_view_new();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_web_view_new();
			else
				return NMWindows.webkit_web_view_new();
		}


		public static void webkit_web_view_load_uri(IntPtr web_view, string uri)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_load_uri(web_view, uri);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_load_uri(web_view, uri);
			else
				NMWindows.webkit_web_view_load_uri(web_view, uri);
		}


		public static string webkit_web_view_get_uri(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return GetString(NMLinux.webkit_web_view_get_uri(web_view));
			else if (EtoEnvironment.Platform.IsMac)
				return GetString(NMMac.webkit_web_view_get_uri(web_view));
			else
				return GetString(NMWindows.webkit_web_view_get_uri(web_view));
		}


		public static void webkit_web_view_load_html(IntPtr web_view, string content, string base_uri)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_load_html(web_view, content, base_uri);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_load_html(web_view, content, base_uri);
			else
				NMWindows.webkit_web_view_load_html(web_view, content, base_uri);
		}


		public static string webkit_web_view_get_title(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return GetString(NMLinux.webkit_web_view_get_title(web_view));
			else if (EtoEnvironment.Platform.IsMac)
				return GetString(NMMac.webkit_web_view_get_title(web_view));
			else
				return GetString(NMWindows.webkit_web_view_get_title(web_view));
		}


		public static void webkit_web_view_reload(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_reload(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_reload(web_view);
			else
				NMWindows.webkit_web_view_reload(web_view);
		}


		public static void webkit_web_view_stop_loading(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_stop_loading(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_stop_loading(web_view);
			else
				NMWindows.webkit_web_view_stop_loading(web_view);
		}


		public static bool webkit_web_view_can_go_back(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_web_view_can_go_back(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_web_view_can_go_back(web_view);
			else
				return NMWindows.webkit_web_view_can_go_back(web_view);
		}


		public static void webkit_web_view_go_back(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_go_back(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_go_back(web_view);
			else
				NMWindows.webkit_web_view_go_back(web_view);
		}


		public static bool webkit_web_view_can_go_forward(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_web_view_can_go_forward(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_web_view_can_go_forward(web_view);
			else
				return NMWindows.webkit_web_view_can_go_forward(web_view);
		}


		public static void webkit_web_view_go_forward(IntPtr web_view)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_go_forward(web_view);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_go_forward(web_view);
			else
				NMWindows.webkit_web_view_go_forward(web_view);
		}


		public static void webkit_web_view_run_javascript(IntPtr web_view, string script, IntPtr cancellable, Delegate callback, IntPtr user_data)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.webkit_web_view_run_javascript(web_view, script, cancellable, callback, user_data);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.webkit_web_view_run_javascript(web_view, script, cancellable, callback, user_data);
			else
				NMWindows.webkit_web_view_run_javascript(web_view, script, cancellable, callback, user_data);
		}


		public static IntPtr webkit_web_view_run_javascript_finish(IntPtr web_view, IntPtr result, IntPtr error)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_web_view_run_javascript_finish(web_view, result, error);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_web_view_run_javascript_finish(web_view, result, error);
			else
				return NMWindows.webkit_web_view_run_javascript_finish(web_view, result, error);
		}


		public static IntPtr webkit_javascript_result_get_global_context(IntPtr js_result)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_javascript_result_get_global_context(js_result);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_javascript_result_get_global_context(js_result);
			else
				return NMWindows.webkit_javascript_result_get_global_context(js_result);
		}


		public static IntPtr webkit_javascript_result_get_value(IntPtr js_result)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_javascript_result_get_value(js_result);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_javascript_result_get_value(js_result);
			else
				return NMWindows.webkit_javascript_result_get_value(js_result);
		}


		public static IntPtr JSValueToStringCopy(IntPtr context, IntPtr value, IntPtr idk)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.JSValueToStringCopy(context, value, idk);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.JSValueToStringCopy(context, value, idk);
			else
				return NMWindows.JSValueToStringCopy(context, value, idk);
		}


		public static int JSStringGetMaximumUTF8CStringSize(IntPtr js_str_value)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.JSStringGetMaximumUTF8CStringSize(js_str_value);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.JSStringGetMaximumUTF8CStringSize(js_str_value);
			else
				return NMWindows.JSStringGetMaximumUTF8CStringSize(js_str_value);
		}


		public static void JSStringGetUTF8CString(IntPtr js_str_value, IntPtr str_value, int str_length)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.JSStringGetUTF8CString(js_str_value, str_value, str_length);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.JSStringGetUTF8CString(js_str_value, str_value, str_length);
			else
				NMWindows.JSStringGetUTF8CString(js_str_value, str_value, str_length);
		}


		public static void JSStringRelease(IntPtr js_str_value)
		{

			if (EtoEnvironment.Platform.IsLinux)
				NMLinux.JSStringRelease(js_str_value);
			else if (EtoEnvironment.Platform.IsMac)
				NMMac.JSStringRelease(js_str_value);
			else
				NMWindows.JSStringRelease(js_str_value);
		}


		public static IntPtr webkit_navigation_policy_decision_get_request(IntPtr decision)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.webkit_navigation_policy_decision_get_request(decision);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.webkit_navigation_policy_decision_get_request(decision);
			else
				return NMWindows.webkit_navigation_policy_decision_get_request(decision);
		}


		public static string webkit_uri_request_get_uri(IntPtr request)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return GetString(NMLinux.webkit_uri_request_get_uri(request));
			else if (EtoEnvironment.Platform.IsMac)
				return GetString(NMMac.webkit_uri_request_get_uri(request));
			else
				return GetString(NMWindows.webkit_uri_request_get_uri(request));
		}


		public static bool gdk_cairo_get_clip_rectangle(IntPtr context, IntPtr rect)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gdk_cairo_get_clip_rectangle(context, rect);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gdk_cairo_get_clip_rectangle(context, rect);
			else
				return NMWindows.gdk_cairo_get_clip_rectangle(context, rect);
		}


		public static IntPtr gdk_get_default_root_window()
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gdk_get_default_root_window();
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gdk_get_default_root_window();
			else
				return NMWindows.gdk_get_default_root_window();
		}


		public static IntPtr gdk_pixbuf_get_from_window(IntPtr window, int x, int y, int width, int height)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.gdk_pixbuf_get_from_window(window, x, y, width, height);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.gdk_pixbuf_get_from_window(window, x, y, width, height);
			else
				return NMWindows.gdk_pixbuf_get_from_window(window, x, y, width, height);
		}


		public static bool pango_font_has_char(IntPtr font, int wc)
		{

			if (EtoEnvironment.Platform.IsLinux)
				return NMLinux.pango_font_has_char(font, wc);
			else if (EtoEnvironment.Platform.IsMac)
				return NMMac.pango_font_has_char(font, wc);
			else
				return NMWindows.pango_font_has_char(font, wc);
		}

	}
}
