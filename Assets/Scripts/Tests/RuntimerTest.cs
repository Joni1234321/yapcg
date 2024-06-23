using System.Collections;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace YAPCG
{
    public class RuntimerTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void RuntimerTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator RuntimerTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        
        [UnityTest, Performance]
        public IEnumerator Rendering_SampleScene()
        {
            using(Measure.Scope("LoadScene"))
            {
                // Add scene to Build Settings before running test 
                SceneManager.LoadScene("Scene"); 
            }
            yield return null;

            yield return Measure.Frames().Run();
        }
    }
}
