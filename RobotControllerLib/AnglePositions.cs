using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restrictor
{
    public class AnglePositions
    {
        public int? LeftShoulderAlong { get; set; }
        public int? LeftShoulderOut { get; set; }
        public int? LeftElbowAlong { get; set; }
        public int? LeftElbowOut { get; set; }
        public int? RightShoulderAlong { get; set; }
        public int? RightShoulderOut { get; set; }
        public int? RightElbowAlong { get; set; }
        public int? RightElbowOut { get; set; }

        public AnglePositions()
        {
            LeftShoulderAlong = 0;
            LeftElbowOut = 0;
            LeftShoulderOut = 0;
            LeftElbowAlong = 0;
            RightElbowAlong = 0;
            RightElbowOut = 0;
            RightShoulderAlong = 0;
            RightShoulderOut = 0;
        }

        public AnglePositions(AnglePositions angles)
        {
            LeftShoulderAlong = angles.LeftShoulderAlong;
            LeftElbowOut = angles.LeftElbowOut;
            LeftShoulderOut = angles.LeftShoulderOut;
            LeftElbowAlong = angles.LeftElbowOut;
            RightElbowAlong = angles.RightElbowAlong;
            RightElbowOut = angles.RightElbowOut;
            RightShoulderAlong = angles.RightShoulderAlong;
            RightShoulderOut = angles.RightShoulderOut;
        }
    }
}
