using System;
using NUnit.Framework;
using Eto.Forms;
namespace Eto.Test.UnitTests.Forms
{
	/// <summary>
	/// Tests to ensure the DataContext and DataContextChanged event act appropriately
	/// - DataContextChanged should only be fired max once per control, regardless of how or when they are constructed and added to the tree
	/// - Themed controls (visual tree) should not participate in logical tree data context
	/// </summary>
	[TestFixture]
	public class DataContextTests : TestBase
	{
		[Handler(typeof(IHandler))]
		public class CustomExpander : Expander
		{
			public new interface IHandler : Expander.IHandler { }
		}

		public class CustomExpanderHandler : Eto.Forms.ThemedControls.ThemedExpanderHandler, CustomExpander.IHandler
		{
			int dataContextChanged;
			int contentDataContextChanged;
			Panel content;

			class MyViewModel2
			{
			}

			public override Control Content
			{
				get { return content.Content; }
				set { content.Content = value; }
			}

			protected override void Initialize()
			{
				base.Initialize();

				content = new Panel();
				Control.DataContextChanged += (sender, e) => dataContextChanged++;
				content.DataContextChanged += (sender, e) => contentDataContextChanged++;

				base.Content = content;

				Assert.AreEqual(0, dataContextChanged);
				Assert.AreEqual(0, contentDataContextChanged);
				Control.DataContext = new MyViewModel2(); // this shouldn't fire data context changes for logical children
				Assert.AreEqual(1, dataContextChanged);
				Assert.AreEqual(1, contentDataContextChanged);
			}

			public override void OnLoad(EventArgs e)
			{
				base.OnLoad(e);
				Assert.IsInstanceOf<MyViewModel2>(Control.DataContext);
				Assert.IsInstanceOf<MyViewModel2>(content.DataContext);

				Control.DataContext = new MyViewModel2(); // this shouldn't fire data context changes for logical children
				Assert.AreEqual(2, dataContextChanged);
				Assert.AreEqual(2, contentDataContextChanged);
				Assert.IsInstanceOf<MyViewModel2>(Control.DataContext);
				Assert.IsInstanceOf<MyViewModel2>(content.DataContext);
			}

			public override void OnLoadComplete(EventArgs e)
			{
				base.OnLoadComplete(e);
				Assert.AreEqual(2, dataContextChanged);
				Assert.AreEqual(2, contentDataContextChanged);
				Assert.IsInstanceOf<MyViewModel2>(Control.DataContext);
				Assert.IsInstanceOf<MyViewModel2>(content.DataContext);
			}
		}

		static DataContextTests()
		{
			Platform.Instance.Add<CustomExpander.IHandler>(() => new CustomExpanderHandler());
		}

		public class MyViewModel
		{
			public int ID { get; set; }
		}

		public class MyViewModelWithEquals
		{
			public int ID { get; set; }

			public override bool Equals(object obj)
			{
				var model = obj as MyViewModelWithEquals;
				if (model == null)
					return false;

				return ID.Equals(model.ID);
			}

			public override int GetHashCode()
			{
				return ID.GetHashCode();
			}
		}

		[Test]
		public void DataContextChangedShouldNotFireWhenNoContext()
		{
			int dataContextChanged = 0;
			Shown(form =>
			{
				form.DataContextChanged += (sender, e) => dataContextChanged++;
				var c = new Panel();
				c.DataContextChanged += (sender, e) => dataContextChanged++;
				form.Content = c;
				Assert.AreEqual(0, dataContextChanged);
				Assert.IsNull(form.DataContext);
				Assert.IsNull(c.DataContext);
			}, () =>
			{
				Assert.AreEqual(0, dataContextChanged);
			});
		}

		[Test]
		public void DataContextChangedShouldFireAfterSet()
		{
			int dataContextChanged = 0;
			MyViewModel dataContext;
			Shown(form =>
			{
				var c = new Panel();
				c.DataContextChanged += (sender, e) => dataContextChanged++;
				c.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);

				c.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(2, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);

				form.Content = c;
				Assert.AreEqual(2, dataContextChanged);
			}, () =>
			{
				Assert.AreEqual(2, dataContextChanged);
			});
		}

		[Test]
		public void DataContextChangedShouldFireForThemedControl()
		{
			int dataContextChanged = 0;
			MyViewModel dataContext = null;
			Panel c = null;
			Shown(form =>
			{
				c = new Panel();
				c.DataContextChanged += (sender, e) => dataContextChanged++;
				var expander = new CustomExpander();
				expander.Content = c;
				form.Content = expander;
				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(c.DataContext);
				Assert.IsInstanceOf<MyViewModel>(form.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
				Assert.AreSame(dataContext, form.Content.DataContext);
				return form;
			}, form =>
			{
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(c?.DataContext);
				Assert.IsInstanceOf<MyViewModel>(form.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
				Assert.AreSame(dataContext, form.Content.DataContext);
			});
		}

		[Test]
		public void DataContextChangedShouldFireWhenSettingContentAfterLoaded()
		{
			int dataContextChanged = 0;
			int contentDataContextChanged = 0;
			MyViewModel dataContext = null;
			Shown(form =>
			{
				form.DataContextChanged += (sender, e) => dataContextChanged++;
				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(form.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
				return form;
			}, form =>
			{
				var c = new Panel();
				c.DataContextChanged += (sender, e) => contentDataContextChanged++;
				form.Content = c;
				Assert.AreEqual(1, contentDataContextChanged);
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsInstanceOf<MyViewModel>(c.DataContext);
				Assert.IsInstanceOf<MyViewModel>(form.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);

				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(2, contentDataContextChanged);
				Assert.AreEqual(2, dataContextChanged);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
			});
		}

		[Test]
		public void DataContextChangedShouldFireWhenSettingContentAfterLoadedWithThemedControl()
		{
			int dataContextChanged = 0;
			int contentDataContextChanged = 0;
			MyViewModel dataContext = null;
			Shown(form =>
			{
				form.DataContextChanged += (sender, e) => dataContextChanged++;
				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.AreSame(dataContext, form.DataContext);
				return form;
			}, form =>
			{
				var c = new Panel();
				c.DataContextChanged += (sender, e) => contentDataContextChanged++;
				form.Content = new CustomExpander { Content = c };
				Assert.AreEqual(1, contentDataContextChanged);
				Assert.AreEqual(1, dataContextChanged);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);

				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(2, contentDataContextChanged);
				Assert.AreEqual(2, dataContextChanged);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
			});
		}

		[Test]
		public void DataContextChangedShouldFireForChildWithCustomModel()
		{
			int dataContextChanged = 0;
			int childDataContextChanged = 0;
			MyViewModel dataContext;
			MyViewModel childDataContext;
			Shown(form =>
			{
				var container = new Panel();
				container.DataContextChanged += (sender, e) => dataContextChanged++;
				container.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.AreSame(dataContext, container.DataContext);

				var child = new Panel();
				child.DataContextChanged += (sender, e) => childDataContextChanged++;
				child.DataContext = childDataContext = new MyViewModel();
				container.Content = child;
				form.Content = container;

				Assert.AreEqual(1, childDataContextChanged);
				Assert.AreSame(dataContext, container.DataContext);
				Assert.AreSame(childDataContext, child.DataContext);
			}, () =>
			{
				Assert.AreEqual(1, dataContextChanged);
				Assert.AreEqual(1, childDataContextChanged);
			});
		}

		[Test]
		public void DataContextChangeShouldFireForControlInStackLayout()
		{
			int dataContextChanged = 0;
			MyViewModel dataContext = null;
			Panel c = null;
			Shown(form =>
			{
				c = new Panel();
				c.DataContextChanged += (sender, e) => 
					dataContextChanged++;

				form.Content = new StackLayout
				{
					Items = { c }
				};
				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsNotNull(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
				return form;
			}, form =>
			{
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsNotNull(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
			});
		}

		[Test]
		public void DataContextChangeShouldFireForControlInTableLayout()
		{
			int dataContextChanged = 0;
			MyViewModel dataContext = null;
			Panel c = null;
			Shown(form =>
			{
				c = new Panel();
				c.DataContextChanged += (sender, e) => dataContextChanged++;

				form.Content = new TableLayout
				{
					Rows = { c }
				};
				form.DataContext = dataContext = new MyViewModel();
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsNotNull(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
				return form;
			}, form =>
			{
				Assert.AreEqual(1, dataContextChanged);
				Assert.IsNotNull(c.DataContext);
				Assert.AreSame(dataContext, c.DataContext);
				Assert.AreSame(dataContext, form.DataContext);
			});
		}

		/// <summary>
		/// Test to ensure that the DataContextChanged event doesn't fire for child controls that already have a DataContext
		/// defined.  See issue #575.
		/// </summary>
		[Test]
		public void DataContextInSubChildShouldNotBeChangedWhenParentIsSet()
		{
			Invoke(() =>
			{
				int childChanged = 0;
				int parentChanged = 0;
				int subChildChanged = 0;
				var parent = new Panel();
				parent.DataContextChanged += (sender, e) => parentChanged++;
				parent.DataContext = new MyViewModel { ID = 1 };
				Assert.AreEqual(1, parentChanged);

				var subChild = new Panel();
				subChild.DataContextChanged += (sender, e) => subChildChanged++;
				subChild.DataContext = new MyViewModel { ID = 2 };
				Assert.AreEqual(1, subChildChanged);

				var child = new Panel();
				child.DataContextChanged += (sender, e) => childChanged++;
				Assert.AreEqual(0, childChanged);
				child.Content = subChild;
				Assert.AreEqual(1, subChildChanged);
				Assert.AreEqual(0, childChanged);

				parent.Content = child;
				Assert.AreEqual(1, childChanged);
				Assert.AreEqual(1, subChildChanged);
				Assert.AreEqual(1, parentChanged);

				Assert.IsInstanceOf<MyViewModel>(parent.DataContext);
				Assert.AreEqual(1, ((MyViewModel)parent.DataContext).ID);
				Assert.IsInstanceOf<MyViewModel>(child.DataContext);
				Assert.AreEqual(1, ((MyViewModel)child.DataContext).ID);
				Assert.IsInstanceOf<MyViewModel>(subChild.DataContext);
				Assert.AreEqual(2, ((MyViewModel)subChild.DataContext).ID);
			});
		}

		[Test]
		public void DataContextWithEqualsShouldSet()
		{
			Invoke(() =>
			{
				int changed = 0;
				var panel = new Panel();
				panel.DataContextChanged += (sender, e) => changed++;

				panel.DataContext = new MyViewModelWithEquals { ID = 10 };
				Assert.AreEqual(1, changed);

				// should be set again, even though they are equal
				panel.DataContext = new MyViewModelWithEquals { ID = 10 };
				Assert.AreEqual(2, changed);

				panel.DataContext = new MyViewModelWithEquals { ID = 20 };
				Assert.AreEqual(3, changed);
			});
		}
	}
}

