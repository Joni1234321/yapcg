using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Planets.Factories
{
    public struct HubFactory
    {
        public EntityManager _;

        private static Entity CreateSkeleton(EntityCommandBuffer _, float3 position)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Name>(e);

            _.AddComponent(e, new Position { Value = position });
            _.AddComponent(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20 });
            return e;
        } 

        public static Entity CreateBigHub(EntityCommandBuffer _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponent(e, new BuildingSlotsLeft { Big = 10, Medium = 5, Small = 5 });
            return e;
        }
        
        public static Entity CreateNormalHub(EntityCommandBuffer _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponent(e, new BuildingSlotsLeft { Big = 5, Medium = 10, Small = 10 });
            return e;
        }
        
        public static Entity CreateSmallHub(EntityCommandBuffer _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponent(e, new BuildingSlotsLeft { Big = 2, Medium = 5, Small = 25 });
            return e;
        }
    }
}