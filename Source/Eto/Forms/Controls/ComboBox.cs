﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a comboBox to select from a list of items
	/// </summary>
	[Handler(typeof(ComboBox.IHandler))]
	public class ComboBox : DropDown
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="ComboBox"/> class.
		/// </summary>
		public ComboBox()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ComboBox(Generator generator)
			: this(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComboBox"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ComboBox(Generator generator, Type type, bool initialize = true)
			: base(generator, type, false)
		{
		}

		/// <summary>
		/// Gets or sets the text of the DropDown.
		/// </summary>
		/// <value>The text content.</value>
		public virtual string Text
		{
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		/// <summary>
		/// Gets or sets the editable of ComboBox.
		/// </summary>
		public virtual bool IsEditable
		{
			get { return Handler.IsEditable; }
			set { Handler.IsEditable = value; }
		}

		/// <summary>
		/// Handler interface for the <see cref="ComboBox"/>
		/// </summary>
		[AutoInitialize(false)]
		public new interface IHandler : DropDown.IHandler
		{
			/// <summary>
			/// Gets or sets the text of the ComboBox.
			/// </summary>
			/// <value>The text content.</value>
			string Text { get; set; }

			/// <summary>
			/// Gets or sets the editable of ComboBox.
			/// </summary>
			bool IsEditable { get; set; }
		}
	}
}
