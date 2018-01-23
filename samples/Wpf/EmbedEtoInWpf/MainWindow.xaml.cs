using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Eto.Forms;

namespace EmbedEtoInWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : System.Windows.Window
	{
		public FrameworkElement EtoControl { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			// Get native control for the panel
			// passing true so that we can embed, otherwise we just get a reference to the control
			var nativeControl = new MyEtoPanel().ToNative(true);

			stackPanel.Children.Add(nativeControl);

				
		}
	}
}
