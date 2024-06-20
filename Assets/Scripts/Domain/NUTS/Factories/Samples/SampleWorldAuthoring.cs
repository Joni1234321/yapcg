using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories.Samples
{
    public class SampleWorldAuthoring : MonoBehaviour
    {
        public SolarSystemFactoryParams SolarSystemFactoryParams = new() { Planets = 7 };
        public Hub.HubFactoryParams[]HubFactoryParams;
        public Deposit.DepositFactoryParams[] DepositFactoryParams;
        private class Baker : Baker<SampleWorldAuthoring>
        {
            public override void Bake(SampleWorldAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);
                EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
                _.InitFactory(authoring.SolarSystemFactoryParams);
                _.InitFactory(authoring.HubFactoryParams);
                _.InitFactory(authoring.DepositFactoryParams);
                AddComponent(e, new LevelQuad {Size = new float2(100, 100)});
            }
        }
    }
}