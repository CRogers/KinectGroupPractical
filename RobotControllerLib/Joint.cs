using System;
using System.Threading;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    class Joint
    {
        public int TargetAngle { get; set; }
        public int CurrentAngle { get; private set; }
        public NxtMotor Motor { get; set; }

        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set
            {
                // If we're turning it on again restart the thread
                if (value && !_active)
                    StartUpdateThread();
            }
        }

        public int UpdateFreq { get; set; }
        public uint MoveStep { get; set; }

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
            }

            UpdateFreq = 10;
            MoveStep = 2;

            // Turn the joint on
            Active = true;
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
                CurrentAngle = (Motor.TachoCount ?? 0) % 360;

                // Move the motor either increasing or decreasing depending on where we are trying to get to
                var power = (sbyte)(TargetAngle > CurrentAngle ? Power : -Power);
                Motor.Run(power, MoveStep);

                // Sleep for some time
                Thread.Sleep(UpdateFreq);
            }
        }
    }
}
