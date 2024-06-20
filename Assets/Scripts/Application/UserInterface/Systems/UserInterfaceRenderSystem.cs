using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using YAPCG.Application.UserInterface.Components;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;
using YAPCG.Engine.Time.Components;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Application.UserInterface.Systems
{
    //[WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct UserInterfaceRenderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TickSpeed>();
            state.RequireForUpdate<TickSpeedLevel>();
            state.RequireForUpdate<FocusedBody>();
            state.RequireForUpdate<HUD.HUDReady>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.GetSingleton<ApplicationSettings>().DisableUIRender)
            {
                HUD.Instance.WorldUserInterface.WorldPlanetControlRenderer.Draw(0); 
                return;
            }
            EntityQuery bodyQuery = SystemAPI.QueryBuilder().WithAll<Body.BodyTag, Position, Body.Owner, ScaleComponent, DiscoverProgress, Name>().Build();

            Entity selected = SystemAPI.GetSingleton<FocusedBody>().Selected;
           
            HUD.Instance.MainUserInterface.UpdateBodyUI(state.EntityManager, selected); 
            HUD.Instance.MainUserInterface.UpdateSpeed(SystemAPI.GetSingleton<TickSpeed>(), SystemAPI.GetSingleton<TickSpeedLevel>()); 
            
            Camera camera = Camera.main;
            NativeArray<Entity> entities = bodyQuery.ToEntityArray(state.WorldUpdateAllocator);
            NativeArray<float3> positions = bodyQuery.ToComponentDataArray<Position>(state.WorldUpdateAllocator).Reinterpret<Position, float3>();
            NativeArray<FixedString64Bytes> names = bodyQuery.ToComponentDataArray<Name>(state.WorldUpdateAllocator).Reinterpret<Name, FixedString64Bytes>();
            NativeArray<DiscoverProgress> discoveryProgress = bodyQuery.ToComponentDataArray<DiscoverProgress>(state.WorldUpdateAllocator);
            NativeArray<Body.Owner> owners = bodyQuery.ToComponentDataArray<Body.Owner>(state.WorldUpdateAllocator);
            NativeArray<StyleClasses.BorderColor> borderColors = CollectionHelper.CreateNativeArray<StyleClasses.BorderColor>(names.Length, state.WorldUpdateAllocator);
            
            for (int i = 0; i < positions.Length; i++)
                positions[i] = camera.WorldToScreenPoint(positions[i]);
            
            for (int i = 0; i < borderColors.Length; i++)
                borderColors[i] = owners[i].ID == 0 ? StyleClasses.BorderColor.Impossible : StyleClasses.BorderColor.Valid;

            int selectedIndex = 0;
            bool selectedClaimable = false;
            for (; selectedIndex < entities.Length; selectedIndex++)
            {
                if (entities[selectedIndex] == selected)
                {
                   selectedClaimable = owners[selectedIndex].ID == Body.Owner.NO_OWNER_ID;
                    break;
                }
            }

            
            HUD.Instance.WorldUserInterface.WorldPlanetControlRenderer.Draw(entities, names, borderColors, positions, selectedIndex, selectedClaimable); 
        }
    }
}