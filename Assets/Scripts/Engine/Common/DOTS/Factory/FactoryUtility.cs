using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public static class FactoryUtility
    {
        public static Entity FactoryEntity = Entity.Null;
        public struct FactoryReadyTag : IComponentData {}
        public static DynamicBuffer<T> InitFactory<T>(this EntityManager _)
            where T : unmanaged, IFactoryParams
        {
            if (!_.Exists(FactoryEntity))
            {
                FactoryEntity = _.CreateEntity();
                _.SetName(FactoryEntity, "Factory Entity");
            }
            if (!_.HasComponent<FactoryReadyTag>(FactoryEntity))
                _.AddComponent<FactoryReadyTag>(FactoryEntity);
            
            return _.AddBuffer<T>(FactoryEntity);
        }
        public static DynamicBuffer<T> InitFactory<T>(this EntityManager _, T data)
            where T : unmanaged, IFactoryParams
        {
            DynamicBuffer<T> buffer = _.InitFactory<T>();
            buffer.Add(data);
            return buffer;
        }
        public static DynamicBuffer<T> InitFactory<T>(this EntityManager _, NativeArray<T> data)
            where T : unmanaged, IFactoryParams
        {
            DynamicBuffer<T> buffer = _.InitFactory<T>();
            buffer.AddRange(data);
            return buffer;
        }
        public static DynamicBuffer<T> InitFactory<T>(this EntityManager _, T[] data)
            where T : unmanaged, IFactoryParams
        {
            DynamicBuffer<T> buffer = _.InitFactory<T>();
            buffer.AddRange(new NativeArray<T>(data, Allocator.Temp));
            return buffer;
        }
        public static void InitFactoryAndDispose<T>(this EntityManager _, NativeArray<T> data)
            where T : unmanaged, IFactoryParams
        {
            _.InitFactory<T>().AddRange(data);
            data.Dispose();
        }
        
        public static void CallFactory<T>(this ref EntityCommandBuffer ecb, EntityManager _, ref Random random, IFactory<T> factory, out NativeList<Entity> spawned) 
            where T : unmanaged, IFactoryParams
        {
            
            DynamicBuffer<T> factoryParams = _.GetBuffer<T>(FactoryEntity);
            spawned = new NativeList<Entity>(Allocator.Temp);
            foreach(T config in factoryParams)
                factory.Spawn(ref random, ref ecb, ref spawned, config);
            
            factoryParams.Clear();
        }
    }
}