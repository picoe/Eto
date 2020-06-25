using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Eto.Forms;
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
using NSRectEdge = MonoMac.AppKit.NSRectEdge;
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
		static Padding s_ButtonPadding = new Padding(6, 6);
		static int s_ButtonSpacing = 12;

		static readonly object UseContentBorder_Key = new object();

		public bool UseContentBorder
		{
			get => Widget.Properties.Get<bool>(UseContentBorder_Key);
			set
			{
				if (Widget.Properties.TrySet(UseContentBorder_Key, value))
				{
					InvalidateMeasure();
				}
			}
		}

		protected override bool DisposeControl => false;

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

			public override double AnimationResizeTime(CGRect newFrame)
			{
				if (AnimationBehavior == NSWindowAnimationBehavior.None)
					return 0;
				return base.AnimationResizeTime(newFrame);
			}
		}

		SizeF GetButtonSize(SizeF availableSize)
		{
			var buttonSize = SizeF.Empty;
			if (Widget.NegativeButtons.Count > 0 || Widget.PositiveButtons.Count > 0)
			{
				bool addSpacing = false;
				foreach (var button in Widget.NegativeButtons.Concat(Widget.PositiveButtons))
				{
					var preferredSize = button.GetPreferredSize(availableSize);
					if (addSpacing)
						buttonSize.Width += s_ButtonSpacing;
					else
						addSpacing = true;
					buttonSize.Width += preferredSize.Width;
					buttonSize.Height = Math.Max(buttonSize.Height, preferredSize.Height);
				}
				buttonSize += s_ButtonPadding.Size;
			}
			return buttonSize;
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);

			var buttonSize = GetButtonSize(availableSize);
			size.Width = Math.Max(size.Width, buttonSize.Width);
			size.Height += buttonSize.Height;
			return size;
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

		protected override EtoWindow CreateControl() => new EtoDialogWindow();

		protected override void Initialize()
		{
			ConfigureWindow();

			base.Initialize();
		}

		bool ShowAttached
		{
			get
			{
				var owner = Control.OwnerWindow;
				if (owner == null)
					return false;

				if (DisplayMode.HasFlag(DialogDisplayMode.Attached))
					return true;

				if (DisplayMode != DialogDisplayMode.Default)
					return false;

				// if the owner can't become main (e.g. NSPanel), show as attached
				return !owner.CanBecomeMainWindow;
			}
		}

		public virtual void ShowModal()
		{
			session = null;
			EnsureOwner();
			Application.Instance.AsyncInvoke(FireOnShown); // fire after dialog is shown

			Widget.Closed += HandleClosed;
			if (ShowAttached)
				MacModal.RunSheet(Widget, Control, Control.OwnerWindow, out session);
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
			EnsureOwner();

			Widget.Closed += HandleClosed;
			if (ShowAttached)
			{
				MacModal.BeginSheet(Widget, Control, Control.OwnerWindow, out session, () => tcs.SetResult(true));
			}
			else
			{
				Control.MakeKeyWindow();
				Application.Instance.AsyncInvoke(() =>
				{
					Application.Instance.AsyncInvoke(FireOnShown); // fire after dialog is shown
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
			{
				var args = new CancelEventArgs();
				Callback.OnClosing(Widget, args);
				if (!args.Cancel)
				{
					session.Stop();
					Callback.OnClosed(Widget, args);
				}
			}
			else
				base.Close();
		}

		protected override bool DefaultSetAsChildWindow => false;

		public override void SetOwner(Window owner)
		{
			base.SetOwner(owner);
			Control.OwnerWindow = owner.ToNative();
		}

		public void InsertDialogButton(bool positive, int index, Button item)
		{
			Control.ContentView.AddSubview(item.ToNative());
			InvalidateMeasure();
		}

		public void RemoveDialogButton(bool positive, int index, Button item)
		{
			item.ToNative().RemoveFromSuperview();
			InvalidateMeasure();
		}

		protected override CGRect ContentFrame
		{
			get
			{
				var availableSize = Control.ContentView.Frame.Size.ToEto();
				var buttonSize = GetButtonSize(availableSize);

				var frame = base.ContentFrame;
				frame.Y += buttonSize.Height;
				frame.Height -= buttonSize.Height;
				return frame;
			}
		}

		void PositionButtons()
		{
			var contentView = Control.ContentView;
			var availableSize = contentView.Frame.Size.ToEto();
			var point = new PointF(availableSize.Width - s_ButtonPadding.Right, s_ButtonPadding.Bottom);

			var buttonSize = GetButtonSize(availableSize);

			Control.SetContentBorderThickness(UseContentBorder ? buttonSize.Height : 0, NSRectEdge.MinYEdge);

			foreach (var button in Widget.PositiveButtons.Reverse().Concat(Widget.NegativeButtons))
			{
				var ctl = button.GetMacViewHandler();
				var size = ctl.GetPreferredSize(availableSize);
				point.X -= size.Width;
				ctl.SetAlignmentFrame(new CGRect(point.ToNS(), size.ToNS()));
				ctl.ContentControl.AutoresizingMask = NSViewResizingMask.MinXMargin;

				point.X -= s_ButtonSpacing;
			}
		}

		public override void PerformContentLayout()
		{
			base.PerformContentLayout();
			PositionButtons();
		}

		public bool IsDisplayedAsSheet => session?.IsSheet == true;

		/// <summary>
		/// Stop the modal session, run the specified action, then restart the modal session.
		/// </summary>
		/// <remarks>
		/// This is useful when you need to return control to the main (or non-modal) window for a short period of time
		/// without actually closing the dialog, then restarting the modal session when complete.
		/// </remarks>
		/// <param name="action">Action to run after the modal session is stopped</param>
		/// <returns>True if the session was restarted, false if there is no modal session running</returns>
		public virtual bool RestartModal(Action action)
		{
			if (session == null)
				return false;
			session.Restart(action, false);
			return true;
		}

		/// <summary>
		/// Stops the modal session asynchronously, runs the specified action, then restarts the modal session.
		/// </summary>
		/// <remarks>
		/// This is useful when you need to return control to the main (or non-modal) window for a short period of time
		/// without actually closing the dialog, then restarting the modal session when complete.
		///
		/// Note that this method returns immediately and runs the action asynchronously on the UI thread.
		/// </remarks>
		/// <param name="action">Action to run after the modal session is stopped</param>
		/// <returns>True if the session was restarted, false if there is no modal session running</returns>
		public virtual bool RestartModalAsync(Action action)
		{
			if (session == null)
				return false;
			session.Restart(action, true);
			return true;
		}
	}
}
