using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RobotSimulator.Utility;
using System.Windows.Threading;
using System.Timers;

namespace RobotSimulator.Model
{
    //This class can take requests to change the motors. The view listens to this class in order to work out what
    //it should do to display the motors.
    //The motors have angles specified from 0 to 255 degrees (corresponding to 0 to 360 degrees):
    class MotorManager : I_Observable<I_MotorListener>
    {
        //The listeners for the motor events:
        private ICollection<I_MotorListener> listeners;

        //These constants refer to each motor:
        public const int NECK_MOTOR1 = 0;
        public const int CHEST_MOTOR1 = 1;
        public const int LEFT_SHOULDER_MOTOR1 = 2;
        public const int LEFT_SHOULDER_MOTOR2 = 3;
        public const int LEFT_ELBOW_MOTOR1 = 4;
        public const int LEFT_ELBOW_MOTOR2 = 5;
        public const int RIGHT_SHOULDER_MOTOR1 = 6;
        public const int RIGHT_SHOULDER_MOTOR2 = 7;
        public const int RIGHT_ELBOW_MOTOR1 = 8;
        public const int RIGHT_ELBOW_MOTOR2 = 9;

        //Keep track of every motor position:
        private double[] speeds; //the amount it may move by on each instance
        private double[] motors; //the value of each motor
        private double[] targets; //the target value of each motor with its timer
        private Timer[] timers; //the timers for each motor
        private const int MOTOR_COUNT = 10; //there are 10 motors

        //Create a new motor manager!
        public MotorManager()
        {
            listeners = new LinkedList<I_MotorListener>();
            motors = new double[MOTOR_COUNT];
            timers = new Timer[MOTOR_COUNT];
            targets = new double[MOTOR_COUNT];
            speeds = new double[MOTOR_COUNT];
            //Initial positions for each motor: (these define their offset effectively)
            setMotor(NECK_MOTOR1, 0);
            setMotor(CHEST_MOTOR1, 0);
            setMotor(LEFT_SHOULDER_MOTOR1, 0);
            setMotor(LEFT_SHOULDER_MOTOR2, 0);
            setMotor(LEFT_ELBOW_MOTOR1, 0);
            setMotor(LEFT_ELBOW_MOTOR2, 0);
            setMotor(RIGHT_SHOULDER_MOTOR1, 0);
            setMotor(RIGHT_SHOULDER_MOTOR2, 0);
            setMotor(RIGHT_ELBOW_MOTOR1, 0);
            setMotor(RIGHT_ELBOW_MOTOR2, 0);
            //The timers are set on demand
            for (int i = 0; i < MOTOR_COUNT; i++)
            {
                targets[i] = 0; //intialise:
                timers[i] = new Timer(1);
                timers[i].Enabled = false;
                timers[i].AutoReset = true; //continually run
                speeds[i] = 0; //can't move
            }
        }
        
        /*
         * Tell a motor to move towards a specific angle over a certain period of time.
         * The step speed tells the motor how much it can move every time the specified
         * amount of time has elapsed. Note that this also defines the direction,
         * since the step speed may be negative.
         * The time intervals are in milliseconds
         * WARNING: this will remove the previous target and timer for this motor
         */
        public void setMotorDegrees(int motor, double degrees, double stepSpeed, double time)
        {
            degrees = degrees % (255 + double.Epsilon);
            if (degrees < 0) { degrees += (255 + double.Epsilon); } //get the degrees range within 255
            //disable the current timer:
            timers[motor].Stop();
            timers[motor].Dispose();
            //Now reset the target:
            targets[motor] = degrees;
            alertMotorSet(motor, degrees);
            //Set the timer:
            speeds[motor] = stepSpeed;
            timers[motor] = new Timer(time);
            timers[motor].AutoReset = true;
            timers[motor].Elapsed += delegate { timerElapsed(motor); };
            timers[motor].Start(); //get going!
        }

        //Set a motor instantaneously.
        //WARNING: this will remove the previous target and timer for this motor
        public void setMotorDegrees(int motor, double degrees)
        {
            degrees = degrees % (255 + double.Epsilon);
            if (degrees < 0) { degrees += (255 + double.Epsilon); } //get the degrees range within 255
            //disable the timer:
            timers[motor].Stop();
            timers[motor].Dispose();
            speeds[motor] = 0;
            //Set the motor:
            targets[motor] = degrees;
            //Alert the listeners of all three events...
            alertMotorSet(motor,degrees); //set
            setMotor(motor, degrees); //moved
            alertMotorReached(motor,degrees); //reached
        }

        private void timerElapsed(int motor)
        {
            //Move the motor closer to its target:
            double degrees = getMotor(motor);
            if (speeds[motor] == 0)
            {
                return; //can't reach
            }
            if (speeds[motor] >= 255 || speeds[motor] <= -255)
            { //immediate reach
                //Disable the clock
                timers[motor].Stop();
                timers[motor].Dispose();
                //This indicates the motor has reached its target!
                setMotor(motor, targets[motor]);
                //Alert the listeners that the motor reached its target
                alertMotorReached(motor, targets[motor]);
                return;
            }
            double degrees2 = degrees + speeds[motor];
            degrees2 = degrees2 % (255 + double.Epsilon);
            bool finished = false;
            //Now figure out what bound this covers:
            if (speeds[motor] > 0)
            {
                if (degrees < degrees2) //all positive...
                {
                    if (degrees <= targets[motor] && degrees2 >= targets[motor])
                    {
                        finished = true;
                    }
                }
                else //The bound is inverted
                {
                    if (degrees <= targets[motor] || degrees2 >= targets[motor])
                    {
                        finished = true;
                    }
                }
            }
            else
            {
                if (degrees > degrees2)
                { //all negative
                    if (degrees >= targets[motor] && degrees2 <= targets[motor])
                    {
                        finished = true;
                    }
                }
                else
                {
                    if (degrees >= targets[motor] || degrees2 <= targets[motor])
                    {
                        finished = true;
                    }
                }
            }
            if (finished)
            {
                //Disable the clock
                timers[motor].Stop();
                timers[motor].Dispose();
                //This indicates the motor has reached its target!
                setMotor(motor, targets[motor]);
                //Alert the listeners that the motor reached its target
                alertMotorReached(motor, targets[motor]);
            }
            else
            {
                degrees += speeds[motor];
                degrees = degrees % (255 + double.Epsilon);
                if (degrees < 0) { degrees += (255 + double.Epsilon); } //get the degrees range within 255
                //Set the motor:
                setMotor(motor, degrees);
            }
        }

        //Retrieve the value of a motor as it is right now:
        public double getMotorDegrees(int motor)
        {
            return getMotor(motor);
        }

        public void addListener(I_MotorListener listener)
        {
            listeners.Add(listener);
        }

        public void removeListener(I_MotorListener listener)
        {
            listeners.Remove(listener);
        }

        //For alerting listeners:
        private void alertMotorSet(int motor, double degrees)
        {
            foreach (var listener in listeners)
            {
                listener.motorSetTo(motor, degrees);
            }
        }
        private void alertMotorMoved(int motor, double degrees)
        {
            foreach (var listener in listeners)
            {
                listener.motorMovedTo(motor, degrees);
            }
        }
        private void alertMotorReached(int motor, double degrees)
        {
            foreach (var listener in listeners)
            {
                listener.motorReached(motor, degrees);
            }
        }
        
        //Return the value of a motor (so you don't fiddle with the arrays):
        private double getMotor(int motor)
        {
            return motors[motor];
        }

        //Set the value of a motor:
        private void setMotor(int motor, double degrees)
        {
            motors[motor] = degrees;
            //Alert the listeners:
            alertMotorMoved(motor, getMotor(motor));
        }
    }
}
