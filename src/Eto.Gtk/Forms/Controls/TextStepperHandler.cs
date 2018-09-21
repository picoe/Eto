using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TextStepperHandler : TextBoxHandler<Gtk.SpinButton, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
	{
		#if GTK2
		static Gtk.Adjustment DefaultAdjustment = new Gtk.Adjustment(0, 0, 2, 1, 1, 0);
		#else
		// in gtk3 the upper adjustment is not inclusive?? ugh
		static Gtk.Adjustment DefaultAdjustment = new Gtk.Adjustment(0, 0, 3, 1, 1, 0);
		#endif
		int disableNotification;

		public TextStepperHandler()
		{
			Control = new Gtk.SpinButton(DefaultAdjustment, 0, 1)
			{
				Wrap = true,
				Numeric = false,
				IsEditable = true,
				WidthChars = 12,
				Text = string.Empty,
				UpdatePolicy = Gtk.SpinButtonUpdatePolicy.Always
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Output += Connector.HandleOutput;
			Control.Input += Connector.HandleInput;
			Widget.TextChanging += (sender, e) => e.Cancel = ReadOnly;
		}

		static object ReadOnly_Key = new object();

		public override bool ReadOnly
		{
			get { return Widget.Properties.Get(ReadOnly_Key, false); }
			set { Widget.Properties.Set(ReadOnly_Key, value, false); }
		}

		static object ValidDirection_Key = new object();

		public StepperValidDirections ValidDirection
		{
			get { return Widget.Properties.Get(ValidDirection_Key, StepperValidDirections.Both); }
			set { Widget.Properties.Set(ValidDirection_Key, value, UpdateState, StepperValidDirections.Both); }
		}

		// not supported at the moment.  
		// Could possibly swap out with standard Entry but could be very tricky to setup all events, properties, etc.
		public bool ShowStepper
		{
			get { return true; }
			set { }
		}

		void UpdateState()
		{
			int oldValue = Control.ValueAsInt;
			disableNotification++;
			switch (ValidDirection)
			{
				case StepperValidDirections.Both:
					Control.Wrap = true;
					Control.Adjustment = DefaultAdjustment;
					Control.Value = 0;
					break;
				case StepperValidDirections.Up:
					Control.Wrap = false;
					Control.Adjustment = DefaultAdjustment;
					Control.Value = 0;
					break;
				case StepperValidDirections.Down:
					Control.Wrap = false;
					Control.Adjustment = DefaultAdjustment;
					Control.Value = 2;
					break;
				case StepperValidDirections.None:
					Control.Wrap = false;
					Control.Adjustment = new Gtk.Adjustment(0, 0, 0, 0, 0, 0);
					break;
			}
			disableNotification--;
		}

		StepperDirection? GetDirection()
		{
			StepperDirection? dir = null;
			int? newValue = null;
			switch (ValidDirection)
			{
				case StepperValidDirections.Both:
					dir = Control.ValueAsInt == 1 ? StepperDirection.Up : StepperDirection.Down;
					newValue = 0;
					break;
				case StepperValidDirections.Up:
					dir = StepperDirection.Up;
					newValue = 0;
					break;
				case StepperValidDirections.Down:
					dir = StepperDirection.Down;
					newValue = 2;
					break;
			}
			if (newValue != null && Control.ValueAsInt != newValue.Value)
			{
				disableNotification++;
				Application.Instance.AsyncInvoke (() => {
					Control.Value = newValue.Value;
					disableNotification--;
				});
			}

			return dir;
		}


		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextStepper.StepEvent:
					Control.ValueChanged += Connector.HandleValueChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new TextStepperConnector Connector { get { return (TextStepperConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextStepperConnector();
		}

		protected class TextStepperConnector : TextBoxConnector
		{
			public new TextStepperHandler Handler { get { return (TextStepperHandler)base.Handler; } }

			[GLib.ConnectBefore]
			public void HandleValueChanged(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				if (h.disableNotification > 0)
				{
					//h.disableNotification--;
					return;
				}
				var dir = h.GetDirection();
				if (dir != null)
					h.Callback.OnStep(h.Widget, new StepperEventArgs(dir.Value));
			}

#if GTK2
			public override void HandleExposeEvent(object o, Gtk.ExposeEventArgs args)
			{
				if (args.Event.Window == Handler.Control.GdkWindow.Children[0]) // skip painting over up/down
					return;
				base.HandleExposeEvent(o, args);
			}
#endif

			[GLib.ConnectBefore]
			public void HandleOutput(object o, Gtk.OutputArgs args)
			{
				args.RetVal = 1;
			}

			[GLib.ConnectBefore]
			public void HandleInput(object o, Gtk.InputArgs args)
			{
				args.NewValue = Handler.Control.Value;
				args.RetVal = 1;
			}
		}

	}
}
