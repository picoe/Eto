namespace Eto.Mac.Forms
{
	public class SaveFileDialogHandler : MacFileDialog<NSSavePanel, SaveFileDialog>, SaveFileDialog.IHandler
	{
		string pendingName;

		public override string FileName
		{
			set
			{
				base.FileName = value;
				SetNameFieldStringValue(Path.GetFileName(value));
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
				if (!HasShown)
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
