using System;
using Eto.Forms;
using System.Threading.Tasks;
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

namespace Eto.Mac.Forms
{
	public class DialogHandler : MacWindow<EtoWindow, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button defaultButton;
		ModalEventArgs session;

		protected override bool DisposeControl { get { return false; } }

		class EtoDialogWindow : EtoWindow
		{
			public new DialogHandler Handler
			{
				get { return base.Handler as DialogHandler; }
				set { base.Handler = value; }
			}

			public EtoDialogWindow()
				: base(new CGRect(0, 0, 200, 200), NSWindowStyle.Closable | NSWindowStyle.Titled, NSBackingStore.Buffered, false)
			{
			}

			[Export("cancelOperation:")]
			public void CancelOperation(IntPtr sender)
			{
				if (Handler.AbortButton != null)
				{
					var handler = Handler.AbortButton.Handler as IMacViewHandler;
					if (handler != null)
					{
						var callback = handler.Callback as Button.ICallback;
						if (callback != null)
							callback.OnClick(Handler.AbortButton, EventArgs.Empty);
					}
				}
			}
		}

		SizeF GetButtonSize(SizeF availableSize)
		{
			var buttonSize = SizeF.Empty;
			if (Widget.NegativeButtons.Count > 0 || Widget.PositiveButtons.Count > 0)
			{
				foreach (var button in Widget.NegativeButtons.Concat(Widget.PositiveButtons))
				{
					var preferredSize = button.GetPreferredSize(availableSize);
					buttonSize.Width += preferredSize.Width;
					buttonSize.Height = Math.Max(buttonSize.Height, preferredSize.Height);
				}
			}
			return buttonSize;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);

			var buttonSize = GetButtonSize(availableSize);
			size.Width = Math.Max(size.Width, buttonSize.Width);
			size.Height += buttonSize.Height + 2;

			return size;
		}

		protected override CGRect AdjustContent(CGRect rect)
		{
			rect = base.AdjustContent(rect);

			var buttonSize = GetButtonSize(Control.ContentView.Frame.Size.ToEto());
			rect.Height -= buttonSize.Height;
			rect.Y += buttonSize.Height;
			return rect;
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public Button AbortButton { get; set; }

		public Button DefaultButton
		{
			get { return defaultButton; }
			set
			{
				defaultButton = value;
				
				if (defaultButton != null)
				{
					var b = defaultButton.ControlObject as NSButton;
					Control.DefaultButtonCell = b == null ? null : b.Cell;
				}
				else
					Control.DefaultButtonCell = null;
			}
		}

		protected override EtoWindow CreateControl()
		{
			return new EtoDialogWindow();
		}

		protected override void Initialize()
		{
			ConfigureWindow();

			base.Initialize();
		}

		public virtual void ShowModal()
		{
			session = null;
			Callback.OnShown(Widget, EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached) && Widget.Owner != null)
				MacModal.RunSheet(Widget, Control, Widget.Owner.ToNative(), out session);
			else
			{
				Control.MakeKeyWindow();
				MacModal.Run(Widget, Control, out session);
			}
		}

		public virtual Task ShowModalAsync()
		{
			var tcs = new TaskCompletionSource<bool>();
			session = null;
			Callback.OnShown(Widget, EventArgs.Empty);

			Widget.Closed += HandleClosed;
			if (DisplayMode.HasFlag(DialogDisplayMode.Attached) && Widget.Owner != null)
			{
				MacModal.BeginSheet(Widget, Control, Widget.Owner.ToNative(), out session, () => tcs.SetResult(true));
			}
			else
			{
				Control.MakeKeyWindow();
				Application.Instance.AsyncInvoke(() =>
				{
					MacModal.Run(Widget, Control, out session);
					tcs.SetResult(true);
				});

			}
			return tcs.Task;
		}

		void HandleClosed(object sender, EventArgs e)
		{
			if (session != null)
				session.Stop();
			Widget.Closed -= HandleClosed;
		}

		public override void Close()
		{
			if (session != null && session.IsSheet)
				session.Stop();
			else
				base.Close();
		}

		public override void SetOwner(Window owner)
		{
			base.SetOwner(owner);
			Control.OwnerWindow = owner.ToNative();
		}

		public void InsertDialogButton(bool positive, int index, Button item)
		{
			Control.ContentView.AddSubview(item.ToNative());
			PositionButtons();
		}

		public void RemoveDialogButton(bool positive, int index, Button item)
		{
			item.ToNative().RemoveFromSuperview();
			if (Widget.Loaded)
				PositionButtons();
		}

		public override void LayoutParent(bool updateSize = true)
		{
			base.LayoutParent(updateSize);
			if (Widget.Loaded)
				PositionButtons();
		}

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			PositionButtons();
		}

		void PositionButtons()
		{
			var availableSize = Control.ContentView.Frame.Size.ToEto();
			var point = new PointF(availableSize.Width, 0);

			foreach (var button in Widget.PositiveButtons.Reverse().Concat(Widget.NegativeButtons))
			{
				var ctl = button.GetContainerView();
				var size = button.GetPreferredSize(availableSize);
				point.X -= size.Width;
				ctl.Frame = new CGRect(point.ToNS(), size.ToNS());
				ctl.AutoresizingMask = NSViewResizingMask.MinXMargin;
			}
		}
	}
}
