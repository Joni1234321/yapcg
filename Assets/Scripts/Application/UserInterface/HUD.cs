using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Resources.View.Custom;
using SlotControl = YAPCG.Resources.View.Custom.SlotControl;

namespace YAPCG.Application.UserInterface
{
    public class HUD : MonoBehaviour
    {
        public UIDocument UIDocument;
        public VisualElement UI;

        public SlotControl SlotControlS, SlotControlM, SlotControlL;
        public Label HubNameLabel;

        public ProgressBarControl DiscoveryProgress;

        public ListView HubList;
        public List<string> HubNames;
        
        public void Start()
        {
            HubNameLabel = UI.Q<Label>("hub-name");

            SlotControlS = UI.Q<SlotControl>("slots-s");
            SlotControlM = UI.Q<SlotControl>("slots-m");
            SlotControlL = UI.Q<SlotControl>("slots-l");

            DiscoveryProgress = UI.Q<ProgressBarControl>("discovery");

            HubList = UI.Q<ListView>("hub-list");
            HubList.itemsSource = HubNames;
            HubList.makeItem = () => new Label();
            HubList.bindItem = (element, i) => (element as Label).text = i.ToString();

        }

        private void Update()
        {
            /*
            NativeArray<Name> names = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(HubTag), typeof(Name))
                .ToComponentDataArray<Name>(Allocator.Temp);
            HubNames.Clear();
            foreach (Name name in names) 
                HubNames.Add(name.Value.ToString());
            names.Dispose();*/
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