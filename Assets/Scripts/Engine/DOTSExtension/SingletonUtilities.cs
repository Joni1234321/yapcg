namespace YAPCG.Engine.DOTSExtension
{
    using Unity.Entities;

namespace THPS.Singletons
{
    public struct DefaultSingleton : IComponentData { }

    public static class SingletonUtilities
    {
        private static Entity _singletonEntity;

        public static void Setup(EntityManager entityManager)
        {
            if (entityManager.Exists(_singletonEntity))
            {
                UnityEngine.Debug.LogError("DefaultSingleton already created!");
                return;
            }

            _singletonEntity = entityManager.CreateEntity();
            entityManager.SetName(_singletonEntity, "Singleton Entity");
            entityManager.AddComponent<DefaultSingleton>(_singletonEntity);
        }

        public static Entity GetDefaultSingletonEntity(this EntityManager entityManager)
        {
            return _singletonEntity;
        }

        public static bool HasSingleton<T>(this EntityManager entityManager) where T : struct, IComponentData
        {
            return entityManager.HasComponent<T>(_singletonEntity);
        }

        public static T GetSingleton<T>(this EntityManager entityManager) where T : unmanaged, IComponentData
        {
            return entityManager.GetComponentData<T>(_singletonEntity);
        }

        public static Entity CreateOrSetSingleton<T>(this EntityManager entityManager, T data) where T : unmanaged, IComponentData
        {
            if (entityManager.HasComponent<T>(_singletonEntity))
                entityManager.SetComponentData(_singletonEntity, data);
            else
                entityManager.AddComponentData(_singletonEntity, data);

            return _singletonEntity;
        }

        public static Entity CreateOrAddSingleton<T>(this EntityManager entityManager) where T : IComponentData
        {
            if (entityManager.HasComponent<T>(_singletonEntity) == false)
                entityManager.AddComponent<T>(_singletonEntity);

            return _singletonEntity;
        }

        public static void RemoveSingletonComponentIfExists<T>(this EntityManager entityManager) where T : IComponentData
        {
            if (entityManager.HasComponent<T>(_singletonEntity))
            {
                entityManager.RemoveComponent<T>(_singletonEntity);
            }
        }

        public static void RemoveSingletonBufferIfExists<T>(this EntityManager entityManager) where T : unmanaged, IBufferElementData
        {
            if (entityManager.HasBuffer<T>(_singletonEntity))
            {
                entityManager.RemoveComponent<T>(_singletonEntity);
            }
        }

        public static DynamicBuffer<T> GetOrCreateSingletonBuffer<T>(this EntityManager entityManager) where T : unmanaged, IBufferElementData
        {
            if (entityManager.HasComponent<T>(_singletonEntity))
            {
                return entityManager.GetBuffer<T>(_singletonEntity);
            }
            else
            {
                return entityManager.AddBuffer<T>(_singletonEntity);
            }
        }

        public static DynamicBuffer<T> CreateOrResetSingletonBuffer<T>(this EntityManager entityManager) where T : unmanaged, IBufferElementData
        {
            if (entityManager.HasComponent<T>(_singletonEntity))
            {
                var buffer = entityManager.GetBuffer<T>(_singletonEntity);
                buffer.Clear();
                return buffer;
            }
            else
            {
                return entityManager.AddBuffer<T>(_singletonEntity);
            }
        }

        public static bool HasSingletonBuffer<T>(this EntityManager entityManager) where T : unmanaged, IBufferElementData
        {
            return entityManager.HasComponent<T>(_singletonEntity);
        }

        public static bool TryGetSingleton<T>(this EntityManager entityManager, out T data) where T : unmanaged, IComponentData
        {
            data = default;
            if (entityManager.HasComponent<T>(_singletonEntity))
            {
                data = entityManager.GetComponentData<T>(_singletonEntity);
                return true;
            }

            return false;
        }


        //public static unsafe RefRW<T> GetSingletonRW<T>(this EntityManager entityManager) where T : unmanaged, IComponentData
        //{
        //    var typeIndex = TypeManager.GetTypeIndex<T>();

        //    var chunk = entityManager.GetChunk(_SingletonEntity);
        //    GetSingletonChunk(typeIndex, out var indexInArchetype, out chunk);

        //    var data = ChunkDataUtility.GetComponentDataRW(chunk, 0, indexInArchetype, entityManager.GlobalSystemVersion);

        //    entityManager.CompleteDependencyBeforeRW<T>();
        //    return new RefRW<T>(data, default);
        //}
    }
}
}