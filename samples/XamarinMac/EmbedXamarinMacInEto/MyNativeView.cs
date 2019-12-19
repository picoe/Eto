using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace EmbedXamarinMacInEto
{
	public partial class MyNativeView : AppKit.NSView
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
