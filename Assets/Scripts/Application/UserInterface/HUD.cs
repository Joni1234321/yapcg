using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.Common.Systems;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Resources.View.Custom;
using SlotControl = YAPCG.Resources.View.Custom.SlotControl;

namespace YAPCG.Application.UserInterface
{
    public class HUD : MonoBehaviour
    {
        public UIDocument UIDocument, WorldUIDocument;
        public VisualElement UI, WorldUI;
        public WorldHUD WorldHUD;
        
        public SlotControl SlotControlS, SlotControlM, SlotControlL;
        public Label HubNameLabel;

        public ProgressBarControl DiscoveryProgress;
        
        public void Start()
        {
            HubNameLabel = UI.Q<Label>("hub-name");

            SlotControlS = UI.Q<SlotControl>("slots-s");
            SlotControlM = UI.Q<SlotControl>("slots-m");
            SlotControlL = UI.Q<SlotControl>("slots-l");

            DiscoveryProgress = UI.Q<ProgressBarControl>("discovery");

            WorldHUD = new WorldHUD(WorldUIDocument.rootVisualElement.Q("world-ui"));
        }

        public void UpdateBodyUI(EntityManager _, Entity hub)
        {
            UI.visible = hub != Entity.Null;
            
            if (hub == Entity.Null)
                return;
            
            FixedString64Bytes bodyName = _.GetComponentData<Name>(hub).Value;
            //BuildingSlotsLeft slotsLeft = _.GetComponentData<BuildingSlotsLeft>(hub);
            DiscoverProgress discovery = _.GetComponentData<DiscoverProgress>(hub);


            HubNameLabel.text = bodyName.ToString();
            /*
            SlotControlS.Value = slotsLeft.Small.ToString();
            SlotControlM.Value = slotsLeft.Medium.ToString();
            SlotControlL.Value = slotsLeft.Large.ToString();*/

            DiscoveryProgress.Max = discovery.MaxValue;
            DiscoveryProgress.Value = discovery.Value;
            DiscoveryProgress.Change = discovery.Progress;
        }
        

        public void UpdateHubUI(EntityManager _, Entity hub)
        {
            UI.visible = hub != Entity.Null;
            
            if (hub == Entity.Null)
                return;
            
            FixedString64Bytes hubName = _.GetComponentData<Name>(hub).Value;
            BuildingSlotsLeft slotsLeft = _.GetComponentData<BuildingSlotsLeft>(hub);
            DiscoverProgress discovery = _.GetComponentData<DiscoverProgress>(hub);


            HubNameLabel.text = hubName.ToString();
            
            SlotControlS.Value = slotsLeft.Small.ToString();
            SlotControlM.Value = slotsLeft.Medium.ToString();
            SlotControlL.Value = slotsLeft.Large.ToString();

            DiscoveryProgress.Max = discovery.MaxValue;
            DiscoveryProgress.Value = discovery.Value;
            DiscoveryProgress.Change = discovery.Progress;
        }

        public static HUD Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null)
                CLogger.WarningDuplication(this, "Second instance of HUD created");

            Instance = this;
            UI = UIDocument.rootVisualElement;
        }
    }
}