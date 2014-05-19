using System;

namespace Eto.Forms
{
	[Handler(typeof(RadioButton.IHandler))]
	public class RadioButton : TextControl
	{
		public event EventHandler<EventArgs> CheckedChanged;
		public event EventHandler<EventArgs> Click;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}

		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}

		public RadioButton()
		{
			Handler.Create(null);
			Initialize();
		}

		public RadioButton(RadioButton controller = null)
		{
			Handler.Create(controller);
			Initialize();
		}

		[Obsolete("Use RadioButton(RadioButton) instead")]
		public RadioButton(RadioButton controller = null, Generator generator = null)
			: this(generator, typeof(IHandler), controller)
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected RadioButton(Generator generator, Type type, RadioButton controller, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create(controller);
			Initialize();
		}

		public virtual bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		public new interface ICallback : TextControl.ICallback
		{
			void OnClick(RadioButton widget, EventArgs e);

			void OnCheckedChanged(RadioButton widget, EventArgs e);
		}

		protected new class Callback : TextControl.Callback, ICallback
		{
			public void OnClick(RadioButton widget, EventArgs e)
			{
				widget.OnClick(e);
			}

			public void OnCheckedChanged(RadioButton widget, EventArgs e)
			{
				widget.OnCheckedChanged(e);
			}
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		[AutoInitialize(false)]
		public new interface IHandler : TextControl.IHandler
		{
			void Create(RadioButton controller);

			bool Checked { get; set; }
		}
	}
}
