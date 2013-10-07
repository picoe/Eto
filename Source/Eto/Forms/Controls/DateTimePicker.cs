using System;

namespace Eto.Forms
{
	[Flags]
	public enum DateTimePickerMode
	{
		Date = 1,
		Time = 2,
		DateTime = Date | Time
	}

	public interface IDateTimePicker : ICommonControl
	{
		DateTime? Value { get; set; }

		DateTime MinDate { get; set; }

		DateTime MaxDate { get; set; }

		DateTimePickerMode Mode { get; set; }
	}
	
	public class DateTimePicker : CommonControl
	{
		new IDateTimePicker Handler { get { return (IDateTimePicker)base.Handler; } }

		public static DateTimePickerMode DefaultMode = DateTimePickerMode.Date;
		
		public event EventHandler<EventArgs> ValueChanged;
		
		public virtual void OnValueChanged (EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged (this, e);
		}
		
		public DateTimePicker ()
			: this (Generator.Current)
		{
		}
		
		public DateTimePicker (Generator generator)
			: this (generator, typeof(IDateTimePicker))
		{
		}
		
		protected DateTimePicker (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public DateTime MinDate {
			get { return Handler.MinDate; }
			set { Handler.MinDate = value; }
		}

		public DateTime MaxDate {
			get { return Handler.MaxDate; }
			set { Handler.MaxDate = value; }
		}
		
		public DateTime? Value {
			get { return Handler.Value; }
			set { Handler.Value = value; }
		}
		
		public DateTimePickerMode Mode {
			get { return Handler.Mode; }
			set { Handler.Mode = value; }
		}
		
		
	}
}

