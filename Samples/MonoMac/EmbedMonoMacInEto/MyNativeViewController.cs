
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace EmbedMonoMacInEto
{
	public partial class MyNativeViewController : MonoMac.AppKit.NSViewController
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

