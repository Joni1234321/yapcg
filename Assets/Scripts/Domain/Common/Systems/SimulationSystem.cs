using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Domain.Common.Components;

namespace YAPCG.Domain.Common.Systems
{
    public partial struct Market : IComponentData
    {
        public int Food, Oxygen, Minerals, Water;
    }
    public struct Population : IComponentData
    {
        public int Farmers, Laborers, Engineers;
        public float FarmersNeed, LaborersNeed, EngineersNeed;
    }
    
    public partial struct SimulationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SimulationJob().Run();
        }

        struct Need
        {
            public Market Existential, Luxurious, Nice;
        }
        // later
        struct Machines
        {
            public int Oxygen, Mines;
        }
    }

    [BurstCompile]
    partial struct SimulationJob : IJobEntity
    {
        void Execute(in Market market)
        {
        }
        
        void Build(in Population population, ref Market market)
        {
            market.Food = population.Farmers;
            market.Minerals = population.Laborers;
            market.Oxygen = population.Engineers * 100;
            market.Water = population.Engineers * 20;
        }

        void Consume(Market market, ref Population population, out Market leftovers)
        {
            int populationSum = population.Farmers + population.Engineers + population.Laborers;
            int oxygenNeed = populationSum;
            int foodNeed = populationSum;
            int waterNeed = populationSum;

            Market bought = new();

            bought.Oxygen = math.max(market.Oxygen, oxygenNeed);
            bought.Food = math.max(market.Food, foodNeed);
            bought.Water = math.max(market.Water, waterNeed);

            market -= bought;

            float oxygenRatio = (float)oxygenNeed / bought.Oxygen;
            float foodRatio = (float)foodNeed / bought.Food;
            float waterRatio = (float)waterNeed / bought.Water;

            leftovers = market;


            // pops
            float decrease = 1 - oxygenNeed;
            float increase = 1.1f;

            int children = (int) (populationSum * increase);

            population.Farmers   = (int)(population.Farmers   * decrease);
            population.Laborers  = (int)(population.Laborers  * decrease);
            population.Engineers = (int)(population.Engineers * decrease);


            // education, so chicldren from engineers start educated and cna become engineers, rest cant. So only promotions.
            if (oxygenRatio < 1 || waterRatio < 1)
            {
                population.Engineers += children;
            }
            else if (foodRatio < 1)
            {                           
                population.Farmers += children;
            }
            else
            {
                population.Laborers += children;
            }
        }

        void Cash(in Market market, ref Points points)
        {
            points.Value += market.Minerals;
        }
    }
    partial struct Market
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Market operator -(Market a, Market b)
        {
            return new Market
            {
                Food = a.Food - b.Food,
                Oxygen = a.Oxygen - b.Oxygen,
                Minerals = a.Minerals - b.Minerals,
                Water = a.Water - b.Water
            };
        }
    }

}