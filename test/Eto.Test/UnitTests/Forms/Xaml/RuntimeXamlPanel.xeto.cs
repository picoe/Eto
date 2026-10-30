using Eto.Serialization.Xaml;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	/// <summary>Opted out of codegen, so it is parsed at runtime into the fields generated for its names.</summary>
	public partial class RuntimeXamlPanel : Panel
	{
		public RuntimeXamlPanel()
		{
			XamlReader.Load(this);
		}

		public Label Label => NameLabel;
	}
}
