using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace EmbedXamarinMacInEto
{
	public partial class MyNativeViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyNativeViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MyNativeViewController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public MyNativeViewController() : base("MyNativeView", NSBundle.MainBundle)
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

		}

		#endregion

		//strongly typed view accessor
		public new MyNativeView View
		{
			get
			{
				return (MyNativeView)base.View;
			}
		}
	}
}
