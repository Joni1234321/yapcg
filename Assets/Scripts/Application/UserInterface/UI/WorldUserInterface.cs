using UnityEngine.UIElements;
using YAPCG.Application.ControlRenderer;

namespace YAPCG.Application.UserInterface.UI
{
    public class WorldUserInterface
    {
        public VisualElement Root;

        public WorldPlanetControlRenderer WorldPlanetControlRenderer;

        public WorldUserInterface(VisualElement root)
        {
            Root = root;
            WorldPlanetControlRenderer = new WorldPlanetControlRenderer(root.Q<VisualElement>("world-ui"));
        }
        
    }
}