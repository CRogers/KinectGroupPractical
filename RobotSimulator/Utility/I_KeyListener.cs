using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RobotSimulator.Utility
{
    public interface I_KeyListener
    {
        //Respond to a key being pressed down. If held down, this event will fire more than once.
        void KeyPressed(KeyEventArgs e);
        //Respond to a key being released
        void KeyReleased(KeyEventArgs e);
    }
}
