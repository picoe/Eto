﻿using System;
using System.Runtime.InteropServices;
using System.Text;

static class WrapperHelper
{
    public static string GetString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            return "";

        int len = 0;
        while (Marshal.ReadByte(ptr, len) != 0)
            len++;

        var bytes = new byte[len];
        Marshal.Copy(ptr, bytes, 0, bytes.Length);

        return Encoding.UTF8.GetString(bytes);
    }
}


static partial class GtkWrapper
{
    public const string NativeLib = "libgtk-3.so.0";

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_object_unref")]
public extern static void g_object_unref(IntPtr obj);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_signal_connect_data")]
public extern static uint g_signal_connect_data(IntPtr instance, string detailed_signal, IntPtr handler, IntPtr data, IntPtr destroy_data, int connect_flags);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_signal_connect")]
public extern static void g_signal_connect(IntPtr instance, string detailed_signal, IntPtr c_handler, IntPtr data);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_artists")]
public extern static string[] gtk_about_dialog_get_artists(IntPtr about);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_authors")]
public extern static string[] gtk_about_dialog_get_authors(IntPtr about);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_comments")]
private extern static IntPtr wgtk_about_dialog_get_comments(IntPtr about);

public static string gtk_about_dialog_get_comments(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_comments(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_copyright")]
private extern static IntPtr wgtk_about_dialog_get_copyright(IntPtr about);

public static string gtk_about_dialog_get_copyright(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_copyright(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_documenters")]
public extern static string[] gtk_about_dialog_get_documenters(IntPtr about);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_license")]
private extern static IntPtr wgtk_about_dialog_get_license(IntPtr about);

public static string gtk_about_dialog_get_license(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_license(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_program_name")]
private extern static IntPtr wgtk_about_dialog_get_program_name(IntPtr about);

public static string gtk_about_dialog_get_program_name(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_program_name(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_version")]
private extern static IntPtr wgtk_about_dialog_get_version(IntPtr about);

public static string gtk_about_dialog_get_version(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_version(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_website")]
private extern static IntPtr wgtk_about_dialog_get_website(IntPtr about);

public static string gtk_about_dialog_get_website(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_website(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_get_website_label")]
private extern static IntPtr wgtk_about_dialog_get_website_label(IntPtr about);

public static string gtk_about_dialog_get_website_label(IntPtr about)
{
    var ret = WrapperHelper.GetString(wgtk_about_dialog_get_website_label(about));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_new")]
public extern static IntPtr gtk_about_dialog_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_artists")]
public extern static void gtk_about_dialog_set_artists(IntPtr about, string[] artists);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_authors")]
public extern static void gtk_about_dialog_set_authors(IntPtr about, string[] authors);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_comments")]
public extern static void gtk_about_dialog_set_comments(IntPtr about, string comments);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_copyright")]
public extern static void gtk_about_dialog_set_copyright(IntPtr about, string copyright);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_documenters")]
public extern static void gtk_about_dialog_set_documenters(IntPtr about, string[] documenters);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_license")]
public extern static void gtk_about_dialog_set_license(IntPtr about, string license);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_logo")]
public extern static void gtk_about_dialog_set_logo(IntPtr about, IntPtr logo);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_program_name")]
public extern static void gtk_about_dialog_set_program_name(IntPtr about, string name);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_version")]
public extern static void gtk_about_dialog_set_version(IntPtr about, string version);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_website")]
public extern static void gtk_about_dialog_set_website(IntPtr about, string website);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_website_label")]
public extern static void gtk_about_dialog_set_website_label(IntPtr about, string website_label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_about_dialog_set_wrap_license")]
public extern static void gtk_about_dialog_set_wrap_license(IntPtr about, bool wrap_license);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_accel_label_new")]
public extern static IntPtr gtk_accel_label_new(string text);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_app_chooser_get_app_info")]
public extern static IntPtr gtk_app_chooser_get_app_info(IntPtr self);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_app_chooser_dialog_new")]
public extern static IntPtr gtk_app_chooser_dialog_new(IntPtr parent, int flags, IntPtr file);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_app_chooser_dialog_set_heading")]
public extern static void gtk_app_chooser_dialog_set_heading(IntPtr self, string heading);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_get_image_position")]
public extern static int gtk_button_get_image_position(IntPtr button);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_get_label")]
private extern static IntPtr wgtk_button_get_label(IntPtr button);

public static string gtk_button_get_label(IntPtr button)
{
    var ret = WrapperHelper.GetString(wgtk_button_get_label(button));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_new")]
public extern static IntPtr gtk_button_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_set_always_show_image")]
public extern static void gtk_button_set_always_show_image(IntPtr button, bool always_show);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_set_image")]
public extern static void gtk_button_set_image(IntPtr button, IntPtr image);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_set_image_position")]
public extern static void gtk_button_set_image_position(IntPtr button, int position);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_button_set_label")]
public extern static void gtk_button_set_label(IntPtr button, string label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_check_button_new")]
public extern static IntPtr gtk_check_button_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_button_new")]
public extern static IntPtr gtk_color_button_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_chooser_dialog_new")]
public extern static IntPtr gtk_color_chooser_dialog_new(string title, IntPtr parent);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_chooser_get_rgba")]
public extern static void gtk_color_chooser_get_rgba(IntPtr chooser, out GdkWrapper.RGBA color);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_chooser_get_use_alpha")]
public extern static bool gtk_color_chooser_get_use_alpha(IntPtr chooser);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_chooser_set_rgba")]
private extern static void wgtk_color_chooser_set_rgba(IntPtr chooser, IntPtr color);

public static void gtk_color_chooser_set_rgba(IntPtr chooser, GdkWrapper.RGBA color)
{
    IntPtr pcolor = Marshal.AllocHGlobal(Marshal.SizeOf(color));
    Marshal.StructureToPtr(color, pcolor, true);
    wgtk_color_chooser_set_rgba(chooser, pcolor);
    Marshal.FreeHGlobal(pcolor);
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_color_chooser_set_use_alpha")]
public extern static void gtk_color_chooser_set_use_alpha(IntPtr chooser, bool use_alpha);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_container_add")]
public extern static void gtk_container_add(IntPtr container, IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_css_provider_load_from_data")]
public extern static bool gtk_css_provider_load_from_data(IntPtr css_provider, string data, int length, IntPtr error);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_css_provider_new")]
public extern static IntPtr gtk_css_provider_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_dialog_add_button")]
public extern static IntPtr gtk_dialog_add_button(IntPtr dialog, string button_text, int response_id);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_dialog_run")]
public extern static int gtk_dialog_run(IntPtr dialog);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_dialog_set_default_response")]
public extern static void gtk_dialog_set_default_response(IntPtr dialog, int response_id);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_get_editable")]
public extern static bool gtk_editable_get_editable(IntPtr entry);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_get_position")]
public extern static int gtk_editable_get_position(IntPtr editable);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_get_selection_bounds")]
public extern static bool gtk_editable_get_selection_bounds(IntPtr editable, out int start_pos, out int end_pos);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_select_region")]
public extern static void gtk_editable_select_region(IntPtr editable, int start_pos, int end_pos);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_set_editable")]
public extern static void gtk_editable_set_editable(IntPtr entry, bool editable);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_editable_set_position")]
public extern static void gtk_editable_set_position(IntPtr editable, int position);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_alignment")]
public extern static float gtk_entry_get_alignment(IntPtr entry);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_has_frame")]
public extern static bool gtk_entry_get_has_frame(IntPtr entry);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_max_length")]
public extern static int gtk_entry_get_max_length(IntPtr entry);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_max_width_chars")]
public extern static int gtk_entry_get_max_width_chars(IntPtr entry);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_placeholder_text")]
private extern static IntPtr wgtk_entry_get_placeholder_text(IntPtr entry);

public static string gtk_entry_get_placeholder_text(IntPtr entry)
{
    var ret = WrapperHelper.GetString(wgtk_entry_get_placeholder_text(entry));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_get_text")]
private extern static IntPtr wgtk_entry_get_text(IntPtr entry);

public static string gtk_entry_get_text(IntPtr entry)
{
    var ret = WrapperHelper.GetString(wgtk_entry_get_text(entry));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_new")]
public extern static IntPtr gtk_entry_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_alignment")]
public extern static void gtk_entry_set_alignment(IntPtr entry, float xalign);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_has_frame")]
public extern static void gtk_entry_set_has_frame(IntPtr entry, bool setting);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_max_length")]
public extern static void gtk_entry_set_max_length(IntPtr entry, int max);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_max_width_chars")]
public extern static void gtk_entry_set_max_width_chars(IntPtr entry, int n_chars);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_placeholder_text")]
public extern static void gtk_entry_set_placeholder_text(IntPtr entry, string text);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_text")]
public extern static void gtk_entry_set_text(IntPtr entry, string text);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_entry_set_width_chars")]
public extern static void gtk_entry_set_width_chars(IntPtr entry, int n_chars);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_event_box_new")]
public extern static IntPtr gtk_event_box_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_get_current_folder")]
private extern static IntPtr wgtk_file_chooser_get_current_folder(IntPtr chooser);

public static string gtk_file_chooser_get_current_folder(IntPtr chooser)
{
    var ret = WrapperHelper.GetString(wgtk_file_chooser_get_current_folder(chooser));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_get_filenames")]
private extern static IntPtr wgtk_file_chooser_get_filenames(IntPtr chooser);

public static GSList gtk_file_chooser_get_filenames(IntPtr chooser)
{
    var ret = (GSList)Marshal.PtrToStructure(wgtk_file_chooser_get_filenames(chooser), typeof(GSList));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_get_select_multiple")]
public extern static bool gtk_file_chooser_get_select_multiple(IntPtr chooser);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_set_current_folder")]
public extern static bool gtk_file_chooser_set_current_folder(IntPtr chooser, string filename);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_set_do_overwrite_confirmation")]
public extern static void gtk_file_chooser_set_do_overwrite_confirmation(IntPtr chooser, bool do_overwrite_confirmation);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_set_select_multiple")]
public extern static void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool select_multiple);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_file_chooser_dialog_new")]
public extern static IntPtr gtk_file_chooser_dialog_new(string title, IntPtr parent, int action);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_image_new")]
public extern static IntPtr gtk_image_new();

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_image_set_from_pixbuf")]
public extern static void gtk_image_set_from_pixbuf(IntPtr image, IntPtr pixbuf);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_line_wrap")]
public extern static bool gtk_label_get_line_wrap(IntPtr label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_line_wrap_mode")]
public extern static int gtk_label_get_line_wrap_mode(IntPtr label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_text")]
private extern static IntPtr wgtk_label_get_text(IntPtr label);

public static string gtk_label_get_text(IntPtr label)
{
    var ret = WrapperHelper.GetString(wgtk_label_get_text(label));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_xalign")]
public extern static float gtk_label_get_xalign(IntPtr label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_get_yalign")]
public extern static float gtk_label_get_yalign(IntPtr label);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_new")]
public extern static IntPtr gtk_label_new(string str);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_set_line_wrap")]
public extern static void gtk_label_set_line_wrap(IntPtr label, bool wrap);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_set_line_wrap_mode")]
public extern static void gtk_label_set_line_wrap_mode(IntPtr label, int wrap_mode);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_set_text")]
public extern static void gtk_label_set_text(IntPtr label, string str);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_set_xalign")]
public extern static void gtk_label_set_xalign(IntPtr label, float xalign);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_label_set_yalign")]
public extern static void gtk_label_set_yalign(IntPtr label, float yalign);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_get_value")]
public extern static double gtk_spin_button_get_value(IntPtr spin_button);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_new_with_range")]
public extern static IntPtr gtk_spin_button_new_with_range(double min, double max, double step);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_set_numeric")]
public extern static void gtk_spin_button_set_numeric(IntPtr spin_button, bool numeric);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_set_update_policy")]
public extern static void gtk_spin_button_set_update_policy(IntPtr spin_button, int policy);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_set_range")]
public extern static void gtk_spin_button_set_range(IntPtr spin_button, double min, double max);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_spin_button_set_value")]
public extern static void gtk_spin_button_set_value(IntPtr spin_button, double value);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_style_context_add_class")]
public extern static void gtk_style_context_add_class(IntPtr context, string class_name);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_style_context_add_provider")]
public extern static void gtk_style_context_add_provider(IntPtr context, IntPtr provider, int priority);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_style_context_get_color")]
public extern static void gtk_style_context_get_color(IntPtr context, int state, out GdkWrapper.RGBA color);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_style_context_remove_class")]
public extern static void gtk_style_context_remove_class(IntPtr context, string class_name);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_toggle_button_get_active")]
public extern static bool gtk_toggle_button_get_active(IntPtr toggle_button);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_toggle_button_get_inconsistent")]
public extern static bool gtk_toggle_button_get_inconsistent(IntPtr toggle_button);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_toggle_button_set_active")]
public extern static void gtk_toggle_button_set_active(IntPtr toggle_button, bool is_active);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_toggle_button_set_inconsistent")]
public extern static void gtk_toggle_button_set_inconsistent(IntPtr toggle_button, bool setting);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_get_allocation")]
public extern static void gtk_widget_get_allocation(IntPtr widget, out Allocation allocation);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_get_halign")]
public extern static int gtk_widget_get_halign(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_get_style_context")]
public extern static IntPtr gtk_widget_get_style_context(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_hide")]
public extern static void gtk_widget_hide(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_queue_resize")]
public extern static void gtk_widget_queue_resize(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_set_font_map")]
public extern static void gtk_widget_set_font_map(IntPtr widget, IntPtr font_map);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_set_halign")]
public extern static void gtk_widget_set_halign(IntPtr widget, int align);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_set_no_show_all")]
public extern static void gtk_widget_set_no_show_all(IntPtr widget, bool no_show_all);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_set_size_request")]
public extern static void gtk_widget_set_size_request(IntPtr widget, int width, int height);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_show")]
public extern static void gtk_widget_show(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_widget_show_all")]
public extern static void gtk_widget_show_all(IntPtr widget);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_window_get_title")]
private extern static IntPtr wgtk_window_get_title(IntPtr window);

public static string gtk_window_get_title(IntPtr window)
{
    var ret = WrapperHelper.GetString(wgtk_window_get_title(window));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_window_set_modal")]
public extern static void gtk_window_set_modal(IntPtr window, bool modal);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_window_set_title")]
public extern static void gtk_window_set_title(IntPtr window, string title);

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "gtk_window_set_transient_for")]
public extern static void gtk_window_set_transient_for(IntPtr window, IntPtr parent);

}
static partial class GioWrapper
{
    public const string NativeLib = "libgtk-3.so.0";

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_app_info_get_executable")]
private extern static IntPtr wg_app_info_get_executable(IntPtr appinfo);

public static string g_app_info_get_executable(IntPtr appinfo)
{
    var ret = WrapperHelper.GetString(wg_app_info_get_executable(appinfo));
    return ret;
}

[DllImport(NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "g_file_new_for_path")]
public extern static IntPtr g_file_new_for_path(string path);

}