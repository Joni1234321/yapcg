using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Application.UserInterface.UI
{
    public static class UserInterfaceUtils
    {
        /// <summary>
        /// Checks if pointer is over ui, by checking the alpha value over the position of the mouse over the current UI
        /// </summary>
        public static bool IsOverVisualElement (VisualElement root, float2 screenPosition)
        {
            float2 pointerPosition = new float2(screenPosition.x, Screen.height - screenPosition.y);
            List<VisualElement> picked = new();
            root.panel.PickAll(pointerPosition, picked);
            
            foreach (VisualElement visualElement in picked)
                if(visualElement != null && visualElement.enabledInHierarchy)
                    if(visualElement.resolvedStyle.backgroundColor.a != 0)
                        return true;
            
            return false;
        }
    }
}