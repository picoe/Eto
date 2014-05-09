using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IColorDialog : ICommonDialog
	{
		Color Color { get; set; }
	}

	[Handler(typeof(IColorDialog))]
	public class ColorDialog : CommonDialog
	{
		new IColorDialog Handler { get { return (IColorDialog)base.Handler; } }

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
			: this(generator, typeof(IColorDialog))
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

		public interface ICallback
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

		protected override object GetCallback() { return callback; }
	}
}

