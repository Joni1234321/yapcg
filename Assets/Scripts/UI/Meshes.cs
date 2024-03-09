using JetBrains.Annotations;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Common;

namespace YAPCG.UI
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

        private EntityQuery _query;
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
            EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            Mesh dmesh = Deposit.RenderMeshArray.Meshes[0];
            Material dmat = Deposit.RenderMeshArray.Materials[0];
            _.CreateSingleton<MeshesReference>();
            _.CreateEntityQuery(ComponentType.ReadWrite<MeshesReference>()).SetSingleton(new MeshesReference() { DepositMesh = new WeakObjectReference<Mesh>(dmesh), DepositMaterial = new WeakObjectReference<Material>(dmat)} );
        }

        private void Load()
        {
            Deposit.Load();
            Hub.Load();
            CLogger.LogLoaded(this, "Deposits and meshes");
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

