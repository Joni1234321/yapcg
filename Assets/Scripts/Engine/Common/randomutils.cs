using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace YAPCG.Engine.Common
{
    public struct randomutils
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
        public static float gauss_distribution(ref Random random)
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
        
        public static double sind(double d) {
            d += math.PI_DBL;
            double x2 = math.floor(d*(1/math.PI2_DBL));
            d -= x2*math.PI2_DBL;
            d -= math.PI_DBL;
   
            x2 = d * d;
   
            return //accurate to 6.82e-8, 3.3x faster than Math.sin, 
                //faster than lookup table in real-world conditions due to no cache misses
                //all values from "Fast Polynomial Approximations to Sine and Cosine", Garret, C. K., 2012
                (((((-2.05342856289746600727e-08*x2 + 2.70405218307799040084e-06)*x2
                    - 1.98125763417806681909e-04)*x2 + 8.33255814755188010464e-03)*x2
                  - 1.66665772196961623983e-01)*x2 + 9.99999707044156546685e-01)*d;
        }

        public static float2 rotate(float2 point, float theta)
        {
            float x = point.x * math.cos(theta) - point.y * math.sin(theta);
            float y = point.x * math.sin(theta) + point.y * math.cos(theta);
            return new float2(x, y);
        }

        public static double cosd(double d) {
            d += math.PI_DBL;
            double x2 = math.floor(d*(1/math.PI2_DBL));
            d -= x2*math.PI2_DBL;
            d -= math.PI_DBL;
   
            d *= d;
   
            return //max error 5.6e-7, 4x faster than Math.cos, 
                //faster than lookup table in real-world conditions due to less cache misses
                //all values from "Fast Polynomial Approximations to Sine and Cosine", Garret, C. K., 2012
                ((((- 2.21941782786353727022e-07*d + 2.42532401381033027481e-05)*d
                   - 1.38627507062573673756e-03)*d + 4.16610337354021107429e-02)*d
                 - 4.99995582499065048420e-01)*d + 9.99999443739537210853e-01;
        }

    }

    public static class RandomExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextGauss(this ref Random random, float mu, float sigma) => mu + randomutils.gauss_distribution(ref random) * sigma;

        /// <summary>
        /// 68.0% mu +- sigma
        /// 95.0% mu +- 2sigma
        /// 99.7% mu +- 3sigma
        /// </summary>
        /// <returns>Gauss between [min, max]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextGauss(this ref Random random, float mu, float sigma, float min, float max)
        {
            float x;
            do { x = random.NextGauss(mu, sigma); } while (x < min || x > max);
            return x;
        }
    }
}