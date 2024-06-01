using Unity.Mathematics;
using YAPCG.Simulation.Units;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public static class OrbitalMechanics
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mu"></param>
        /// <param name="semiMajorAxis">semimajor</param>
        /// <returns></returns>
        public static SiTime GetOrbitalPeriod(StandardGravitationalParameter mu, float semiMajorAxis)
        {
            // https://en.wikipedia.org/wiki/Orbital_period
            // T = 2Pi * sqrt(a^3/GM)
            return new SiTime (math.PI2 * semiMajorAxis * math.sqrt(semiMajorAxis / mu.Value));
        }
        
        
        /// <summary>
        /// Returns the time it will take to orbit around object
        /// T = 2 PI r * sqrt (r / Mu)
        /// </summary>
        /// <param name="mu">Mu of the thing they orbit</param>
        /// <param name="radius">radius or semimajor axis of an ellipse</param>
        /// <returns>Returns time for planet to orbit the object given Mu </returns>
        public static TimeConverter GetOrbitalPeriod(StandardGravitationalParameter mu, Length radius)
        {
            // Maybe make fast inverse sqr root
            float k = math.PI2 / math.sqrt(mu.Value);
            float r = radius.To(Length.UnitType.Meters);
            return new TimeConverter(k * r * math.sqrt(r), TimeConverter.UnitType.Seconds);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Orbital_period
        /// Synodic period is the time it takes for two planets to be opposite each other in the solar system
        /// </summary>
        /// <returns>The synodic period between the two orbits</returns>
        public static TimeConverter GetSynodicPeriod(TimeConverter orbitPeriod1, TimeConverter orbitPeriod2)
        {
            // Where T1 is less than T2 
            // Formula is 1/T = 1/T1 - 1/T2
            // This is rewritten as T = (T1 X T2) / (T2 - T1)
            float t1 = orbitPeriod1.To(TimeConverter.UnitType.Seconds);
            float t2 = orbitPeriod2.To(TimeConverter.UnitType.Seconds);
            return new TimeConverter((t1 * t2) / math.abs(t2 - t1), TimeConverter.UnitType.Seconds);
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
        public static TimeConverter GetTimeUntilAngleDifference(float angle, TimeConverter departureOrbitalPeriod, TimeConverter destinationOrbitalPeriod, float departureOffsetAngle = .0f, float destinationOffsetAngle = .0f)
        {
            if (angle == 0)
                return new TimeConverter(0, TimeConverter.UnitType.Seconds);
            
            // Solved the equation
            // angle = 2 PI * (T / T2 - T / T1) + a2 - a1
            // So it became
            // T = (angle + a1 - a2) * T2 * T1 / (2 * Pi * (T1 - T2))
            float t1 = departureOrbitalPeriod.To(TimeConverter.UnitType.Seconds);
            float t2 = destinationOrbitalPeriod.To(TimeConverter.UnitType.Seconds);
            float a1 = departureOffsetAngle;
            float a2 = destinationOffsetAngle;
            float seconds = (angle + a1 - a2) * t1 * t2 / (2 * math.PI * (t1 - t2));
            return new TimeConverter(seconds, TimeConverter.UnitType.Seconds);
        }



    }    
    

}