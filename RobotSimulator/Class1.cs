using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotSimulator
{
    class Class1
    {
        private static Random r = new Random();
        public void thing() {
            int ran = r.Next(0,100000000);
            double rand = ((double)ran) / 100000000.0;
        }
    }
}
