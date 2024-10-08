using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor.CLI {
    interface ICliController {
        string BinaryFileName {get;}
        string PlatformName {get;}
        bool CanOpenInBackground {get;}

        Task Start(StartArgs args);
        
        Task Stop();
    }
}