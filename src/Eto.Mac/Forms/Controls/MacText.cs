using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System;
using System.Diagnostics;

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

	public interface IMacText
	{
		void SetLastSelection(Range<int>? range);

		AutoSelectMode AutoSelectMode { get; }
	}

	static class MacText
	{
		public static object AutoSelectMode_Key = new object();
		public static object HasInitialFocus_Key = new object();
	}

	public abstract class MacText<TControl, TWidget, TCallback> : MacControl<TControl, TWidget, TCallback>, TextControl.IHandler, IMacText
		where TControl : NSTextField
		where TWidget : TextControl
		where TCallback : TextControl.ICallback
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

		public virtual string Text
		{
			get { return Control.AttributedStringValue.Value; }
			set
			{
				var oldValue = Control.AttributedStringValue;
				var newText = value ?? string.Empty;
				var oldText = oldValue.Value;
				if (newText != oldText)
				{
					if (TextChanging(oldText, newText))
						return;
					if (HasFont)
						Control.AttributedStringValue = Font.AttributedString(newText, oldValue);
					else
						Control.AttributedStringValue = oldValue.ToMutable(newText);
					
					Callback.OnTextChanged(Widget, EventArgs.Empty);
				}
			}
		}

		protected virtual bool TextChanging(string oldText, string newText)
		{
			return false;
		}

		public virtual Color TextColor
		{
			get { return Control.TextColor.ToEto(); }
			set { Control.TextColor = value.ToNSUI(); }
		}

		public int CaretIndex
		{
			get { return InitialSelection?.Start ?? (int?)Control.CurrentEditor?.SelectedRange.Location ?? LastSelection?.Start ?? 0; }
			set
			{
				var editor = Control.CurrentEditor;
				if (editor == null)
				{
					InitialSelection = new Range<int>(value, value - 1);
					return;
				}
				var range = new NSRange(value, 0);
				editor.SelectedRange = range;
				editor.ScrollRangeToVisible(range);
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

		public AutoSelectMode AutoSelectMode
		{
			get { return Widget.Properties.Get(MacText.AutoSelectMode_Key, AutoSelectMode.OnFocus); }
			set { Widget.Properties.Set(MacText.AutoSelectMode_Key, value, AutoSelectMode.OnFocus); }
		}

		protected override void Initialize()
		{
			base.Initialize();
			Widget.GotFocus += Widget_GotFocus;
			Widget.MouseDown += Widget_MouseDown;
		}

		void Widget_MouseDown(object sender, MouseEventArgs e)
		{
			if (AutoSelectMode == AutoSelectMode.Always)
			{
				SelectAll();
				e.Handled = true;
			}
		}


		bool HasInitialFocus
		{
			get { return Widget.Properties.Get(MacText.HasInitialFocus_Key, false); }
			set { Widget.Properties.Set(MacText.HasInitialFocus_Key, value, false); }
		}

		void Widget_GotFocus(object sender, EventArgs e)
		{
			var editor = Control.CurrentEditor;
			if (editor == null)
				return;

			if (InitialSelection != null)
			{
				editor.SelectedRange = InitialSelection.Value.ToNS();
				InitialSelection = null;
			}
			else if (AutoSelectMode == AutoSelectMode.Never)
			{
				if (LastSelection != null)
				{
					editor.SelectedRange = LastSelection.Value.ToNS();
				}
				else
				{
					var len = Text?.Length ?? 0;
					editor.SelectedRange = new NSRange(0, len);
				}
			}
			HasInitialFocus = true;
		}

		public void SelectAll()
		{
			Selection = new Range<int>(0, (Text?.Length ?? 0) - 1);
		}


		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Eto.Forms.Control.TextInputEvent:
				case Eto.Forms.Control.LostFocusEvent:
					// Handled by MacFieldEditor
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override bool HasFocus
		{
			get
			{
				if (base.HasFocus)
					return true;
				if (ShouldHaveFocus != null)
					return ShouldHaveFocus.Value;
				var fieldEditor = Control.Window?.FirstResponder as MacFieldEditor;
				if (fieldEditor != null)
				{
					return ReferenceEquals(fieldEditor.WeakDelegate, Control)
						|| ReferenceEquals(fieldEditor.Handler, this);
				}
				return false;
			}
		}

		protected override void InnerMapPlatformCommand(string systemAction, Command command, NSObject control)
		{
			var window = Widget.ParentWindow?.Handler as IMacWindow;
			if (window == null)
			{
				Debug.WriteLine("Warning: Cannot map commands to text fields before they have been added to their window");
				return;
			}

			base.InnerMapPlatformCommand(systemAction, command, window.FieldEditor);
		}

		public virtual void SetLastSelection(Range<int>? range)
		{
			if (ShouldHaveFocus != true)
				LastSelection = range;
		}
	}
}

