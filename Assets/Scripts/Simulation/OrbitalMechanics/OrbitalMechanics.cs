using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.OrbitalMechanics;
using YAPCG.Engine.Physics;
using YAPCG.Simulation.Units;
using Mass = YAPCG.Simulation.Units.Mass;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public class OrbitalMechanics
    {
        /// <summary>
        /// Returns the time it will take to orbit around object
        /// T = 2 PI r * sqrt (r / Mu)
        /// </summary>
        /// <param name="mu">Mu of the thing they orbit</param>
        /// <param name="radius">radius or semimajor axis of an ellipse</param>
        /// <returns>Returns time for planet to orbit the object given Mu </returns>
        public static Time GetOrbitalPeriod(StandardGravitationalParameterOld mu, Length radius)
        {
            StandardGravitationalParameter newMu = new StandardGravitationalParameter { Value = mu.val };
            return GetOrbitalPeriod(newMu, radius);
        }

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
        /// Calculates the time it takes to Hohmann transfer to another planet
        /// </summary>
        /// <param name="mu">Mu of the thing they both orbit</param>
        /// <param name="radius1">Distance to the thing they orbit 1</param>
        /// <param name="radius2">Distance to the thing they orbit 2</param>
        /// <returns></returns>
        public static Time HohmannTransferDuration(StandardGravitationalParameterOld mu, Length radius1, Length radius2)
        {
            // new orbit is between 1 and 2, therefore its the average radius of the two
            // https://www.youtube.com/watch?v=O_EsXfVN988
            double r1 = radius1.To(Length.UnitType.Meters);
            double r2 = radius2.To(Length.UnitType.Meters);
            double r = (r1 + r2) / 2.0;
            double t = GetOrbitalPeriod(mu, new Length(r, Length.UnitType.Meters)).To(Time.UnitType.Seconds);
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


        public static double GetHohmannDeltaV(StandardGravitationalParameterOld mu, Length departureRadius, Length destinationRadius)
        {
            double r1 = departureRadius.To(Length.UnitType.Meters);
            double r2 = destinationRadius.To(Length.UnitType.Meters);
            double delta1 = math.sqrt(1 / r1) * (math.sqrt(2 * r2 / (r1 + r2)) - 1);
            double delta2 = math.sqrt(1 / r2) * (1 - math.sqrt(2 * r1 / (r1 + r2)));
            return math.sqrt(mu.val) * (math.abs(delta1) + math.abs(delta2));
        }
    }    
    
    /// <summary>
    /// Returns mu
    /// https://en.wikipedia.org/wiki/Standard_gravitational_parameter
    /// </summary>
    /// <returns></returns>
    public struct StandardGravitationalParameterOld
    {
        /// <summary>
        /// Given in N * M^2 * KG^(-2)
        /// </summary>
        public const double GRAVITATIONAL_CONSTANT = 0.000_000_000_066_743;

        public double val;

        public StandardGravitationalParameterOld(Mass mass)
        {
            val = GRAVITATIONAL_CONSTANT * mass.To(Mass.UnitType.KiloGrams);
        }
    }
    
    public struct StandardGravitationalParameter : IComponentData
    {
        public const double GRAVITATIONAL_CONSTANT = 0.000_000_000_066_743;
        public double Value;
    }
}