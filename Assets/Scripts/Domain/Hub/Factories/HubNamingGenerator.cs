using Unity.Collections;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.Hub.Factories
{
    public struct HubNamingGenerator
    {
        private static readonly FixedString32Bytes[] SPACE_NAME = 
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

        private static readonly FixedString32Bytes[] SPACE_ENDINGS = {
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

        public static FixedString64Bytes Get(ref Random random)
        {
            var name = SPACE_NAME[random.NextInt(SPACE_NAME.Length)];
            var ending = SPACE_ENDINGS[random.NextInt(SPACE_ENDINGS.Length)];
            FixedString64Bytes re = "";
            re.Append(name);
            re.Append(ending);
            return re;
        }
        
    }
}