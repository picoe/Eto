using Eto.Serialization.Json;
using NUnit.Framework;
using System.Text;
using JsonSerializationException = Newtonsoft.Json.JsonSerializationException;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class JsonLocalAssemblyTests
	{
		const string ChildJson = @"{ ""Content"": { ""$type"": ""Eto.Test.UnitTests.Forms.TestXamlChild"" } }";

		static Stream ToStream(string json) => new MemoryStream(Encoding.UTF8.GetBytes(json));

		[Test]
		public void TypeWithoutAssemblyShouldUseInstanceAssembly()
		{
			TestBase.Invoke(() =>
			{
				var json = @"{ ""Items"": [ { ""$type"": ""Eto.Test.UnitTests.Forms.TestXamlChild"", ""$name"": ""myControl1"" } ] }";
				var parent = JsonReader.Load(ToStream(json), new TestXamlParent());

				Assert.That(parent.myControl1, Is.InstanceOf<TestXamlChild>());
			});
		}

		[Test]
		public void TypeWithoutAssemblyShouldUseSpecifiedAssembly()
		{
			TestBase.Invoke(() =>
			{
				var namespaces = new DefaultNamespaceManager { LocalAssembly = typeof(TestXamlChild).Assembly };
				var panel = JsonReader.Load<Panel>(ToStream(ChildJson), namespaces);

				Assert.That(panel.Content, Is.InstanceOf<TestXamlChild>());
			});
		}

		[Test]
		public void TypeWithoutAssemblyShouldFailWithoutLocalAssembly()
		{
			TestBase.Invoke(() =>
			{
				Assert.That(() => JsonReader.Load<Panel>(ToStream(ChildJson)), Throws.InstanceOf<JsonSerializationException>());
			});
		}

		[Test]
		public void TypeWithoutAssemblyShouldBeFoundInDesignMode()
		{
			TestBase.Invoke(() =>
			{
				var old = Control.IsDesignMode;
				Control.IsDesignMode = true;
				try
				{
					var panel = JsonReader.Load<Panel>(ToStream(ChildJson));

					Assert.That(panel.Content, Is.InstanceOf<TestXamlChild>());
				}
				finally
				{
					Control.IsDesignMode = old;
				}
			});
		}

		[Test]
		public void NamesAfterNestedUserControlShouldBindToOuterInstance()
		{
			TestBase.Invoke(() =>
			{
				var json = @"{ ""Items"": [
					{ ""$type"": ""Eto.Test.UnitTests.Forms.TestJsonDesignUserControl"" },
					{ ""$type"": ""Panel"", ""$name"": ""panel1"" }
				] }";
				var parent = JsonReader.Load(ToStream(json), new TestXamlParent());

				Assert.That(parent.panel1, Is.Not.Null);
			});
		}
	}
}
