using System;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(ComboBox.IHandler))]
	public class ComboBox : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		public ComboBox()
		{
			Handler.Create(false);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		/// <param name="isEditable">If isEditable=true, the comboBox allow input text.</param>
		public ComboBox(bool isEditable = false)
		{
			Handler.Create(isEditable);
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="isEditable"></param>
		[Obsolete("Use default constructor instead")]
		public ComboBox(Generator generator, bool isEditable = false)
			: this(generator, typeof(IHandler), isEditable)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="isEditable"></param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ComboBox(Generator generator, Type type, bool isEditable = false, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create(isEditable);
			Initialize();
		}

		/// <summary>
		/// Gets or sets the text of the ComboBox.
		/// </summary>
		/// <value>The text content.</value>
		public virtual string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ComboBox"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
			/// <summary>
			/// Used when creating a new instance of the ComboBox to specify isEditable
			/// </summary>
			/// <param name="isEditable">If isEditable=true, the comboBox allow input text.</param>
			void Create(bool isEditable);

			/// <summary>
			/// Gets or sets the text of the ComboBox.
			/// </summary>
			/// <value>The text content.</value>
			string Text { get; set; }
		}
	}
}
