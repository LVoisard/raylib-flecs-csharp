namespace raylib_flecs_csharp.Helper
{
    public enum CollisionLayers
    {
        Player = 0x0001,
        PlayerSpawned = 0x0002,
        Enemy = 0x0004,
        EnemySpawned = 0x0008,
    }

    public record struct CollisionFilter(CollisionLayers self, CollisionLayers collidesWith);

    public static class Physics
    {
        public static CollisionFilter PlayerCollisionFilter = new CollisionFilter(CollisionLayers.Player, CollisionLayers.Player | CollisionLayers.Enemy | CollisionLayers.EnemySpawned);
        public static CollisionFilter PlayerSpawnedCollisionFilter = new CollisionFilter(CollisionLayers.PlayerSpawned, CollisionLayers.Enemy | CollisionLayers.EnemySpawned);
        public static CollisionFilter EnemyCollisionFilter = new CollisionFilter(CollisionLayers.Enemy, CollisionLayers.Player | CollisionLayers.Enemy | CollisionLayers.PlayerSpawned);
        public static CollisionFilter EnemySpawnedCollisionFilter = new CollisionFilter(CollisionLayers.EnemySpawned, CollisionLayers.Player | CollisionLayers.PlayerSpawned);

        public static bool Collide(CollisionFilter a, CollisionFilter b)
        {
            return (a.collidesWith & b.self) != 0;
        }
    }
}
