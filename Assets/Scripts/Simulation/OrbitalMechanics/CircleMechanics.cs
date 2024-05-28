using Unity.Mathematics;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public static class CircleMechanics
    {
        /// <summary>
        /// Return the angle a point on the circle will have after time in radians
        /// </summary>
        /// <param name="period"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float GetAngleAfterTime(float period, float time)
        {
            if (period == 0)
                return 0;
            return 2 * math.PI * time / period;
        }
    }
}