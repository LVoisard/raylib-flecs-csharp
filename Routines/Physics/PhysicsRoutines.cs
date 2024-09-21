using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp.Routines.Physics
{
    public class PhysicsRoutines : AbstractRoutineCollection
    {
        private Entity physicsPhase;
        public PhysicsRoutines(World world) : base(world) {}

        protected override void InitRoutinePipeline()
        {
            physicsPhase = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PostUpdate);
        }

        protected override void InitRoutines()
        {
            world.Routine<Position2D, CollisionRadius>()
                .Each((Entity self, ref Position2D pos, ref CollisionRadius col) =>
                {
                    Position2D selfPos = pos;
                    CollisionRadius selfCol = col;

                    // bool collides = false;
                    world.Each((Entity other, ref Position2D otherPos, ref CollisionRadius otherCol) =>
                    {
                        if (self.Id == other.Id) return;
                        if (Utils.DistanceFromTo(selfPos, otherPos) < selfCol.Value + otherCol.Value) {
                            Vector2 dir = Utils.GetDirectionVector(selfPos, otherPos);
                            dir = dir / dir.Length();
                            Vector2 offset = dir * (selfCol.Value + otherCol.Value);
                            otherPos.X = selfPos.X + offset.X;
                            otherPos.Y = selfPos.Y + offset.Y;

                            if (other.Has<PlayerControlled>() && self.Get<Team>() != other.Get<Team>())
                            {
                                other.Add<TakeDamage>();
                            }
                        }
                    });

                });
        }
    }
}
