using System;
using Eto.Forms;
using Eto.Drawing;
using swc = Windows.UI.Xaml.Controls;
using swm = Windows.UI.Xaml.Media;
using swi = Windows.UI.Xaml.Input;
using sw = Windows.UI.Xaml;
using wf = Windows.Foundation;

namespace Eto.WinRT.Forms.Controls
{
	/// <summary>
	/// Label handler.
	/// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <copyright>(c) 2012-2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class LabelHandler : WpfCommonControl<swc.TextBlock, Label, Label.ICallback>, Label.IHandler
	{
#if TODO_XAML
		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			Control.TextDecorations = decorations;
		}
#endif

		public LabelHandler ()
		{
			//text = new swc.AccessText();
			Control = new swc.TextBlock
			{
				Padding = new sw.Thickness(0),
			};
			//Control.Target = Control;
			TextAlignment = TextAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
			Wrap = WrapMode.Word;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					// do nothing, label doesn't get updated by the user
					break;

				default:
					base.AttachEvent(id);
					break;
			}
		}

		public TextAlignment TextAlignment
		{
			get { 
#if TODO_XAML
				switch (Control.HorizontalContentAlignment) {
					case sw.HorizontalAlignment.Left:
						return HorizontalAlign.Left;
					case sw.HorizontalAlignment.Right:
						return HorizontalAlign.Right;
					case sw.HorizontalAlignment.Center:
						return HorizontalAlign.Center;
					default:
						throw new NotSupportedException ();
				}
#else
				throw new NotImplementedException();
#endif
			}
			set
			{
#if TODO_XAML
				switch (value) {
					case HorizontalAlign.Center:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
						Control.TextAlignment = sw.TextAlignment.Center;
						break;
					case HorizontalAlign.Left:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Left;
						Control.TextAlignment = sw.TextAlignment.Left;
						break;
					case HorizontalAlign.Right:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Right;
						Control.TextAlignment = sw.TextAlignment.Right;
						break;
					default:
						throw new NotSupportedException();
				}
#endif
			}
		}

		public override Color BackgroundColor
		{
#if TODO_XAML
#else
			get;
			set; 
#endif
		}

		public VerticalAlignment VerticalAlignment
		{
			get
			{
#if TODO_XAML
				switch (Control.VerticalContentAlignment) {
					case sw.VerticalAlignment.Top:
						return VerticalAlign.Top;
					case sw.VerticalAlignment.Bottom:
						return VerticalAlign.Bottom;
					case sw.VerticalAlignment.Center:
						return VerticalAlign.Middle;
					default:
						throw new NotSupportedException ();
				}
#else
				throw new NotImplementedException();
#endif
			}
			set
			{
#if TODO_XAML
				switch (value) {
					case VerticalAlign.Top:
						Control.VerticalContentAlignment = sw.VerticalAlignment.Top;
						break;
					case VerticalAlign.Bottom:
						Control.VerticalContentAlignment = sw.VerticalAlignment.Bottom;
						break;
					case VerticalAlign.Middle:
						Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
						break;
					default:
						throw new NotSupportedException ();
				}
#endif
			}
		}

		public WrapMode Wrap
		{
			get
			{
				switch (Control.TextWrapping) {
					case sw.TextWrapping.NoWrap:
						return WrapMode.None;
					case sw.TextWrapping.Wrap:
						return WrapMode.Word;
#if TODO_XAML
					case sw.TextWrapping.WrapWithOverflow:
						return WrapMode.Character;
#endif
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case WrapMode.Word:
						Control.TextWrapping = sw.TextWrapping.Wrap;
						break;
#if TODO_XAML
					case WrapMode.Character:
						Control.TextWrapping = sw.TextWrapping.WrapWithOverflow;
						break;
#endif
					case WrapMode.None:
						Control.TextWrapping = sw.TextWrapping.NoWrap;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Color TextColor
		{
			get { return Control.Foreground.ToEtoColor(); }
			set { Control.Foreground = value.ToWpfBrush(Control.Foreground); }
		}

		public string Text
		{
#if TODO_XAML
			// TODO: investigate support for accelerator mnemonics in Xaml.
#endif
			get { return Control.Text; }
			set { Control.Text = value; }
		}
	}
}