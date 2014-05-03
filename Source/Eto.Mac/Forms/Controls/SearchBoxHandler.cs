using System;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// TODO: Try to eliminate code duplication between this class
	/// and TextBoxHandler. 
	/// </summary>
	public class SearchBoxHandler : MacText<NSSearchField, SearchBox>, ISearchBox, ITextBoxWithMaxLength
	{		
		class EtoTextField : NSSearchField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SearchBoxHandler Handler
			{ 
				get { return (SearchBoxHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow == null)
					return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorObject == Control;
			}
		}

		public SearchBoxHandler()
		{
			Control = new EtoTextField
			{
				Handler = this,
				Bezeled = true,
				Editable = true,
				Selectable = true,
				Formatter = new EtoFormatter { Handler = this }
			};

			Control.Cell.Scrollable = true;
			Control.Cell.Wraps = false;
			Control.Cell.UsesSingleLineMode = true;

			MaxLength = -1;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			size.Width = Math.Max(100, size.Height);
			return size;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += HandleTextChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleTextChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as SearchBoxHandler;
			if (handler != null)
			{
				handler.Widget.OnTextChanged(EventArgs.Empty);
			}
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		public int MaxLength
		{
			get;
			set;
		}

		public string PlaceholderText
		{
			get { return Control.Cell.PlaceholderString; }
			set { Control.Cell.PlaceholderString = value ?? string.Empty; }
		}

		public void SelectAll()
		{
			Control.SelectText(Control);
		}
	}
}
