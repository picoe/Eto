using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class LabelHandler : GtkControl<LabelHandler.EtoLabel, Label, Label.ICallback>, Label.IHandler
	{
		readonly Gtk.EventBox eventBox;
		TextAlignment horizontalAlign = TextAlignment.Left;
		VerticalAlignment verticalAlign = VerticalAlignment.Top;

		public override Gtk.Widget ContainerControl => eventBox;

		public override Gtk.Widget EventControl => eventBox;

		public class EtoLabel : Gtk.Label
		{
#if GTK2
			int? wrapWidth;

			public void ResetWidth()
			{
				wrapWidth = null;
			}

			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				int width, height;
				if (wrapWidth != null && wrapWidth > 0) {
					Layout.Width = (int)(wrapWidth * Pango.Scale.PangoScale);
					Layout.GetPixelSize(out width, out height);
					requisition.Width = width;
					requisition.Height = height;
				}
				else {
					Layout.GetPixelSize(out width, out height);

					requisition.Width = width;
					requisition.Height = height;
				}
			}

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				base.OnSizeAllocated(allocation);
				SetWrapWidth(allocation.Width);
			}

			protected override void OnRealized()
			{
				// the allocation may default to 1, in that case ignore OnRealized
				if (Allocation.Width > 1 && wrapWidth != Allocation.Width)
				{
					CalculateHeight(Allocation.Width);
				}

				base.OnRealized();
			}

			void SetWrapWidth(int width)
			{
				if (!IsRealized || SingleLineMode || width == 0)
				{
					HeightRequest = -1;
					wrapWidth = null;
					return;
				}
				if (wrapWidth != width)
				{
					CalculateHeight(width);
				}
			}

			void CalculateHeight(int width)
			{
				Layout.Width = (int)(width * Pango.Scale.PangoScale);
				int pixWidth, pixHeight;
				Layout.GetPixelSize(out pixWidth, out pixHeight);
				HeightRequest = pixHeight;
				wrapWidth = width;
			}
#else
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

#if GTKCORE
			protected override void OnGetPreferredHeightAndBaselineForWidth(int width, out int minimum_height, out int natural_height, out int minimum_baseline, out int natural_baseline)
			{
				if (width == 0)
					width = int.MaxValue;
				base.OnGetPreferredHeightAndBaselineForWidth(width, out minimum_height, out natural_height, out minimum_baseline, out natural_baseline);
			}
#endif
#endif

		}

		public LabelHandler()
		{
			eventBox = new EtoEventBox { Handler = this };
#if GTK2
			eventBox.ResizeMode = Gtk.ResizeMode.Immediate;
#endif
			Control = new EtoLabel();
			Control.Xalign = 0;
			Control.Yalign = 0;
			eventBox.Child = Control;
			Wrap = WrapMode.Word;
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
				SetWrap(value);
			}
		}

		void SetWrap(WrapMode mode)
		{
			Control.ResetWidth();
			switch (mode)
			{
				case WrapMode.None:
					Control.Wrap = false;
					Control.LineWrap = false;
					Control.SingleLineMode = true;
					break;
				case WrapMode.Word:
					Control.Wrap = true;
					Control.Layout.Wrap = Pango.WrapMode.WordChar;
					Control.LineWrapMode = Pango.WrapMode.WordChar;
					Control.LineWrap = true;
					Control.SingleLineMode = false;
					break;
				case WrapMode.Character:
					Control.Wrap = true;
					Control.Layout.Wrap = Pango.WrapMode.Char;
					Control.LineWrapMode = Pango.WrapMode.Char;
					Control.LineWrap = true;
					Control.SingleLineMode = false;
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
			get { return horizontalAlign; }
			set
			{
				horizontalAlign = value;
				SetAlignment();
			}
		}

		void SetAlignment()
		{
			Control.ResetWidth();
			Control.Justify = horizontalAlign.ToGtk();
			Control.Xalign = horizontalAlign.ToAlignment();
			Control.Yalign = verticalAlign.ToAlignment();
#if GTK2
			eventBox.ResizeChildren();
#endif
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
				Control.ResetWidth();
				base.Font = value;
				Control.Attributes = value != null ? ((FontHandler)value.Handler).Attributes : null;
			}
		}
	}
}
