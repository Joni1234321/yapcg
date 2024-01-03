using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Resources.View.Custom;

namespace YAPCG.UI
{
    public class HUD : MonoBehaviour
    {
        public UIDocument UIDocument;
        public VisualElement UI;

        public SlotControl SlotControlS, SlotControlM, SlotControlL;

        private EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;

        private Entity _focusedEntity = Entity.Null;
        
        public void Start()
        {
            SlotControlS = UI.Q<SlotControl>("slots-s");
            SlotControlM = UI.Q<SlotControl>("slots-m");
            SlotControlL = UI.Q<SlotControl>("slots-l");
        }

        public void Update()
        {
            
        }
        
        
        public static HUD Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
                Debug.LogError("Second instance of HUD created");

            Instance = this;
            UI = UIDocument.rootVisualElement;
        }
    }
}