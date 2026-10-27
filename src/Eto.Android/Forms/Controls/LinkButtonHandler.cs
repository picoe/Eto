using Eto.Android.Forms.Controls;

using aa = Android.App;
using ac = Android.Content;
using ao = Android.OS;
using ar = Android.Runtime;
using av = Android.Views;
using aw = Android.Widget;
using ag = Android.Graphics;

namespace Eto.Android
{
	internal class LinkButtonHandler : AndroidCommonControl<LinkButtonHandler.EtoLinkButton, LinkButton, LinkButton.ICallback>, LinkButton.IHandler
	{
		internal class EtoLinkButton : aw.TextView
		{
			public EtoLinkButton(ac.Context context)
				: base(context)
			{
			}
		}

		public LinkButtonHandler()
		{
			Control = new EtoLinkButton(Platform.AppContextThemed);
			Control.Click += (sender, e) => Callback.OnClick(Widget, EventArgs.Empty);
		}

		// TODO
		public WrapMode Wrap
		{
			get;
			set;
		}

		TextTrimming trimming;

		public TextTrimming Trimming
		{
			get { return trimming; }
			set
			{
				trimming = value;
				// A TextView only truncates at a character boundary, so a word ellipsis gets that.
				Control.Ellipsize = value == TextTrimming.None ? null : global::Android.Text.TextUtils.TruncateAt.End;
			}
		}

		public Color DisabledTextColor
		{
			get
			{
				// TODO: Wrong
				return TextColor;
			}

			set
			{
				return;
				throw new NotImplementedException();
			}
		}
	}
}
