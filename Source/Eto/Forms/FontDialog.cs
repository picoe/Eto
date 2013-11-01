using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IFontDialog : ICommonDialog
	{
		Font Font { get; set; }
	}

	public class FontDialog : CommonDialog
	{
		new IFontDialog Handler { get { return (IFontDialog)base.Handler; } }

		public const string FontChangedEvent = "FontDialog.FontChanged";

		EventHandler<EventArgs> _FontChanged;

		public event EventHandler<EventArgs> FontChanged {
			add {
				HandleEvent (FontChangedEvent);
				_FontChanged += value;
			}
			remove { _FontChanged -= value; }
		}

		public virtual void OnFontChanged (EventArgs e)
		{
			if (_FontChanged != null)
				_FontChanged (this, e);
		}

		static FontDialog()
		{
			EventLookup.Register(typeof(FontDialog), "OnFontChanged", FontDialog.FontChangedEvent);
		}

		public FontDialog (Generator generator = null)
			: base(generator, typeof(IFontDialog), true)
		{
		}

		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}
	}
}

