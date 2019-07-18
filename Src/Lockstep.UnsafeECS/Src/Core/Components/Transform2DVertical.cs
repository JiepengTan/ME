using System.Runtime.InteropServices;
using Lockstep.Math;

namespace Lockstep.UnsafeECS {
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 1)]
    public struct Transform2DVertical :IBuildInCompnent{
        public const int ComponentIdx = 1;
        public LFloat pos;
        public LFloat height;
    }
}