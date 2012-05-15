using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restrictor;

namespace RobotSimulator
{
    public class AngleSet
    {
        public Double rightShoulder1;
        public Double rightShoulder2;
        public Double rightElbow1;
        public Double rightElbow2;
        public Double leftShoulder1;
        public Double leftShoulder2;
        public Double leftElbow1;
        public Double leftElbow2;
        public AngleSet()
        {
            // initialise all the angles to 0
            rightShoulder1 = 0;
            rightShoulder2 = 0;
            rightElbow1 = 0;
            rightElbow2 = 0;
            leftShoulder1 = 0;
            leftShoulder2 = 0;
            leftElbow1 = 0;
            leftElbow2 = 0;
        }

        /// <summary>
        /// Convert the AngleSet object to an AnglePositions object.
        /// </summary>
        /// <returns>The AnglePositions object</returns>
        public AnglePositions toAnglePositions()
        {
            AnglePositions angles = new AnglePositions();
            angles.RightElbowOut = (int)rightElbow2;
            angles.RightElbowAlong = (int)rightElbow1;
            angles.RightShoulderOut = (int)rightShoulder2;
            angles.RightShoulderAlong = (int)rightShoulder1;
            angles.LeftElbowOut = (int)leftElbow2;
            angles.LeftElbowAlong = (int)leftElbow1;
            angles.LeftShoulderOut = (int)leftShoulder2;
            angles.LeftShoulderAlong = (int)leftShoulder1;
            return angles;
        }

    }
}
