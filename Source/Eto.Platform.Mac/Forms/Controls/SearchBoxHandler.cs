using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac.Forms.Controls
{
	/// <summary>
	/// TODO: Try to eliminate code duplication between this class
	/// and TextBoxHandler. 
	/// </summary>
	public class SearchBoxHandler : MacText<NSSearchField, SearchBox>, ISearchBox, ITextBoxWithMaxLength
	{		
		class EtoTextField : NSSearchField, IMacControl
		{
			object IMacControl.Handler { get { return Handler; } }

			public SearchBoxHandler Handler { get; set; }
			
		}
		
		public override bool HasFocus {
			get {
				if (Widget.ParentWindow == null) return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorObject == Control;
			}
		}
		
		public override NSSearchField CreateControl()
		{
			return new EtoTextField {
				Handler = this,
				Bezeled = true,
				Editable = true,
				Selectable = true,
				Formatter = new MyFormatter{ Handler = this }
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Cell.Scrollable = true;
			Control.Cell.Wraps = false;
			Control.Cell.UsesSingleLineMode = true;

			MaxLength = -1;
		}

		protected override Size GetNaturalSize(Size availableSize)
		{
			var size = base.GetNaturalSize (availableSize);
			size.Width = Math.Max (100, size.Height);
			return size;
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case TextArea.TextChangedEvent:
				Control.Changed += delegate {
					Widget.OnTextChanged (EventArgs.Empty);
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public bool ReadOnly {
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}
		
		public int MaxLength {
			get;
			set;
		}
		
		public string PlaceholderText {
			get { return ((NSTextFieldCell)Control.Cell).PlaceholderString; }
			set { ((NSTextFieldCell)Control.Cell).PlaceholderString = value ?? string.Empty; }
		}

		public void SelectAll()
		{
			Control.SelectText (Control);
		}
	}
}
