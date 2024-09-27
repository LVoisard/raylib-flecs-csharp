using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Helper;
using static raylib_flecs_csharp.Helper.Physics;
using System.Numerics;
using System.ComponentModel;

namespace raylib_flecs_csharp.Systems.Physics
{
    public class PhysicsSystems : AbstractSystemCollection
    {
        private Entity fixedUpdated;
        private Entity collisionPhase;
        private Entity physicsCleanupPhase;

        private TimerEntity physicsTimer;
        private readonly float physicsFrameTime = 0.02f;
        public PhysicsSystems(World world) : base(world)
        {

            world.Component<ContainedIn>().Entity.Add(Ecs.Exclusive);
        }


        protected override void InitSystemPipeline()
        {
            fixedUpdated = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnUpdate);

            collisionPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnValidate);

            physicsCleanupPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PostUpdate);
        }

        protected override void InitCustomIntervals()
        {
            physicsTimer = world.Timer()
                .Interval(physicsFrameTime);
        }

        protected override void InitSystems()
        {
            QueryBuilder innerQuery = world.QueryBuilder<Position2D, CollisionRadius, CollisionFilter>()
               .Without<Trigger>();

            world.Routine<Position2D, CollisionRadius, CollisionFilter>("Detect Collisions")
                //.With<ContainedIn>(Ecs.Wildcard)
                .Without<Trigger>()
                .TickSource(physicsTimer)
                .Kind(collisionPhase)
                .MultiThreaded()
                // need to change checking team, should use a collision mask
                .Each((Iter it, int i, ref Position2D pos, ref CollisionRadius col, ref CollisionFilter filter) =>
                {
                    Entity self = it.Entity(i);
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;
                    CollisionFilter selfFilter = filter;
                    //Entity containedInEntity = it.Pair(3).Second();
                    //Position2D cellPos = containedInEntity.Get<Position2D>();
                    //List<Entity> neighbourEntities = new List<Entity>();

                    //for (int x = -1; x < 2; x++)
                    //{
                    //    for (int y = -1; y < 2; y++)
                    //    {
                    //        if (x == 0 && y == 0) continue;
                    //        var container = world.Lookup("World Partition Container");
                    //        using Query qq = world.QueryBuilder<Position2D>().With(Ecs.ChildOf, container).Build();
                    //        Entity e = qq.Find((ref Position2D p) => p.X == cellPos.X + x && p.Y == cellPos.Y + y);
                    //        qq.Dispose();
                    //        if (e != 0)
                    //            neighbourEntities.Add(e);
                    //    }
                    //}


                    var builder = world.QueryBuilder<Position2D, CollisionRadius, CollisionFilter>();//.With<ContainedIn>(containedInEntity);
                                                                                    //.GroupBy<ContainedIn>();
                    //foreach (Entity e in neighbourEntities)
                    //    builder.Or().With<ContainedIn>(e);

                    using Query q = builder.Build();

                    q.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol, ref CollisionFilter otherFilter) =>
                    {
                        // can't collide with self
                        if (self.Id == other.Id) return;
                        if (other.Has<Trigger>()) return;

                        // check collision compatibility

                        if (!Collide(selfFilter, otherFilter)) return;

                        if (Utils.DistanceSquaredFromTo(selfPos, otherPos) < MathF.Pow(selfCol.Value + otherCol.Value, 2))
                        {
                            if (!self.Has<Immovable>())
                            {
                                Vector2 dir = Utils.GetDirectionVector(other.Get<Position2D>(), self.Get<Position2D>());
                                Vector2 offset = dir * 1.01f * (self.Get<CollisionRadius>().Value + other.Get<CollisionRadius>().Value);

                                self.Set<Position2D>(new(other.Get<Position2D>().X + offset.X, other.Get<Position2D>().Y + offset.Y));
                            }
                            self.Add<CollidedWith>(other);
                        }
                    });

                    q.Dispose();
                });

            world.Routine<Position2D, CollisionRadius, CollisionFilter>("Detect Triggers")
                //.With<ContainedIn>(Ecs.Wildcard)
                .With<Trigger>()
                .TickSource(physicsTimer)
                .Kind(collisionPhase)
                .Immediate()
                // need to change checking team, should use a collision mask
                .Each((Iter it, int i, ref Position2D pos, ref CollisionRadius col, ref CollisionFilter filter) =>
                {
                    Entity self = it.Entity(i);
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;
                    CollisionFilter selfFilter = filter;
                    //Entity containedInEntity = it.Pair(3).Second();


                    using Query q = world.QueryBuilder<Position2D, CollisionRadius, CollisionFilter>().Build();
                                                                                    //.With<ContainedIn>(containedInEntity).Build();

                    q.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol, ref CollisionFilter otherFilter) =>
                    {
                        // can't collide with self
                        if (self.Id == other.Id) return;

                        // check collision compatibility

                        if (!Collide(selfFilter, otherFilter)) return;

                        if (Utils.DistanceSquaredFromTo(selfPos, otherPos) < MathF.Pow(selfCol.Value + otherCol.Value, 2))
                        {
                            self.Add<TriggeredWith>(other);
                        }
                    });

                    q.Dispose();

                });


            world.Routine<Position2D, InputDirection2D, Speed>("Update Positions")
                .TickSource(physicsTimer)
                .Kind(fixedUpdated)
                .Each((Iter it, int i, ref Position2D pos, ref InputDirection2D dir, ref Speed sp) =>
                {

                    pos.X += dir.X * it.DeltaSystemTime() * sp.Value;
                    pos.Y += dir.Y * it.DeltaSystemTime() * sp.Value;
                });

            //world.Routine<Position2D>("Assign collision entities to partitions")
            //    .With<CollisionFilter>()
            //    .TickSource(physicsTimer)
            //    .Kind(fixedUpdated)
            //    //.Rate(5) // update every 0.1 seconds
            //    .MultiThreaded()
            //    .Each((Entity e, ref Position2D pos) =>
            //    {
            //        int cellSize = 64;
            //        int xPos = (int)float.Floor(pos.X / cellSize);
            //        int yPos = (int)float.Floor(pos.Y / cellSize);

            //        bool containsPartition = false;
            //        //if (e.Has<ContainedIn>(Ecs.Wildcard))
            //        //    e.Remove<ContainedIn>(Ecs.Wildcard);
            //        var container = world.Lookup("World Partition Container");
            //        container.Children((Entity child) =>
            //        {
            //            if (child.Get<Position2D>().X == xPos && child.Get<Position2D>().Y == yPos)
            //            {
            //                containsPartition = true;

            //                e.Add<ContainedIn>(child);
            //                return;
            //            }
            //        });

            //        if (!containsPartition)
            //        {
            //            var entity = world.Entity().Set<Position2D>(new(xPos, yPos)).ChildOf(container);
            //            e.Add<ContainedIn>(entity);
            //        }

            //    });

            //world.Routine("Partition Cleanup")
            //    .With(Ecs.ChildOf, world.Lookup("World Partition Container"))
            //    .TickSource(physicsTimer)
            //    .Kind(Ecs.PreUpdate)
            //    .Immediate()
            //    .Each((Entity e) =>
            //    {
            //        using Query q = world.QueryBuilder().With<ContainedIn>(e).Build();
            //        int count = q.Count();
            //        q.Dispose();
            //        if (count == 0)
            //            e.Destruct();

            //    });

            world.Routine("Collision Cleanup Objects that destroy after colliding or triggering")
                .With<CollidedWith>(Ecs.Wildcard).Or().With<TriggeredWith>(Ecs.Wildcard)
                .With<DestroyOnCollision>().Or().With<DestroyOnTrigger>()
                .Kind(Ecs.PreStore)
                .Each((Entity e) =>
                {
                    e.Destruct();
                });

            world.Routine("Collision Cleanup")
                .With<CollidedWith>(Ecs.Wildcard)
                .Kind(Ecs.PreStore)
                .Each((Entity e) =>
                {
                    e.Remove<CollidedWith>(Ecs.Wildcard);
                });
        }
    }
}
