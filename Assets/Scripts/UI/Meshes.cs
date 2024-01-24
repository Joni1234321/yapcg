using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace YAPCG.UI
{
    public class Meshes : MonoBehaviour
    {
        public MeshMaterial Deposit, Hub;
        
        public static Meshes Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
                Debug.LogError("Second instance of HUD created");

            Instance = this;
        }

        public void Start()
        {
            EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity e = _.CreateEntity();
            RenderMeshDescription description = new RenderMeshDescription(ShadowCastingMode.Off, receiveShadows: false);
            RenderMeshArray array = new RenderMeshArray(
                new[] { Deposit.Material },
                new[] { Deposit.Mesh }
            );
            float4x4 basic = float4x4.TRS(float3.zero, quaternion.identity, new float3(1, 1, 1));
            //RenderMeshUtility.AddComponents(e, _, description, array, MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));
            _.AddComponentData(e, new LocalToWorld { Value = basic });
            _.SetName(e, "RenderMeshUtiltiy");
        }
    }

    [System.Serializable]
    public struct MeshMaterial
    {
        public Material Material;
        public Mesh Mesh;
    }
}