using Restrictor;

namespace RobotSimulator.Model
{
    public interface IRestrictor
    {
        /// <summary>
        /// Takes in a array of possibly dangerous angles and strips out the dangerous ones
        /// </summary>
        /// <param name="angles">Possibly dangerous angles</param>
        /// <returns>Restricted where dangerous ones are replaced with null</returns>
        AnglePositions MakeSafe(AnglePositions angles);
    }
}
