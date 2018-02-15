
namespace EmbedMonoMacInEto
{
	
	// Should subclass MonoMac.AppKit.NSView
	[MonoMac.Foundation.Register("MyNativeView")]
	public partial class MyNativeView
	{
	}
	
	// Should subclass MonoMac.AppKit.NSViewController
	[MonoMac.Foundation.Register("MyNativeViewController")]
	public partial class MyNativeViewController
	{
	}
}

