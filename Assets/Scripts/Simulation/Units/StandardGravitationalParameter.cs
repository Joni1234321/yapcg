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
        public const double GRAVITATIONAL_CONSTANT = 0.000_000_000_066_743;
        public double Value;
        
        public StandardGravitationalParameter(Mass mass)
        {
            Value = GRAVITATIONAL_CONSTANT * mass.To(Mass.UnitType.KiloGrams);
        }
    }
}