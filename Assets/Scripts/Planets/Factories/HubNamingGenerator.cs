using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace YAPCG.Planets.Factories
{
    public struct HubNamingGenerator
    {
        private static Random RANDOM = Random.CreateFromIndex(0);
        private static readonly List<string> SPACE_NAME = new()
        {
            "Celestial",
            "Stellar",
            "Galactic",
            "Cosmic",
            "Astro",
            "Nebula",
            "Infinity",
            "Star",
            "Solar",
            "Orion",
            "Interstellar",
            "Sky",
            "Astral",
            "Cosmos",
            "Nova",
            "Quasar"
        };

        private static readonly List<string> SPACE_ENDINGS = new()
        {
            "Hub",
            "Station",
            "Gateway",
            "Port",
            "Nex",
            "Harbor",
            "Junction",
            "Node",
            "Link",
            "Nest",
            "Connect",
            "Core",
            "Outpost",
            "Hub",
            "Post",
            "Center"
        };

        public static FixedString64Bytes Get() => 
            SPACE_NAME[RANDOM.NextInt(SPACE_NAME.Count)] + SPACE_ENDINGS[RANDOM.NextInt(SPACE_ENDINGS.Count)];
        
    }
}