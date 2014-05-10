using System;
using Eto.Drawing;

namespace Eto.Forms
{
	[Handler(typeof(ColorDialog.IHandler))]
	public class ColorDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public event EventHandler<EventArgs> ColorChanged;

		protected virtual void OnColorChanged(EventArgs e)
		{
			if (ColorChanged != null)
				ColorChanged(this, e);
		}

		public ColorDialog()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ColorDialog(Generator generator)
			: this(generator, typeof(ColorDialog.IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ColorDialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public Color Color
		{
			get { return Handler.Color; }
			set { Handler.Color = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		public new interface ICallback
		{
			void OnColorChanged(ColorDialog widget, EventArgs e);
		}

		protected class Callback : ICallback
		{
			public void OnColorChanged(ColorDialog widget, EventArgs e)
			{
				widget.OnColorChanged(e);
			}
		}

		public new interface IHandler : CommonDialog.IHandler
		{
			Color Color { get; set; }
		}
	}
}

