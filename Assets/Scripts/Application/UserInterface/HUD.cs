using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Application.UserInterface.UI;
using YAPCG.Engine.Common;

namespace YAPCG.Application.UserInterface
{
    [ExecuteInEditMode]
    public class HUD : MonoBehaviour
    {
        public UIDocument UIDocument, WorldUIDocument;
        public WorldUserInterface WorldUserInterface;
        public MainUserInterface MainUserInterface;
        
        public static HUD Instance { get; private set; }
        
        private void Start()
        {
            if (Instance != null)
            {
                CLogger.WarningDuplication(this, "Second instance of HUD created");
                Destroy(Instance.gameObject);
            }
            CLogger.LogLoaded(this, "HUD");
            
            Instance = this;
            MainUserInterface = new MainUserInterface(UIDocument.rootVisualElement);
            WorldUserInterface = new WorldUserInterface(WorldUIDocument.rootVisualElement);
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton<HUDReady>();
        }
        
        public bool IsOverUserInterface(float2 screenPosition)
        {
            return UserInterfaceUtililty.IsOverVisualElement(screenPosition, WorldUserInterface.Root, MainUserInterface.Root);
        }
        
        
        public struct HUDReady : IComponentData {}
    }
}