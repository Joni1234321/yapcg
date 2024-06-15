using Unity.Collections;
using Unity.Entities;
using YAPCG.Domain.NUTS.Factories.Samples;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct WorldFactorySystem : ISystem
    {
        private SampleWorldFactory _worldFactory; 
        public void OnCreate(ref SystemState state)
        {
            _worldFactory = new ();
            _worldFactory.Setup(ref state);
            SetupOtherSingletons(ref state);

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


        void SetupOtherSingletons(ref SystemState state)
        {
            ActionsEntity.Entity = state.EntityManager.CreateEntity();
            state.EntityManager.SetName(ActionsEntity.Entity, "Actions Entity");
            state.EntityManager.AddComponent<Body.ActionClaim>(ActionsEntity.Entity);
        }
    }
}