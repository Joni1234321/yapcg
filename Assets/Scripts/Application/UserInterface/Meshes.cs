using JetBrains.Annotations;
using Unity.Entities;
using Unity.Entities.Content;
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
    public struct MeshesReference : IComponentData
    {
        public bool LoadStarted;
        public WeakObjectReference<Mesh> Mesh;
        public WeakObjectReference<Material> Material;

        public MeshesReference(Mesh mesh, Material material)
        {
            LoadStarted = false;
            Mesh = new WeakObjectReference<Mesh>(mesh);
            Material = new WeakObjectReference<Material>(material);
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
        public MeshMaterial deposit, hub, body, sun, orbit;
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
                authoring.deposit.material.SetFloat(SHADER_SCALE, authoring.sharedSizes.HubRadius);
                MeshesSingleton meshesSingleton = new MeshesSingleton()
                {
                    Deposit = ToMeshesReference(authoring.deposit),
                    Planet = ToMeshesReference(authoring.body),
                    Sun = ToMeshesReference(authoring.sun),
                    Orbit = ToMeshesReference(authoring.orbit),
                };
                
                AddComponent(e, meshesSingleton);
                AddComponent(e, authoring.sharedSizes);
                return;

                MeshesReference ToMeshesReference (MeshMaterial m) => new(m.mesh, m.material);
            }
            
            private void Load(Meshes authoring)
            {
                authoring.deposit.Load();
                authoring.hub.Load();
                authoring.body.Load();
                authoring.sun.Load();
                authoring.orbit.Load();
                CLogger.LogLoaded(authoring, "Deposits and meshes");
            }
        }
    }
    
    [System.Serializable]
    public struct MeshMaterial
    {
        [SerializeField] public Material material;
        [SerializeField] public Mesh mesh;

        public RenderMeshArray RenderMeshArray;

        public void Load()
        {
            RenderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
        }            
    }
}

