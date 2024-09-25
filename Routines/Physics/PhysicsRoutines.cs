using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using System.Numerics;

namespace raylib_flecs_csharp.Routines.Physics
{
    public class PhysicsRoutines : AbstractRoutineCollection
    {
        private Entity physicsPhase;
        private Entity collisionPhase;
        private Entity physicsCleanupPhase;

        public PhysicsRoutines(World world) : base(world) 
        {
        }


        protected override void InitRoutinePipeline()
        {
            physicsPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnUpdate);

            collisionPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnValidate);

            physicsCleanupPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PostUpdate);
        }

        protected override void InitRoutines()
        {
            world.Routine<Position2D, CollisionRadius, CollisionFilter>("Detect Collisions")
                .Kind(collisionPhase)
                .Immediate()
                // need to change checking team, should use a collision mask
                .Each((Entity self, ref Position2D pos, ref CollisionRadius col, ref CollisionFilter filter) =>
                {
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;
                    CollisionFilter selfFilter = filter;

                    world.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol, ref CollisionFilter otherFilter) =>
                    {
                        // can't collide with self
                        if (self.Id == other.Id) return;

                        // check collision compatibility

                        if (!Physics.Collide(selfFilter, otherFilter)) return;

                        if (Utils.DistanceFromTo(selfPos, otherPos) < selfCol.Value + otherCol.Value) {
                            // other.Set<CollisionRecord>(new (self));
                            //Entity e = world.Entity();
                            self.Set<CollisionRecord>(new (self, other));
                        }
                    });

                });

            world.Routine<Position2D, InputDirection2D, Speed>("Update Positions")
                .Kind(physicsPhase)
                .Each((Iter it, int i, ref Position2D pos, ref InputDirection2D dir, ref Speed sp) =>
                {
                    pos.X += dir.X * it.DeltaTime() * sp.Value;
                    pos.Y += dir.Y * it.DeltaTime() * sp.Value;
                });

            world.Routine<CollisionRecord>("Collision Detected")
                .Kind(physicsCleanupPhase)
                .Each((ref CollisionRecord rec) =>
                {                    
                    Vector2 dir = Utils.GetDirectionVector(rec.self.Get<Position2D>(), rec.other.Get<Position2D>());
                    Vector2 offset = dir * (rec.self.Get<CollisionRadius>().Value + rec.other.Get<CollisionRadius>().Value);
                    
                    if (!rec.other.Has<Immovable>() && !rec.other.Has<CollisionTrigger>())
                        rec.other.Set<Position2D>(new (rec.self.Get<Position2D>().X + offset.X, rec.self.Get<Position2D>().Y + offset.Y));   
                });

            world.Routine("Collision Cleanup Objects that destroy after colliding")
                .With<CollisionRecord>()
                .With<DestroyOnCollision>()
                .Kind(Ecs.PreUpdate)
                .Each((Entity e) =>
                {
                    e.Destruct();
                });

            world.Routine("Collision Cleanup")
                .With<CollisionRecord>()
                .Kind(Ecs.PreUpdate)
                .Each((Entity e) =>
                {
                    e.Remove<CollisionRecord>();
                });
        }        
    }

    public enum CollisionLayers 
    {
        Player = 0x0001,
        PlayerSpawned = 0x0002,
        Enemy = 0x0004,
        EnemySpawned = 0x0008,
    }

    public static class Physics
    {
        public static CollisionFilter PlayerCollisionFilter = new CollisionFilter(CollisionLayers.Player, CollisionLayers.Player | CollisionLayers.Enemy | CollisionLayers.EnemySpawned);
        public static CollisionFilter PlayerSpawnedCollisionFilter = new CollisionFilter(CollisionLayers.PlayerSpawned, CollisionLayers.Enemy | CollisionLayers.EnemySpawned);
        public static CollisionFilter EnemyCollisionFilter = new CollisionFilter(CollisionLayers.Enemy, CollisionLayers.Player | CollisionLayers.Enemy | CollisionLayers.PlayerSpawned);
        public static CollisionFilter EnemySpawnedCollisionFilter = new CollisionFilter(CollisionLayers.EnemySpawned, CollisionLayers.Player | CollisionLayers.PlayerSpawned);

        public static bool Collide(CollisionFilter a, CollisionFilter b)
        {
            return (a.collidesWith & b.self) != 0 && (a.self & b.collidesWith) != 0;
        }
    }    
}
