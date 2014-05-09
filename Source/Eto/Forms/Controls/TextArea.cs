using Eto.Drawing;
using System;

namespace Eto.Forms
{
	public interface ITextArea : ITextControl
	{
		bool ReadOnly { get; set; }

		bool Wrap { get; set; }

		void Append(string text, bool scrollToCursor);

		string SelectedText { get; set; }

		Range Selection { get; set; }

		void SelectAll();

		int CaretIndex { get; set; }
	}

	[Handler(typeof(ITextArea))]
	public class TextArea : TextControl
	{
		new ITextArea Handler { get { return (ITextArea)base.Handler; } }

		public static Size DefaultSize = new Size(100, 60);

		#region Events

		public const string SelectionChangedEvent = "TextArea.SelectionChanged";

		public event EventHandler<EventArgs> SelectionChanged
		{
			add { Properties.AddHandlerEvent(SelectionChangedEvent, value); }
			remove { Properties.RemoveEvent(SelectionChangedEvent, value); }
		}

		protected virtual void OnSelectionChanged(EventArgs e)
		{
			Properties.TriggerEvent(SelectionChangedEvent, this, e);
		}

		public const string CaretIndexChangedEvent = "TextArea.CaretIndexChanged";

		public event EventHandler<EventArgs> CaretIndexChanged
		{
			add { Properties.AddHandlerEvent(CaretIndexChangedEvent, value); }
			remove { Properties.RemoveEvent(CaretIndexChangedEvent, value); }
		}

		protected virtual void OnCaretIndexChanged(EventArgs e)
		{
			Properties.TriggerEvent(CaretIndexChangedEvent, this, e);
		}

		#endregion

		static TextArea()
		{
			EventLookup.Register<TextArea>(c => c.OnSelectionChanged(null), TextArea.SelectionChangedEvent);
			EventLookup.Register<TextArea>(c => c.OnCaretIndexChanged(null), TextArea.CaretIndexChangedEvent);
		}

		public TextArea()
		{
		}

		[Obsolete("Use default constructor instead")]
		public TextArea(Generator generator) : this(generator, typeof(ITextArea))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TextArea(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public bool ReadOnly
		{
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}

		public bool Wrap
		{
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}

		public string SelectedText
		{
			get { return Handler.SelectedText; }
			set { Handler.SelectedText = value; }
		}

		public Range Selection
		{
			get { return Handler.Selection; }
			set { Handler.Selection = value; }
		}

		public void SelectAll()
		{
			Handler.SelectAll();
		}

		public int CaretIndex
		{
			get { return Handler.CaretIndex; }
			set { Handler.CaretIndex = value; }
		}

		public void Append(string text, bool scrollToCursor = false)
		{
			Handler.Append(text, scrollToCursor);
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : TextControl.ICallback
		{
			void OnSelectionChanged(TextArea widget, EventArgs e);
			void OnCaretIndexChanged(TextArea widget, EventArgs e);
		}

		protected class Callback : TextControl.Callback, ICallback
		{
			public void OnSelectionChanged(TextArea widget, EventArgs e)
			{
				widget.OnSelectionChanged(e);
			}
			public void OnCaretIndexChanged(TextArea widget, EventArgs e)
			{
				widget.OnCaretIndexChanged(e);
			}
		}
	}
}
