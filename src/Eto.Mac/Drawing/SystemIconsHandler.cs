using Eto.Mac.Drawing;

namespace Eto.Mac.Drawing;

public class SystemIconsHandler : SystemIcons.IHandler
{

	public Icon GetFileIcon(string fileName, SystemIconSize size)
	{
		var ws = new NSWorkspace();
		var image = ws.IconForFileType(Path.GetExtension(fileName));
		return WithSize(new Icon(new IconHandler(image)), size);
	}

	public Icon Get(SystemIconType type, SystemIconSize size)
	{
		var icon = type switch
		{
			SystemIconType.OpenDirectory => new Icon(new IconHandler(NSImage.ImageNamed(NSImageName.Folder))),
			SystemIconType.CloseDirectory => new Icon(new IconHandler(NSImage.ImageNamed(NSImageName.Folder))),
			SystemIconType.Question => GetResourceIcon("GenericQuestionMarkIcon.icns"),
			SystemIconType.Error => GetResourceIcon("AlertStopIcon.icns"),
			SystemIconType.Information => GetResourceIcon("AlertNoteIcon.icns"),
			SystemIconType.Warning => new Icon(new IconHandler(NSImage.ImageNamed(NSImageName.Caution))),
			_ => throw new NotSupportedException(),
		};

		return WithSize(icon, size);
	}

	private static Icon WithSize(Icon icon, SystemIconSize size)
	{
		var pixels = size switch
		{
			SystemIconSize.Large => 32,
			SystemIconSize.Small => 16,
			_ => throw new NotSupportedException()
		};

		return icon.WithSize(pixels, pixels);
	}

	private static Icon GetResourceIcon(string name)
	{
		const string basePath = "/System/Library/CoreServices/CoreTypes.bundle/Contents/Resources";
		var path = Path.Combine(basePath, name);
		if (File.Exists(path))
		{
			return new Icon(path);
		}
		return null;
	}
}

