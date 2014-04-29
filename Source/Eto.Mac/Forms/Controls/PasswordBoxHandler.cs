using System;
using Eto.Forms;
using MonoMac.AppKit;
using Eto.Mac.Forms.Controls;

namespace Eto.Mac.Forms.Controls
{
	public class PasswordBoxHandler : MacText<NSTextField, PasswordBox>, IPasswordBox, ITextBoxWithMaxLength
	{
		class EtoTextField : NSSecureTextField, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public PasswordBoxHandler Handler
			{ 
				get { return (PasswordBoxHandler)WeakHandler.Target; }
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

		public override NSTextField CreateControl()
		{
			return new EtoTextField
			{
				Handler = this,
				Bezeled = true,
				Editable = true,
				Selectable = true,
				Formatter = new EtoFormatter { Handler = this }
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
				handler.Widget.OnTextChanged(EventArgs.Empty);
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

		public Char PasswordChar
		{ // not supported on OSX
			get { return '\0'; }
			set { }
		}
	}
}
