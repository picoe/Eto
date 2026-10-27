using NUnit.Framework;

namespace Eto.Test.UnitTests
{
	[Handler(typeof(IHandler))]
	public class ThreadedWidget : Widget
	{
		public new interface IHandler : Widget.IHandler { }
	}

	public class ThreadedWidgetHandler : WidgetHandler<ThreadedWidget>, ThreadedWidget.IHandler
	{
	}

	[TestFixture]
	public class PlatformTests : TestBase
	{
		[Test]
		public void CreatingWidgetsConcurrentlyShouldNotThrow()
		{
			const int threadCount = 16;
			var platform = Platform.Instance;
			var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

			for (int iteration = 0; iteration < 200 && exceptions.IsEmpty; iteration++)
			{
				// re-adding clears the handler cache so every thread hits the cache miss path at once
				platform.Add<ThreadedWidget.IHandler>(() => new ThreadedWidgetHandler());

				using var barrier = new Barrier(threadCount);
				var threads = Enumerable.Range(0, threadCount).Select(i => new Thread(() =>
				{
					try
					{
						using (platform.Context)
						using (platform.ThreadStart())
						{
							barrier.SignalAndWait();
							new ThreadedWidget().Dispose();
						}
					}
					catch (Exception ex)
					{
						exceptions.Enqueue(ex);
					}
				})).ToList();
				threads.ForEach(t => t.Start());
				threads.ForEach(t => t.Join());
			}

			Assert.That(exceptions, Is.Empty);
		}

		[Test, InvokeOnUI]
		public void ReinitializingPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				Platform.Initialize(Platform.Instance.GetType().AssemblyQualifiedName);
			});
		}

		[Test, InvokeOnUI]
		public void ReinitializingPlatformWithCurrentInstanceShouldNotThrowException()
		{
			Platform.Initialize(Platform.Instance);
		}
	}
}
