using System;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Entities.Serialization;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;

namespace YAPCG.Application.UserInterface
{
    public struct MeshesSingleton : IComponentData
    {
        public MeshesReference Deposit, Planet, Sun, Orbit;
    }
    
    [System.Serializable]
    public struct MeshesReference : IComponentData
    {
        [NonSerialized] public bool LoadStarted;
        public WeakObjectReference<Mesh> Mesh;
        public WeakObjectReference<Material> Material;

        public MeshesReference(WeakObjectReference<Mesh> mesh, WeakObjectReference<Material> material)
        {
            LoadStarted = false;
            
            Mesh = mesh;
            Material = material;
        }

        public void LoadAsync()
        {
            Material.LoadAsync();
            Mesh.LoadAsync();
            LoadStarted = true;
        }
        public bool Loaded() => Mesh.LoadingStatus == ObjectLoadingStatus.Completed && Material.LoadingStatus == ObjectLoadingStatus.Completed;
    }
    
    public class Meshes : MonoBehaviour
    {
        public MeshesReference deposit, hub, body, sun, orbit;
        public SharedSizes sharedSizes;
        
        // Can be null, but assume it isnt
        [NotNull] public static Meshes Instance { private set; get; }
        
        private void Awake()
        {
            if (Instance != null)
            {
                CLogger.WarningDuplication(this, "Second instance of Meshes created");
                return;
            }

            Instance = this;
        }


        class Baker : Baker<Meshes>
        {
            private static readonly int SHADER_SCALE = Shader.PropertyToID("_Scale");
            public override void Bake(Meshes authoring)
            {
                Load(authoring);
                Entity e = GetEntity(TransformUsageFlags.None);
                //authoring.deposit.material.Result.SetFloat(SHADER_SCALE, authoring.sharedSizes.HubRadius);
                MeshesSingleton meshesSingleton = new MeshesSingleton()
                {
                    Deposit = authoring.deposit,
                    Planet = authoring.body,
                    Sun = authoring.sun,
                    Orbit = authoring.orbit,
                };
                
                AddComponent(e, meshesSingleton);
                AddComponent(e, authoring.sharedSizes);

            }
            
            private void Load(Meshes authoring)
            {
                CLogger.LogLoaded(authoring, "Deposits and meshes");
            }
        }
    }

}

