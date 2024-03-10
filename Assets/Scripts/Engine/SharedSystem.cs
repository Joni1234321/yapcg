using Unity.Burst;
using Unity.Entities;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SharedSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            state.EntityManager.CreateSingleton(new SharedRandom { Random = Random.CreateFromIndex(29) });
            CLogger.LogLoaded(null, "SharedSystem");
            state.Enabled = false;
        }
    }
}