using UIKit;
using Eto.iOS.Drawing;
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

		// a UILabel starts out word wrapping, which is what the old Control.LineBreakMode getter reported
		WrapMode wrap = WrapMode.Word;
		TextTrimming trimming;

		public WrapMode Wrap
		{
			get => wrap;
			set
			{
				wrap = value;
				SetLineBreakMode();
			}
		}

		public TextTrimming Trimming
		{
			get => trimming;
			set
			{
				trimming = value;
				SetLineBreakMode();
			}
		}

		/// <summary>
		/// UIKit says how a line ends with a single value, so wrapping and truncating cannot both apply.
		/// Truncating a wrapped label would need to know how many lines it is allowed, which nothing tells
		/// us, so wrapping wins and the trimming is left off - see Eto.Forms.TextTrimming.
		/// </summary>
		void SetLineBreakMode()
		{
			LayoutIfNeeded(() =>
			{
				if (wrap == WrapMode.None && trimming != TextTrimming.None)
				{
					Control.LineBreakMode = UILineBreakMode.TailTruncation;
					return;
				}
				switch (wrap)
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

		public Eto.Drawing.Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}
	}
}

