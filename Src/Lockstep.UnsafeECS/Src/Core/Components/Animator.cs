using System.Runtime.InteropServices;

namespace Lockstep.UnsafeECS {
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 5)]
    public struct Animator :IBuildInCompnent{
        public const int ComponentIdx = 5;
    }
}