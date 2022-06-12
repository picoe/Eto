using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;
using System.IO;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ClipboardTests : BaseDataObjectTests<Clipboard>
	{
		protected override bool IsClipboard => true;
	}

	[TestFixture]
	public class DataObjectTests : BaseDataObjectTests<DataObject>
	{
		protected override bool IsClipboard => false;
	}

	public abstract class BaseDataObjectTests<T> : TestBase
		where T : IDataObject, IDisposable, new()
	{
		protected abstract bool IsClipboard { get; }

		[Test]
		public void GettingAndSettingTextShouldNotCrash()
		{
			Invoke(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					// this crashes on WPF on some machines.. don't know why as I can't repro the issue.
					var clipboard = new T();
					var val = "Hello" + i;
					clipboard.Text = val;
					Assert.AreEqual(val, clipboard.Text);
				}
			});
		}

		public enum DataType
		{
			Text,
			Html,
			//Icon, // TODO: not yet implemented fully, if even possible on all platforms
			Bitmap,
			String,
			Data,
			Uris,
		}

		const string SampleText = "Hello";
		const string SampleStringType = "eto-string";
		const string SampleDataType = "eto-data";
		const string SampleHtml = "<strong>Some Html</strong>";

		static byte[] SampleByteData => new byte[] { 10, 20, 30 };

		// note: windows only supports a single web uri, but multiple file uris
		static Uri[] SampleUrlUris => new[] { new Uri("http://google.com") };
		static Uri[] SampleFileUris
		{
			get {
				var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.Documents);
				return new[] { new Uri(path) };
			}
		}

		static Uri[] SampleBothUris => SampleUrlUris.Concat(SampleFileUris).ToArray();

		public static DataType[] GetDataTypes() => Enum.GetValues(typeof(DataType)).Cast<DataType>().ToArray();

		void TestIsNullExcept(T dataObject, params DataType[] except)
		{
			foreach (var testingType in GetDataTypes())
			{
				if (except?.Contains(testingType) == true)
					continue;
				TestIsNull(dataObject, testingType);
			}
		}

		void TestIsNull(T dataObject, DataType type)
		{
			switch (type)
			{
				case DataType.Text:
					Assert.IsFalse(dataObject.ContainsText);
					Assert.IsNull(dataObject.Text);
					break;
				case DataType.Html:
					Assert.IsFalse(dataObject.ContainsHtml);
					Assert.IsNull(dataObject.Html);
					break;
				//case DataType.Icon:
				case DataType.Bitmap:
					Assert.IsFalse(dataObject.ContainsImage);
					Assert.IsNull(dataObject.Image);
					break;
				case DataType.String:
					CollectionAssert.DoesNotContain(SampleStringType, dataObject.Types);
					Assert.IsNull(dataObject.GetString(SampleStringType));
					break;
				case DataType.Data:
					CollectionAssert.DoesNotContain(SampleDataType, dataObject.Types);
					Assert.IsNull(dataObject.GetData(SampleDataType));
					break;
				case DataType.Uris:
					Assert.IsFalse(dataObject.ContainsUris);
					Assert.IsNull(dataObject.Uris);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		void TestValues(T dataObject, params DataType[] types)
		{
			foreach (var type in types)
				TestValue(dataObject, type);
		}

		void TestValue(T dataObject, DataType type)
		{
			switch (type)
			{
				case DataType.Text:
					Assert.IsTrue(dataObject.ContainsText);
					Assert.IsNotNull(dataObject.Text);
					Assert.AreEqual(SampleText, dataObject.Text);
					break;
				case DataType.Html:
					Assert.IsTrue(dataObject.ContainsHtml);
					Assert.IsNotNull(dataObject.Html);
					Assert.AreEqual(SampleHtml, dataObject.Html);
					break;
				//case DataType.Icon:
					//Assert.IsNotNull(dataObject.Image);
					//break;
				case DataType.Bitmap:
					Assert.IsTrue(dataObject.ContainsImage);
					Assert.IsNotNull(dataObject.Image);
					break;
				case DataType.String:
					Assert.Contains(SampleStringType, dataObject.Types);
					Assert.IsNotNull(dataObject.GetString(SampleStringType));
					Assert.AreEqual(SampleText, dataObject.GetString(SampleStringType));
					break;
				case DataType.Data:
					Assert.Contains(SampleDataType, dataObject.Types);
					Assert.IsNotNull(dataObject.GetData(SampleDataType));
					Assert.AreEqual(SampleByteData, dataObject.GetData(SampleDataType));
					break;
				case DataType.Uris:
					Assert.IsTrue(dataObject.ContainsUris);
					Assert.IsNotNull(dataObject.Uris);
					if (Platform.Instance.IsGtk && EtoEnvironment.Platform.IsMac && dataObject.Uris.Length != SampleBothUris.Length)
						Assert.Warn("Gtk on macOS only returns a single URI for some reason.");
					else
						CollectionAssert.AreEquivalent(SampleBothUris, dataObject.Uris);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		void SetValues(T dataObject, params DataType[] types)
		{
			foreach (var type in types)
				SetValue(dataObject, type);
		}

		void SetValue(T dataObject, DataType type)
		{
			switch (type)
			{
				case DataType.Text:
					dataObject.Text = SampleText;
					break;
				case DataType.Html:
					dataObject.Html = SampleHtml;
					break;
				//case DataType.Icon:
					//dataObject.Image = TestIcons.Logo;
					//break;
				case DataType.Bitmap:
					dataObject.Image = TestIcons.TestImage;
					break;
				case DataType.String:
					dataObject.SetString(SampleText, SampleStringType);
					break;
				case DataType.Data:
					dataObject.SetData(SampleByteData, SampleDataType);
					break;
				case DataType.Uris:
					dataObject.Uris = SampleBothUris;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		[TestCaseSource(nameof(GetDataTypes))]
		public void IndividualValuesShouldBeIndependent(DataType property)
		{
			Invoke(() =>
			{
				using (var clipboard = new T())
				{
					SetValue(clipboard, property);
					TestValue(clipboard, property);
					// test all other entries are blank!
					TestIsNullExcept(clipboard, property);
				}
			});
			// if it's a clipboard, test a new instance of the clipboard has the same values that we set.
			if (IsClipboard)
			Invoke(() =>
			{
				using (var clipboard = new T())
				{
					TestValue(clipboard, property);
					TestIsNullExcept(clipboard, property);
				}
			});
		}

		[TestCaseSource(nameof(GetDataTypes))]
		public void ClearingBeforeSettingShouldNotCrash(DataType property)
		{
			Invoke(() =>
			{
				using (var clipboard = new T())
				{
					clipboard.Clear();
					SetValue(clipboard, property);
					TestValue(clipboard, property);
					// test all other entries are blank!
					TestIsNullExcept(clipboard, property);
				}
			});
			// if it's a clipboard, test a new instance of the clipboard has the same values that we set.
			if (IsClipboard)
				Invoke(() =>
				{
					using (var clipboard = new T())
					{
						TestValue(clipboard, property);
						TestIsNullExcept(clipboard, property);
					}
				});
		}


		[Test]
		public void SettingMultipleFormatsShouldWork()
		{
			var typesToTest = new[]
			{
				DataType.Text,
				DataType.Html,
				DataType.String,
				DataType.Data
			};

			Invoke(() =>
			{
				using (var clipboard = new T())
				{
					SetValues(clipboard, typesToTest);
					TestValues(clipboard, typesToTest);

					TestIsNullExcept(clipboard, typesToTest);
				}
			});
			if (IsClipboard)
				Invoke(() =>
				{
					using (var clipboard = new T())
					{
						TestValues(clipboard, typesToTest);
						TestIsNullExcept(clipboard, typesToTest);

						clipboard.Clear();
						TestIsNullExcept(clipboard); // test all!
					}

					using (var clipboard = new T())
					{
						TestIsNullExcept(clipboard);
						CollectionAssert.DoesNotContain("eto-woot", clipboard.Types);
						CollectionAssert.DoesNotContain("eto-byte-data", clipboard.Types);
						Assert.AreEqual(null, clipboard.Text);
						Assert.AreEqual(null, clipboard.Html);
						Assert.AreEqual(null, clipboard.Image);
						Assert.AreEqual(null, clipboard.GetString("eto-woot"));
						Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
					}
				});
		}
	}
}
