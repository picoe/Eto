namespace Eto.Forms;

/// <summary>
/// Common interface for controls that support mnemonics (e.g. &amp; character in text)
/// </summary>
/// <remarks>
/// Not all platforms support mnemonics, such as macOS.
/// However, you can still show the mnemonic character in the text by using the &amp; character.
/// </remarks>
public interface IMnemonicControl
{
	/// <summary>
	/// Text to display on the control, including mnemonics (e.g. &amp; character to indicate a mnemonic key) when <see cref="UseMnemonic"/> is true.
	/// </summary>
	public string Text { get; set; }

	/// <summary>
	/// Gets or sets whether the control should use mnemonics in the text (e.g. &amp; character to indicate a mnemonic key)
	/// </summary>
	/// <remarks>
	/// This is true by default, and allows the control to process mnemonics in the text with an ampersand character (e.g. "Click &amp;here" will allow the user to press Alt+H to activate the control).
	/// To show a literal ampersand character, you can use a double ampersand (e.g. "Click &amp;&amp; here").
	/// Not all platforms support mnemonics, such as macOS, so even if this is set to true, it may not work as expected on those platforms.
	/// 
	/// Setting this to false will stop the control from processing mnemonics in the text, and you no longer need to use double ampersands to show an ampersand character.
	/// </remarks>
	public bool UseMnemonic { get; set; }

	/// <summary>
	/// Gets or sets whether the mnemonic character should always be shown, even if the user has not pressed the mnemonic key (e.g. Alt key on Windows)
	/// </summary>
	/// <remarks>
	/// Not all platforms support this, but on those that do, it will always show the mnemonic character in the text.
	/// On macOS, this will show the mnemonic character in the text, but it will still not be activated by pressing the mnemonic key (e.g. Alt key).
	/// This is useful if you have other logic hat requires the mnemonic character to be visible, such as for accessibility purposes.
	/// </remarks>
	public bool AlwaysShowMnemonic { get; set; }
}