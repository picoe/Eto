using System;
using System.Linq;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Forms.Controls;
using System.Collections.Generic;

namespace Eto.WinForms.Forms
{

	public abstract class WindowsContainer<TControl, TWidget, TCallback> : WindowsControl<TControl, TWidget, TCallback>, Container.IHandler
		where TControl : swf.Control
		where TWidget : Container
		where TCallback : Control.ICallback
	{

		static readonly object resumeModeKey = new object();
		bool? ResumeMode
		{
			get { return Widget.Properties.Get<bool?>(resumeModeKey); }
			set { Widget.Properties.Set(resumeModeKey, value); }
		}

		protected override void Initialize()
		{
			base.Initialize();
			SuspendControl();
			ResumeMode = true;
		}
		
		public override void OnLoad(EventArgs e)
		{
			var resumeMode = ResumeMode;
			if (resumeMode != null)
			{
				// resume (and perform) layout if needed before we're shown
				ResumeControl(resumeMode.Value);
			}
			base.OnLoad(e);
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			if (ResumeMode == null)
			{
				SuspendControl();
				ResumeMode = true;
			}
		}

		protected virtual void SuspendControl()
		{
#if DEBUG
			if (ResumeMode != null)
				throw new InvalidOperationException("Suspending control when we shouldn't be");
#endif
			Control.SuspendLayout();
		}

		protected virtual void ResumeControl(bool performLayout = true)
		{
#if DEBUG
			if (ResumeMode == null)
				throw new InvalidOperationException("Resuming control when we shouldn't be");
#endif
			ResumeMode = null;
			Control.ResumeLayout(performLayout);
		}

		public override void BeforeAddControl(bool top = true)
		{
			foreach (var h in Widget.VisualControls.Select(r => r.GetWindowsHandler()).Where(r => r != null))
			{
				h.BeforeAddControl(Widget.Loaded);
			}
			if (ResumeMode != null)
			{
				// if we're not the top level control being added, defer when we get loaded
				if (!top && !Widget.Loaded)
				{
					ResumeMode = top;
					return;
				}
				// resume all non-top level controls
				ResumeControl(top);
			}
			base.BeforeAddControl(top);
		}

		public bool RecurseToChildren { get { return true; } }

		public override Size? GetDefaultSize(Size availableSize)
		{
			var container = ContainerControl;
			var min = container.MinimumSize;
			if (min != sd.Size.Empty)
			{
				var parent = container.Parent;
				if (parent != null)
					parent.SuspendLayout();
				container.MinimumSize = sd.Size.Empty;
				var size = container.GetPreferredSize(availableSize.ToSD()).ToEto();
				container.MinimumSize = min;
				if (parent != null)
					parent.ResumeLayout(false);
				return size;
			}
			else
				return ContainerControl.GetPreferredSize(availableSize.ToSD()).ToEto();
		}

		static readonly object enableRedrawDuringSuspendKey = new object();
		public bool EnableRedrawDuringSuspend
		{
			get { return Widget.Properties.Get<bool?>(enableRedrawDuringSuspendKey) ?? false; }
			set { Widget.Properties[enableRedrawDuringSuspendKey] = value ? (bool?)true : null; }
		}

		public override Size GetPreferredSize(Size availableSize, bool useCache)
		{
			var size = base.GetPreferredSize(availableSize, useCache);
			return Size.Max(MinimumSize, size);
		}

		static readonly object minimumSizeKey = new object();
		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(minimumSizeKey) ?? Size.Empty; }
			set
			{
				if (value != MinimumSize)
				{
					Widget.Properties[minimumSizeKey] = value;
					SetMinimumSize(useCache: true);
				}
			}
		}

		static readonly object restoreRedrawKey = new object();

		public override void SuspendLayout()
		{
			base.SuspendLayout();
			if (!EnableRedrawDuringSuspend && Control.IsHandleCreated && EtoEnvironment.Platform.IsWindows)
			{
				Widget.Properties[restoreRedrawKey] = (int)Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, IntPtr.Zero, IntPtr.Zero) == 0;
			}
		}

		public override void ResumeLayout()
		{
			base.ResumeLayout();
			if (Widget.Properties.Get<bool?>(restoreRedrawKey) ?? false)
			{
				Win32.SendMessage(Control.Handle, Win32.WM.SETREDRAW, new IntPtr(1), IntPtr.Zero);
				Control.Refresh();
				Widget.Properties[restoreRedrawKey] = null;
			}
		}

		public override IEnumerable<Control> VisualControls => Widget.Controls;
	}
}
