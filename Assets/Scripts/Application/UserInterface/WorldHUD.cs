using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using YAPCG.Application.ControlRenderer;
using YAPCG.Resources.View.Custom;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Application.UserInterface
{
    public class WorldHUD
    {
        public WorldPlanetControlRenderer WorldPlanetControlRenderer;

        public WorldHUD(VisualElement root)
        {
            WorldPlanetControlRenderer = new WorldPlanetControlRenderer(root);
        }

    }
}