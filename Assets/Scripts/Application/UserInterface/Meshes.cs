using JetBrains.Annotations;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.DOTSExtension;

namespace YAPCG.Application.UserInterface
{
    public struct MeshesSingleton : IComponentData
    {
        public MeshesReference Deposit, Body;
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
        public MeshMaterial Deposit, Hub, Body;
        public SharedSizes SharedSizes;
        
        private EntityManager _;
        private static readonly int SHADER_SCALE = Shader.PropertyToID("_Scale");

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
            Load();
            _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            AddMeshesReferenceSingleton(_);
            _.AddSingleton(SharedSizes);
        }

        private void Load()
        {
            Deposit.Load();
            Hub.Load();
            Body.Load();
            CLogger.LogLoaded(this, "Deposits and meshes");
        }

        void AddMeshesReferenceSingleton(EntityManager _)
        {
            Material dmat = Deposit.RenderMeshArray.Materials[0];
            MeshesReference deposit = new MeshesReference(Deposit.RenderMeshArray.Meshes[0], dmat);
            dmat.SetFloat(SHADER_SCALE, SharedSizes.HubRadius);

            MeshesSingleton meshesSingleton = new MeshesSingleton()
            {
                Deposit = deposit,
                Body = new MeshesReference(Body.RenderMeshArray.Meshes[0], Body.RenderMeshArray.Materials[0])
            };

            
            _.AddSingleton(meshesSingleton);
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

