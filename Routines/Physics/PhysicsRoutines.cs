using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using System.Collections;
using System.Numerics;

namespace raylib_flecs_csharp.Routines.Physics
{
    public class PhysicsRoutines : AbstractRoutineCollection
    {
        private Entity physicsPhase;
        private Entity collisionPhase;
        private Entity physicsCleanupPhase;
        public PhysicsRoutines(World world) : base(world) {
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
            world.Routine<Position2D, CollisionRadius, Team>("Detect Collisions")
                .Kind(collisionPhase)
                .Immediate()
                // need to change checking team, should use a collision mask
                .Each((Entity self, ref Position2D pos, ref CollisionRadius col, ref Team team) =>
                {
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;
                    Team selfTeam = team;

                    world.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol, ref Team otherTeam) =>
                    {
                        if (self.Id == other.Id) return;
                        if (selfTeam == otherTeam) return;
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
                .Kind(physicsCleanupPhase)
                .Each((Entity self, ref CollisionRecord rec) =>
                {                    
                    Vector2 dir = Utils.GetDirectionVector(self.Get<Position2D>(), rec.other.Get<Position2D>());
                    Vector2 offset = dir * (self.Get<CollisionRadius>().Value + rec.other.Get<CollisionRadius>().Value);
                    
                    if (!rec.other.Has<Immovable>() && !rec.other.Has<CollisionTrigger>())
                        rec.other.Set<Position2D>(new (self.Get<Position2D>().X + offset.X, self.Get<Position2D>().Y + offset.Y));   
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

        public class Physics 
        {

            private List<CollisionLayer> collisionLayers = new List<CollisionLayer>();

            private List<BitArray> collisionLayersInteractions = new List<BitArray>();

            public CollisionLayer RegisterCollisionLayer(string name)
            {
                if (collisionLayers.Any(x => x.GetName().Equals(name)))
                    return collisionLayers.First(x => x.GetName().Equals(name));

                var cl = new CollisionLayer(name);
                collisionLayers.Add(cl);

                foreach (var layer in collisionLayersInteractions) {
                    layer.Length += 1;
                    layer[layer.Length] = true;
                }

                collisionLayersInteractions.Add(new BitArray(collisionLayers.Count));
                collisionLayersInteractions[collisionLayersInteractions.Count].SetAll(true);

                return cl;
            }

            public bool CanCollide(CollisionLayer a, CollisionLayer b) 
            {
                return collisionLayersInteractions[a.GetID()][b.GetID()];
            }

            private static int layerIdCount = 0;
            public struct CollisionLayer {
                
                private string name;
                private int id;
                public CollisionLayer(string name) {
                    this.name = name;
                    this.id = layerIdCount++;
                }

                public string GetName() { return name; }
                public int GetID() { return id; }
            }
        }
    }
}
