using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Eto.Forms;

namespace Eto.Test.Sections
{
	public class EventStringWriter : StringWriter
	{
		public event Action<string> Flushed;

		public virtual bool AutoFlush { get; set; }

		public EventStringWriter(bool autoFlush = true)
		{
			AutoFlush = autoFlush;
		}

		public void Clear()
		{
			GetStringBuilder().Clear();
		}

		protected void OnFlush()
		{
			var sb = GetStringBuilder();
			var eh = Flushed;
			if (eh != null)
				eh(sb.ToString());
		}

		public override void Flush()
		{
			base.Flush();
			OnFlush();
		}

		public override void Write(char value)
		{
			base.Write(value);
			if (AutoFlush)
				Flush();
		}

		public override void Write(string value)
		{
			base.Write(value);
			if (AutoFlush)
				Flush();
		}

		public override void Write(char[] buffer, int index, int count)
		{
			base.Write(buffer, index, count);
			if (AutoFlush)
				Flush();
		}
	}

#if !PCL
	public class UnitTestSection : Scrollable
	{
		public UnitTestSection()
		{
			var layout = new DynamicLayout();
			var button = new Button { Text = "Start Tests" };
			layout.Add(null);
			layout.Add(button);
			layout.Add(null);

			Content = layout;

			button.Click += (s, e) =>
			{
				button.Enabled = false;
				var thread = new Thread(() =>
				{
					using (Generator.ThreadStart())
					{
						var oldOut = Console.Out;
						var oldErr = Console.Error;
						var output = new EventStringWriter();
						output.Flushed += val => Application.Instance.Invoke(() =>
						{
							foreach (var line in val.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
								Log.Write(null, line);
							output.Clear();
						});
						Console.SetOut(output);
						Console.SetError(output);
						try
						{
							var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
							#if DESKTOP
							NUnit.ConsoleRunner.Runner.Main(new[]
							{
								"-noshadow",
								"-nothread",
								"-nologo",
								"-nodots",
								"-domain=None",
								"-work=" + dir,
								typeof(Eto.Test.UnitTests.Startup).Assembly.Location
							});
							#endif
						}
						finally
						{
							Console.SetOut(oldOut);
							Console.SetError(oldErr);
							Application.Instance.Invoke(() => button.Enabled = true);
						}
					}
				});
				thread.Start();
			};
		}
	}
#endif
}
