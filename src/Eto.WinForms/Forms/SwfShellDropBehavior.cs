using System;
using sd = System.Drawing;
using swf = System.Windows.Forms;

namespace Eto.WinForms.Forms
{
	public class SwfShellDropBehavior
	{
		bool hasEntered;
		swf.Control _control;

		public SwfShellDropBehavior(swf.Control control)
		{
			_control = control;
			_control.AllowDrop = true;
			_control.DragEnter += Control_DragEnter;
			_control.DragDrop += Control_DragDrop;
			_control.DragOver += Control_DragOver;
			_control.DragLeave += Control_DragLeave;
		}

		public void Detach()
		{
			_control.DragEnter -= Control_DragEnter;
			_control.DragDrop -= Control_DragDrop;
			_control.DragOver -= Control_DragOver;
			_control.DragLeave -= Control_DragLeave;
		}

		private void Control_DragLeave(object sender, EventArgs e)
		{
			var control = (swf.Control)sender;
			if (hasEntered)
				swf.DropTargetHelper.DragLeave(control);
			hasEntered = false;
		}

		sd.Point GetPoint(swf.Control control, swf.DragEventArgs e)
		{
			// TODO: when the application is unaware of DPI, 
			// the image is shown in the wrong spot when dragging for explorer, etc.
			// we need to get the dpi for the control even though we should be unaware of dpi..
			return new sd.Point(e.X, e.Y);
		}

		private void Control_DragOver(object sender, swf.DragEventArgs e)
		{
			var control = (swf.Control)sender;
			if (swf.DropTargetHelper.IsSupported(e.Data))
				swf.DropTargetHelper.DragOver(GetPoint(control, e), e.Effect);
		}

		private void Control_DragDrop(object sender, swf.DragEventArgs e)
		{
			var control = (swf.Control)sender;
			if (swf.DropTargetHelper.IsSupported(e.Data))
			{
				swf.DropTargetHelper.Drop(e.Data, GetPoint(control, e), e.Effect);
				hasEntered = true;
			}
		}

		private void Control_DragEnter(object sender, swf.DragEventArgs e)
		{
			var control = (swf.Control)sender;
			if (swf.DropTargetHelper.IsSupported(e.Data))
			{
				swf.DropTargetHelper.DragEnter(control, e.Data, GetPoint(control, e), e.Effect);
				hasEntered = true;
			}
		}
	}
}
