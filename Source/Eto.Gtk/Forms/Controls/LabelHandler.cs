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
			int wrapWidth;

			#if GTK2
			protected override void OnSizeRequested(ref Gtk.Requisition requisition)
			{
				//base.OnSizeRequested (ref requisition);
				int width, height;
				Layout.GetPixelSize(out width, out height);
				requisition.Width = width;
				requisition.Height = height;
			}
			#else
			protected override void OnGetPreferredWidth (out int minimum_width, out int natural_width)
			{
				base.OnGetPreferredWidth (out minimum_width, out natural_width);
				//minimum_width = natural_width; // = 500; //this.Layout.Width;
			}

			protected override void OnAdjustSizeRequest (Gtk.Orientation orientation, out int minimum_size, out int natural_size)
			{
				base.OnAdjustSizeRequest (orientation, out minimum_size, out natural_size);
				if (orientation == Gtk.Orientation.Horizontal)
					minimum_size = natural_size;
			}

#endif

			protected override void OnSizeAllocated(Gdk.Rectangle allocation)
			{
				base.OnSizeAllocated(allocation);
				SetWrapWidth(allocation.Width);
			}

			void SetWrapWidth(int width)
			{
				if (!IsRealized || SingleLineMode || width == 0)
					return;
				Layout.Width = (int)(width * Pango.Scale.PangoScale);
				if (wrapWidth != width)
				{
					wrapWidth = width;
					QueueResize();
				}
			}
		}

		public LabelHandler()
		{
			eventBox = new Gtk.EventBox();
			//eventBox.VisibleWindow = false;
			Control = new EtoLabel
			{
				SingleLineMode = false,
				LineWrap = true,
				LineWrapMode = Pango.WrapMode.Word
			};
			Control.SetAlignment(0, 0);
			eventBox.Child = Control;
		}

		public WrapMode Wrap
		{
			get
			{
				if (!Control.LineWrap)
					return WrapMode.None;
				if (Control.LineWrapMode == Pango.WrapMode.Word)
					return WrapMode.Word;
				return WrapMode.Character;
			}
			set
			{
				switch (value)
				{
					case WrapMode.None:
						Control.Wrap = false;
						Control.LineWrap = false;
						Control.SingleLineMode = true;
						break;
					case WrapMode.Word:
						Control.Wrap = true;
						Control.LineWrapMode = Pango.WrapMode.Word;
						Control.LineWrap = true;
						Control.SingleLineMode = false;
						break;
					case WrapMode.Character:
						Control.Wrap = true;
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
			get { return Control.Style.Foreground(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyFg(Gtk.StateType.Normal, value.ToGdk()); }
		}

		public override string Text
		{
			get { return Control.Text.ToEtoMnemonic(); }
			set { Control.TextWithMnemonic = value.ToPlatformMnemonic(); }
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
			float xalignment;
			float yalignment;
			switch (horizontalAlign)
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
			switch (verticalAlign)
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
			Control.Justify = horizontalAlign.ToGtk();
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
				base.Font = value;
				Control.Attributes = value != null ? ((FontHandler)value.Handler).Attributes : null;
			}
		}
	}
}
