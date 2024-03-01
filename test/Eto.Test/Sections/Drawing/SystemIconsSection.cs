
namespace Eto.Test.Sections.Drawing;

[Section("Drawing", typeof(SystemIcons))]
public class SystemIconsSection : Scrollable
{
	Panel _iconPanel;
	Panel _fileIconPanel;
	SystemIconSize _iconSize;
	private string _filePath;


	IEnumerable<(string name, Icon icon)> GetIcons()
	{
		foreach (var type in Enum.GetValues(typeof(SystemIconType)).OfType<SystemIconType?>())
		{
			yield return (type.ToString(), SystemIcons.Get(type.Value, IconSize));
		}
	}
	
	SystemIconSize IconSize
	{
		get => _iconSize;
		set
		{
			_iconSize = value;
			SetSystemIcons();
			SetFileIcon();
		}
	}
	
	string FilePath
	{
		get => _filePath;
		set
		{
			_filePath = value;
			SetFileIcon();
		}
	}

	public SystemIconsSection()
	{
		_iconPanel = new Panel();
		_fileIconPanel = new Panel();

		var fileSelect = new FilePicker();
		fileSelect.Bind(c => c.FilePath, this, c => c.FilePath);

		var iconSizeDropDown = new EnumDropDown<SystemIconSize>();
		iconSizeDropDown.SelectedValueBinding.Bind(this, c => c.IconSize);

		var layout = new DynamicLayout { Padding = 10, DefaultSpacing = new Size(5, 5) };
		
		layout.BeginCentered();
		layout.AddRow("Size:", iconSizeDropDown);
		layout.EndCentered();

		layout.BeginCentered();
		layout.AddRow("File:", fileSelect);
		layout.EndCentered();
		layout.Add(_fileIconPanel);

		layout.Add(_iconPanel, yscale: true);

		SetSystemIcons();
		Content = layout;
	}
	
	private void SetFileIcon()
	{
		if (FilePath != null && File.Exists(FilePath))
			_fileIconPanel.Content = SystemIcons.GetFileIcon(FilePath, IconSize);
		else
			_fileIconPanel.Content = null;
	}

	private void SetSystemIcons()
	{
		var layout = new DynamicLayout();
		layout.BeginCentered(spacing: new Size(10, 10), yscale: true);

		layout.BeginVertical(spacing: new Size(5, 5));
		layout.BeginHorizontal();
		int count = 0;
		foreach (var type in GetIcons())
		{
			var icon = type.icon;
			var text = type.name;
			var image = new ImageView();
			var label = new Label
			{
				Text = text,
				TextAlignment = TextAlignment.Center
			};
			image.Image = icon;

			layout.Add(new TableLayout(image, label));

			if (count++ > 3)
			{
				count = 0;
				layout.EndBeginHorizontal();
			}
		}
		layout.EndHorizontal();
		layout.EndVertical();

		layout.EndCentered();
		
		_iconPanel.Content = layout;
	}

}
