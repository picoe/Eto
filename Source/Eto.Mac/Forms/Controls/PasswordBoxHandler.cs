using System;
using Eto.Forms;
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class PasswordBoxHandler : MacText<NSTextField, PasswordBox, PasswordBox.ICallback>, PasswordBox.IHandler, ITextBoxWithMaxLength
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

		public PasswordBoxHandler()
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

		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				if (value != Enabled)
				{
					if (!value)
						Widget.Properties.Set(Text_Key, Text);
					base.Enabled = value;
					if (value)
					{
						Text = Widget.Properties.Get<string>(Text_Key);
						Widget.Properties.Set<string>(Text_Key, null);
					}

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
