using System;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(ComboBox.IHandler))]
	public class ComboBox : ListControl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		public ComboBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ComboBox(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ComboBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Handler interface for the <see cref="ComboBox"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
		}
	}
}
