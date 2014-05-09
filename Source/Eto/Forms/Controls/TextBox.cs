using System;

namespace Eto.Forms
{
	[Handler(typeof(TextBox.IHandler))]
	public class TextBox : TextControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; }}
		
		public TextBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public TextBox (Generator generator)
			: this(generator, typeof(IHandler))
		{
			
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TextBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public bool ReadOnly {
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}
		
		public int MaxLength {
			get { return Handler.MaxLength; }
			set { Handler.MaxLength = value; }
		}
		
		public string PlaceholderText {
			get { return Handler.PlaceholderText; }
			set { Handler.PlaceholderText = value; }
		}

		public void SelectAll()
		{
			Handler.SelectAll();
		}

		public interface IHandler : TextControl.IHandler
		{
			bool ReadOnly { get; set; }

			int MaxLength { get; set; }

			void SelectAll();

			string PlaceholderText { get; set; }
		}
	}
}
