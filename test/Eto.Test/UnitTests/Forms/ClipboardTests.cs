using NUnit.Framework;
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
			SerializableObject,
			NormalObject,
			UnsafeObject
		}

		const string SampleText = "Hello";
		const string SampleStringType = "eto-string";
		const string SampleDataType = "eto-data";
		const string SampleSerializableObjectType = "eto-serializable-object";
		const string SampleObjectType = "eto-object";
		const string SampleUnsafeObjectType = "eto-unsafe-object";
		const string SampleHtml = "<strong>Some Html</strong>";

		[Serializable]
		public class SerializableObject : ISerializable
		{
			
			public string SomeValue { get; set; }
			public ChildObject ChildObject { get; set; } = new ChildObject();
			public SerializableObject()
			{
			}
			
			public SerializableObject(SerializationInfo info, StreamingContext context)
			{
				SomeValue = info.GetString("SomeValue");
				ChildObject = info.GetValue("Child", typeof(ChildObject)) as ChildObject;
			}
			
			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("SomeValue", SomeValue);
				info.AddValue("Child", ChildObject);
			}
		}
		
		[Serializable]
		public class SomeOtherObject
		{
			public string SomeValue { get; set; }

			public ChildObject ChildObject { get; set; } = new ChildObject();
		}
		
		public class ChildObject
		{
			public bool SomeProperty { get; set; } = new Random().Next() % 2 == 0;
		}

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
				case DataType.SerializableObject:
					CollectionAssert.DoesNotContain(SampleSerializableObjectType, dataObject.Types);
					Assert.IsNull(dataObject.GetObject<SerializableObject>(SampleSerializableObjectType));
					break;
				case DataType.NormalObject:
					CollectionAssert.DoesNotContain(SampleObjectType, dataObject.Types);
					Assert.IsNull(dataObject.GetObject<SomeOtherObject>(SampleObjectType));
					break;
				case DataType.UnsafeObject:
					CollectionAssert.DoesNotContain(SampleUnsafeObjectType, dataObject.Types);
					Assert.IsNull(dataObject.GetObject(SampleUnsafeObjectType));
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
				case DataType.SerializableObject:
					Assert.Contains(SampleSerializableObjectType, dataObject.Types);
					var obj = dataObject.GetObject<SerializableObject>(SampleSerializableObjectType);
					Assert.IsNotNull(obj);
					Assert.AreEqual(obj.SomeValue, SampleText);
					break;
				case DataType.NormalObject:
					Assert.Contains(SampleObjectType, dataObject.Types);
					var obj2 = dataObject.GetObject<SomeOtherObject>(SampleObjectType);
					Assert.IsNotNull(obj2);
					Assert.AreEqual(obj2.SomeValue, SampleText);
					break;
				case DataType.UnsafeObject:
					Assert.Contains(SampleUnsafeObjectType, dataObject.Types);
					var obj3 = dataObject.GetObject(SampleUnsafeObjectType) as SomeOtherObject;
					Assert.IsNotNull(obj3);
					Assert.AreEqual(obj3.SomeValue, SampleText);
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
				case DataType.SerializableObject:
					dataObject.SetObject(new SerializableObject { SomeValue = SampleText }, SampleSerializableObjectType);
					break;
				case DataType.NormalObject:
					dataObject.SetObject(new SomeOtherObject { SomeValue = SampleText }, SampleObjectType);
					break;
				case DataType.UnsafeObject:
					dataObject.SetObject(new SomeOtherObject { SomeValue = SampleText }, SampleUnsafeObjectType);
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
				DataType.Data,
				DataType.SerializableObject,
				DataType.NormalObject,
				DataType.UnsafeObject
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
