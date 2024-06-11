using Unity.Entities;

namespace YAPCG.Engine.Common.DOTS
{
    public static class Globals
    {
        public static EntityManager _ = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
}