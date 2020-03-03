using System;
using Eto.Forms;
using Eto.Drawing;
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

namespace Eto.Mac.Forms
{
	interface IColorDialogHandler
	{
		Color Color { get; set; }
		ColorDialog Widget { get; }
		NSColorPanel Control { get; }
		ColorDialog.ICallback Callback { get; }

		void OnWillClose(NSNotification notification);
		void OnDidResignKey(NSNotification notification);
		void OnColorChanged();
	}

	class ColorWindowDelegate : NSWindowDelegate
	{
		static readonly NSString s_ColorProperty = new NSString("color");
		const string s_ChangeColorMethodName = "changeColor:";
		static readonly Selector s_selChangeColor = new Selector(s_ChangeColorMethodName);

		static ColorWindowDelegate Instance { get; set; }

		WeakReference handler;
		public IColorDialogHandler Handler { get => (IColorDialogHandler)handler.Target; set => handler = new WeakReference(value); }

		public ColorWindowDelegate(IColorDialogHandler handler)
		{
			Handler = handler;
			Attach();
		}

		[Export(s_ChangeColorMethodName)]
		public void ChangeColor(NSColorPanel panel)
		{
			Handler?.OnColorChanged();

			// the ColorDialog was probably GC'd, so unhook gracefully
			if (Handler == null)
				Detach();
		}

		public override void WillClose(NSNotification notification)
		{
			Handler?.OnWillClose(notification);
			Detach();
		}

		public override void DidResignKey(NSNotification notification)
		{
			Handler?.OnDidResignKey(notification);
			Detach();
		}

		[Export("observeValueForKeyPath:ofObject:change:context:")]
		public void ObserveValueForKeyPath(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (keyPath == s_ColorProperty)
			{
				Handler?.OnColorChanged();

				// the ColorDialog was probably GC'd, so unhook gracefully
				if (Handler == null)
					Detach();
			}
		}

		public void Detach()
		{
			var control = Handler?.Control ?? NSColorPanel.SharedColorPanel;
			if (control.Delegate == this)
			{
				control.Delegate = null;
				control.SetTarget(null);
				control.SetAction(null);
				control.RemoveObserver(this, s_ColorProperty);
			}
			if (ReferenceEquals(this, Instance))
				Instance = null;
		}

		public void Attach()
		{
			Instance?.Detach();

			var control = Handler?.Control ?? NSColorPanel.SharedColorPanel;
			// set delegate so we know when it resigns key or is closed
			control.Delegate = this;
			// set target
			control.SetTarget(this);
			control.SetAction(s_selChangeColor);
			// set KVO observer as target doesn't work when called from a modal dialog
			control.AddObserver(this, s_ColorProperty, NSKeyValueObservingOptions.New, IntPtr.Zero);
			Instance = this;
		}
	}

	public class ColorDialogHandler : MacObject<NSColorPanel, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler, IColorDialogHandler
	{
		ColorWindowDelegate _delegate;
		Color _color = Colors.White;
		Color? _lastColor;

		protected override NSColorPanel CreateControl() => NSColorPanel.SharedColorPanel;

		protected override bool DisposeControl => false;

		public Color Color
		{
			get => _color;
			set
			{
				_color = value;
				_lastColor = value;
			}
		}

		public bool AllowAlpha { get; set; }

		public bool SupportsAllowAlpha => true;

		#region IDialog implementation

		public virtual DialogResult ShowDialog(Window parent)
		{
			_delegate = new ColorWindowDelegate(this);

			Control.WorksWhenModal = true;
			Control.Color = Color.ToNSUI();
			Control.ShowsAlpha = AllowAlpha;

			NSApplication.SharedApplication.OrderFrontColorPanel(Control);

			// we detach when we resign key as there's no other way to know.
			Control.MakeKeyWindow();

			return DialogResult.None; // signal that we are returning right away!
		}

		protected virtual void OnWillClose(NSNotification notification)
		{
			// extension to do custom stuff when the panel is closed
		}

		void IColorDialogHandler.OnWillClose(NSNotification notification) => OnWillClose(notification);

		protected virtual void OnDidResignKey(NSNotification notification)
		{
			// extension to do custom stuff when the panel loses key focus
		}

		void IColorDialogHandler.OnDidResignKey(NSNotification notification) => OnDidResignKey(notification);

		public virtual void OnColorChanged()
		{
			_color = Control.Color.UsingColorSpace(NSColorSpace.DeviceRGB).ToEto(false);
			if (_color == _lastColor)
				return;
			_lastColor = _color;
			Callback.OnColorChanged(Widget, EventArgs.Empty);
		}

		void IColorDialogHandler.OnColorChanged() => OnColorChanged();

		#endregion

		protected override void Dispose(bool disposing)
		{
			_delegate?.Detach();
			_delegate = null;
			base.Dispose(disposing);
		}
	}
}

