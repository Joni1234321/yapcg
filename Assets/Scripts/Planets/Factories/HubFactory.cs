using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Core.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Planets.Factories
{
    public struct HubFactory
    {
        public EntityManager _;

        private static Entity CreateSkeleton(EntityManager _, float3 position)
        {
            Entity e = _.CreateEntity();
            _.AddComponentData(e, new Position { Value = position });
            _.AddComponentData(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20 });
            _.AddComponentData(e, new Name { Value = HubNamingGenerator.Get()});
            return e;
        } 

        public static Entity CreateBigHub(EntityManager _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponentData(e, new BuildingSlotsLeft { Big = 10, Medium = 5, Small = 5 });
            return e;
        }
        
        public static Entity CreateNormalHub(EntityManager _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponentData(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20 });
            _.AddComponentData(e, new BuildingSlotsLeft { Big = 5, Medium = 10, Small = 10 });
            return e;
        }
        
        public static Entity CreateSmallHub(EntityManager _, float3 position)
        {
            Entity e = CreateSkeleton(_, position);
            _.AddComponentData(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20 });
            _.AddComponentData(e, new BuildingSlotsLeft { Big = 2, Medium = 5, Small = 25 });
            return e;
        }
    }
}