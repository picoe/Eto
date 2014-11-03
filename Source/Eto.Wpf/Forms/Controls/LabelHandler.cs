using System;
using Eto.Forms;
using Eto.Drawing;
using swc = System.Windows.Controls;
using swm = System.Windows.Media;
using swi = System.Windows.Input;
using sw = System.Windows;

namespace Eto.Wpf.Forms.Controls
{
	public class LabelHandler : WpfControl<swc.Label, Label, Label.ICallback>, Label.IHandler
	{
		readonly swc.AccessText text;

		public class EtoLabel : swc.Label
		{
			protected override void OnAccessKey(swi.AccessKeyEventArgs e)
			{
				// move focus to the next control after the label
				var tRequest = new swi.TraversalRequest(swi.FocusNavigationDirection.Next);
				MoveFocus(tRequest);
			}

			protected override sw.Size MeasureOverride(sw.Size constraint)
			{
				var size = base.MeasureOverride(constraint);
				size.Width += 1;
				return size;
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

		public HorizontalAlign HorizontalAlign
		{
			get { return Control.HorizontalContentAlignment.ToEto(); }
			set
			{
				Control.HorizontalContentAlignment = value.ToWpf();
				text.TextAlignment = value.ToWpfTextAlignment();
			}
		}

		public VerticalAlign VerticalAlign
		{
			get { return Control.VerticalContentAlignment.ToEto(); }
			set { Control.VerticalContentAlignment = value.ToWpf(); }
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

		public override Color TextColor
		{
			get { return text.Foreground.ToEtoColor(); }
			set { text.Foreground = value.ToWpfBrush(text.Foreground); }
		}

		public string Text
		{
			get { return text.Text.ToEtoMneumonic(); }
			set { text.Text = value.ToWpfMneumonic(); }
		}
	}
}
