namespace Eto;

/// <summary>
/// Operating system platform information
/// </summary>
/// <remarks>
/// Access this information from <see cref="EtoEnvironment.Platform"/>
/// </remarks>
public sealed class OperatingSystemPlatform
{
	/// <summary>
	/// Gets a value indicating that the current .NET runtime is mono
	/// </summary>
	public bool IsMono => _isMono ??= GetIsMono();

	/// <summary>
	/// Gets a value indicating that the current .NET runtime is .NET Core
	/// </summary>
	public bool IsNetCore => _isNetCore ??= GetIsNetCore();

	/// <summary>
	/// Gets a value indicating that the current .NET runtime is .NET Core
	/// </summary>
	public bool IsNetFramework => _isNetFramework ??= GetIsNetFramework();

	/// <summary>
	/// Gets a value indicating that the current OS is windows system
	/// </summary>
	public bool IsWindows => _isWindows ??= GetIsWindows();

	/// <summary>
	/// Gets a value indicating that the current OS is a Windows Runtime (WinRT) system.
	/// </summary>
	public bool IsWinRT => _isWinRT ??= GetIsWinRT();

	/// <summary>
	/// Gets a value indicating that the current OS is a unix-based system
	/// </summary>
	/// <remarks>
	/// This will be true for both Unix (e.g. OS X) and all Linux variants.
	/// </remarks>
	public bool IsUnix => !IsWindows;

	/// <summary>
	/// Gets a value indicating that the current OS is a Mac OS X system
	/// </summary>
	public bool IsMac => _isMac ??= GetIsMac();

	/// <summary>
	/// Gets a value indicating that the current OS is a Linux system
	/// </summary>
	public bool IsLinux => _isLinux ??= GetIsLinux();


	bool? _isMono;
	static bool GetIsMono() => Type.GetType("Mono.Runtime", false) != null || Type.GetType("Mono.Interop.IDispatch", false) != null;

	bool? _isNetCore;
	static bool GetIsNetCore() =>
		RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase)
		|| (
			RuntimeInformation.FrameworkDescription.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase)
			&& !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase)
		);

	bool? _isNetFramework;
	static bool GetIsNetFramework() => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

	bool? _isWindows;
	static bool GetIsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	bool? _isWinRT;
	static bool GetIsWinRT() => Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime", false) != null;

	bool? _isMac;
	static bool GetIsMac() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

	bool? _isLinux;
	static bool GetIsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}