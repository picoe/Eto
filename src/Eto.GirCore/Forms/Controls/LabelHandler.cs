// using Eto.GirSharp.Drawing;

using System.Runtime.CompilerServices;

namespace Eto.GirCore.Forms.Controls
{
	public class LabelHandler : GirControl<Gtk.Label, Label, Label.ICallback>, Label.IHandler
	{
		static readonly object TextColorKey = new object();
		string _text;
		TextAlignment horizontalAlign = TextAlignment.Left;
		VerticalAlignment verticalAlign = VerticalAlignment.Top;

		/*
				public class EtoLabel : Gtk.Label
				{
					public void ResetWidth()
					{
					}

					protected override Gtk.SizeRequestMode OnGetRequestMode() => Gtk.SizeRequestMode.HeightForWidth;

					protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
					{
						base.OnGetPreferredWidth(out minimum_width, out natural_width);

						// label should allow shrinking, natural width is used instead
						minimum_width = 0;
					}

					protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
					{
						if (width == 0)
							width = int.MaxValue;
						base.OnGetPreferredHeightForWidth(width, out minimum_height, out natural_height);
					}

					protected override void OnGetPreferredHeightAndBaselineForWidth(int width, out int minimum_height, out int natural_height, out int minimum_baseline, out int natural_baseline)
					{
						if (width == 0)
							width = int.MaxValue;
						base.OnGetPreferredHeightAndBaselineForWidth(width, out minimum_height, out natural_height, out minimum_baseline, out natural_baseline);
					}

				}
				*/

		public LabelHandler()
		{
			Control = Gtk.Label.New(null);
			Control.UseUnderline = true;
			Control.Xalign = 0;
			Control.Yalign = 0;
			Wrap = WrapMode.Word;
		}

		public WrapMode Wrap
		{
			get
			{
				if (!Control.Wrap)
					return WrapMode.None;
				if (Control.WrapMode == Pango.WrapMode.Char)
					return WrapMode.Character;
				return WrapMode.Word;
			}
			set
			{
				SetWrap(value);
			}
		}

		void SetWrap(WrapMode mode)
		{
			// Control.ResetWidth();
			switch (mode)
			{
				case WrapMode.None:
					Control.Wrap = false;
					break;
				case WrapMode.Word:
					Control.Wrap = true;
					Control.WrapMode = Pango.WrapMode.WordChar;
					break;
				case WrapMode.Character:
					Control.Wrap = true;
					Control.WrapMode = Pango.WrapMode.Char;
					break;
				default:
					throw new NotSupportedException();
			}
			Control.QueueResize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public virtual Color TextColor
		{
			get => GetProperty<Color?>(TextColorKey) ?? Colors.Black;
			set => SetProperty(TextColorKey, value, $"color: {value.ToHex()};");
		}

		T GetProperty<T>(object key)
		{
			if (Widget.Properties.TryGetValue(key, out var value))
				return (T)value;
			return default;
		}
		
		void SetProperty<T>(object key, T value, string style, [CallerMemberName] string caller = null)
		{
			if (style != null)
				AddStyle(style, caller);
			Widget.Properties[key] = value;
		}
		
		
		Gtk.CssProvider cssProvider = null;
		Dictionary<string, string> styleCache = new Dictionary<string, string>();
		void AddStyle(string style, [CallerMemberName] string caller = null)
		{
			if (cssProvider == null)
			{
				cssProvider = new Gtk.CssProvider();
				Control.GetStyleContext().AddProvider(cssProvider, 600); // Gtk.STYLE_PROVIDER_PRIORITY_APPLICATION
			}
			styleCache[caller] = style;
			cssProvider.LoadFromString(string.Join("\n", styleCache.Values));
		}

		public override string Text
		{
			get => _text;
			set
			{
				// Control.ResetWidth();
				_text = value;
				if (Control.UseUnderline)
					Control.SetTextWithMnemonic(_text.ToPlatformMnemonic());
				else
					Control.SetText(_text);
				InvalidateMeasure();
			}
		}

		public TextAlignment TextAlignment
		{
			get { return horizontalAlign; }
			set
			{
				horizontalAlign = value;
				SetAlignment();
			}
		}

		void SetAlignment()
		{
			// Control.ResetWidth();
			Control.Justify = horizontalAlign.ToGtk();
			Control.Xalign = horizontalAlign.ToGtkAlign();
			Control.Yalign = verticalAlign.ToGtkAlign();
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return verticalAlign; }
			set
			{
				verticalAlign = value;
				SetAlignment();
			}
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				// Control.ResetWidth();
				base.Font = value;
				throw new NotImplementedException();
				// Control.Attributes = value != null ? ((FontHandler)value.Handler).Attributes : null;
			}
		}

		public bool UseMnemonic
		{
			get => Control.UseUnderline;
			set
			{
				if (value == Control.UseUnderline)
					return; // no change
				var text = Text;
				Control.UseUnderline = value;
				Text = text;
			}
		}

		public bool AlwaysShowMnemonic
		{
			get => false;
			set { /* not supported in GTK */ }
		}
	}
}
