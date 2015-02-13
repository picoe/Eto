using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{

	public class CustomTextFieldEditor : CustomFieldEditor, IMacControl
	{
		public WeakReference WeakHandler { get; set; }
	}

	public abstract class MacText<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler
		where TControl: NSTextField
		where TWidget: TextControl
		where TCallback: TextControl.ICallback
	{
		public override Color BackgroundColor
		{
			get { return Control.BackgroundColor.ToEto(); }
			set
			{ 
				var color = value.ToNSUI();
				Control.BackgroundColor = color;
				if (Widget.Loaded && HasFocus)
				{
					var editor = Control.CurrentEditor;
					if (editor != null)
					{
						editor.BackgroundColor = color;
						editor.DrawsBackground = true;
					}
				}
			}
		}

		public virtual string Text
		{
			get { return Control.AttributedStringValue.Value; }
			set { Control.AttributedStringValue = Font.AttributedString(value ?? string.Empty, Control.AttributedStringValue); }
		}

		public virtual Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		static readonly object CustomFieldEditorKey = new object();

		public override NSObject CustomFieldEditor { get { return Widget.Properties.Get<NSObject>(CustomFieldEditorKey); } }

		protected override void InnerMapPlatformCommand(string systemAction, Command command, NSObject control)
		{
			if (CustomFieldEditor == null)
			{
				Widget.Properties[CustomFieldEditorKey] = new CustomTextFieldEditor { Widget = Widget, WeakHandler = new WeakReference(this) };
			}
			base.InnerMapPlatformCommand(systemAction, command, CustomFieldEditor);
		}
	}
}

