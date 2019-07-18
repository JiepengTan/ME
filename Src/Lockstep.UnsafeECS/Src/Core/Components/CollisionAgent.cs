using System.Runtime.InteropServices;
using Lockstep.Math;

namespace Lockstep.UnsafeECS {
    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    [CoreComponent(Index = 3)]
    public struct CollisionAgent :IBuildInCompnent{
        public const int ComponentIdx = 3;
        public LVector2 pos;
        public LFloat deg;
        
        public LFloat yPos;
        public LFloat height;
        
        public LVector2 vel;
        public LFloat degVel;

        //circle aabb obb collision Info
        public LFloat colRadius;
        public LVector2 colDir;
        public LVector2 colSize;
        public ushort polygonId;
        
        public int layer;
        public bool isTrigger;
    }
}