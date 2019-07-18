using Lockstep.ECS.ECDefine;

namespace Lockstep.ECS.ECDefine.UnsafeECS {
    public class OnDamage : ISignal {
        public DamageStructure dmg;
    }

    public class OnBossDeath : ISignal { }

    public class OnCastProjectile : ISignal {
        public EntityRef projectileSource;
        public Vector2 forward;
        public Vector2 right;
        public float projectileAngle;
        public EProjectileType projectileType;
        public ProjectileSpec projectileSpec;
    }

    public class CollisionDefine : ICollisionDefine {
        [Signal]
        public void CollisionFunc(Projectile projectile, Goblin goblin){ }

        [Signal]
        public void CollisionFunc(Projectile projectile, Enemy goblin){ }

        [Signal]
        public void CollisionFunc(Projectile projectile){ }
    }
}