using System;
using System.Runtime.InteropServices;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class TextStepperHandler : TextBoxBaseHandler<Gtk.Widget, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
    {
        private int _ignoreevents;
        private StepperValidDirections _validDirection;

        public TextStepperHandler()
        {
            Control = new Gtk.Widget(GtkWrapper.gtk_spin_button_new_with_range(0, 2, 1));
            GtkWrapper.gtk_spin_button_set_numeric(Control.Handle, false);
            ReadOnly = false;
            ValidDirection = StepperValidDirections.Both;

            ConnectSignal("input", (Func<IntPtr, IntPtr, IntPtr, int>)HandleInput, true);
            ConnectSignal("output", (Func<IntPtr, IntPtr, bool>)HandleOutput, true);
            ConnectSignal("value-changed", (Action<IntPtr, IntPtr>)HandleValueChanged);

            Text = "";
        }

        private static int HandleInput(IntPtr spin_button, IntPtr new_value, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as TextStepperHandler;
            var newalue = handler.GetValue();
            var bytes = BitConverter.GetBytes(newalue);
            Marshal.Copy(bytes, 0, new_value, bytes.Length);

            return 1;
        }

        private static bool HandleOutput(IntPtr spin_button, IntPtr user_data)
        {
            return true;
        }

        private static void HandleValueChanged(IntPtr spin_button, IntPtr user_data)
        {
            var handler = ((GCHandle)user_data).Target as TextStepperHandler;
            if (handler._ignoreevents > 0)
                return;

            handler._ignoreevents++;
            var tmpvalue = (int)GtkWrapper.gtk_spin_button_get_value(spin_button);

            if (tmpvalue != (int)handler.GetValue())
            {
                var dir = StepperDirection.Up;

                if (handler._validDirection == StepperValidDirections.Down)
                    dir = StepperDirection.Down;
                else if (handler._validDirection == StepperValidDirections.Both && tmpvalue < 1)
                    dir = StepperDirection.Down;

                handler.ValidDirection = handler._validDirection;
                handler.Callback.OnStep(handler.Widget, new StepperEventArgs(dir));
            }
            
            handler._ignoreevents--;
        }

        private double GetValue()
        {
            switch(_validDirection)
            {
                case StepperValidDirections.Both:
                    return 1;
                case StepperValidDirections.Down:
                    return 2;
                case StepperValidDirections.Up:
                case StepperValidDirections.None:
                    return 0;
            }

            return 0;
        }

        public StepperValidDirections ValidDirection
        {
            get => _validDirection;
            set
            {
                _ignoreevents++;
                _validDirection = value;
                GtkWrapper.gtk_spin_button_set_range(Handle, 0, _validDirection == StepperValidDirections.None ? 0 : 2);

                switch (_validDirection)
                {
                    case StepperValidDirections.Both:
                        GtkWrapper.gtk_spin_button_set_value(Handle, 1);
                        break;
                    case StepperValidDirections.Down:
                        GtkWrapper.gtk_spin_button_set_value(Handle, 2);
                        break;
                    case StepperValidDirections.Up:
                    case StepperValidDirections.None:
                        GtkWrapper.gtk_spin_button_set_value(Handle, 0);
                        break;
                }
                _ignoreevents--;
            }
        }

        public bool ShowStepper 
        {
            get => true;
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AttachEvent(string id)
        {
            switch (id)
            {
                case TextStepper.StepEvent:
                    break;
                default:
                    base.AttachEvent(id);
                    break;
            }
        }
    }
}
