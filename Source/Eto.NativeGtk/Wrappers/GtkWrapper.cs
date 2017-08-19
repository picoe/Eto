﻿using System;
using System.Runtime.InteropServices;

static partial class GtkWrapper
{
    public const string NativeLib = "libgtk-3.so.0";

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      g_object_unref (IntPtr obj);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static string[]  gtk_about_dialog_get_artists (IntPtr about);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static string[]  gtk_about_dialog_get_authors (IntPtr about);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_comments")]
    private extern static IntPtr wgtk_about_dialog_get_comments (IntPtr about);
	
	public static string gtk_about_dialog_get_comments (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_comments(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_copyright")]
    private extern static IntPtr wgtk_about_dialog_get_copyright (IntPtr about);
	
	public static string gtk_about_dialog_get_copyright (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_copyright(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static string[]  gtk_about_dialog_get_documenters (IntPtr about);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_license")]
    private extern static IntPtr wgtk_about_dialog_get_license (IntPtr about);
	
	public static string gtk_about_dialog_get_license (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_license(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_program_name")]
    private extern static IntPtr wgtk_about_dialog_get_program_name (IntPtr about);
	
	public static string gtk_about_dialog_get_program_name (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_program_name(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_version")]
    private extern static IntPtr wgtk_about_dialog_get_version (IntPtr about);
	
	public static string gtk_about_dialog_get_version (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_version(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_website")]
    private extern static IntPtr wgtk_about_dialog_get_website (IntPtr about);
	
	public static string gtk_about_dialog_get_website (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_website(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_website_label")]
    private extern static IntPtr wgtk_about_dialog_get_website_label (IntPtr about);
	
	public static string gtk_about_dialog_get_website_label (IntPtr about)
	{
		return WrapperHelper.GetString(wgtk_about_dialog_get_website_label(about));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_about_dialog_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_artists (IntPtr about, string[] artists);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_authors (IntPtr about, string[] authors);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_comments (IntPtr about, string comments);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_copyright (IntPtr about, string copyright);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_documenters (IntPtr about, string[] documenters);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_license (IntPtr about, string license);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_logo (IntPtr about, IntPtr logo);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_program_name (IntPtr about, string name);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_version (IntPtr about, string version);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_website (IntPtr about, string website);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_website_label (IntPtr about, string website_label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_about_dialog_set_wrap_license (IntPtr about, bool wrap_license);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_accel_label_new (string text);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_button_get_image_position (IntPtr button);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_get_label")]
    private extern static IntPtr wgtk_button_get_label (IntPtr button);
	
	public static string gtk_button_get_label (IntPtr button)
	{
		return WrapperHelper.GetString(wgtk_button_get_label(button));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_button_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_button_set_always_show_image (IntPtr button, bool always_show);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_button_set_image (IntPtr button, IntPtr image);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_button_set_image_position (IntPtr button, int position);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_button_set_label (IntPtr button, string label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_check_button_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_color_button_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_color_chooser_dialog_new (string title, IntPtr parent);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_color_chooser_get_rgba (IntPtr chooser, out RGBA color);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_color_chooser_get_use_alpha (IntPtr chooser);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_color_chooser_set_rgba (IntPtr chooser, double[] color);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_color_chooser_set_use_alpha (IntPtr chooser, bool use_alpha);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_container_add (IntPtr container, IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_css_provider_load_from_data (IntPtr css_provider, string data, int length, IntPtr error);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_css_provider_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_dialog_run (IntPtr dialog);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_editable_get_editable (IntPtr entry);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_editable_get_position (IntPtr editable);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_editable_get_selection_bounds (IntPtr editable, out int start_pos, out int end_pos);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_editable_select_region (IntPtr editable, int start_pos, int end_pos);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_editable_set_editable (IntPtr entry, bool editable);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_editable_set_position (IntPtr editable, int position);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static float     gtk_entry_get_alignment (IntPtr entry);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_entry_get_has_frame (IntPtr entry);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_entry_get_max_length (IntPtr entry);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_entry_get_max_width_chars (IntPtr entry);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_placeholder_text")]
    private extern static IntPtr wgtk_entry_get_placeholder_text (IntPtr entry);
	
	public static string gtk_entry_get_placeholder_text (IntPtr entry)
	{
		return WrapperHelper.GetString(wgtk_entry_get_placeholder_text(entry));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_text")]
    private extern static IntPtr wgtk_entry_get_text (IntPtr entry);
	
	public static string gtk_entry_get_text (IntPtr entry)
	{
		return WrapperHelper.GetString(wgtk_entry_get_text(entry));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_entry_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_alignment (IntPtr entry, float xalign);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_has_frame (IntPtr entry, bool setting);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_max_length (IntPtr entry, int max);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_max_width_chars (IntPtr entry, int n_chars);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_placeholder_text (IntPtr entry, string text);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_text (IntPtr entry, string text);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_entry_set_width_chars (IntPtr entry, int n_chars);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_event_box_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_image_new ();

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_image_set_from_pixbuf (IntPtr image, IntPtr pixbuf);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_label_get_line_wrap (IntPtr label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_label_get_line_wrap_mode (IntPtr label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_text")]
    private extern static IntPtr wgtk_label_get_text (IntPtr label);
	
	public static string gtk_label_get_text (IntPtr label)
	{
		return WrapperHelper.GetString(wgtk_label_get_text(label));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static float     gtk_label_get_xalign (IntPtr label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static float     gtk_label_get_yalign (IntPtr label);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_label_new (string str);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_label_set_line_wrap (IntPtr label, bool wrap);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_label_set_line_wrap_mode (IntPtr label, int wrap_mode);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_label_set_text (IntPtr label, string str);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_label_set_xalign (IntPtr label, float xalign);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_label_set_yalign (IntPtr label, float yalign);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_style_context_add_class (IntPtr context, string class_name);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_style_context_add_provider (IntPtr context, IntPtr provider, int priority);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_style_context_get_color (IntPtr context, int state, out RGBA color);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_style_context_remove_class (IntPtr context, string class_name);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_toggle_button_get_active (IntPtr toggle_button);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static bool      gtk_toggle_button_get_inconsistent (IntPtr toggle_button);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_toggle_button_set_active (IntPtr toggle_button, bool is_active);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_toggle_button_set_inconsistent (IntPtr toggle_button, bool setting);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_get_allocation (IntPtr widget, out GtkAllocation allocation);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static int       gtk_widget_get_halign (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static IntPtr    gtk_widget_get_style_context (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_hide (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_queue_resize (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_set_font_map (IntPtr widget, IntPtr font_map);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_set_halign (IntPtr widget, int align);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_set_no_show_all (IntPtr widget, bool no_show_all);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_set_size_request (IntPtr widget, int width, int height);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_show (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_widget_show_all (IntPtr widget);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_window_get_title")]
    private extern static IntPtr wgtk_window_get_title (IntPtr window);
	
	public static string gtk_window_get_title (IntPtr window)
	{
		return WrapperHelper.GetString(wgtk_window_get_title(window));
	}

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_window_set_modal (IntPtr window, bool modal);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_window_set_title (IntPtr window, string title);

	[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl)]
    public extern static void      gtk_window_set_transient_for (IntPtr window, IntPtr parent);
}
