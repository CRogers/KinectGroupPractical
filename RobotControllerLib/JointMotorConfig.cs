using System;
using System.Collections.Generic;
using NKH.MindSqualls;

namespace RobotControllerLib
{
    class JointMotorConfig
    {
        public int[] ComPorts { get; private set; }
        public Dictionary<string, Tuple<int, NxtMotorPort?>> JointMotorMapping { get; private set; }

        // Save this here for regexing to produce code : lShTw, lShEx, lElTw, lElEx, rShTw, rShEx, rElTw, rElEx,
        public JointMotorConfig(int comPort1 = -1, int comPort2 = -1, int comPort3 = -1,
                                int lShTwBrick = -1, NxtMotorPort? lShTwPort = null,
                                int lShExBrick = -1, NxtMotorPort? lShExPort = null,
                                int lElTwBrick = -1, NxtMotorPort? lElTwPort = null,
                                int lElExBrick = -1, NxtMotorPort? lElExPort = null,
                                int rShTwBrick = -1, NxtMotorPort? rShTwPort = null,
                                int rShExBrick = -1, NxtMotorPort? rShExPort = null,
                                int rElTwBrick = -1, NxtMotorPort? rElTwPort = null,
                                int rElExBrick = -1, NxtMotorPort? rElExPort = null)

        {
            ComPorts = new[] { comPort1, comPort2, comPort3 };
            JointMotorMapping = new Dictionary<string, Tuple<int, NxtMotorPort?>>
            {
                { "lShTw", Tuple.Create(lShTwBrick, lShTwPort) },
                { "lShEx", Tuple.Create(lShExBrick, lShExPort) },
                { "lElTw", Tuple.Create(lElTwBrick, lElTwPort) },
                { "lElEx", Tuple.Create(lElExBrick, lElExPort) },
                { "rShTw", Tuple.Create(rShTwBrick, rShTwPort) },
                { "rShEx", Tuple.Create(rShExBrick, rShExPort) },
                { "rElTw", Tuple.Create(rElTwBrick, rElTwPort) },
                { "rElEx", Tuple.Create(rElExBrick, rElExPort) }
            };
        }

        public static JointMotorConfig Default = new JointMotorConfig(3, lShTwBrick: 0, lShTwPort: NxtMotorPort.PortA, lShExBrick: 0, lShExPort: NxtMotorPort.PortB);
    }
}
