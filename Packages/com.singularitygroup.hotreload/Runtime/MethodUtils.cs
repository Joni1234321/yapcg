#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Reflection;

namespace SingularityGroup.HotReload {
    static class MethodUtils {
#if ENABLE_MONO
        public static unsafe void DisableVisibilityChecks(MethodBase method) {
            if(IntPtr.Size == sizeof(long)) {
                var ptr = (MonoMethod64*)method.MethodHandle.Value.ToPointer();
                ptr->monoMethodFlags |= MonoMethodFlags.skip_visibility;
            } else {
                var ptr = (MonoMethod32*)method.MethodHandle.Value.ToPointer();
                ptr->monoMethodFlags |= MonoMethodFlags.skip_visibility;
            }
        }
#else
        public static void DisableVisibilityChecks(MethodBase method) { }
#endif
    }
}
#endif
