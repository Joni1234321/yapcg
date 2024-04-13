using Unity.Mathematics;

namespace YAPCG.Engine.Common
{
    public struct mathutils
    {
        private const float STEEPNESS = 2;

        
        /// <summary>
        /// result is ]-1, 1[
        /// </summary>
        public static float hyberbolic(float x) => math.tanh(x);

        /// <summary>
        /// dx/dt tanh
        /// </summary>
        public static float sech2(float x) => 1 / math.pow(math.cosh(x), 2);
        
        
        public static float logistic(float x) => 1 / (1 + math.exp(-x));

        /// <summary>
        /// x and result is between [0, 1] 
        /// </summary>
        public static float tanh_norm(float x) => 0.5f * (math.tanh((x - .5f) * STEEPNESS) + 1);

        /// <summary>
        /// result ]0,1[
        /// </summary>
        public static float tanh_diff(float x) => sech2(x * STEEPNESS);
    }
}