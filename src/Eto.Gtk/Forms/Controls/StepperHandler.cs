
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	/// <summary>
	/// Note: not used currently as we can't hide the text input portion of the SpinButton
	/// </summary>
	public class StepperHandler : GtkControl<Gtk.SpinButton, Stepper, Stepper.ICallback>, Stepper.IHandler
	{
#if GTK2
		static Gtk.Adjustment DefaultAdjustment = new Gtk.Adjustment(0, 0, 2, 1, 1, 1);
#else
		// in gtk3 the upper adjustment is not inclusive?? ugh
		static Gtk.Adjustment DefaultAdjustment = new Gtk.Adjustment(0, 0, 3, 1, 1, 1);
#endif
		int disableNotification;

		public StepperHandler()
		{
			Control = new Gtk.SpinButton(DefaultAdjustment, 0, 1)
			{
				Wrap = true,
				Numeric = false,
				IsEditable = false,
				WidthChars = 0,
				Text = string.Empty,
				UpdatePolicy = Gtk.SpinButtonUpdatePolicy.Always
			};
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Output += Connector.HandleOutput;
			Control.Input += Connector.HandleInput;
		}

		static object ValidDirection_Key = new object();

		public StepperValidDirections ValidDirection
		{
			get { return Widget.Properties.Get(ValidDirection_Key, StepperValidDirections.Both); }
			set { Widget.Properties.Set(ValidDirection_Key, value, UpdateState, StepperValidDirections.Both); }
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
				Application.Instance.AsyncInvoke(() =>
				{
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
				case Stepper.StepEvent:
					Control.ValueChanged += Connector.HandleValueChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new StepperConnector Connector { get { return (StepperConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new StepperConnector();
		}

		protected class StepperConnector : GtkControlConnector
		{
			public new StepperHandler Handler { get { return (StepperHandler)base.Handler; } }

			[GLib.ConnectBefore]
			public void HandleValueChanged(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				var dir = h.GetDirection();
				if (dir != null)
					h.Callback.OnStep(h.Widget, new StepperEventArgs(dir.Value));
			}

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