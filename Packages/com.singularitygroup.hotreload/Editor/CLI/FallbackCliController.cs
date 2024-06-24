
using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor.CLI {
    class FallbackCliController : ICliController {
        public string BinaryFileName => "";
        public string PlatformName => "";
        public bool CanOpenInBackground => false;
        public Task Start(StartArgs args) => Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;
    }
}