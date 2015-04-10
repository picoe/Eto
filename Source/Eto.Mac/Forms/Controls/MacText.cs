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

	public interface IMacText
	{
		TextControl.ICallback Callback { get; }
	}

	public abstract class MacText<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler, IMacText
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

		TextControl.ICallback IMacText.Callback
		{
			get { return Callback; }
		}

		public virtual string Text
		{
			get { return Control.AttributedStringValue.Value; }
			set
			{ 
				if (value != Text)
				{
					Control.AttributedStringValue = Font.AttributedString(value ?? string.Empty, Control.AttributedStringValue);
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public virtual Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		public int CaretIndex
		{
			get { return (int)Control.CurrentEditor.SelectedRange.Location; }
			set
			{
				var range = new NSRange(value, 0);
				Control.CurrentEditor.SelectedRange = range;
				Control.CurrentEditor.ScrollRangeToVisible(range);
			}
		}

		public Range<int> Selection
		{
			get { return Control.CurrentEditor.SelectedRange.ToEto(); }
			set { Control.CurrentEditor.SelectedRange = value.ToNS(); }
		}

		static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.TextInputEvent:
					SetCustomFieldEditor();
					AddMethod(selResignFirstResponder, new Action<IntPtr, IntPtr, IntPtr>(TriggerTextInput), "v@:@", CustomFieldEditor);
					break;
				case Eto.Forms.Control.LostFocusEvent:
					SetCustomFieldEditor();
					// lost focus is on the custom field editor, not on the control itself (it loses focus immediately due to the field editor)
					AddMethod(selResignFirstResponder, new Func<IntPtr, IntPtr, bool>(TriggerLostFocus), "B@:", CustomFieldEditor);
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		static readonly object CustomFieldEditorKey = new object();

		public override NSObject CustomFieldEditor { get { return Widget.Properties.Get<NSObject>(CustomFieldEditorKey); } }

		public void SetCustomFieldEditor()
		{
			if (CustomFieldEditor != null)
				return;
			Widget.Properties[CustomFieldEditorKey] = new CustomTextFieldEditor
			{
				WeakHandler = new WeakReference(this)
			};
		}

		public override bool HasFocus
		{
			get
			{
				return (
				    base.HasFocus
				    || (ShouldHaveFocus ?? (CustomFieldEditor != null && Control.Window != null && Control.Window.FirstResponder == CustomFieldEditor))
				);
			}
		}

		protected override void InnerMapPlatformCommand(string systemAction, Command command, NSObject control)
		{
			SetCustomFieldEditor();
			base.InnerMapPlatformCommand(systemAction, command, CustomFieldEditor);
		}
	}
}

