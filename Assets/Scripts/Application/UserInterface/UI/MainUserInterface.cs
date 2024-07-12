using System;
using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.DOTSExtension.THPS.Singletons;
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
        
        public SlotControl SlotControlLand3, SlotControlLand4, SlotControlLand5;
        public Label NameLabel;

        public Label SpeedLabel;
        public ProgressBarControl DiscoveryProgress;

        public Button ClaimButton;

        public NumberLabel Gravity, Escape, Radius, Mass, Distance, Sidereal, Mu;
        private Entity _selected;

        private EntityQuery _actionClaimQuery;
        public MainUserInterface(VisualElement root)
        {
            Root = root;
            SpeedLabel = root.Q<Label>("speed");
            BodyMenu = root.Q<VisualElement>("body-menu");
            
            NameLabel = BodyMenu.Q<Label>("name");

            SlotControlLand3 = BodyMenu.Q<SlotControl>("land-orbits");
            SlotControlLand4 = BodyMenu.Q<SlotControl>("land-probes");
            SlotControlLand5 = BodyMenu.Q<SlotControl>("land-people");
            
            DiscoveryProgress = BodyMenu.Q<ProgressBarControl>("discovery");

            ClaimButton = BodyMenu.Q<Button>("claim");
            
            Gravity = BodyMenu.Q<NumberLabel>("gravity");
            Escape = BodyMenu.Q<NumberLabel>("escape");
            Radius = BodyMenu.Q<NumberLabel>("radius");
            Mass = BodyMenu.Q<NumberLabel>("mass");
            Distance = BodyMenu.Q<NumberLabel>("distance");
            Sidereal = BodyMenu.Q<NumberLabel>("sidereal");
            Mu = BodyMenu.Q<NumberLabel>("mu");

            _actionClaimQuery = new EntityQueryBuilder(Allocator.Temp).WithAllRW<Body.ActionClaim>().Build(World.DefaultGameObjectInjectionWorld.EntityManager);
            ClaimButton.clickable.clicked += () =>
            {
                if (_selected == Entity.Null)
                    return;
                Body.ActionClaim actionClaim = new Body.ActionClaim { Body = _selected, OwnerID = Body.Owner.YOU_OWNER_ID };
                _actionClaimQuery.SetSingleton(actionClaim);
            };
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
            _selected = body;
            if (body == Entity.Null)
            {
                BodyMenu.visible = false;
                ClaimButton.style.display = DisplayStyle.None;
                return;
            }

            
            FixedString64Bytes bodyName = _.GetComponentData<Name>(body).Value;
            DiscoverProgress discovery  = _.GetComponentData<DiscoverProgress>(body);
            LandDiscovery landDiscovery = _.GetComponentData<LandDiscovery>(body);
            Body.BodyInfo info          = _.GetComponentData<Body.BodyInfo>(body);
            Body.Owner owner            = _.GetComponentData<Body.Owner>(body);

            BodyMenu.visible = true;
            ClaimButton.style.display =  owner.ID == Body.Owner.NO_OWNER_ID ? DisplayStyle.Flex : DisplayStyle.None;

            NameLabel.text = bodyName.ToString();

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
            
            SlotControlLand3.Value = landDiscovery.Orbit.ToString();
            SlotControlLand4.Value = landDiscovery.Probes.ToString();
            SlotControlLand5.Value = landDiscovery.People.ToString();
        }

    }
}