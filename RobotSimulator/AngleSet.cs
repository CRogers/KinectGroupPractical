using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator
{
    class AngleSet
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

    }
}
