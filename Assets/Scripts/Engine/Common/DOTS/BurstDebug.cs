using Unity.Burst;

namespace YAPCG.Engine.Common.DOTS
{
    public class BurstDebug
    {
        [BurstDiscard]
        private static void SetIfManaged(ref bool b) => b = false;

        public static bool IsBurst()
        {
            var b = true;
            SetIfManaged(ref b);
            return b;
        }
    }
}