using System;
using System.Runtime.InteropServices;

namespace Lockstep.UnsafeECS {
    public interface IComponent { }

    public interface IEntity { }

    public interface IAsset { }

    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public struct AssetRef<T> where T : IAsset { }

    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public struct EntityRef<T> where T : IEntity { }

    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public struct PlayerRef { }

    [StructLayout(LayoutKind.Sequential, Pack = Define.PackSize)]
    public struct Button { }


    public class Define {
        public const int PackSize = 4;
        public static Type[] AllBuildInComponentTypes = new[] {
            typeof(Transform2D),
            typeof(Transform2DVertical),
            typeof(NavMeshAgent),
            typeof(CollisionAgent),
            typeof(Prefab),
            typeof(Animator),
        };
    }

    [System.FlagsAttribute()]
    public enum ComponentTypes : long {
        Transform2D = 1 << 0,
        Transform2DVertical = 1 << 1,
        NavMeshAgent = 1 << 2,
        CollisionAgent = 1 << 3,
        Prefab = 1 << 4,
        Animator = 1 << 5,
    }

    public class CoreComponentAttribute : Attribute {
        public int Index;
    }
    public class AssetLinkAttribute : Attribute {
        public Type type;
        public AssetLinkAttribute():base(){ }
        public AssetLinkAttribute(Type type):base(){
            this.type = type;

        }
    }
    
}