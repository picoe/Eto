using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Eto.Platform.Mac.Forms.Controls;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{

	public class PasswordBoxHandler : MacText<NSTextField, PasswordBox>, IPasswordBox, ITextBoxWithMaxLength
	{

		class EtoTextField : NSSecureTextField, IMacControl
		{
			object IMacControl.Handler { get { return Handler; } }

			public PasswordBoxHandler Handler { get; set; }
		}
		
		public override bool HasFocus {
			get {
				if (Widget.ParentWindow == null)
					return false;
				return ((IMacWindow)Widget.ParentWindow.Handler).FieldEditorObject == Control;
			}
		}

		public override NSTextField CreateControl()
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

		public Char PasswordChar { // not supported on OSX
			get { return '\0'; }
			set { }
		}
	}
}
