using Unity.Entities;

namespace YAPCG.Simulation.Units
{
    /// <summary>
    /// Returns mu
    /// https://en.wikipedia.org/wiki/Standard_gravitational_parameter
    /// </summary>
    public struct StandardGravitationalParameter : IComponentData
    {
        /// <summary>
        /// Given in N * M^2 * KG^(-2)
        /// </summary>
        public const float GRAVITATIONAL_CONSTANT = 0.000_000_000_066_743f;
        public float Value;
        
        /// <summary>
        /// Given in N * M^2 * KG^(-2)
        /// </summary>
        /// <param name="mass">kg</param>
        public StandardGravitationalParameter(float mass)
        {
            Value = GRAVITATIONAL_CONSTANT * mass;
        }
    }
}