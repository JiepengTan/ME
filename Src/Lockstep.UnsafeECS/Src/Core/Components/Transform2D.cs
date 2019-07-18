using System.Runtime.InteropServices;
using Lockstep.Math;

namespace Lockstep.UnsafeECS {
    public interface IBuildInCompnent { }

    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 0)]
    public struct Transform2D :IBuildInCompnent{
        public const int ComponentIdx = 0;
        public LVector2 pos;
        public LFloat deg;
    }
}