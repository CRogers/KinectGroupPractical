using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator.Model
{
    public interface I_MotorListener
    {
        /*
         * This event is fired whenever a request is made to tell the motor to begin
         * moving towards some specific angle. Note that this is before the motor has actually
         * begun moving towards this angle.
         */ 
        void motorSetTo(int motor, double degrees);

        /*
         * Respond to a motor being set to a certain value:
         * The first argument tells you which motor has been set.
         * The second argument tells you what it was set to.
         * This is fired continuously as the motor moves around.
         */
        void motorMovedTo(int motor, double degrees);

        /*
         * Similar to MotorSet, but this time it will fire only when the motor has reached
         * some genuine target. Since the motors move with a certain speed, when given
         * a target a motor will not automatically reach that target. This method is fired only
         * when a motor reaches its target (a motor's target may change before it reaches its previously assigned target,
         * in which case this event won't be fired. Hence, this event is not always fired).
         */ 
        void motorReached(int motor, double degrees);
    }
}
