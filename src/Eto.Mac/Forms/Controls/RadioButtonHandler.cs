using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;

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
	public class RadioButtonHandler : MacButton<NSButton, RadioButton, RadioButton.ICallback>, RadioButton.IHandler
	{
		static readonly object RadioGroup_Key = new object();

		RadioGroup Group
		{
			get { return Widget.Properties.Get<RadioGroup>(RadioGroup_Key); }
			set { Widget.Properties[RadioGroup_Key] = value; }
		}

		class RadioGroup : List<RadioButtonHandler>
		{
			object lastChecked;

			public void SetButtonStates(object newButton)
			{
				foreach (var button in this)
				{
					if (ReferenceEquals(button, newButton))
						continue;
					if (button.Checked)
					{
						button.Control.State = NSCellStateValue.Off;
						button.Callback.OnCheckedChanged(button.Widget, EventArgs.Empty);
					}
					else if (ReferenceEquals(lastChecked, button))
					{
						button.Callback.OnCheckedChanged(button.Widget, EventArgs.Empty);
					}
				}
				lastChecked = newButton;
			}
		}

		public class EtoRadioCenteredButtonCell : EtoCenteredButtonCell
		{
			// radio buttons get clipped at the top in small/mini mode using the alignment rects. macOS 10.14.6
			// see Eto.Test.Mac.UnitTests.RadioButtonTests.ButtonShouldNotBeClipped()
			protected override nfloat Offset => ControlSize != NSControlSize.Regular ? 0.5f : 0;

			protected override nfloat GetDefaultHeight()
			{
				switch (ControlSize)
				{
					default:
					case NSControlSize.Regular:
						return 14;
					case NSControlSize.Small:
						return 12;
					case NSControlSize.Mini:
						return 10;
				}
			}
		}

		public class EtoRadioButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public RadioButtonHandler Handler
			{
				get { return WeakHandler?.Target as RadioButtonHandler; }
				set { WeakHandler = new WeakReference(value); }
			}

			static nfloat defaultHeight;
			static EtoRadioButton()
			{
				var b = new EtoRadioButton();
				b.SizeToFit();
				defaultHeight = b.Frame.Height;
			}

			public EtoRadioButton()
			{
				Cell = new EtoRadioCenteredButtonCell();
				Title = string.Empty;
				SetButtonType(NSButtonType.Radio);
			}

			public override bool SendAction(Selector theAction, NSObject theTarget)
			{
				// prevent appkit from deselecting other radio buttons with the same superview/action that may not be in this group.
				// radio buttons may be in different containers!
				Handler?.TriggerClick();
				return true;
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSButton CreateControl() => new EtoRadioButton();

		void TriggerClick()
		{
			TriggerMouseCallback();

			Group.SetButtonStates(this);

			Callback.OnClick(Widget, EventArgs.Empty);
			Callback.OnCheckedChanged(Widget, EventArgs.Empty);

			if (Control.AcceptsFirstResponder())
				Control.Window?.MakeFirstResponder(Control);
		}

		public void Create(RadioButton controller)
		{
			var controllerHandler = controller?.Handler as RadioButtonHandler;
			Group = controllerHandler?.Group ?? new RadioGroup();

			Group.Add(this);
		}

		public bool Checked
		{
			get { return Control.State == NSCellStateValue.On; }
			set
			{
				if (value != Checked)
				{
					Control.State = value ? NSCellStateValue.On : NSCellStateValue.Off;
					if (value)
						Group.SetButtonStates(this);

					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}
	}
}
