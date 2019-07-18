using System.Runtime.InteropServices;

namespace Lockstep.UnsafeECS {
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 4)]
    public struct Prefab :IBuildInCompnent{
        public const int ComponentIdx = 4;
    }
}