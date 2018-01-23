using System;
using UIKit;
using Eto.Forms;
using Eto.iOS.Drawing;
using Eto.Drawing;

namespace Eto.iOS.Forms.Controls
{
	public class LabelHandler : IosView<UILabel, Label, Label.ICallback>, Label.IHandler
	{
		public LabelHandler()
		{
			Control = new UILabel();
		}

		public string Text
		{
			get { return Control.Text; }
			set
			{ 
				LayoutIfNeeded(() => Control.Text = value);
			}
		}

		public TextAlignment TextAlignment
		{
			get { return Control.TextAlignment.ToEto(); }
			set
			{
				LayoutIfNeeded(() => Control.TextAlignment = value.ToUI());
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get;
			set;
		}

		public override Eto.Drawing.Font Font
		{
			get { return base.Font; }
			set
			{
				LayoutIfNeeded(() =>
				{
					base.Font = value;
					Control.Font = value.ToUI();
				});
			}
		}

		public WrapMode Wrap
		{
			get
			{ 
				switch (Control.LineBreakMode)
				{
					case UILineBreakMode.CharacterWrap:
						return WrapMode.Character;
					case UILineBreakMode.WordWrap:
						return WrapMode.Word;
					case UILineBreakMode.Clip:
					default:
						return WrapMode.None;
				}
			}
			set
			{
				LayoutIfNeeded(() =>
				{
					switch (value)
					{
						case WrapMode.Character:
							Control.LineBreakMode = UILineBreakMode.CharacterWrap;
							break;
						case WrapMode.Word:
							Control.LineBreakMode = UILineBreakMode.WordWrap;
							break;
						case WrapMode.None:
							Control.LineBreakMode = UILineBreakMode.Clip;
							break;
						default:
							throw new NotSupportedException();
					}
				});
			}
		}

		public Eto.Drawing.Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}
	}
}

