using Eto.Forms;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Linq;

#if PORTABLE
using Portable.Xaml;
using Portable.Xaml.Markup;
#else
using System.Xaml;
using System.Windows.Markup;
#endif

namespace Eto.Serialization.Xaml.Extensions
{
	[MarkupExtensionReturnType(typeof(object))]
	[ContentProperty("Default")]
	public class OnExtension : MarkupExtension
	{
		public object Mac { get; set; }

		public object Wpf { get; set; }

		public object WinForms { get; set; }

		public object Gtk { get; set; }

		public object Ios { get; set; }

		public object Android { get; set; }

		public object Windows { get; set; }

		public object Linux { get; set; }

		public object Osx { get; set; }

		public object Desktop { get; set; }

		public object Mobile { get; set; }

		[ConstructorArgument("defaultValue")]
		public object Default { get; set; }

		public OnExtension()
		{
		}

		public OnExtension(object defaultValue)
		{
			Default = defaultValue;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var p = Platform.Instance;
			if (p.IsMac && Mac != null)
				return Mac;
			if (p.IsWpf && Wpf != null)
				return Wpf;
			if (p.IsWinForms && WinForms != null)
				return WinForms;
			if (p.IsGtk && Gtk != null)
				return Gtk;
			if (p.IsIos && Ios != null)
				return Ios;
			if (p.IsAndroid && Android != null)
				return Android;

			var os = EtoEnvironment.Platform;
			if (os.IsMac && Osx != null)
				return Osx;
			if (os.IsWindows && Windows != null)
				return Windows;
			if (os.IsLinux && Linux != null)
				return Linux;

			if (p.IsDesktop && Desktop != null)
				return Desktop;
			if (p.IsMobile && Mobile != null)
				return Mobile;

			return Default;
		}
	}
}