using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct WorldFactorySystem : ISystem
    {
        private SampleWorldFactory _worldFactory; 
        public void OnCreate(ref SystemState state)
        {
            _worldFactory = new ();
            _worldFactory.Setup(ref state);
            state.RequireForUpdate<SharedRandom>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            _worldFactory.Spawn(ref ecb, ref state, ref random);
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }
    }
}