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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
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

			public int MaxLength
			{
				get { return Handler.MaxLength; }
			}

			public EtoSecureTextField()
			{
				Bezeled = true;
				Editable = true;
				Selectable = true;
				Cell.Scrollable = true;
				Cell.Wraps = false;
				Cell.UsesSingleLineMode = true;
				Formatter = new EtoFormatter { Handler = this };
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
