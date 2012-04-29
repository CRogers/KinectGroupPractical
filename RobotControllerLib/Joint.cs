using System;
using System.Threading;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    public class Joint
    {
        private int initialTacho;

        public int TargetAngle { get; set; }
        public int CurrentAngle { get; private set; }
        public NxtMotor Motor { get; set; }
        public int DegreeScaleFactor { get; set; }

        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set
            {
                // If we're turning it on again restart the thread
                if (value && !_active)
                    StartUpdateThread();
                _active = value;
            }
        }

        public byte _power = 100;
        public byte Power
        {
            get { return _power; }
            set
            {
                if(value > 100)
                    throw new ArgumentOutOfRangeException("value", "Power value must be between 0-100");
                _power = value;
            }
        }


        public Joint(NxtMotor motor)
        {
            if (motor != null) {
                Motor = motor;
                motor.ResetMotorPosition(true);
                
                // Poll so tacho is not null
                motor.Poll();
                initialTacho = motor.GetNormalisedTacho();

                // Turn the joint on
                Active = true;
            }

            DegreeScaleFactor = 1;
        }


        protected void StartUpdateThread()
        {
            // Quickly spawn the update thread using an Async delegate call
            new Action(UpdateContinuous).BeginInvoke(null, null);
        }

        protected void UpdateContinuous()
        {
            while(Active) {
                // Convert the tachoCount (degrees moved by the motor) to absolute degrees
                // TODO: What happens if the joint goes backwards and this becomes negative?
                CurrentAngle = ((Motor.GetNormalisedTacho() - initialTacho) / DegreeScaleFactor) % 360;

                // Move the motor either increasing or decreasing depending on where we are trying to get to
                var power = (sbyte)(TargetAngle > CurrentAngle ? Power : -Power);
                var rawDiff = Math.Abs(TargetAngle - CurrentAngle);
                var diff = rawDiff * DegreeScaleFactor;

                Console.WriteLine("Curr ang: {0}, Target: {1}, Diff: {2}, Power: {3}", CurrentAngle, TargetAngle, diff, power);

                if (rawDiff > 5) {
                    Motor.RunUntil(power, (uint) diff);
                }
                else {
                    Thread.Sleep(50);
                }
            }
        }
    }
}
