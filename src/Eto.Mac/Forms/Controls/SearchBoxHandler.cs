namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// TODO: Try to eliminate code duplication between this class
	/// and TextBoxHandler. 
	/// </summary>
	public class SearchBoxHandler : MacText<NSSearchField, SearchBox, SearchBox.ICallback>, SearchBox.IHandler, ITextBoxWithMaxLength
	{
		class EtoTextField : NSSearchField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public SearchBoxHandler Handler
			{ 
				get { return (SearchBoxHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoTextField(IntPtr handle) : base(handle)
			{
			}

			public EtoTextField()
			{
				Bezeled = true;
				Editable = true;
				Selectable = true;
				Cell.Scrollable = true;
				Cell.Wraps = false;
				Cell.UsesSingleLineMode = true;
			}
			
			[Export("textViewDidChangeSelection:")]
			public void TextViewDidChangeSelection(NSNotification notification)
			{
				var h = Handler;
				if (h != null)
				{
					var textView = (NSTextView)notification.Object;
					h.SetLastSelection(textView.SelectedRange.ToEto());
				}
			}
			
		}

		public override bool HasFocus
		{
			get
			{
				if (Widget.ParentWindow == null)
					return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorClient == Control;
			}
		}

		protected override NSSearchField CreateControl()
		{
			return new EtoTextField();
		}

		EtoFormatter formatter;

		protected override void Initialize()
		{
			var control = Control;

			control.Formatter = formatter = new EtoFormatter { Handler = this };

			MaxLength = -1;

			base.Initialize();
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
				handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
			}
		}
		
		protected override bool SelectAllOnMouseDown(MouseEventArgs e)
		{
			var cancelRect = Control.Cell.CancelButtonRectForBounds(Control.Bounds).ToEto();
			if (!cancelRect.Contains(e.Location))
				return base.SelectAllOnMouseDown(e);
			return false;
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
	}
}
