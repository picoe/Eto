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
					Assert.That(clipboard.Text, Is.EqualTo(val));
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
			get
			{
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
					Assert.That(dataObject.ContainsText, Is.False);
					Assert.That(dataObject.Text, Is.Null);
					break;
				case DataType.Html:
					Assert.That(dataObject.ContainsHtml, Is.False);
					Assert.That(dataObject.Html, Is.Null);
					break;
				//case DataType.Icon:
				case DataType.Bitmap:
					Assert.That(dataObject.ContainsImage, Is.False);
					Assert.That(dataObject.Image, Is.Null);
					break;
				case DataType.String:
					Assert.That(dataObject.Types, Does.Not.Contain(SampleStringType));
					Assert.That(dataObject.GetString(SampleStringType), Is.Null);
					break;
				case DataType.Data:
					Assert.That(dataObject.Types, Does.Not.Contain(SampleDataType));
					Assert.That(dataObject.GetData(SampleDataType), Is.Null);
					break;
				case DataType.Uris:
					Assert.That(dataObject.ContainsUris, Is.False);
					Assert.That(dataObject.Uris, Is.Null);
					break;
				case DataType.SerializableObject:
					Assert.That(dataObject.Types, Does.Not.Contain(SampleSerializableObjectType));
					Assert.That(dataObject.GetObject<SerializableObject>(SampleSerializableObjectType), Is.Null);
					break;
				case DataType.NormalObject:
					Assert.That(dataObject.Types, Does.Not.Contain(SampleObjectType));
					Assert.That(dataObject.GetObject<SomeOtherObject>(SampleObjectType), Is.Null);
					break;
				case DataType.UnsafeObject:
					Assert.That(dataObject.Types, Does.Not.Contain(SampleUnsafeObjectType));
					Assert.That(dataObject.GetObject(SampleUnsafeObjectType), Is.Null);
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
					Assert.That(dataObject.ContainsText, Is.True);
					Assert.That(dataObject.Text, Is.Not.Null);
					Assert.That(dataObject.Text, Is.EqualTo(SampleText));
					break;
				case DataType.Html:
					Assert.That(dataObject.ContainsHtml, Is.True);
					Assert.That(dataObject.Html, Is.Not.Null);
					Assert.That(dataObject.Html, Is.EqualTo(SampleHtml));
					break;
				//case DataType.Icon:
				//Assert.That(dataObject.Image, Is.Not.Null);
				//break;
				case DataType.Bitmap:
					Assert.That(dataObject.ContainsImage, Is.True);
					Assert.That(dataObject.Image, Is.Not.Null);
					break;
				case DataType.String:
					Assert.That(dataObject.Types, Contains.Item(SampleStringType));
					Assert.That(dataObject.GetString(SampleStringType), Is.Not.Null);
					Assert.That(dataObject.GetString(SampleStringType), Is.EqualTo(SampleText));
					break;
				case DataType.Data:
					Assert.That(dataObject.Types, Contains.Item(SampleDataType));
					Assert.That(dataObject.GetData(SampleDataType), Is.Not.Null);
					Assert.That(dataObject.GetData(SampleDataType), Is.EqualTo(SampleByteData));
					break;
				case DataType.Uris:
					Assert.That(dataObject.ContainsUris, Is.True);
					Assert.That(dataObject.Uris, Is.Not.Null);
					if (Platform.Instance.IsGtk && EtoEnvironment.Platform.IsMac && dataObject.Uris.Length != SampleBothUris.Length)
						Assert.Warn("Gtk on macOS only returns a single URI for some reason.");
					else
						Assert.That(dataObject.Uris, Is.EquivalentTo(SampleBothUris));
					break;
				case DataType.SerializableObject:
					Assert.That(dataObject.Types, Contains.Item(SampleSerializableObjectType));
					var obj = dataObject.GetObject<SerializableObject>(SampleSerializableObjectType);
					Assert.That(obj, Is.Not.Null);
					Assert.That(SampleText, Is.EqualTo(obj.SomeValue));
					break;
				case DataType.NormalObject:
					Assert.That(dataObject.Types, Contains.Item(SampleObjectType));
					var obj2 = dataObject.GetObject<SomeOtherObject>(SampleObjectType);
					Assert.That(obj2, Is.Not.Null);
					Assert.That(SampleText, Is.EqualTo(obj2.SomeValue));
					break;
				case DataType.UnsafeObject:
					Assert.That(dataObject.Types, Contains.Item(SampleUnsafeObjectType));
					var obj3 = dataObject.GetObject(SampleUnsafeObjectType) as SomeOtherObject;
					Assert.That(obj3, Is.Not.Null);
					Assert.That(SampleText, Is.EqualTo(obj3.SomeValue));
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
						Assert.That(clipboard.Types, Does.Not.Contain("eto-woot"));
						Assert.That(clipboard.Types, Does.Not.Contain("eto-byte-data"));
						Assert.That(clipboard.Text, Is.EqualTo(null));
						Assert.That(clipboard.Html, Is.EqualTo(null));
						Assert.That(clipboard.Image, Is.EqualTo(null));
						Assert.That(clipboard.GetString("eto-woot"), Is.EqualTo(null));
						Assert.That(clipboard.GetData("eto-byte-data"), Is.EqualTo(null));
					}
				});
		}

		[Test]
		public void DisposedClipboardShouldNotBreak() => Async(() =>
		{
			var clipboard1 = new Clipboard();
			clipboard1.Text = "Hello";
			Assert.That(clipboard1.Text, Is.EqualTo("Hello"), "#1");
			clipboard1.Dispose();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var clipboard2 = new Clipboard();
			Assert.That(clipboard2.Text, Is.EqualTo("Hello"), "#1");
			clipboard2.Text = "Hello2";
			Assert.That(clipboard2.Text, Is.EqualTo("Hello2"), "#1");
			clipboard2.Dispose();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			using var clipboard3 = new Clipboard();
			Assert.That(clipboard3.Text, Is.EqualTo("Hello2"), "#1");
			clipboard3.Dispose();
			return Task.CompletedTask;
		});
	}
}
