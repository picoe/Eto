using System;

namespace Eto.Forms
{
	[Handler(typeof(CheckBox.IHandler))]
	public class CheckBox : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when <see cref="Checked"/> property is changed by the user
		/// </summary>
		public event EventHandler<EventArgs> CheckedChanged
		{
			add { Properties.AddEvent(CheckedChangedKey, value); }
			remove { Properties.RemoveEvent(CheckedChangedKey, value); }
		}

		static readonly object CheckedChangedKey = new object();

		/// <summary>
		/// Raises the <see cref="CheckedChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			Properties.TriggerEvent(CheckedChangedKey, this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBox"/> class.
		/// </summary>
		public CheckBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBox"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		[Obsolete("Use default constructor instead")]
		public CheckBox(Generator generator) : this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CheckBox"/> class.
		/// </summary>
		/// <param name="generator">Generator to create the handler</param>
		/// <param name="type">Handler type to create, must be an instance of <see cref="IHandler"/></param>
		/// <param name="initialize">Initialize the handler if true, false if the caller will initialize</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CheckBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the checked state
		/// </summary>
		/// <remarks>
		/// When <see cref="ThreeState"/> is true, null signifies an indeterminate value.
		/// </remarks>
		/// <value>The checked value</value>
		public virtual bool? Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this CheckBox allows three states: true, false, or null
		/// </summary>
		/// <value><c>true</c> if three state; otherwise, <c>false</c>.</value>
		public bool ThreeState
		{
			get { return Handler.ThreeState; }
			set { Handler.ThreeState = value; }
		}

		/// <summary>
		/// Gets a binding for the <see cref="Checked"/> property
		/// </summary>
		/// <value>The binding for the checked property.</value>
		public ObjectBinding<CheckBox, bool?> CheckedBinding
		{
			get
			{
				return new ObjectBinding<CheckBox, bool?>(
					this, 
					c => c.Checked, 
					(c, v) => c.Checked = v, 
					(c, h) => c.CheckedChanged += h, 
					(c, h) => c.CheckedChanged -= h
				);
			}
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : TextControl.ICallback
		{
			void OnCheckedChanged(CheckBox widget, EventArgs e);
		}

		protected class Callback : TextControl.Callback, ICallback
		{
			public void OnCheckedChanged(CheckBox widget, EventArgs e)
			{
				widget.OnCheckedChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="CheckBox"/> control
		/// </summary>
		public interface IHandler : TextControl.IHandler
		{
			/// <summary>
			/// Gets or sets the checked state
			/// </summary>
			/// <remarks>
			/// When <see cref="ThreeState"/> is true, null signifies an indeterminate value.
			/// </remarks>
			/// <value>The checked value</value>
			bool? Checked { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this CheckBox allows three states: true, false, or null
			/// </summary>
			/// <value><c>true</c> if three state; otherwise, <c>false</c>.</value>
			bool ThreeState { get; set; }
		}

	}
}
