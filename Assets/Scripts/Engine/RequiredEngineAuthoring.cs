﻿using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;

namespace YAPCG.Engine
{
    public class RequiredEngineAuthoring : MonoBehaviour
    {
        public uint Seed = 29;

        class Baker : Baker<RequiredEngineAuthoring>
        {
            public override void Bake(RequiredEngineAuthoring authoring)
            {
                CLogger.LogLoaded(authoring, "Shared Components");

                Entity e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new SharedRandom { Random = Unity.Mathematics.Random.CreateFromIndex(authoring.Seed) });
            }
        }
        
    }
}