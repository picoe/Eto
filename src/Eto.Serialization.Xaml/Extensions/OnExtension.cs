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
#if PORTABLE
	[XamlCodeGen(nameof(Provide))]
#endif
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

#if PORTABLE
		/// <summary>
		/// Gets the value for the current platform, used by compiled xeto in place of <see cref="ProvideValue"/>.
		/// </summary>
		public static T Provide<T>(
			XamlOptional<T> @default = default,
			XamlOptional<T> mac = default,
			XamlOptional<T> wpf = default,
			XamlOptional<T> winForms = default,
			XamlOptional<T> gtk = default,
			XamlOptional<T> ios = default,
			XamlOptional<T> android = default,
			XamlOptional<T> windows = default,
			XamlOptional<T> linux = default,
			XamlOptional<T> osx = default,
			XamlOptional<T> desktop = default,
			XamlOptional<T> mobile = default)
		{
			var p = Platform.Instance;
			if (p.IsMac && mac.HasValue)
				return mac.Value;
			if (p.IsWpf && wpf.HasValue)
				return wpf.Value;
			if (p.IsWinForms && winForms.HasValue)
				return winForms.Value;
			if (p.IsGtk && gtk.HasValue)
				return gtk.Value;
			if (p.IsIos && ios.HasValue)
				return ios.Value;
			if (p.IsAndroid && android.HasValue)
				return android.Value;

			var os = EtoEnvironment.Platform;
			if (os.IsMac && osx.HasValue)
				return osx.Value;
			if (os.IsWindows && windows.HasValue)
				return windows.Value;
			if (os.IsLinux && linux.HasValue)
				return linux.Value;

			if (p.IsDesktop && desktop.HasValue)
				return desktop.Value;
			if (p.IsMobile && mobile.HasValue)
				return mobile.Value;

			return @default.Value;
		}
#endif
	}
}