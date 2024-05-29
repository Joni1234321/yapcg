using Unity.Mathematics;

namespace YAPCG.Simulation.OrbitalMechanics
{
    // https://github.com/lbaars/orbit-nerd-scripts
    public static class EllipseMechanics
    {

        /// <summary>
        /// Get point
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="nu">true anomaly</param>
        /// <returns></returns>
        public static float2 GetPoint(float a, float b, float nu)
        {
            float x = a * math.cos(nu);
            float y = b * math.sin(nu);
            return new float2(x, y);
        }
        
        /// <summary>
        /// Calculates the true anomaly (degrees from planet to orbit) 
        /// </summary>
        /// <param name="meanAnomaly">M</param>
        /// <param name="eccentricity">e</param>
        /// <returns>nu</returns>
        public static float MeanAnomalyToTrueAnomaly(float meanAnomaly, float eccentricity)
        {
            float eccentricAnomaly = MeanAnomalyToEccentricAnomaly(meanAnomaly, eccentricity);
            float trueAnomaly = EccentricAnomalyToTrueAnomaly(eccentricAnomaly, eccentricity);
            return trueAnomaly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meanAnomaly">M</param>
        /// <param name="eccentricity">e</param>
        /// <returns>E</returns>
        public static float MeanAnomalyToEccentricAnomaly(float meanAnomaly, float eccentricity)
        {
            float eNew = meanAnomaly + eccentricity;
            if (meanAnomaly > math.PI)
                eNew = meanAnomaly - eccentricity;
            float eOld = eNew + 0.001f;

            while (math.abs(eNew - eOld) > .00001f)
            {
                eOld = eNew;
                eNew = eOld + (meanAnomaly - eOld +
                               eccentricity * math.sin(eOld)) / (1 - eccentricity * math.cos(eOld));
            }
            
            return eNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eccentricAnomaly">E</param>
        /// <param name="eccentricity">e</param>
        /// <returns>nu</returns>
        public static float EccentricAnomalyToTrueAnomaly(float eccentricAnomaly, float eccentricity)
        {
            float trueAnomaly = math.atan2(math.sin(eccentricAnomaly) * math.sqrt(1 - eccentricity * eccentricity),
                math.cos(eccentricAnomaly) - eccentricity);
            trueAnomaly %= math.PI2;
            if (trueAnomaly < 0)
                trueAnomaly += math.PI2;

            return trueAnomaly;
        }

        public static float EccentricAnomalyToMeanAnomaly(float eccentricAnomaly, float eccentricity)
        {
           float meanAnomaly = eccentricAnomaly - eccentricity * math.sin(eccentricAnomaly);
            meanAnomaly %= math.PI2;
            if (meanAnomaly < 0)
                meanAnomaly += math.PI2;
            return meanAnomaly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueAnomaly">nu</param>
        /// <param name="eccentricity">e</param>
        /// <returns>E</returns>
        public static float TrueAnomalyToEccentricAnomaly(float trueAnomaly, float eccentricity)
        {
            float eccentricAnomaly = math.atan2(math.sin(trueAnomaly) * math.sqrt(1 - eccentricity * eccentricity), eccentricity + math.cos(trueAnomaly));
            return eccentricAnomaly;
        }

        public static float TrueAnomalyToMeanAnomaly(float trueAnomaly, float eccentricity)
        {
            float eccentricAnomaly = TrueAnomalyToEccentricAnomaly(trueAnomaly, eccentricity);
            float meanAnomaly = EccentricAnomalyToMeanAnomaly(eccentricAnomaly, eccentricity);
            return meanAnomaly;
        }
        
        /// <summary>
        /// Returns the distance from center of ellipse to focus point
        /// </summary>
        /// <param name="semiMajorAxis">a</param>
        /// <param name="closestDistanceToOrbitingObject">distance to Focus point</param>
        /// <returns></returns>
        public static float GetLinearEccentricity(float semiMajorAxis, float closestDistanceToOrbitingObject)
        {
            //https://en.wikipedia.org/wiki/Ellipse Linear eccentricity
            return math.abs(semiMajorAxis - closestDistanceToOrbitingObject);
        } 
        
        /// <summary>
        /// Returns b of the ellipse
        /// </summary>
        /// <param name="semiMajorAxis">a</param>
        /// <param name="linearEccentricity">c</param>
        /// <returns></returns>
        public static float GetSemiMinorAxis(float semiMajorAxis, float linearEccentricity)
        {
            //https://en.wikipedia.org/wiki/Ellipse Linear eccentricity
            // b^2 = a^2 - c^2
            return math.sqrt(semiMajorAxis * semiMajorAxis - linearEccentricity * linearEccentricity);
        }

        /// <summary>
        /// Returns position on plane
        /// </summary>
        /// <param name="semiMajorAxis"></param>
        /// <param name="semiMinorAxis"></param>
        /// <param name="eccentricAnomaly"></param>
        /// <returns></returns>
        public static float2 GetPositionInOrbit(float semiMajorAxis, float semiMinorAxis, float eccentricAnomaly)
        {
            float x = semiMajorAxis * math.cos(eccentricAnomaly);
            float y = semiMinorAxis * math.sin(eccentricAnomaly);
            return new float2(x, y);
        }
    }

}