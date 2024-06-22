using Unity.Mathematics;
using YAPCG.Engine.Common;

namespace YAPCG.Simulation.OrbitalMechanics
{
    public static class ExtraMechanics
    {
        public struct OrbitData
        {
            public float SemiMajorAxis, SemiMinorAxis;
            public float2 Center;
            public float Rotation;
        }
        
        public static float3 CalculatePositionOnOrbit(OrbitData orbitData, float trueAnomaly)
        {
            float2 point = EllipseMechanics.GetPoint(orbitData.SemiMajorAxis, orbitData.SemiMinorAxis, trueAnomaly);
            float2 rotated = randomutils.rotate(point, orbitData.Rotation);

            return new float3(rotated + orbitData.Center, 0).xzy;
        }



        public static OrbitData CreateOrbitHoffmanTransfer(OrbitData orbit1, OrbitData orbit2)
        {
            const float ORBITING_BODY_POSITION = 0; // sun is the foci
            const float TRUE_ANOMALY_AT_DEPARTUE = 0;
            
            float GetDistanceToSun(OrbitData orbit, float trueAnomaly)
            {
                float3 position = CalculatePositionOnOrbit(orbit, trueAnomaly) - ORBITING_BODY_POSITION;
                return math.length(position);
            }
                
            float r1 = GetDistanceToSun(orbit1, TRUE_ANOMALY_AT_DEPARTUE);
            float r2 = GetDistanceToSun(orbit2, TRUE_ANOMALY_AT_DEPARTUE + math.PI);
            
            float a = (r1 + r2) * 0.5f;
            float c = EllipseMechanics.GetLinearEccentricity(a, ORBITING_BODY_POSITION); // sun is the foci
            float b = EllipseMechanics.GetSemiMinorAxis(a, c);

            float rotation = TRUE_ANOMALY_AT_DEPARTUE;
            
            if (r1 > r2)
                rotation += math.PI;
            
            return new OrbitData()
            {
                SemiMajorAxis = a,
                SemiMinorAxis = b,
                Center = ORBITING_BODY_POSITION,
                Rotation = rotation,
            };
        }
    }
}