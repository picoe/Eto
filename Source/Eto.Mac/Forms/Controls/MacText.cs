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

		Range<int>? LastSelection { get; set; }
	}

	public abstract class MacText<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler, IMacText
		where TControl: NSTextField
		where TWidget: TextControl
		where TCallback: TextControl.ICallback
	{

		public Range<int>? LastSelection { get; set; }

		public Range<int>? InitialSelection { get; set; }

		public override Color BackgroundColor
		{
			get { return Control.BackgroundColor.ToEto(); }
			set
			{ 
				var color = value.ToNSUI();
				Control.BackgroundColor = color;
				Control.DrawsBackground = value.A > 0;
				if (Widget.Loaded && HasFocus)
				{
					var editor = Control.CurrentEditor;
					if (editor != null)
					{
						editor.BackgroundColor = color;
						editor.DrawsBackground = value.A > 0;
					}
				}
			}
		}

		public bool ShowBorder
		{
			get { return Control.Bezeled; }
			set { Control.Bezeled = value; }
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
			get
			{ 
				var editor = Control.CurrentEditor;
				if (editor == null)
					return InitialSelection ?? LastSelection ?? new Range<int>(0, -1);
				return editor.SelectedRange.ToEto(); 
			}
			set
			{
				
				var editor = Control.CurrentEditor;
				if (editor == null)
					InitialSelection = value;
				else
				{
					InitialSelection = null;
					editor.SelectedRange = value.ToNS();
				}
			}
		}

		public TextAlignment TextAlignment
		{
			get { return Control.Alignment.ToEto(); }
			set
			{
				// need to set current editor first when the control has focus, otherwise it won't be reflected for some reason after it loses focus.
				var editor = Control.CurrentEditor;
				if (editor != null)
					editor.Alignment = value.ToNS();
				Control.Alignment = value.ToNS();
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			Widget.GotFocus += Widget_GotFocus;
			SetCustomFieldEditor();
		}

		void Widget_GotFocus(object sender, EventArgs e)
		{
			// set the selected range when the control gets focus
			var editor = Control.CurrentEditor;
			if (InitialSelection != null && editor != null)
			{
				editor.SelectedRange = InitialSelection.Value.ToNS();
				InitialSelection = null;
			}
		}

		public void SelectAll()
		{
			Control.SelectText(Control);
		}

		static readonly IntPtr selResignFirstResponder = Selector.GetHandle("resignFirstResponder");
		static readonly IntPtr selInsertText = Selector.GetHandle("insertText:");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.TextInputEvent:
					SetCustomFieldEditor();
					AddMethod(selInsertText, new Action<IntPtr, IntPtr, IntPtr>(TriggerTextInput), "v@:@", CustomFieldEditor);
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

