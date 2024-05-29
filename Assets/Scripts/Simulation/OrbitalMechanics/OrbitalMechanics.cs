using Unity.Mathematics;
using YAPCG.Simulation.Units;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public static class OrbitalMechanics
    {
        /// <summary>
        /// Returns the time it will take to orbit around object
        /// T = 2 PI r * sqrt (r / Mu)
        /// </summary>
        /// <param name="mu">Mu of the thing they orbit</param>
        /// <param name="radius">radius or semimajor axis of an ellipse</param>
        /// <returns>Returns time for planet to orbit the object given Mu </returns>
        public static Time GetOrbitalPeriod(StandardGravitationalParameter mu, Length radius)
        {
            const double TWO_PI = 2 * math.PI;
            
            // Maybe make fast inverse sqr root
            double k = TWO_PI / math.sqrt(mu.Value);

            double r = radius.To(Length.UnitType.Meters);
            return new Time(k * r * math.sqrt(r), Time.UnitType.Seconds);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Orbital_period
        /// Synodic period is the time it takes for two planets to be opposite each other in the solar system
        /// </summary>
        /// <returns>The synodic period between the two orbits</returns>
        public static Time GetSynodicPeriod(Time orbitPeriod1, Time orbitPeriod2)
        {
            // Where T1 is less than T2 
            // Formula is 1/T = 1/T1 - 1/T2
            // This is rewritten as T = (T1 X T2) / (T2 - T1)
            double t1 = orbitPeriod1.To(Time.UnitType.Seconds);
            double t2 = orbitPeriod2.To(Time.UnitType.Seconds);
            return new Time((t1 * t2) / math.abs(t2 - t1), Time.UnitType.Seconds);
        }


        /// <summary>
        /// Returns the amount of time until the two bodies will have the wished angle between them
        /// </summary>
        /// <param name="angle">Angle between the two bodies</param>
        /// <param name="departureOrbitalPeriod">Orbital of planet from</param>
        /// <param name="destinationOrbitalPeriod">Orbital of planet to</param>
        /// <param name="departureOffsetAngle"> offset from day 0</param>
        /// <param name="destinationOffsetAngle">  offset from day 0 </param>
        /// <returns>Time until the two planets will be angle away from each other</returns>
        public static Time GetTimeUntilAngleDifference(double angle, Time departureOrbitalPeriod, Time destinationOrbitalPeriod, double departureOffsetAngle = .0, double destinationOffsetAngle = .0)
        {
            if (angle == 0)
                return new Time(0, Time.UnitType.Seconds);
            
            // Solved the equation
            // angle = 2 PI * (T / T2 - T / T1) + a2 - a1
            // So it became
            // T = (angle + a1 - a2) * T2 * T1 / (2 * Pi * (T1 - T2))
            double t1 = departureOrbitalPeriod.To(Time.UnitType.Seconds);
            double t2 = destinationOrbitalPeriod.To(Time.UnitType.Seconds);
            double a1 = departureOffsetAngle;
            double a2 = destinationOffsetAngle;
            double seconds = (angle + a1 - a2) * t1 * t2 / (2 * math.PI * (t1 - t2));
            return new Time(seconds, Time.UnitType.Seconds);
        }



    }    
    

}