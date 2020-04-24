using System;
using Eto.Forms;

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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Forms
{
	public class MessageBoxHandler : MessageBox.IHandler
	{
		public string Text { get; set; }

		public string Caption { get; set; }

		public MessageBoxType Type { get; set; }

		public MessageBoxButtons Buttons { get; set; }

		public MessageBoxDefaultButton DefaultButton { get; set; }

		public DialogResult ShowDialog(Control parent)
		{
			var alert = new NSAlert();

			AddButtons(alert);

			alert.AlertStyle = Convert(Type);
			alert.MessageText = Caption ?? string.Empty;
			alert.InformativeText = Text ?? string.Empty;
			var ret = MacModal.Run(alert, parent);
			switch (Buttons)
			{
				default:
					return DialogResult.Ok;
				case MessageBoxButtons.OKCancel:
					return (ret == 1000) ? DialogResult.Ok : DialogResult.Cancel;
				case MessageBoxButtons.YesNo:
					return (ret == 1000) ? DialogResult.Yes : DialogResult.No;
				case MessageBoxButtons.YesNoCancel:
					return (ret == 1000) ? DialogResult.Yes : (ret == 1001) ? DialogResult.Cancel : DialogResult.No;
			}
		}

		class CancelView : NSView
		{
			public int Code { get; set; }

			public override bool PerformKeyEquivalent(NSEvent theEvent)
			{
				var typed = theEvent.CharactersIgnoringModifiers;
				var mods = theEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask;
				var isCmdDown = mods.HasFlag(NSEventModifierMask.CommandKeyMask);
				if ((mods == 0 && theEvent.KeyCode == 53) || (isCmdDown && typed == "."))
				{
					if (Window.IsSheet)
						NSApplication.SharedApplication.EndSheet(Window, Code);
					else
						NSApplication.SharedApplication.StopModalWithCode(Code);
				}
				return base.PerformKeyEquivalent(theEvent);
			}
		}

		string OkButton => Application.Instance.Localize(this, "OK");
		string CancelButton => Application.Instance.Localize(this, "Cancel");
		string YesButton => Application.Instance.Localize(this, "Yes");
		string NoButton => Application.Instance.Localize(this, "No");

		void AddButtons(NSAlert alert)
		{
			switch (Buttons)
			{
				case MessageBoxButtons.OK:
					alert.AddButton(OkButton);
					break;
				case MessageBoxButtons.OKCancel:
					{
						var ok = alert.AddButton(OkButton);
						var cancel = alert.AddButton(CancelButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								ok.KeyEquivalent = string.Empty;
								cancel.KeyEquivalent = "\r";
								SetResponder(alert, ok);
								SetCancelCode(alert, 1001);
								break;
						}
					}
					break;
				case MessageBoxButtons.YesNo:
					{
						var yes = alert.AddButton(YesButton);
						var no = alert.AddButton(NoButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.No:
							case MessageBoxDefaultButton.Default:
								yes.KeyEquivalent = string.Empty;
								no.KeyEquivalent = "\r";
								SetResponder(alert, yes);
								SetCancelCode(alert, 1001);
								break;
						}
					}
					break;
				case MessageBoxButtons.YesNoCancel:
					{
						var yes = alert.AddButton(YesButton);
						var cancel = alert.AddButton(CancelButton);
						var no = alert.AddButton(NoButton);
						switch (DefaultButton)
						{
							case MessageBoxDefaultButton.Yes:
								break;
							case MessageBoxDefaultButton.No:
								yes.KeyEquivalent = string.Empty;
								no.KeyEquivalent = "\r";
								SetResponder(alert, yes);
								break;
							case MessageBoxDefaultButton.Cancel:
							case MessageBoxDefaultButton.Default:
								yes.KeyEquivalent = string.Empty;
								cancel.KeyEquivalent = "\r";
								SetResponder(alert, yes);
								SetCancelCode(alert, 1001);
								break;
						}
					}
					break;
			}
		}

		private static void SetResponder(NSAlert alert, NSButton button)
		{
			// make the specified button the first responder for the window
			// so the user can press space to select it
			alert.Window.MakeFirstResponder(button);
		}

		private static void SetCancelCode(NSAlert alert, int code)
		{
			// set an accessory view to listen for escape key and cmd+.
			alert.AccessoryView = new CancelView { Code = code };
		}

		static NSAlertStyle Convert(MessageBoxType type)
		{
			switch (type)
			{
				case MessageBoxType.Information:
				case MessageBoxType.Question:
					return NSAlertStyle.Informational;
				case MessageBoxType.Warning:
					return NSAlertStyle.Warning;
				case MessageBoxType.Error:
					return NSAlertStyle.Critical;
				default:
					throw new NotSupportedException();
			}
		}
	}
}
