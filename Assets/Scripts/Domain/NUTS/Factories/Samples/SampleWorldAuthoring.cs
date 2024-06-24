using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories.Samples
{
    public class SampleWorldAuthoring : MonoBehaviour
    {
        public bool PressThisToUpdate;

        public SolarSystemFactoryParams SolarSystemFactoryParams = new() { Planets = 7 };
        public Hub.HubFactoryParams[]HubFactoryParams;
        public Deposit.DepositFactoryParams[] DepositFactoryParams;

        private void Awake()
        {
            PressThisToUpdate = true;
        }

        private class Baker : Baker<SampleWorldAuthoring>
        {
            public override void Bake(SampleWorldAuthoring authoring)
            {
                EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
                #if UNITY_EDITOR
                if (!EditorApplication.isPlaying && !authoring.PressThisToUpdate)
                {
                    _.InitFactory<Factories.SolarSystemFactoryParams>();
                    _.InitFactory<Hub.HubFactoryParams>();
                    _.InitFactory<Deposit.DepositFactoryParams>();
                    return;
                }
                #endif
                
                authoring.PressThisToUpdate = false;
                Entity e = GetEntity(TransformUsageFlags.None);
                _.DestroyEntity(_.CreateEntityQuery(ComponentType.ReadOnly<Body.BodyTag>()));
                _.DestroyEntity(_.CreateEntityQuery(ComponentType.ReadOnly<Hub.HubTag>()));
                _.DestroyEntity(_.CreateEntityQuery(ComponentType.ReadOnly<Deposit.DepositTag>()));
                _.InitFactory(authoring.SolarSystemFactoryParams);
                _.InitFactory(authoring.HubFactoryParams);
                _.InitFactory(authoring.DepositFactoryParams);
                AddComponent(e, new LevelQuad {Size = new float2(100, 100)});
            }
        }
    }
}