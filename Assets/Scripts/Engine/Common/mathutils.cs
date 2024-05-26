using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace YAPCG.Engine.Common
{
    public struct mathutils
    {
        private const float STEEPNESS = 2f;

        
        /// <summary>
        /// result is ]-1, 1[
        /// </summary>
        public static float hyberbolic(float x) => math.tanh(x);

        /// <summary>
        /// dx/dt tanh
        /// </summary>
        public static float sech2(float x) => 1f / math.pow(math.cosh(x), 2f);
        
        
        public static float logistic(float x) => 1f / (1f + math.exp(-x));

        /// <summary>
        /// x and result is between [0, 1] 
        /// </summary>
        public static float tanh_norm(float x) => 0.5f * (math.tanh((x - .5f) * STEEPNESS) + 1);

        /// <summary>
        /// result ]0,1[
        /// </summary>
        public static float tanh_diff(float x) => sech2(x * STEEPNESS);

        
        /// <summary>
        /// box muller
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static float gauss_distribution(Random random)
        {
            // https://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/
            float v1, v2, s;
            do {
                v1 = 2.0f * random.NextFloat() - 1.0f;
                v2 = 2.0f * random.NextFloat() - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || s == 0f);
            return v1 * math.sqrt(-2.0f * math.log(s) / s);
        }

    }

    public static class RandomExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextGauss(this Random random, float mu, float sigma) => mu + mathutils.gauss_distribution(random) * sigma;

        /// <summary>
        /// 68.0% mu +- sigma
        /// 95.0% mu +- 2sigma
        /// 99.7% mu +- 3sigma
        /// </summary>
        /// <returns>Gauss between [min, max]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextGauss(this Random random, float mu, float sigma, float min, float max)
        {
            float x;
            do { x = random.NextGauss(mu, sigma); } while (x < min || x > max);
            return x;
        }
    }
}