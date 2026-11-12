using Eto.Serialization.Json;
using NUnit.Framework;
using System.Text;
using JsonSerializationException = Newtonsoft.Json.JsonSerializationException;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class JsonReferenceTests
	{
		static Stream ToStream(string json) => new MemoryStream(Encoding.UTF8.GetBytes(json));

		static string DialogJson(string button, string reference, bool forward)
		{
			var content = $"Content: {{ $type: 'StackLayout', Items: [ {{ {button}, Text: 'OK' }} ] }}";
			reference = $"DefaultButton: {reference}";
			return forward ? $"{{ {reference}, {content} }}" : $"{{ {content}, {reference} }}";
		}

		static void AssertDefaultButton(Dialog dialog)
		{
			var button = ((StackLayout)dialog.Content).Items[0].Control;
			Assert.That(button, Is.InstanceOf<Button>());
			Assert.That(dialog.DefaultButton, Is.SameAs(button));
		}

		[TestCase("$type: 'Button', $name: 'ok'", "{ $ref: 'ok' }", false)]
		[TestCase("$type: 'Button', $name: 'ok'", "{ $ref: 'ok' }", true)]
		[TestCase("$type: 'Button', $name: 'ok'", "'{x:Reference ok}'", true)]
		[TestCase("$type: 'Button', $name: 'ok'", "'{x:Reference Name=ok}'", true)]
		[TestCase("$type: 'Button', ID: 'ok'", "{ $ref: 'ok' }", true)]
		[TestCase("$type: 'Button', $id: 'ok'", "{ $ref: 'ok' }", true)]
		[TestCase("$id: 'ok', $type: 'Button'", "{ $ref: 'ok' }", false)] // Newtonsoft reads a leading $id itself
		[TestCase("$id: 'ok', $type: 'Button'", "{ $ref: 'ok' }", true)]
		public void ReferenceShouldResolve(string button, string reference, bool forward)
		{
			TestBase.Invoke(() => AssertDefaultButton(JsonReader.Load<Dialog>(ToStream(DialogJson(button, reference, forward)))));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ReferenceShouldResolveWhenLoadingIntoInstance(bool forward)
		{
			TestBase.Invoke(() =>
			{
				var dialog = new Dialog();
				JsonReader.Load(ToStream(DialogJson("$type: 'Button', $name: 'ok'", "{ $ref: 'ok' }", forward)), dialog);
				AssertDefaultButton(dialog);
			});
		}

		[TestCase("{ $ref: 'missing' }")]
		[TestCase("'{x:Reference missing}'")]
		public void MissingReferenceShouldThrow(string reference)
		{
			TestBase.Invoke(() =>
			{
				var ex = Assert.Throws<JsonSerializationException>(() => JsonReader.Load<Dialog>(ToStream(DialogJson("$type: 'Button', $name: 'ok'", reference, false))));
				Assert.That(ex.Message, Does.Contain("'missing'"));
			});
		}

		[Test]
		public void ReferenceWithOtherPropertiesShouldThrow()
		{
			TestBase.Invoke(() =>
			{
				Assert.Throws<JsonSerializationException>(() => JsonReader.Load<Dialog>(ToStream(DialogJson("$type: 'Button', $name: 'ok'", "{ $ref: 'ok', Text: 'x' }", false))));
			});
		}

		[TestCase("{ $ref: 'ok' }", false)]
		[TestCase("{ $ref: 'ok' }", true)]
		[TestCase("'{x:Reference ok}'", true)]
		public void JetoReferenceShouldResolve(string reference, bool forward)
		{
			TestBase.Invoke(() => AssertDefaultButton(Eto.Serialization.Xaml.JetoReader.Load<Dialog>(new StringReader(DialogJson("$type: 'Button', $name: 'ok'", reference, forward)), null)));
		}
	}
}
