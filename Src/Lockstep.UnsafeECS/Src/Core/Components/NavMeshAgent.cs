using System.Runtime.InteropServices;

namespace Lockstep.UnsafeECS {
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 2)]
    public struct NavMeshAgent :IBuildInCompnent{
        public const int ComponentIdx = 2;
    }
}