using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Components;
using YAPCG.Resources.View.Custom;
using YAPCG.Resources.View.Custom.Util;
using YAPCG.Simulation.Units;
using Length = YAPCG.Simulation.Units.Length;

namespace YAPCG.Application.UserInterface.UI
{
    public class MainUserInterface
    {
        public VisualElement Root, BodyMenu;
        
        public SlotControl SlotControlS, SlotControlM, SlotControlL;
        public Label HubNameLabel;

        public Label SpeedLabel;
        public ProgressBarControl DiscoveryProgress;

        public NumberLabel Gravity, Escape, Radius, Mass, Distance, Sidereal, Mu;

        public MainUserInterface(VisualElement root)
        {
            Root = root;
            SpeedLabel = root.Q<Label>("speed");
            BodyMenu = root.Q<VisualElement>("body-menu");
            
            HubNameLabel = BodyMenu.Q<Label>("name");

            SlotControlS = BodyMenu.Q<SlotControl>("slots-s");
            SlotControlM = BodyMenu.Q<SlotControl>("slots-m");
            SlotControlL = BodyMenu.Q<SlotControl>("slots-l");

            DiscoveryProgress = BodyMenu.Q<ProgressBarControl>("discovery");

            Gravity = BodyMenu.Q<NumberLabel>("gravity");
            Escape = BodyMenu.Q<NumberLabel>("escape");
            Radius = BodyMenu.Q<NumberLabel>("radius");
            Mass = BodyMenu.Q<NumberLabel>("mass");
            Distance = BodyMenu.Q<NumberLabel>("distance");
            Sidereal = BodyMenu.Q<NumberLabel>("sidereal");
            Mu = BodyMenu.Q<NumberLabel>("mu");
        }

        public void UpdateSpeed(in TickSpeed speed, in TickSpeedLevel tickSpeedLevel)
        {
            string text = speed.SpeedUp.ToString(CultureInfo.CurrentCulture);
            if (SpeedLabel.text == text)
                return;
            
            SpeedLabel.RemoveFromClassList(StyleClasses.Transitions.Opacity.ToClass());
            SpeedLabel.style.opacity = 1.0f;
            SpeedLabel.text = text;
            SpeedLabel.schedule.Execute(() =>
            {
                SpeedLabel.AddToClassList(StyleClasses.Transitions.Opacity.ToClass());
                SpeedLabel.schedule.Execute(() => SpeedLabel.style.opacity = 0.0f).ExecuteLater(0);
            }).ExecuteLater(2500);
        }
        
        public void UpdateBodyUI(EntityManager _, Entity body)
        {
            if (body == Entity.Null)
            {
                BodyMenu.visible = false;
                return;
            }

            BodyMenu.visible = true;
            FixedString64Bytes bodyName = _.GetComponentData<Name>(body).Value;
            DiscoverProgress discovery  = _.GetComponentData<DiscoverProgress>(body);
            Body.BodyInfo info          = _.GetComponentData<Body.BodyInfo>(body);
            
            HubNameLabel.text = bodyName.ToString();

            DiscoveryProgress.Max    = discovery.MaxValue;
            DiscoveryProgress.Value  = discovery.Value;
            DiscoveryProgress.Change = discovery.Progress;

            Mass.Value     =  info.EarthMass.ToString($"0.# M.{"E".ToSubscript()}", CultureInfo.CurrentCulture);
            Radius.Value   =  info.EarthRadius.ToString($"0.# R.{"E".ToSubscript()}", CultureInfo.CurrentCulture);
            Escape.Value   = (info.EscapeVelocity / 1000f).ToString("0.## km/s", CultureInfo.CurrentCulture);
            Gravity.Value  = (info.EarthGravity * 1000f).ToString($"0 mg", CultureInfo.CurrentCulture);
            Mu.Value       = info.Mu.ToScientific($"m{"3".ToSuperscript()}/s{"2".ToSuperscript()}");

            if (_.HasComponent<Body.Orbit>(body))
            {
                Body.Orbit orbit = _.GetComponentData<Body.Orbit>(body);
                Distance.Value = new Length(orbit.AU, Length.UnitType.AstronomicalUnits).To(Length.UnitType.LightSecond).ToString("0.## ls");
                Sidereal.Value = (orbit.Period.Days / 29.53f).ToString("0.## month", CultureInfo.CurrentCulture);
            }
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