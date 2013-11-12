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

		public event EventHandler<EventArgs> FontChanged
		{
			add { Properties.AddHandlerEvent(FontChangedEvent, value); }
			remove { Properties.RemoveEvent(FontChangedEvent, value); }
		}

		public virtual void OnFontChanged(EventArgs e)
		{
			Properties.TriggerEvent(FontChangedEvent, this, e);
		}

		static FontDialog()
		{
			EventLookup.Register(typeof(FontDialog), "OnFontChanged", FontDialog.FontChangedEvent);
		}

		public FontDialog(Generator generator = null)
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

