using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : WpfControl<swc.Button, Button>, IButton
	{
		Image image;
		swc.Image swcimage;
		swc.TextBlock label;
		swc.Grid grid;
		ButtonImagePosition imagePosition;

		public ButtonHandler ()
		{
			Control = new swc.Button ();
			Control.Click += (sender, e) => {
				Widget.OnClick (EventArgs.Empty);
			};
			Control.MinWidth = Button.DefaultSize.Width;
			Control.MinHeight = Button.DefaultSize.Height;
			label = new swc.TextBlock {
				VerticalAlignment = sw.VerticalAlignment.Center,
				HorizontalAlignment = sw.HorizontalAlignment.Center,
				Padding = new sw.Thickness (3, 0, 3, 0),
				Visibility = sw.Visibility.Collapsed
			};
			swc.Grid.SetColumn (label, 1);
			swc.Grid.SetRow (label, 1);
			swcimage = new swc.Image ();
			SetImagePosition ();
			grid = new swc.Grid ();
			grid.ColumnDefinitions.Add (new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			grid.ColumnDefinitions.Add (new swc.ColumnDefinition { Width = new sw.GridLength (1, sw.GridUnitType.Star) });
			grid.ColumnDefinitions.Add (new swc.ColumnDefinition { Width = sw.GridLength.Auto });
			grid.RowDefinitions.Add (new swc.RowDefinition { Height = sw.GridLength.Auto });
			grid.RowDefinitions.Add (new swc.RowDefinition { Height = new sw.GridLength (1, sw.GridUnitType.Star) });
			grid.RowDefinitions.Add (new swc.RowDefinition { Height = sw.GridLength.Auto });
			grid.Children.Add (swcimage);
			grid.Children.Add (label);

			/*
			var panel = new swc.Control { IsTabStop = false };
			panel.HorizontalAlignment = sw.HorizontalAlignment.Stretch;
			panel.VerticalAlignment = sw.VerticalAlignment.Stretch;
			swc.Grid.SetColumn (panel, 1);
			swc.Grid.SetRow (panel, 1);
			grid.Children.Add (panel);
			 * */
			Control.Content = grid;
		}

		public string Text
		{
			get { return Conversions.ConvertMneumonicFromWPF (label.Text); }
			set {
				label.Text = value.ToWpfMneumonic ();
				SetImagePosition ();
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				image = value;
				swcimage.Source = image.ToWpf ();
			}
		}

		void SetImagePosition ()
		{
			bool hideLabel = string.IsNullOrEmpty (label.Text);
			int col, row;
			switch (imagePosition) {
			case ButtonImagePosition.Left:
				col = 0; row = 1;
				Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
				Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
				break;
			case ButtonImagePosition.Right:
				col = 2; row = 1;
				Control.HorizontalContentAlignment = sw.HorizontalAlignment.Stretch;
				Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
				break;
			case ButtonImagePosition.Above:
				col = 1; row = 0;
				Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
				Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
				break;
			case ButtonImagePosition.Below:
				col = 1; row = 2;
				Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
				Control.VerticalContentAlignment = hideLabel ? sw.VerticalAlignment.Center : sw.VerticalAlignment.Stretch;
				break;
			case ButtonImagePosition.Overlay:
				col = 1; row = 1;
				Control.HorizontalContentAlignment = sw.HorizontalAlignment.Center;
				Control.VerticalContentAlignment = sw.VerticalAlignment.Center;
				break;
			default:
				throw new NotSupportedException ();
			}

			swc.Grid.SetColumn (swcimage, col);
			swc.Grid.SetRow (swcimage, row);
			label.Visibility = hideLabel ? sw.Visibility.Collapsed : sw.Visibility.Visible;
		}

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				if (imagePosition != value) {
					imagePosition = value;
					SetImagePosition ();
				}
			}
		}
	}
}
