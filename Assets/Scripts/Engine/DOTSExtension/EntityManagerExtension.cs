using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Engine.DOTSExtension
{

    public static class EntityManagerExtension
    {
        public static void AddSingleton<T>(this EntityManager _, T value) where T : unmanaged, IComponentData
        {
            _.CreateSingleton<T>();
            new EntityQueryBuilder(Allocator.Temp).WithAllRW<T>().Build(_).SetSingleton(value);
        }

    }
}