using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Wpf.Drawing;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms.Controls
{
	public class LabelHandler : WpfControl<swc.Label, Label>, ILabel
	{
		swc.AccessText text;

		public class EtoLabel : swc.Label
		{
			protected override void OnAccessKey(swi.AccessKeyEventArgs e)
			{
				// move focus to the next control after the label
				var tRequest = new swi.TraversalRequest(swi.FocusNavigationDirection.Next);
				this.MoveFocus(tRequest);
			}
		}

		protected override void SetDecorations(sw.TextDecorationCollection decorations)
		{
			text.TextDecorations = decorations;
		}

		public LabelHandler ()
		{
			text = new swc.AccessText();
			Control = new EtoLabel
			{
				Padding = new sw.Thickness(0),
				Content = text
			};
			Control.Target = Control;
			HorizontalAlign = HorizontalAlign.Left;
			VerticalAlign = VerticalAlign.Top;
			Wrap = WrapMode.Word;
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case TextControl.TextChangedEvent:
					// do nothing, label doesn't get updated by the user
					break;

				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public HorizontalAlign HorizontalAlign
		{
			get { 
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
			}
			set
			{
				switch (value) {
					case HorizontalAlign.Center:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
						text.TextAlignment = sw.TextAlignment.Center;
						break;
					case HorizontalAlign.Left:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Left;
						text.TextAlignment = sw.TextAlignment.Left;
						break;
					case HorizontalAlign.Right:
						Control.HorizontalContentAlignment = sw.HorizontalAlignment.Right;
						text.TextAlignment = sw.TextAlignment.Right;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public VerticalAlign VerticalAlign
		{
			get
			{
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
			}
			set
			{
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
			}
		}

		public WrapMode Wrap
		{
			get
			{
				switch (text.TextWrapping) {
					case sw.TextWrapping.NoWrap:
						return WrapMode.None;
					case sw.TextWrapping.Wrap:
						return WrapMode.Word;
					case sw.TextWrapping.WrapWithOverflow:
						return WrapMode.Character;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case WrapMode.Word:
						text.TextWrapping = sw.TextWrapping.Wrap;
						break;
					case WrapMode.Character:
						text.TextWrapping = sw.TextWrapping.WrapWithOverflow;
						break;
					case WrapMode.None:
						text.TextWrapping = sw.TextWrapping.NoWrap;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Color TextColor
		{
			get { return text.Foreground.ToEtoColor(); }
			set { text.Foreground = value.ToWpfBrush(text.Foreground); }
		}

		public string Text
		{
			get { return text.Text.ToEtoMneumonic(); }
			set { text.Text = value.ToWpfMneumonic(); ; }
		}
	}
}
