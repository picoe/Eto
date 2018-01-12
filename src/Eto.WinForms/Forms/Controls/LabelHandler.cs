using System;
using System.IO;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eto.WinForms.Forms.Controls
{
	public class LabelHandler : WindowsControl<LabelHandler.EtoLabel, Label, Label.ICallback>, Label.IHandler
	{
		public class EtoLabel : swf.Label
		{
			WrapMode wrapMode;
			TextAlignment horizontalAlign;
			sd.SizeF? measuredSize;
			int proposedSizeCache;
			sd.SizeF? measuredSizeMax;
			VerticalAlignment verticalAlign;
			swf.TextFormatFlags textFormat;

			struct Position
			{
				public sd.Size Size;
				public string Text;
			}
			List<Position> positions;
			sd.Size? positionSize;

			public void ClearSize()
			{
				measuredSize = measuredSizeMax = null;
				positions = null;
			}

			int lastWidth;
			protected override void OnSizeChanged(EventArgs e)
			{
				base.OnSizeChanged(e);
				if (lastWidth != Width && IsHandleCreated)
				{
					ClearSize();
					lastWidth = Width;
					BeginInvoke(new Action(Handler.SizeChanged));
				}
			}

			public override sd.Font Font
			{
				get { return base.Font; }
				set
				{
					ClearSize();
					base.Font = value;
				}
			}

			public override string Text
			{
				get { return base.Text; }
				set
				{
					ClearSize();
					base.Text = value;
				}
			}

			public WrapMode Wrap
			{
				get { return wrapMode; }
				set
				{
					wrapMode = value;
					SetStringFormat();
				}
			}

			public TextAlignment TextAlignment
			{
				get { return horizontalAlign; }
				set
				{
					horizontalAlign = value;
					SetStringFormat();
				}
			}

			public VerticalAlignment VerticalAlignment
			{
				get { return verticalAlign; }
				set
				{
					verticalAlign = value;
					SetStringFormat();
				}
			}

			public LabelHandler Handler { get; }

			public EtoLabel(LabelHandler handler)
			{
				Handler = handler;
				AutoSize = true;
				Wrap = WrapMode.Word;
			}

			public override sd.Size GetPreferredSize(sd.Size proposedSize)
			{
				var bordersAndPadding = Padding.Size;
				if (proposedSize.Width <= 1)
					proposedSize.Width = int.MaxValue;
				else if (Width > 1)
					proposedSize.Width = Width;

				sd.SizeF size;
				if (proposedSize.Width == int.MaxValue)
				{
					if (measuredSizeMax == null && string.IsNullOrEmpty(Text))
					{
						var emptySize = swf.TextRenderer.MeasureText(" ", Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
						measuredSizeMax = new sd.SizeF(0, emptySize.Height);
					}
					else if (measuredSizeMax == null)
					{
						proposedSize.Height = Math.Max(0, proposedSize.Height - bordersAndPadding.Height);
						measuredSizeMax = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
					}
					size = measuredSizeMax.Value;
				}
				else if (measuredSize == null || proposedSizeCache != proposedSize.Width)
				{
					/*
					if (measuredSize != null)
						Debug.WriteLine("Miss! {0} != {1}, {2}", proposedSizeCache, proposedSize.Width, Text);
					else
						Debug.WriteLine(string.Format("Calc! {0}", Text));*/
					proposedSizeCache = proposedSize.Width;
					proposedSize -= bordersAndPadding;
					proposedSize.Height = Math.Max(0, proposedSize.Height);
					if (wrapMode == WrapMode.Character)
						size = CalculatePositions(proposedSize, true);
					else
						size = swf.TextRenderer.MeasureText(Text, Font, new sd.Size(proposedSize.Width, int.MaxValue), textFormat);
					measuredSize = size;
				}
				else
					size = measuredSize.Value;

				size += bordersAndPadding;
				size.Width = Math.Max(MinimumSize.Width, size.Width);
				size.Height = Math.Max(MinimumSize.Height, size.Height);
				return sd.Size.Ceiling(size);
			}

			sd.SizeF CalculatePositions(sd.Size proposedSize, bool force)
			{
				if (!force && (positions != null && positionSize == proposedSize))
					return sd.SizeF.Empty;

				positionSize = proposedSize;
				var size = sd.SizeF.Empty;
				sd.Size lineSize = sd.Size.Empty;
				var text = Text;
				positions = new List<Position>();
				while (text.Length > 0)
				{
					var lineText = string.Empty;
					for (int len = 0; len <= text.Length; len++)
					{
						var t = text.Substring(0, len);
						var ls = swf.TextRenderer.MeasureText(t, Font, proposedSize, textFormat);
						if (ls.Width < proposedSize.Width)
						{
							lineText = t;
							lineSize = ls;
						}
						else
							break;
					}
					if (lineText.Length == 0)
						break;
					positions.Add(new Position { Size = lineSize, Text = lineText });
					size.Height += lineSize.Height;
					size.Width = Math.Max(lineSize.Width, size.Width);
					text = text.Substring(lineText.Length);
				}
				return size;
			}

			void SetStringFormat()
			{
				textFormat = swf.TextFormatFlags.Default;
				switch (Wrap)
				{
					case WrapMode.None:
						textFormat |= swf.TextFormatFlags.SingleLine;
						break;
					case WrapMode.Word:
						textFormat |= swf.TextFormatFlags.WordBreak;
						break;
					case WrapMode.Character:
						break;
				}
				switch (TextAlignment)
				{
					case TextAlignment.Left:
						textFormat |= swf.TextFormatFlags.Left;
						break;
					case TextAlignment.Right:
						textFormat |= swf.TextFormatFlags.Right;
						break;
					case TextAlignment.Center:
						textFormat |= swf.TextFormatFlags.HorizontalCenter;
						break;
				}
				switch (VerticalAlignment)
				{
					case VerticalAlignment.Top:
					case VerticalAlignment.Stretch:
						textFormat |= swf.TextFormatFlags.Top;
						break;
					case VerticalAlignment.Bottom:
						textFormat |= swf.TextFormatFlags.Bottom;
						break;
					case VerticalAlignment.Center:
						textFormat |= swf.TextFormatFlags.VerticalCenter;
						break;
				}
				ClearSize();
			}

			protected override void OnPaint(swf.PaintEventArgs e)
			{
				var rect = new sd.Rectangle(0, 0, Bounds.Width, Bounds.Height);
				if (wrapMode == WrapMode.Character)
				{
					CalculatePositions(rect.Size, false);
					foreach (var position in positions)
					{
						var r = rect;
						r.Height = position.Size.Height;
						swf.TextRenderer.DrawText(e.Graphics, position.Text, Font, r, ForeColor, textFormat);
						rect.Y += r.Height;
					}
					return;
				}
				swf.TextRenderer.DrawText(e.Graphics, Text, Font, rect, ForeColor, textFormat);
			}
		}

		public LabelHandler()
		{
			Control = new EtoLabel(this);
		}

		protected override void Initialize()
		{
			base.Initialize();
			SuspendLayout();
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ResumeLayout();
		}

		public override Color TextColor
		{
			get { return Control.ForeColor.ToEto(); }
			set { Control.ForeColor = value.ToSD(); }
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment; }
			set
			{
				if (Control.TextAlignment != value)
				{
					Control.TextAlignment = value;
					Control.Invalidate();
				}
			}
		}

		public WrapMode Wrap
		{
			get { return Control.Wrap; }
			set
			{
				if (value != Control.Wrap)
				{
					Control.Wrap = value;
					SetMinimumSize(true);
					Control.Invalidate();
				}
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get { return Control.VerticalAlignment; }
			set
			{
				if (Control.VerticalAlignment != value)
				{
					Control.VerticalAlignment = value;
					Control.Invalidate();
				}
			}
		}

		public override void SetScale(bool xscale, bool yscale)
		{
			base.SetScale(xscale, yscale);
			SetAutoSize();
		}

		void SizeChanged()
		{
			if (Widget != null && Widget.Loaded)
				SetMinimumSize(true);
		}
	}
}