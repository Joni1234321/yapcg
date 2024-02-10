using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using YAPCG.Engine.Input;
using YAPCG.Engine.SystemGroups;
using YAPCG.Hub.Systems;

namespace YAPCG.UI
{
    public class Meshes : MonoBehaviour
    {
        public MeshMaterial Deposit, Hub;

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