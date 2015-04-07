using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System.Threading;

namespace Eto.GtkSharp.Forms.Controls
{
	public class LabelHandler : GtkControl<LabelHandler.EtoLabel, Label, Label.ICallback>, Label.IHandler
	{
		readonly Gtk.EventBox eventBox;
		static readonly object TextAlignment_Key = new object();
		static readonly object VerticalAlignment_Key = new object();

		public override Gtk.Widget ContainerControl
		{
			get { return eventBox; }
		}

		public override Gtk.Widget EventControl
		{
			get { return eventBox; }
		}

		public class EtoLabel : Gtk.Label
		{
			int? wrapWidth;

			public void ResetWidth()
			{
				wrapWidth = -1;
				HeightRequest = -1;
			}

			#if GTK2

			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				int width, height;
				Layout.GetPixelSize(out width, out height);
				requisition.Width = width;
				requisition.Height = height;
			}
			#else
			protected override void OnGetPreferredHeightForWidth(int width, out int minimum_height, out int natural_height)
			{
				var oldWidth = Layout.Width;
				int pixelWidth, pixelHeight;
				Layout.GetPixelSize(out pixelWidth, out pixelHeight);
				natural_height = pixelHeight;
				minimum_height = pixelHeight;

				Layout.Width = oldWidth;
			}

			protected override void OnGetPreferredWidth(out int minimum_width, out int natural_width)
			{
				var width = Layout.Width;
				Layout.Width = int.MaxValue;
				int pixelWidth, pixelHeight;
				Layout.GetPixelSize(out pixelWidth, out pixelHeight);
				minimum_width = 0;
				natural_width = pixelWidth;
				Layout.Width = width;
			}
			#endif

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				base.OnSizeAllocated(allocation);
				if (Wrap || wrapWidth == null)
					SetWrapWidth(allocation.Width);
			}

			void SetWrapWidth(int width)
			{
				if (!IsRealized || SingleLineMode || width == 0)
					return;
				if (wrapWidth != width)
				{
					Layout.Width = (int)(width * Pango.Scale.PangoScale);
					int pixWidth, pixHeight;
					Layout.GetPixelSize(out pixWidth, out pixHeight);
					HeightRequest = pixHeight;
					wrapWidth = width;
				}
			}
		}

		public LabelHandler()
		{
			eventBox = new Gtk.EventBox();
			//eventBox.VisibleWindow = false;
			Control = new EtoLabel();
			Wrap = WrapMode.Word;
			Control.SetAlignment(0, 0);
			eventBox.Child = Control;
		}

		public WrapMode Wrap
		{
			get
			{
				if (!Control.LineWrap)
					return WrapMode.None;
				if (Control.LineWrapMode == Pango.WrapMode.Char)
					return WrapMode.Character;
				return WrapMode.Word;
			}
			set
			{
				Control.ResetWidth();
				switch (value)
				{
					case WrapMode.None:
						Control.Wrap = false;
						Control.LineWrap = false;
						Control.SingleLineMode = true;
						break;
					case WrapMode.Word:
						// set to false then true to ensure size is recalculated at runtime
						Control.Wrap = false;
						Control.Wrap = true;
						Control.Layout.Wrap = Pango.WrapMode.WordChar;
						Control.LineWrapMode = Pango.WrapMode.WordChar;
						Control.LineWrap = true;
						Control.SingleLineMode = false;
						break;
					case WrapMode.Character:
						Control.Wrap = false;
						Control.Wrap = true;
						Control.Layout.Wrap = Pango.WrapMode.Char;
						Control.LineWrapMode = Pango.WrapMode.Char;
						Control.LineWrap = true;
						Control.SingleLineMode = false;
						break;
					default:
						throw new NotSupportedException();
				}
			}
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
			get { return Control.GetForeground(); }
			set { Control.SetForeground(value); }
		}

		public override string Text
		{
			get { return Control.Text.ToEtoMnemonic(); }
			set
			{
				Control.ResetWidth();
				Control.TextWithMnemonic = value.ToPlatformMnemonic();
			}
		}

		public TextAlignment TextAlignment
		{
			get { return Widget.Properties.Get<TextAlignment>(TextAlignment_Key); }
			set
			{
				Widget.Properties.Set(TextAlignment_Key, value);
				SetAlignment();
			}
		}

		void SetAlignment()
		{
			float xalignment;
			float yalignment;
			switch (TextAlignment)
			{
				default:
					xalignment = 0F;
					break;
				case TextAlignment.Center:
					xalignment = 0.5F;
					break;
				case TextAlignment.Right:
					xalignment = 1F;
					break;
			}
			switch (VerticalAlignment)
			{
				case VerticalAlignment.Center:
					yalignment = 0.5F;
					break;
				default:
					yalignment = 0F;
					break;
				case VerticalAlignment.Bottom:
					yalignment = 1F;
					break;
			}
			Control.SetAlignment(xalignment, yalignment);
			Control.Justify = TextAlignment.ToGtk();
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Widget.Properties.Get<VerticalAlignment>(VerticalAlignment_Key); }
			set
			{
				Widget.Properties.Set(VerticalAlignment_Key, value);
				SetAlignment();
			}
		}

		public override Font Font
		{
			get { return base.Font; }
			set
			{
				Control.ResetWidth();
				base.Font = value;
				Control.Attributes = value != null ? ((FontHandler)value.Handler).Attributes : null;
			}
		}

		public override Size GetPreferredSize(Size availableSize)
		{
			if (!Control.IsRealized)
				Control.Realize();

			var oldWidth = Control.Layout.Width;
			Control.Layout.Width = Control.Wrap ? (int)Math.Round(availableSize.Width * Pango.Scale.PangoScale) : int.MaxValue;
			int width, height;
			Control.Layout.GetPixelSize(out width, out height);
			Control.Layout.Width = oldWidth;
			return new Size(width, height);
		}

		#if GTK3
		public override void SetScale(bool scaled)
		{
			base.SetScale(scaled);
			Control.Scaled = scaled;
		}
		#endif
	}
}
