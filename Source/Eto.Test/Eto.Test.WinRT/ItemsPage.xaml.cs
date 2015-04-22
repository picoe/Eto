using Eto.Test.WinRT.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Eto.Forms;
using Eto.WinRT.Forms;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Eto.Test.WinRT
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
	public sealed partial class ItemsPage : Eto.Test.WinRT.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
			
			if (ToolBarView.IsSupported)
			{
				var toolBarView  = new ToolBarView
				{
					Content = new ToolBar 
					{
						Items = 
						{
							new ButtonToolItem { Text = "Button1", Image = TestIcons.TestImage, ToolTip="Button1" },
							new ButtonToolItem { Text = "Button2", Image = TestIcons.TestImage, ToolTip="Button2" },
							new ButtonToolItem { Text = "Button3", Image = TestIcons.TestImage, ToolTip="Button3" },
							new ButtonToolItem { Text = "Button4", Image = TestIcons.TestImage, ToolTip="Button4" }
						}
					}, Dock = DockPosition.Top
				};

				this.BottomAppBar = toolBarView.ControlObject as CommandBar;
			}
			this.BottomAppBar.IsOpen = true;
			this.BottomAppBar.IsSticky = true;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroups = AllTests.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var groupId = ((TestGroup)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(SplitPage), groupId);
        }
    }
}
