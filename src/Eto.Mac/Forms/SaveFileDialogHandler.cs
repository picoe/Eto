using System;
using System.IO;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		bool hasShown;
		string selectedFileName;


		public override string FileName
		{
			get => hasShown ? base.FileName : selectedFileName;
			set
			{
				selectedFileName = value;
				var name = value;
				if (!string.IsNullOrEmpty(name))
				{
					var dir = Path.GetDirectoryName(name);
					if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
						Directory = new Uri(dir);
					name = Path.GetFileName(name);
				}
				Control.NameFieldStringValue = name ?? string.Empty;
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
			var result = base.ShowDialog(parent);
			if (result == DialogResult.Ok)
			{
				selectedFileName = null;
				hasShown = true;
			}

			return result;
		}
	}
}
