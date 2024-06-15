using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Components;
using YAPCG.Resources.View.Custom;

namespace YAPCG.Application.UserInterface.UI
{
    public class MainUserInterface
    {
        public VisualElement Root;
        
        public SlotControl SlotControlS, SlotControlM, SlotControlL;
        public Label HubNameLabel;

        public ProgressBarControl DiscoveryProgress;

        public MainUserInterface(VisualElement root)
        {
            Root = root;
            
            HubNameLabel = root.Q<Label>("hub-name");

            SlotControlS = root.Q<SlotControl>("slots-s");
            SlotControlM = root.Q<SlotControl>("slots-m");
            SlotControlL = root.Q<SlotControl>("slots-l");

            DiscoveryProgress = root.Q<ProgressBarControl>("discovery");
        }
        
        public void UpdateBodyUI(EntityManager _, Entity body)
        {
            if (body == Entity.Null)
            {
                Root.visible = false;
                return;
            }

            Root.visible = true;
            FixedString64Bytes bodyName = _.GetComponentData<Name>(body).Value;
            DiscoverProgress discovery = _.GetComponentData<DiscoverProgress>(body);

            HubNameLabel.text = bodyName.ToString();

            DiscoveryProgress.Max = discovery.MaxValue;
            DiscoveryProgress.Value = discovery.Value;
            DiscoveryProgress.Change = discovery.Progress;
        }
        
        public void UpdateHubUI(EntityManager _, Entity hub)
        {
            Root.visible = hub != Entity.Null;
            
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
        
        
    }
}