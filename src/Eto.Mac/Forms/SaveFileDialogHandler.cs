using System;
using Eto.Forms;

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
