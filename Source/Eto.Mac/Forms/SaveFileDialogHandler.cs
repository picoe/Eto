using System;
using Eto.Forms;
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
	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		public override string FileName
		{
			get
			{
				return base.FileName;
			}
			set
			{
				Control.NameFieldStringValue = value;
			}
		}

		protected override bool DisposeControl { get { return false; } }

		protected override NSSavePanel CreateControl()
		{
			return NSSavePanel.SavePanel;
		}

		protected override void Initialize()
		{
			Control.AllowsOtherFileTypes = true;
			Control.CanSelectHiddenExtension = true;
			base.Initialize();
		}
	}
}
