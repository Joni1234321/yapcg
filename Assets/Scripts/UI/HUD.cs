using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Planets;
using YAPCG.Resources.View.Custom;

namespace YAPCG.UI
{
    public class HUD : MonoBehaviour
    {
        public UIDocument UIDocument;
        public VisualElement UI;

        public SlotControl SlotControlS, SlotControlM, SlotControlL;
        public Label HubNameLabel;
        
        public void Start()
        {
            SlotControlS = UI.Q<SlotControl>("slots-s");
            SlotControlM = UI.Q<SlotControl>("slots-m");
            SlotControlL = UI.Q<SlotControl>("slots-l");
            HubNameLabel = UI.Q<Label>("hub-name");
        }

        public void UpdateHubUI(EntityManager _, Entity hub)
        {
            FixedString64Bytes hubName = _.GetComponentData<Name>(hub).Value;
            BuildingSlotsLeft slotsLeft = _.GetComponentData<BuildingSlotsLeft>(hub);


            HubNameLabel.text = hubName.ToString();
            SlotControlS.Value = slotsLeft.Small.ToString();
            SlotControlM.Value = slotsLeft.Medium.ToString();
            SlotControlL.Value = slotsLeft.Large.ToString();
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