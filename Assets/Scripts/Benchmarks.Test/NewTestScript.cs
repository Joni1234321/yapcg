using System.Collections;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;

namespace YAPCG.Benchmarks.Test
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test, Performance] 
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        [Test, Performance]
        public void Vector2_operations()
        {
            var a = Vector2.one;
            var b = Vector2.zero;

            Measure.Method(() =>
                {
                    Vector2.MoveTowards(a, b, 0.5f);
                    Vector2.ClampMagnitude(a, 0.5f);
                    Vector2.Reflect(a, b);
                    Vector2.SignedAngle(a, b);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .Run();
            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2.Reflect(a, b);
                        Vector2.SignedAngle(a, b);                        
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(400)
                .Run();
        }
    }
}
