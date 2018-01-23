using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Eto.Forms;

namespace Eto.Test.WinRT.Data
{
    /// <summary>
    /// A Data source of all the test sections and tests.
    /// </summary>
	/// <copyright>(c) 2014 by Vivek Jhaveri</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public sealed class AllTests
    {
        static AllTests _sampleDataSource = new AllTests();

        ObservableCollection<TestGroup> _allGroups = new ObservableCollection<TestGroup>();
        public ObservableCollection<TestGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<TestGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            return _sampleDataSource.AllGroups;
        }

        public static TestGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static TestItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public AllTests()
        {
			var groupSections = TestSections.Get(TestApplication.DefaultTestAssemblies()).ToList();
			for (var i = 0; i < groupSections.Count; ++i)
			{
				var groupSection = groupSections[i];
				var testSections = groupSection.ToList(); // the children

				var testGroup = new TestGroup(
					uniqueId: "Group-" + i,
					title: groupSection.Text,
					subtitle: string.Format("{0} Tests", testSections.Count),
					imagePath: "Assets/DarkGray.png", 
					description: ""); // TODO: add a Description field to tests.
				AllGroups.Add(testGroup);
				
				for (var j = 0; j < testSections.Count; ++j)
				{
					var testSection = groupSection[j];
					var testItem = new TestItem(
						uniqueId: string.Format("Group-{0}-Item-{1}", i, j),
						title: testSection.Text,
						subtitle: "",
						imagePath: "Assets/LightGray.png",
						description: "",
						content: "Hello!",
						group: testGroup,
						getControl: () => GetControl(testSection));
					testGroup.Items.Add(testItem);
				}
			}
        }

		/// <summary>
		/// Creates a control used to populate the test pane.
		/// This is invoked from SplitPage.xaml via {Binding ContentControl}.
		/// </summary>
		FrameworkElement GetControl(Section testSection)
		{
			FrameworkElement result = null;
			var b = testSection as ISection;
			if (b != null)
			{
				var content = b.CreateContent();
				try
				{
					content.AttachNative();
					result = Eto.WinRT.Forms.ControlExtensions.GetContainerControl(content);
				}
				catch (Exception ex)
				{
					Log.Write(this, "Error loading section: {0}", ex.GetBaseException());
				}
			}
			return result;
		}
    }
}
