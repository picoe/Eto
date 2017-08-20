void      g_object_unref (IntPtr obj);
uint      g_signal_connect_data(IntPtr instance, string detailed_signal, IntPtr handler, IntPtr data, IntPtr destroy_data, int connect_flags);
void      g_signal_connect (IntPtr instance, string detailed_signal, IntPtr c_handler, IntPtr data)

string[]  gtk_about_dialog_get_artists (IntPtr about);
string[]  gtk_about_dialog_get_authors (IntPtr about);
string    gtk_about_dialog_get_comments (IntPtr about);
string    gtk_about_dialog_get_copyright (IntPtr about);
string[]  gtk_about_dialog_get_documenters (IntPtr about);
string    gtk_about_dialog_get_license (IntPtr about);
string    gtk_about_dialog_get_program_name (IntPtr about);
string    gtk_about_dialog_get_version (IntPtr about);
string    gtk_about_dialog_get_website (IntPtr about);
string    gtk_about_dialog_get_website_label (IntPtr about);
IntPtr    gtk_about_dialog_new ();
void      gtk_about_dialog_set_artists (IntPtr about, string[] artists);
void      gtk_about_dialog_set_authors (IntPtr about, string[] authors);
void      gtk_about_dialog_set_comments (IntPtr about, string comments);
void      gtk_about_dialog_set_copyright (IntPtr about, string copyright);
void      gtk_about_dialog_set_documenters (IntPtr about, string[] documenters);
void      gtk_about_dialog_set_license (IntPtr about, string license);
void      gtk_about_dialog_set_logo (IntPtr about, IntPtr logo);
void      gtk_about_dialog_set_program_name (IntPtr about, string name);
void      gtk_about_dialog_set_version (IntPtr about, string version);
void      gtk_about_dialog_set_website (IntPtr about, string website);
void      gtk_about_dialog_set_website_label (IntPtr about, string website_label);
void      gtk_about_dialog_set_wrap_license (IntPtr about, bool wrap_license);

IntPtr    gtk_accel_label_new (string text);

IntPtr    gtk_adjustment_new (double value, double lower, double upper, double step_increment, double page_increment, double page_size);

int       gtk_button_get_image_position (IntPtr button);
string    gtk_button_get_label (IntPtr button);
IntPtr    gtk_button_new ();
void      gtk_button_set_always_show_image (IntPtr button, bool always_show);
void      gtk_button_set_image (IntPtr button, IntPtr image);
void      gtk_button_set_image_position (IntPtr button, int position);
void      gtk_button_set_label (IntPtr button, string label);

IntPtr    gtk_check_button_new ();
IntPtr    gtk_color_button_new ();

IntPtr    gtk_color_chooser_dialog_new (string title, IntPtr parent);
void      gtk_color_chooser_get_rgba (IntPtr chooser, out RGBA color);
bool      gtk_color_chooser_get_use_alpha (IntPtr chooser);
void      gtk_color_chooser_set_rgba (IntPtr chooser, GtkWrapper.RGBA color);
void      gtk_color_chooser_set_use_alpha (IntPtr chooser, bool use_alpha);

void      gtk_container_add (IntPtr container, IntPtr widget);

bool      gtk_css_provider_load_from_data (IntPtr css_provider, string data, int length, IntPtr error);
IntPtr    gtk_css_provider_new ();

int       gtk_dialog_run (IntPtr dialog);

bool      gtk_editable_get_editable (IntPtr entry);
int       gtk_editable_get_position (IntPtr editable);
bool      gtk_editable_get_selection_bounds (IntPtr editable, out int start_pos, out int end_pos)
void      gtk_editable_select_region (IntPtr editable, int start_pos, int end_pos);
void      gtk_editable_set_editable (IntPtr entry, bool editable);
void      gtk_editable_set_position (IntPtr editable, int position);

float     gtk_entry_get_alignment (IntPtr entry);
bool      gtk_entry_get_has_frame (IntPtr entry);
int       gtk_entry_get_max_length (IntPtr entry);
int       gtk_entry_get_max_width_chars (IntPtr entry);
string    gtk_entry_get_placeholder_text (IntPtr entry);
string    gtk_entry_get_text (IntPtr entry);
IntPtr    gtk_entry_new ();
void      gtk_entry_set_alignment (IntPtr entry, float xalign);
void      gtk_entry_set_has_frame (IntPtr entry, bool setting);
void      gtk_entry_set_max_length (IntPtr entry, int max);
void      gtk_entry_set_max_width_chars (IntPtr entry, int n_chars);
void      gtk_entry_set_placeholder_text (IntPtr entry, string text);
void      gtk_entry_set_text (IntPtr entry, string text);
void      gtk_entry_set_width_chars (IntPtr entry, int n_chars);

IntPtr    gtk_event_box_new ();

IntPtr    gtk_image_new ();
void      gtk_image_set_from_pixbuf (IntPtr image, IntPtr pixbuf);

bool      gtk_label_get_line_wrap (IntPtr label);
int       gtk_label_get_line_wrap_mode (IntPtr label);
string    gtk_label_get_text (IntPtr label);
float     gtk_label_get_xalign (IntPtr label);
float     gtk_label_get_yalign (IntPtr label);
IntPtr    gtk_label_new (string str);
void      gtk_label_set_line_wrap (IntPtr label, bool wrap);
void      gtk_label_set_line_wrap_mode (IntPtr label, int wrap_mode);
void      gtk_label_set_text (IntPtr label, string str);
void      gtk_label_set_xalign (IntPtr label, float xalign);
void      gtk_label_set_yalign (IntPtr label, float yalign);

IntPtr    gtk_spin_button_new (IntPtr adjustment, double climb_rate, uint digits);
void      gtk_spin_button_set_numeric (IntPtr spin_button, bool numeric);

void      gtk_style_context_add_class (IntPtr context, string class_name);
void      gtk_style_context_add_provider (IntPtr context, IntPtr provider, int priority);
void      gtk_style_context_get_color (IntPtr context, int state, out RGBA color);
void      gtk_style_context_remove_class (IntPtr context, string class_name);

bool      gtk_toggle_button_get_active (IntPtr toggle_button);
bool      gtk_toggle_button_get_inconsistent (IntPtr toggle_button);
void      gtk_toggle_button_set_active (IntPtr toggle_button, bool is_active);
void      gtk_toggle_button_set_inconsistent (IntPtr toggle_button, bool setting);

void      gtk_widget_get_allocation (IntPtr widget, out GtkAllocation allocation);
int       gtk_widget_get_halign (IntPtr widget);
IntPtr    gtk_widget_get_style_context (IntPtr widget);
void      gtk_widget_hide (IntPtr widget);
void      gtk_widget_queue_resize (IntPtr widget);
void      gtk_widget_set_font_map (IntPtr widget, IntPtr font_map);
void      gtk_widget_set_halign (IntPtr widget, int align);
void      gtk_widget_set_no_show_all (IntPtr widget, bool no_show_all);
void      gtk_widget_set_size_request (IntPtr widget, int width, int height);
void      gtk_widget_show (IntPtr widget);
void      gtk_widget_show_all (IntPtr widget);

string    gtk_window_get_title (IntPtr window);
void      gtk_window_set_modal (IntPtr window, bool modal);
void      gtk_window_set_title (IntPtr window, string title);
void      gtk_window_set_transient_for (IntPtr window, IntPtr parent);
