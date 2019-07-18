using Lockstep.ECS.ECDefine;
namespace  Lockstep.ECS.ECDefine.UnsafeECS {

    public partial class Global : IGlobal {
        [EntityCount(4)] public Spawner[] PlayerSpawner;
        [EntityCount(30)] public Spawner[] EnemySpawner;
        [EntityCount(10)] public Spawner[] PowerSpawner;
    }
}