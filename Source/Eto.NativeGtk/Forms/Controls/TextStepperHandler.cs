using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
    public class TextStepperHandler : TextBoxBaseHandler<Gtk.Widget, TextStepper, TextStepper.ICallback>, TextStepper.IHandler
    {
        private IntPtr _adjustment;

        public TextStepperHandler()
        {
            _adjustment = GtkWrapper.gtk_adjustment_new(0, 0, 3, 1, 1, 1);
            Control = new Gtk.Widget(GtkWrapper.gtk_spin_button_new(_adjustment, 0, 0));
            GtkWrapper.gtk_spin_button_set_numeric(Control.Handle, false);
            ReadOnly = false;
            Text = "";

            ConnectSignal("output", (Func<IntPtr, IntPtr, bool>)HandleOutput, 0);
        }

        private static bool HandleOutput(IntPtr spin_button, IntPtr user_data)
        {
            return true;
        }

        public StepperValidDirections ValidDirection
        {
            get => StepperValidDirections.Both;
            set
            {
                
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
