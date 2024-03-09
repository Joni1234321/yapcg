using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Entities.Serialization;
using Unity.Rendering;
using UnityEngine;

namespace YAPCG.UI
{
    public struct MeshesReference : IComponentData
    {
        public bool LoadStarted;
        public WeakObjectReference<Mesh> DepositMesh;
        public WeakObjectReference<Material> DepositMaterial;
    }
    public class MeshesSingleton : IComponentData
    {
        public MeshMaterial Deposit, Hub;
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
                Debug.LogWarning("Second instance of Meshes created");
                return;
            }

            Instance = this;
            Load();
            EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<MeshesSingleton>().Build(_);
            _.CreateSingleton<MeshesSingleton>();
            query.SetSingleton(new MeshesSingleton {  Deposit =  Deposit, Hub = Hub });

            Mesh dmesh = Deposit.RenderMeshArray.Meshes[0];
            Material dmat = Deposit.RenderMeshArray.Materials[0];
            _.CreateSingleton<MeshesReference>();
            _.CreateEntityQuery(ComponentType.ReadWrite<MeshesReference>()).SetSingleton(new MeshesReference() { DepositMesh = new WeakObjectReference<Mesh>(dmesh), DepositMaterial = new WeakObjectReference<Material>(dmat)} );
        }

        private void Load()
        {
            Debug.Log("Loading Assets");
            Deposit.Load();
            Hub.Load();
        }
    }

    [System.Serializable]
    public struct MeshMaterial
    {
        [SerializeField] private Material material;
        [SerializeField] private Mesh mesh;

        public RenderMeshArray RenderMeshArray;

        public void Load()
        {
            RenderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
        }            
    }
}

