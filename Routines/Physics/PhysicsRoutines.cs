using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using System.Numerics;

namespace raylib_flecs_csharp.Routines.Physics
{
    public class PhysicsRoutines : AbstractRoutineCollection
    {
        private Entity collisionPhase;
        private Entity physicsPhase;
        private Entity physicsCleanupPhase;
        private readonly float physicsDeltaTime = 0.0167f;
        public PhysicsRoutines(World world) : base(world) {
        }


        protected override void InitRoutinePipeline()
        {
            collisionPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PreUpdate);

            physicsPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PostUpdate);

            physicsCleanupPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(physicsPhase);
        }

        protected override void InitRoutines()
        {

            world.Routine<Position2D, CollisionRadius>("Detect Collisions")
                .Kind(collisionPhase)
                .Each((Entity self, ref Position2D pos, ref CollisionRadius col) =>
                {
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;

                    world.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol) =>
                    {
                        if (self.Id == other.Id) return;
                        if (other.Has<CollisionRecord>()) return;
                        if (Utils.DistanceFromTo(selfPos, otherPos) < selfCol.Value + otherCol.Value) {
                            other.Set<CollisionRecord>(new (self));
                            self.Set<CollisionRecord>(new (other));
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
                .Kind(physicsPhase)
                .Each((Entity self, ref CollisionRecord rec) =>
                {
                    if (!rec.other.IsAlive()) return;
                    Vector2 dir = Utils.GetDirectionVector(self.Get<Position2D>(), rec.other.Get<Position2D>());
                    Vector2 offset = dir / dir.Length() * (self.Get<CollisionRadius>().Value + rec.other.Get<CollisionRadius>().Value);
                    
                    if (!rec.other.Has<Immovable>())
                        rec.other.Set<Position2D>(new (self.Get<Position2D>().X + offset.X, self.Get<Position2D>().Y + offset.Y));
                });

            world.Routine("Collision Cleanup")
                .With<CollisionRecord>()
                .Kind(physicsCleanupPhase)
                .Each((Entity e) =>
                {
                    e.Remove<CollisionRecord>();
                });
                
        }
    }
}
