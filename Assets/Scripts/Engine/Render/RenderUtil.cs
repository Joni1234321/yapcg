using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using YAPCG.UI;

namespace YAPCG.Engine.Render
{
    public static class RenderUtil
    {
        private static readonly MaterialMeshInfo INFO = MaterialMeshInfo.FromRenderMeshArrayIndices(0,0);

        public static void AddComponents(EntityCommandBuffer _, Entity e, RenderMeshArray renderArray)
        {
            _.AddComponent<RenderPositionOnlyTag>(e);
            _.AddSharedComponentManaged(e, renderArray);

            // For the future
            //_.AddComponent(e, INFO);
            //RenderMeshUtility.AddComponents(e, _, new RenderMeshDescription(ShadowCastingMode.Off, receiveShadows: false), Meshes.Instance.Deposit.RenderMeshArray, INFO);
        }
                
    }
}