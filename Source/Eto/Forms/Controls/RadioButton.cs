using System;

namespace Eto.Forms
{
	[Handler(typeof(RadioButton.IHandler))]
	public class RadioButton : TextControl
	{
		public event EventHandler<EventArgs> CheckedChanged;
		public event EventHandler<EventArgs> Click;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
		public void OnCheckedChanged (EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged (this, e);
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
		public RadioButton (RadioButton controller = null, Generator generator = null)
			: this(generator, typeof(IHandler), controller)
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected RadioButton (Generator generator, Type type, RadioButton controller, bool initialize = true)
			: base (generator, type, false)
		{
			Handler.Create (controller);
			Initialize();
		}

		public virtual bool Checked {
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		[AutoInitialize(false)]
		public new interface IHandler : TextControl.IHandler
		{
			void Create (RadioButton controller);

			bool Checked { get; set; }
		}
	}
}
