
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace EmbedMonoMacInEto
{
	public partial class MyNativeView : MonoMac.AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyNativeView(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MyNativeView(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion
	}
}

