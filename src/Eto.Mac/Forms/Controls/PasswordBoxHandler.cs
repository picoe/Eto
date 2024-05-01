using Eto.Mac.Forms.Controls;
namespace Eto.Mac.Forms.Controls
{
	public class EtoSecureTextFieldCell : NSSecureTextFieldCell, IColorizeCell
	{
		ColorizeView colorize;

		public EtoSecureTextFieldCell()
		{
			StringValue = string.Empty;
		}

		public Color? Color 
		{ 
			get => colorize?.Color;
			set => ColorizeView.Create(ref colorize, value);
		}

		public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
		{
			colorize?.End();
			base.DrawInteriorWithFrame(cellFrame, inView);
		}
		public override void DrawWithFrame(CGRect cellFrame, NSView inView)
		{
			colorize?.Begin(cellFrame, inView);
			base.DrawWithFrame(cellFrame, inView);
		}
	}

	public class PasswordBoxHandler : MacText<NSTextField, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler, ITextBoxWithMaxLength
	{
		public class EtoSecureTextField : NSSecureTextField, IMacControl, ITextBoxWithMaxLength
		{
			public WeakReference WeakHandler { get; set; }

			public PasswordBoxHandler Handler
			{
				get { return (PasswordBoxHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); }
			}

			public int MaxLength => Handler.MaxLength;

			EtoFormatter formatter;

			public EtoSecureTextField(IntPtr handle)
				: base(handle)
			{
			}

			public EtoSecureTextField()
			{
				Cell = new EtoSecureTextFieldCell();
				Bezeled = true;
				Editable = true;
				Selectable = true;
				Cell.Scrollable = true;
				Cell.Wraps = false;
				Cell.UsesSingleLineMode = true;
				Formatter = formatter = new EtoFormatter { Handler = this };
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

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			size.Width = Math.Max(100, size.Height);
			return size;
		}

		protected override NSTextField CreateControl()
		{
			return new EtoSecureTextField();
		}

		protected override void Initialize()
		{
			MaxLength = -1;

			base.Initialize();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += HandleChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static void HandleChanged(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as PasswordBoxHandler;
			if (handler != null)
				handler.Callback.OnTextChanged(handler.Widget, EventArgs.Empty);
		}

		public bool ReadOnly
		{
			get { return !Control.Editable; }
			set { Control.Editable = !value; }
		}

		static readonly object Text_Key = new object();

		protected override bool ControlEnabled
		{
			get => base.ControlEnabled;
			set
			{
				if (!value)
					Widget.Properties.Set(Text_Key, Text);
				base.ControlEnabled = value;
				if (value)
				{
					Text = Widget.Properties.Get<string>(Text_Key);
					Widget.Properties.Set<string>(Text_Key, null);
				}
			}
		}

		public override string Text
		{
			get { return Widget.Properties.Get<string>(Text_Key, base.Text); }
			set
			{
				base.Text = value;
				if (!Enabled)
					Widget.Properties.Set(Text_Key, value);
			}
		}

		public int MaxLength
		{
			get;
			set;
		}

		public Char PasswordChar
		{ // not supported on OSX
			get { return '\0'; }
			set { }
		}
	}
}
