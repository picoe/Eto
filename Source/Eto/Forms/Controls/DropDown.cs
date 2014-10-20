using System;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(DropDown.IHandler))]
	public class DropDown : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="DropDown"/> class.
		/// </summary>
		public DropDown()
		{
			Handler.Create();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DropDown"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public DropDown(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DropDown"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected DropDown(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create();
			Initialize();
		}

		/// <summary>
		/// Handler interface for the <see cref="DropDown"/>
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : ListControl.IHandler
		{
			/// <summary>
			/// Used when creating a new instance of the DropDown
			/// </summary>
			void Create();
		}
	}
}
