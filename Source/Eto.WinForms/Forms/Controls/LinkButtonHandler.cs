using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms.Controls
{
	public class LinkButtonHandler : WindowsControl<swf.LinkLabel, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		public class EtoLink : swf.LinkLabel
		{
			public override sd.Font Font
			{
				get
				{
					return base.Font;
				}
				set
				{
					base.Font = value;
					ClearSize();
				}
			}

			public override string Text
			{
				get
				{
					return base.Text;
				}
				set
				{
					base.Text = value;
					ClearSize();
				}
			}
			void ClearSize()
			{
				measuredSize = measuredSizeMax = null;
			}

			swf.TextFormatFlags textFormat = swf.TextFormatFlags.Default;
			sd.SizeF? measuredSize;
			sd.Size proposedSizeCache;
			sd.SizeF? measuredSizeMax;

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var bordersAndPadding = Margin.Size; // this.SizeFromClientSize (SD.Size.Empty);
				if( proposedSize.Width <= 1 )
					proposedSize.Width = int.MaxValue;

				if( proposedSize.Width == int.MaxValue )
				{
					if( measuredSizeMax == null && string.IsNullOrEmpty(Text) )
					{
						var emptySize = swf.TextRenderer.MeasureText(" ", Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
						measuredSizeMax = new sd.SizeF(0, emptySize.Height);
					}
					else if( measuredSizeMax == null )
					{
						proposedSize -= bordersAndPadding;
						proposedSize.Height = Math.Max(0, proposedSize.Height);
						measuredSizeMax = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
					}
					measuredSize = measuredSizeMax;
				}
				else if( measuredSize == null || proposedSizeCache != proposedSize )
				{
					proposedSizeCache = proposedSize;
					proposedSize -= bordersAndPadding;
					proposedSize.Height = Math.Max(0, proposedSize.Height);
					measuredSize = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
				}
				var size = measuredSize.Value;
				size += bordersAndPadding;
				if( size.Width < MinimumSize.Width )
					size.Width = MinimumSize.Width;
				if( size.Height < MinimumSize.Height )
					size.Height = MinimumSize.Height;
				return sd.Size.Ceiling(size);
			}
		}
		
		public LinkButtonHandler()
		{
			Control = new EtoLink
			{
				AutoSize = true
			};
		}

		public override Color TextColor
		{
			get { return Control.LinkColor.ToEto(); }
			set { Control.LinkColor = value.ToSD(); }
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case LinkButton.ClickEvent:
					Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public Color DisabledTextColor
		{
			get { return Control.DisabledLinkColor.ToEto(); }
			set { Control.DisabledLinkColor = value.ToSD(); }
		}

		static readonly Win32.WM[] intrinsicEvents = { Win32.WM.LBUTTONDOWN, Win32.WM.LBUTTONUP, Win32.WM.LBUTTONDBLCLK };
		public override bool ShouldBubbleEvent(swf.Message msg)
		{
			return !intrinsicEvents.Contains((Win32.WM)msg.Msg) && base.ShouldBubbleEvent(msg);
		}
	}
}
