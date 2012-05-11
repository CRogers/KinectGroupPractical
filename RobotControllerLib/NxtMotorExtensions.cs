using System;
using System.Threading;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    public static class NxtMotorExtensions
    {
        /// <summary>
        /// Will run the motor at a specified power for about the number of degrees specified (approximate due to latency
        /// resulting from bluetooth polling). Unlike Run method will block until the movement is performed.
        /// </summary>
        /// <param name="motor">The motor to run</param>
        /// <param name="power">The speed to run the motor</param>
        /// <param name="degrees">The approximate number of degrees to turn the motor</param>
        public static void RunUntil(this NxtMotor motor, sbyte power, uint degrees)
        {
            var powerPositive = power >= 0;

            // Get the starting and ending position on the tachometer
            motor.Poll();
            var initTacho = motor.TachoCount.Value;
            var endTacho = initTacho + (powerPositive ? degrees : -degrees);

            Console.WriteLine("Degrees: {0}", degrees);
            Console.WriteLine("Poll Interval: {0}", motor.PollInterval);
            Thread.Sleep(2000);

            // Run motor and keep running motor at power
            motor.Run(power, 0);

            // We stop the motor running 30 degrees before the endpoint as the motor keeps running after that time.
            while ((powerPositive && (motor.TachoCount + 30 < endTacho) || (!powerPositive && (motor.TachoCount - 30 > endTacho)))) {
                //motor.Poll();
                Console.WriteLine("Current: {0}, End: {1}", motor.TachoCount, endTacho);
                Thread.Sleep(motor.PollInterval);
            }

            // Stop the motor
            motor.Brake();
            
            // Give it some time to stop
            Thread.Sleep(20);
        }

        public static void RunWait(this NxtMotor motor, sbyte power, uint degrees)
        {
            Console.WriteLine("\tRunWait - Enter");
            motor.Poll();
            var initTacho = motor.TachoCount.Value;

            motor.Run(power, degrees);
            Thread.Sleep(motor.PollInterval * 2);

            bool powerPositive = power >= 0;
            int quarterDegrees = initTacho + (int)(powerPositive ? degrees : -degrees)/4;

            var lastTacho = motor.TachoCount.Value;
            for (int i = 0; i < 20; i++)
            {
                var tcount = motor.TachoCount.Value;
                if ((powerPositive && tcount < quarterDegrees) || (!powerPositive && tcount > quarterDegrees) || lastTacho != tcount)
                {
                    i = 0;
                }
                lastTacho = tcount;
                Thread.Sleep(8);
            }

            Console.WriteLine("\tRunWait - Exit");
        }

        public static int GetNormalisedTacho(this NxtMotor motor)
        {
            return motor.TachoCount.Value + int.MaxValue/2;
        }
    }
}
