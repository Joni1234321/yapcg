using Unity.Collections;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine.Common
{
    public struct NamingGenerator
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

        
        private static readonly string[] PLANET_PREFIXES = 
        {
            "Zan", "Pro", "Gor", "Xan", "Ver", "Tar", "Hel", "Bet", "Nor", "Tra"
        };

        private static readonly string[] PLANET_SUFFIXES = 
        {
            "onia", "ar", "ion", "arious", "us", "ara", "on", "ia", "ix", "ara"
        };

        private static readonly string[] PLANET_MIDDLE = 
        {
            "th", "nd", "l", "v", "m", "r", "t", "z", "n", "x"
        };

        public static FixedString64Bytes GetPlanetName(ref Random random)
        {
            var name = PLANET_PREFIXES[random.NextInt(PLANET_PREFIXES.Length)];
            var midle = PLANET_MIDDLE[random.NextInt(PLANET_MIDDLE.Length)];
            var ending = PLANET_SUFFIXES[random.NextInt(PLANET_SUFFIXES.Length)];
            FixedString64Bytes re = "";
            re.Append(name);
            re.Append(midle);
            re.Append(ending);
            return re;
        }
    }
}