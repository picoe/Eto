using System;
using Eto.Drawing;

namespace Eto.Forms
{
	[Handler(typeof(FontDialog.IHandler))]
	public class FontDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
			EventLookup.Register<FontDialog>(c => c.OnFontChanged(null), FontDialog.FontChangedEvent);
		}

		public FontDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public FontDialog(Generator generator)
			: base(generator, typeof(IHandler), true)
		{
		}

		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}

		public new interface IHandler : CommonDialog.IHandler
		{
			Font Font { get; set; }
		}
	}
}

