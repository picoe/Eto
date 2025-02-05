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
					SetAllowedFileTypes();
					var dir = Path.GetDirectoryName(name);
					if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
						Directory = new Uri(dir);
					name = Path.GetFileName(name);
				}
				Control.NameFieldStringValue = name ?? string.Empty;
				hasShown = false;
			}
		}

		protected override NSSavePanel CreateControl()
		{
			return NSSavePanel.SavePanel;
		}

		protected override void Initialize()
		{
			Control.ExtensionHidden = false;
			Control.AllowsOtherFileTypes = true;
			Control.CanSelectHiddenExtension = true;
			base.Initialize();
		}

		public override DialogResult ShowDialog(Window parent)
		{
			hasShown = true;
			var result = base.ShowDialog(parent);
			if (result == DialogResult.Ok)
			{
				selectedFileName = null;
			}

			return result;
		}

		protected override void OnFileTypeChanged()
		{
			base.OnFileTypeChanged();
			var extensions = Widget.CurrentFilter?.Extensions;
			if (extensions == null)
				return;

			var currentExtension = Path.GetExtension(Control.NameFieldStringValue);

			// If the new file type supports the extension, don't change it
			if (extensions.Select(r => r.TrimStart('*')).Any(r => r == currentExtension))
			{
				if (!hasShown)
				{
					// need to reset the value, otherwise for unknown file types it doubles up the extension
					var name = Control.NameFieldStringValue;
					Control.NameFieldStringValue = string.Empty;
					Control.NameFieldStringValue = name;
				}
				return;
			}
			var newExtension = extensions.FirstOrDefault()?.TrimStart('*', '.');
			if (!string.IsNullOrEmpty(newExtension))
			{
				var fileName = Control.NameFieldStringValue;
				if (fileName != null)
					Control.NameFieldStringValue = $"{Path.GetFileNameWithoutExtension(fileName)}.{newExtension}";
			}			
		}
	}
}
