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
    public struct MeshesReference : IComponentData
    {
        public bool LoadStarted;
        public WeakObjectReference<Mesh> DepositMesh;
        public WeakObjectReference<Material> DepositMaterial;
    }
    public class Meshes : MonoBehaviour
    {
        public MeshMaterial Deposit, Hub;
        public SharedSizes SharedSizes;
        
        private EntityManager _;
        private static readonly int HUB_RADIUS = Shader.PropertyToID("_Scale");

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
            CLogger.LogLoaded(this, "Deposits and meshes");
        }

        void AddMeshesReferenceSingleton(EntityManager _)
        {
            Mesh dmesh = Deposit.RenderMeshArray.Meshes[0];
            Material dmat = Deposit.RenderMeshArray.Materials[0];
            MeshesReference meshRef = new MeshesReference
            {
                DepositMesh = new WeakObjectReference<Mesh>(dmesh),
                DepositMaterial = new WeakObjectReference<Material>(dmat)
            };
            
            dmat.SetFloat(HUB_RADIUS, SharedSizes.HubRadius);
            
            _.AddSingleton(meshRef);
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

