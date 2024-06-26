using Unity.Entities;

namespace YAPCG.Application.Input
{
    public struct ActionInput : IComponentData
    {
        public bool ShouldBuildHub;
        public bool Next, Previous;
        public bool LeftClickSelectBody, DeselectBody;

        public bool SpeedIncrease, SpeedDecrease, SpeedPause;

        public float Zoom;
        public bool ResetView;
    }
}