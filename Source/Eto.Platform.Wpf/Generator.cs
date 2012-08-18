using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Drawing;
using System.Text.RegularExpressions;
using Eto.Forms;
using swi = System.Windows.Input;
using swm = System.Windows.Media;
using sw = System.Windows;
using System.Reflection;
using System.IO;

namespace Eto.Platform.Wpf
{
	public class Generator : Eto.Generator
	{
		public override string ID
		{
			get { return Generators.Wpf; }
		}

		static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly> ();

		static Generator ()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
				var assemblyName = new AssemblyName (args.Name);
				if (assemblyName.Name.EndsWith (".resources")) return null;

				string resourceName = "Eto.Platform.Wpf.CustomControls.Assemblies." + assemblyName.Name + ".dll";
				Assembly assembly = null;
				lock (loadedAssemblies) {
					if (!loadedAssemblies.TryGetValue (resourceName, out assembly)) {
						using (var stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream (resourceName)) {
							if (stream != null) {
								using (var binaryReader = new BinaryReader (stream)) {
									assembly = Assembly.Load (binaryReader.ReadBytes ((int)stream.Length));
									loadedAssemblies.Add (resourceName, assembly);
								}
							}
						}
					}
				}
				return assembly;
			};
		}

		public static Size GetSize (sw.FrameworkElement element)
		{
			if (!double.IsNaN(element.ActualWidth) && !double.IsNaN(element.ActualHeight))
				return new Size ((int)element.ActualWidth, (int)element.ActualHeight);
			else
				return new Size ((int)(double.IsNaN(element.Width) ? -1 : element.Width), (int)(double.IsNaN(element.Height) ? -1 : element.Height));
		}

		public static void SetSize (sw.FrameworkElement element, Size size)
		{
			element.Width = size.Width == -1 ? double.NaN : size.Width;
			element.Height = size.Height == -1 ? double.NaN : size.Height;
		}

		public static swm.Color Convert(Color value)
		{
			return swm.Color.FromArgb((byte)(value.A * byte.MaxValue), (byte)(value.R * byte.MaxValue), (byte)(value.G * byte.MaxValue), (byte)(value.B * byte.MaxValue));
		}

		public static Color Convert(swm.Color value)
		{
			return new Color { A = value.A / 255f, R = value.R / 255f, G = value.G / 255f, B = value.B / 255f };
		}

		public static Padding Convert (sw.Thickness value)
		{
			return new Padding ((int)value.Left, (int)value.Top, (int)value.Right, (int)value.Bottom);
		}

		public static sw.Thickness Convert (Padding value)
		{
			return new sw.Thickness (value.Left, value.Top, value.Right, value.Bottom);
		}

		public static Rectangle Convert (sw.Rect value)
		{
			return new Rectangle ((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
		}

		public static sw.Rect Convert (Rectangle value)
		{
			return new sw.Rect (value.X, value.Y, value.Width, value.Height);
		}

		public static Size Convert (sw.Size value)
		{
			return new Size ((int)value.Width, (int)value.Height);
		}

		public static sw.Size Convert (Size value)
		{
			return new sw.Size (value.Width, value.Height);
		}

		public static Point Convert (sw.Point value)
		{
			return new Point ((int)value.X, (int)value.Y);
		}

		public static sw.Point Convert (Point value)
		{
			return new sw.Point (value.X, value.Y);
		}


		public static string ConvertMneumonicToWPF (string value)
		{
			if (value == null) return null;
			return value.Replace ("_", "__").Replace ("&", "_");
		}

		public static string ConvertMneumonicFromWPF (object obj)
		{
			var value = obj as string;
			if (value == null) return null;
			return Regex.Replace (value, "(?<![_])[_]", (match) => { if (match.Value == "__") return "_"; else return "&"; });
		}

		public static KeyPressEventArgs Convert (swi.KeyEventArgs e)
		{
			var key = KeyMap.Convert (e.Key, swi.Keyboard.Modifiers);
			return new KeyPressEventArgs (key) {
				Handled = e.Handled
			};
		}

		public static MouseEventArgs ConvertMouseEvent (sw.IInputElement control, swi.MouseEventArgs e)
		{
			var buttons = MouseButtons.None;
			if (e.LeftButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Primary;
			if (e.RightButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Alternate;
			if (e.MiddleButton == swi.MouseButtonState.Pressed) buttons |= MouseButtons.Middle;
			var modifiers = Key.None;
			var location = Generator.Convert(e.GetPosition(control));

			return new MouseEventArgs(buttons, modifiers, location);
		}

		public static swm.BitmapScalingMode Convert (ImageInterpolation value)
		{
			switch (value) {
			case ImageInterpolation.Default:
				return swm.BitmapScalingMode.Unspecified;
			case ImageInterpolation.None:
				return swm.BitmapScalingMode.NearestNeighbor;
			case ImageInterpolation.Low:
				return swm.BitmapScalingMode.LowQuality;
			case ImageInterpolation.Medium:
				return swm.BitmapScalingMode.HighQuality;
			case ImageInterpolation.High:
				return swm.BitmapScalingMode.HighQuality;
			default:
				throw new NotSupportedException ();
			}
		}

		public static ImageInterpolation Convert (swm.BitmapScalingMode value)
		{
			switch (value) {
			case swm.BitmapScalingMode.HighQuality:
				return ImageInterpolation.High;
			case swm.BitmapScalingMode.LowQuality:
				return ImageInterpolation.Low;
			case swm.BitmapScalingMode.NearestNeighbor:
				return ImageInterpolation.None;
			case swm.BitmapScalingMode.Unspecified:
				return ImageInterpolation.Default;
			default:
				throw new NotSupportedException ();
			}
		}
	}
}
