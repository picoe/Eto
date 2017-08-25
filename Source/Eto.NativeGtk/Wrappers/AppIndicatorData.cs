void      app_indicator_dispose(IntPtr gobject);
int       app_indicator_get_status(IntPtr self);
string    app_indicator_get_title(IntPtr self);
IntPtr    app_indicator_new(string id, string icon_name, int category);
void      app_indicator_set_icon(IntPtr self, string icon_name);
void      app_indicator_set_menu(IntPtr self, IntPtr menu);
void      app_indicator_set_status(IntPtr self, int status);
void      app_indicator_set_title(IntPtr self, string title);