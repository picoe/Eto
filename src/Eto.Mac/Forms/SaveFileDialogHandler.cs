namespace Eto.Mac.Forms
{
	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		bool hasShown;
		string selectedFileName;
		string pendingName;


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
				SetNameFieldStringValue(name);
				hasShown = false;
			}
		}

		/// <summary>
		/// Sets the name field, updating the allowed file types first as macOS appends the extension
		/// as soon as the name is set and there's no way to undo that afterwards.
		/// </summary>
		void SetNameFieldStringValue(string name)
		{
			pendingName = name;
			SetAllowedFileTypes();
			pendingName = null;
			Control.NameFieldStringValue = name ?? string.Empty;
		}

		internal override List<string> GetNativeFileTypes(List<string> filters)
		{
			// macOS doesn't consider a leading period to be an extension, so NSSavePanel appends the
			// filter's extension to a name like ".gitignore", showing ".gitignore.gitignore".
			// Leave the panel unrestricted in that case so the name is shown as-is, the delegate
			// still filters which files are enabled in the list.
			return IsExtensionOnlyName(pendingName ?? Control.NameFieldStringValue) ? null : filters;
		}

		static bool IsExtensionOnlyName(string name) => !string.IsNullOrEmpty(name) && name[0] == '.' && name.IndexOf('.', 1) < 0;

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

			var fileName = Control.NameFieldStringValue;
			var currentExtension = Path.GetExtension(fileName);

			// If the new file type supports the extension, don't change it
			if (extensions.Select(r => r.TrimStart('*')).Any(r => r == currentExtension))
			{
				if (!hasShown)
				{
					// need to reset the value, otherwise for unknown file types it doubles up the extension
					Control.NameFieldStringValue = string.Empty;
					SetNameFieldStringValue(fileName);
				}
				return;
			}
			var newExtension = extensions.FirstOrDefault()?.TrimStart('*', '.');
			if (!string.IsNullOrEmpty(newExtension) && fileName != null)
			{
				SetNameFieldStringValue($"{Path.GetFileNameWithoutExtension(fileName)}.{newExtension}");
			}
		}
	}
}
