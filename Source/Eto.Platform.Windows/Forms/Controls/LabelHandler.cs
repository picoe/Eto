using System;
using System.Reflection;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows
{
	public class LabelHandler : WindowsControl<LabelHandler.MyLabel, Label>, ILabel
	{
		Font font;
		
		public class MyLabel : System.Windows.Forms.Label
		{
			public WrapMode Wrap { get; set; }
			
			public HorizontalAlign HorizontalAlign { get; set; }
			
			public VerticalAlign VerticalAlign { get; set; }
			
			public MyLabel()
			{
				Wrap = WrapMode.Word;
			}
			
			protected override void OnPaint (System.Windows.Forms.PaintEventArgs e)
			{
				using (var format = new SD.StringFormat ()) 
				using (var b = new SD.SolidBrush (this.ForeColor))
				{
					format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
					switch (this.Wrap) {
					case WrapMode.None:
						format.Trimming = System.Drawing.StringTrimming.None;
						format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
					case WrapMode.Word:
						format.Trimming = System.Drawing.StringTrimming.Word;
						format.FormatFlags = 0;
						break;
					case WrapMode.Character:
						format.Trimming = System.Drawing.StringTrimming.Character;
						format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
						break;
					}
					
					if (this.Wrap == WrapMode.Character)
					{
						// draw string one line at a time to trim to character..
						int charactersFitted, linesFilled;
						string text = this.Text;
						SD.PointF drawPoint = SD.PointF.Empty;
						var font = this.Font;
						var height = font.GetHeight (e.Graphics);
						while (!string.IsNullOrEmpty (text))
						{
							e.Graphics.MeasureString (text, font, this.Bounds.Size, format, out charactersFitted, out linesFilled);
							
							e.Graphics.DrawString (text.Substring (0, charactersFitted), font, b, drawPoint, format);
							
							if (charactersFitted >= text.Length) break;
							text = text.Substring (charactersFitted);
							
							drawPoint.Y += height;
						}
					}
					else
					{
						var size = e.Graphics.MeasureString (this.Text, this.Font, this.Bounds.Width, format);
						var rect = new SD.RectangleF (0, 0, this.Bounds.Width, this.Bounds.Height);
						
						if (size.Height < rect.Height)
						{
							switch (this.VerticalAlign)
							{
							case VerticalAlign.Bottom:
								rect.Y = rect.Height - size.Height;
								rect.Height = size.Height;
								break;
							case VerticalAlign.Middle:
								rect.Y = (rect.Height - size.Height) / 2;
								rect.Height = size.Height;
								break;
							}
						}
						
						if (size.Width < rect.Width)
						{
							switch (this.HorizontalAlign)
							{
							case HorizontalAlign.Right:
								rect.X = rect.Width - size.Width;
								rect.Width = size.Width;
								break;
							case HorizontalAlign.Center:
								rect.X = (rect.Width - size.Width) / 2;
								rect.Width = size.Width;
								break;
							}
						}
						
						switch (this.HorizontalAlign)
						{
						case HorizontalAlign.Right:
							format.Alignment = System.Drawing.StringAlignment.Far;
							break;
						case HorizontalAlign.Center:
							format.Alignment = System.Drawing.StringAlignment.Center;
							break;
						}
						
						e.Graphics.DrawString (this.Text, this.Font, b, rect, format);
					}
				}
			}			
		}
		
		public LabelHandler ()
		{
			Control = new MyLabel ();
			Control.AutoSize = true;
		}
		
		public HorizontalAlign HorizontalAlign {
			get {
				return Control.HorizontalAlign;
			}
			set {
				Control.HorizontalAlign = value;
			}
		}
		
		public WrapMode Wrap {
			get {
				return Control.Wrap;
			}
			set {
				Control.Wrap = value;
			}
		}
		
		public Font Font {
			get {
				return font;
			}
			set {
				font = value;
				if (font != null)
					Control.Font = (SD.Font)font.ControlObject;
				else
					Control.Font = null;
			}
		}
		
		public VerticalAlign VerticalAlign {
			get {
				return Control.VerticalAlign;
			}
			set {
				Control.VerticalAlign = value;
			}
		}

	}
}
