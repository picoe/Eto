namespace Eto.GtkSharp
{
#if GTK2
	public struct RGBA
	{

	}
#endif

	public static class Gtk3Compatibility
	{
#if GTK2
		public static void SetSourceColor(this Cairo.Context context, Cairo.Color color)
		{
			context.SetSourceRGBA(color.R, color.G, color.B, color.A);
		}

		public static void SetAllocation(this Gtk.Widget widget, Gdk.Rectangle rectangle)
		{
			widget.Allocation = rectangle;
		}
		
		public static Gtk.StateType ToGtk(this GtkStateFlags flags)
		{
			switch (flags)
			{
				case GtkStateFlags.Normal:
					return Gtk.StateType.Normal;
				case GtkStateFlags.Active:
					return Gtk.StateType.Active;
				case GtkStateFlags.Prelight:
					return Gtk.StateType.Prelight;
				case GtkStateFlags.Insensitive:
					return Gtk.StateType.Insensitive;
				case GtkStateFlags.Selected:
					return Gtk.StateType.Selected;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gdk.Window GetWindow(this Gtk.Widget widget)
		{
			if (widget is Gtk.Button b)
			{
				var eventWindowPtr = NativeMethods.gtk_button_get_event_window(b.Handle);
				if (eventWindowPtr != IntPtr.Zero)
				{
					var window = GLib.Object.GetObject(eventWindowPtr) as Gdk.Window;
					if (window != null)
						return window;
				}
			}
			return widget.GdkWindow;
		}

		public static Gtk.Requisition GetPreferredSize(this Gtk.Widget widget)
		{
			return widget.SizeRequest();
		}

		public static Pango.FontDescription GetFont(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.FontDescription;
		}

		public static void SetFont(this Gtk.Widget widget, Pango.FontDescription font)
		{
			widget.ModifyFont(font);
		}

		public static Color GetBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Background(state.ToGtk()).ToEto();
		}

		public static void SetBackground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBg(state.ToGtk(), color.ToGdk());
		}

		public static void ClearBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBg(state.ToGtk());
		}

		public static Color GetBase(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Base(state.ToGtk()).ToEto();
		}

		public static void SetBase(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyBase(state.ToGtk(), color.ToGdk());
		}

		public static Color GetTextColor(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Text(state.ToGtk()).ToEto();
		}

		public static void SetTextColor(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyText(state.ToGtk(), color.ToGdk());
		}

		public static Color GetForeground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.Style.Foreground(state.ToGtk()).ToEto();
		}

		public static void SetForeground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.ModifyFg(state.ToGtk(), color.ToGdk());
		}

		public static Gdk.Atom[] ListTargets(this Gtk.SelectionData data) => data.Targets;

		public static Gdk.Atom[] ListTargets(this Gdk.DragContext context) => context.Targets;

		public static Gdk.Atom GetDataType(this Gtk.SelectionData data) => data.Type;
#else
		public static Gtk.StateFlags ToGtk(this GtkStateFlags flags)
		{
			switch (flags)
			{
				case GtkStateFlags.Normal:
					return Gtk.StateFlags.Normal;
				case GtkStateFlags.Active:
					return Gtk.StateFlags.Active;
				case GtkStateFlags.Insensitive:
					return Gtk.StateFlags.Insensitive;
				case GtkStateFlags.Prelight:
					return Gtk.StateFlags.Prelight;
				case GtkStateFlags.Selected:
					return Gtk.StateFlags.Selected;
				default:
					throw new NotSupportedException();
			}
		}

		public static Gdk.Window GetWindow(this Gtk.Widget widget)
		{
			var window = (widget as Gtk.Button)?.EventWindow;
			return window ?? widget.Window;
		}

		public static Gtk.Requisition GetPreferredSize(this Gtk.Widget widget)
		{
			Gtk.Requisition minimum_size, natural_size;
			widget.GetPreferredSize(out minimum_size, out natural_size);
			return natural_size;
		}

		public static Cairo.Color ToCairo(this Gdk.RGBA color)
		{
			return new Cairo.Color(color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Color ToEto(this Gdk.RGBA color)
		{
			return new Color((float)color.Red, (float)color.Green, (float)color.Blue, (float)color.Alpha);
		}

		public static Gdk.RGBA ToRGBA(this Color color)
		{
			return new Gdk.RGBA { Red = color.R, Green = color.G, Blue = color.B, Alpha = color.A };
		}

		class StyleOverrides
		{
			public readonly Gtk.CssProvider Provider = new Gtk.CssProvider();
			public readonly Dictionary<(GtkStateFlags state, string property), string> Rules = new Dictionary<(GtkStateFlags, string), string>();
		}

		static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, StyleOverrides> styleOverrides = new System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, StyleOverrides>();

		static string ToCssSelector(GtkStateFlags state)
		{
			switch (state)
			{
				case GtkStateFlags.Normal:
					return "*";
				case GtkStateFlags.Active:
					return "*:active";
				case GtkStateFlags.Prelight:
					return "*:hover";
				case GtkStateFlags.Insensitive:
					return "*:disabled";
				case GtkStateFlags.Selected:
					return "*:selected";
				default:
					throw new NotSupportedException();
			}
		}

		// Applies to this widget only, at the same priority the obsolete gtk_widget_override_* calls used.
		static void SetStyle(Gtk.Widget widget, GtkStateFlags state, string property, string value)
		{
			var style = styleOverrides.GetValue(widget, w =>
			{
				var overrides = new StyleOverrides();
				w.StyleContext.AddProvider(overrides.Provider, Gtk.StyleProviderPriority.Application);
				return overrides;
			});
			if (value != null)
				style.Rules[(state, property)] = value;
			else if (!style.Rules.Remove((state, property)))
				return;

			var css = new StringBuilder();
			// normal state first so the more specific state rules win
			foreach (var group in style.Rules.GroupBy(r => r.Key.state).OrderBy(r => r.Key))
			{
				css.Append(ToCssSelector(group.Key)).Append(" {");
				foreach (var rule in group)
					css.Append(' ').Append(rule.Key.property).Append(": ").Append(rule.Value).Append(';');
				css.AppendLine(" }");
			}
			style.Provider.LoadFromData(css.ToString());
		}

		static string ToCss(this Gdk.RGBA color) => color.ToString();

		static readonly string[] fontProperties = { "font-family", "font-size", "font-style", "font-variant", "font-weight", "font-stretch" };

		static IEnumerable<(string property, string value)> ToCss(this Pango.FontDescription font)
		{
			var fields = font.SetFields;
			if (fields.HasFlag(Pango.FontMask.Family) && !string.IsNullOrEmpty(font.Family))
			{
				var families = font.Family.Split(',').Select(r => "\"" + r.Trim().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"");
				yield return ("font-family", string.Join(", ", families));
			}
			if (fields.HasFlag(Pango.FontMask.Size))
			{
				var size = (font.Size / (double)Pango.Scale.PangoScale).ToString(CultureInfo.InvariantCulture);
				yield return ("font-size", size + (font.SizeIsAbsolute ? "px" : "pt"));
			}
			if (fields.HasFlag(Pango.FontMask.Style))
				yield return ("font-style", font.Style == Pango.Style.Italic ? "italic" : font.Style == Pango.Style.Oblique ? "oblique" : "normal");
			if (fields.HasFlag(Pango.FontMask.Variant))
				yield return ("font-variant", font.Variant == Pango.Variant.SmallCaps ? "small-caps" : "normal");
			if (fields.HasFlag(Pango.FontMask.Weight))
			{
				// older GTK only parses multiples of 100
				var weight = Math.Max(100, Math.Min(900, (int)Math.Round((int)font.Weight / 100.0) * 100));
				yield return ("font-weight", weight.ToString(CultureInfo.InvariantCulture));
			}
			if (fields.HasFlag(Pango.FontMask.Stretch))
			{
				var stretch = Regex.Replace(font.Stretch.ToString(), "(?<=[a-z])(?=[A-Z])", "-").ToLowerInvariant();
				yield return ("font-stretch", stretch);
			}
		}

		public static Pango.FontDescription GetFont(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			// copy before the value frees the description it holds
			using (var value = widget.StyleContext.GetProperty("font", state.ToGtk()))
				return ((Pango.FontDescription)value.Val)?.Copy();
		}

		public static void SetFont(this Gtk.Widget widget, Pango.FontDescription font)
		{
			var values = font?.ToCss().ToDictionary(r => r.property, r => r.value);
			foreach (var property in fontProperties)
			{
				string value = null;
				values?.TryGetValue(property, out value);
				SetStyle(widget, GtkStateFlags.Normal, property, value);
			}
		}

		public static Color GetBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			using (var value = widget.StyleContext.GetProperty("background-color", state.ToGtk()))
				return ((Gdk.RGBA)value.Val).ToEto();
		}

		public static void SetBackground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			SetStyle(widget, state, "background-color", color.ToRGBA().ToCss());
			// themes paint gradients on some widgets (e.g. buttons) over the colour
			SetStyle(widget, state, "background-image", "none");
		}

		public static void ClearBackground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			SetStyle(widget, state, "background-color", null);
			SetStyle(widget, state, "background-image", null);
		}

		public static Color GetBase(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.GetBackground(state);
		}

		public static void SetBase(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.SetBackground(color, state);
		}

		public static Color GetTextColor(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetColor(state.ToGtk()).ToEto();
		}

		public static void SetTextColor(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			SetStyle(widget, state, "color", color.ToRGBA().ToCss());
		}

		public static Color GetForeground(this Gtk.Widget widget, GtkStateFlags state = GtkStateFlags.Normal)
		{
			return widget.StyleContext.GetColor(state.ToGtk()).ToEto();
		}

		public static void SetForeground(this Gtk.Widget widget, Color color, GtkStateFlags state = GtkStateFlags.Normal)
		{
			widget.SetTextColor(color, state);
		}

		public static Gdk.Atom GetDataType(this Gtk.SelectionData data) => data.DataType;

#if !GTKCORE
		public static Gtk.Widget GetChildAt(this Gtk.Grid grid, int left, int top)
		{
			var ptr = NativeMethods.gtk_grid_get_child_at(grid.Handle, left, top);
			return GLib.Object.GetObject(ptr) as Gtk.Widget;
		}
#endif
#endif

	}

#if GTK2
	public interface ITreeModelImplementor : Gtk.TreeModelImplementor
	{
	}

#else

	public interface ITreeModelImplementor : Gtk.ITreeModelImplementor
	{
	}

#endif
}

