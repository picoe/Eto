using Eto.Drawing;
using System;

namespace Eto.Forms
{
	public interface ITextArea : ITextControl
	{
		bool ReadOnly { get; set; }
		
		bool Wrap { get; set; }
		
		void Append (string text, bool scrollToCursor);

		string SelectedText { get; set; }

		Range Selection { get; set; }

		void SelectAll ();

		int CaretIndex { get; set; }
	}
	
	public class TextArea : TextControl
	{
		new ITextArea Handler { get { return (ITextArea)base.Handler; } }

		public static Size DefaultSize = new Size (100, 60);

		#region Events

		public const string SelectionChangedEvent = "TextArea.SelectionChanged";

		EventHandler<EventArgs> _SelectionChanged;

		public event EventHandler<EventArgs> SelectionChanged {
			add {
				HandleEvent (SelectionChangedEvent);
				_SelectionChanged += value;
			}
			remove { _SelectionChanged -= value; }
		}

		public virtual void OnSelectionChanged (EventArgs e)
		{
			if (_SelectionChanged != null)
				_SelectionChanged (this, e);
		}

		public const string CaretIndexChangedEvent = "TextArea.CaretIndexChanged";

		EventHandler<EventArgs> _CaretIndexChanged;

		public event EventHandler<EventArgs> CaretIndexChanged {
			add {
				HandleEvent (CaretIndexChangedEvent);
				_CaretIndexChanged += value;
			}
			remove { _CaretIndexChanged -= value; }
		}

		public virtual void OnCaretIndexChanged (EventArgs e)
		{
			if (_CaretIndexChanged != null)
				_CaretIndexChanged (this, e);
		}

		#endregion

		static TextArea()
		{
			EventLookup.Register(typeof(TextArea), "OnSelectionChanged", TextArea.SelectionChangedEvent);
			EventLookup.Register(typeof(TextArea), "OnCaretIndexChanged", TextArea.CaretIndexChangedEvent);
		}

		public TextArea ()
			: this (Generator.Current)
		{
		}

		public TextArea (Generator g) : this (g, typeof(ITextArea))
		{
		}
		
		protected TextArea (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public bool ReadOnly {
			get { return Handler.ReadOnly; }
			set { Handler.ReadOnly = value; }
		}
		
		public bool Wrap {
			get { return Handler.Wrap; }
			set { Handler.Wrap = value; }
		}

		public string SelectedText {
			get { return Handler.SelectedText; }
			set { Handler.SelectedText = value; }
		}

		public Range Selection {
			get { return Handler.Selection; }
			set { Handler.Selection = value; }
		}

		public void SelectAll ()
		{
			Handler.SelectAll ();
		}

		public int CaretIndex
		{
			get { return Handler.CaretIndex; }
			set { Handler.CaretIndex = value; }
		}

		public void Append (string text, bool scrollToCursor = false)
		{
			Handler.Append (text, scrollToCursor);
		}
	}
}
