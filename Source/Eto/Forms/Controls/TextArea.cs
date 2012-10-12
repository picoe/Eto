using Eto.Drawing;
using System;
using System.Collections;

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
		ITextArea handler;

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
			handler = (ITextArea)base.Handler;
		}
		
		public bool ReadOnly {
			get { return handler.ReadOnly; }
			set { handler.ReadOnly = value; }
		}
		
		public bool Wrap {
			get { return handler.Wrap; }
			set { handler.Wrap = value; }
		}

		public string SelectedText {
			get { return handler.SelectedText; }
			set { handler.SelectedText = value; }
		}

		public Range Selection {
			get { return handler.Selection; }
			set { handler.Selection = value; }
		}

		public void SelectAll ()
		{
			handler.SelectAll ();
		}

		public int CaretIndex
		{
			get { return handler.CaretIndex; }
			set { handler.CaretIndex = value; }
		}

		public void Append (string text, bool scrollToCursor = false)
		{
			handler.Append (text, scrollToCursor);
		}
	}
}
