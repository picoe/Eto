using System;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		bool hasShown;

		public override string FileName
		{
			get => hasShown ? base.FileName : Control.NameFieldStringValue;
			set
			{
				Control.NameFieldStringValue = value ?? string.Empty;
				hasShown = false;
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

		public override DialogResult ShowDialog(Window parent)
		{
			hasShown = true;
			var result = base.ShowDialog(parent);
			if (result != DialogResult.Ok)
				hasShown = false;
			return result;
		}
	}
}
