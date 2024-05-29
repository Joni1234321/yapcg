using Unity.Mathematics;
using YAPCG.Simulation.Units;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public static class HohmannMechanics
    {
        /// <summary>
        /// Calculates the time it takes to Hohmann transfer to another planet
        /// </summary>
        /// <param name="mu">Mu of the thing they both orbit</param>
        /// <param name="radius1">Distance to the thing they orbit 1</param>
        /// <param name="radius2">Distance to the thing they orbit 2</param>
        /// <returns></returns>
        public static Time HohmannTransferDuration(StandardGravitationalParameter mu, Length radius1, Length radius2)
        {
            // new orbit is between 1 and 2, therefore its the average radius of the two
            // https://www.youtube.com/watch?v=O_EsXfVN988
            double r1 = radius1.To(Length.UnitType.Meters);
            double r2 = radius2.To(Length.UnitType.Meters);
            double r = (r1 + r2) / 2.0;
            double t = OrbitalMechanics.GetOrbitalPeriod(mu, new Length(r, Length.UnitType.Meters)).To(Time.UnitType.Seconds);
            return  new Time(t / 2.0, Time.UnitType.Seconds);
        }


        /// <summary>
        /// Returns how much difference in degree of the two planets position in their orbit has to be
        /// Retursn the how many degrees awaay the target planet has to be from the launching planet at launch
        /// </summary>
        /// <param name="destinationOrbitPeriod"></param>
        /// <param name="transferDuration"></param>
        /// <returns>Returns it in Radians</returns>
        public static double HohmannAngleDifferenceAtLaunch(Time destinationOrbitPeriod, Time transferDuration)
        {
            // Comes from formula: (T_orbit/2 - T_duration) / T_orbit = ratio of orbit
            // Which gives
            // ratio of orbit = .5 - T_duration / T_orbit
            double tOrbit = destinationOrbitPeriod.To(Time.UnitType.Seconds);
            double tDuration = transferDuration.To(Time.UnitType.Seconds);
            if (tDuration == 0 || tOrbit == 0)
                return 0;
            double ratio = tDuration / tOrbit;
            return ( .5 - ratio) * 2 * math.PI;
        }
        
        public static double GetHohmannDeltaV(StandardGravitationalParameter mu, Length departureRadius, Length destinationRadius)
        {
            double r1 = departureRadius.To(Length.UnitType.Meters);
            double r2 = destinationRadius.To(Length.UnitType.Meters);
            double delta1 = math.sqrt(1 / r1) * (math.sqrt(2 * r2 / (r1 + r2)) - 1);
            double delta2 = math.sqrt(1 / r2) * (1 - math.sqrt(2 * r1 / (r1 + r2)));
            return math.sqrt(mu.Value) * (math.abs(delta1) + math.abs(delta2));
        }

    }
}