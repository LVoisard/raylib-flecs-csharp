using Flecs.NET.Core;
using raylib_flecs_csharp.Components;
using Raylib_cs;
using System.Collections.Generic;

namespace raylib_flecs_csharp.Routines.PlayerInput
{
    public class PlayerInputRoutines : AbstractRoutineCollection
    {
        private Entity inputRoutine;
        private Entity postInputRoutine;

        public PlayerInputRoutines(World world) : base(world) { }

        protected override void InitRoutinePipeline()
        {
            inputRoutine = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PreUpdate)
                .ChildOf(Ecs.PreUpdate);

            postInputRoutine = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PreUpdate)
                .ChildOf(Ecs.PreUpdate);

        }

        protected override void InitRoutines()
        {
            world.Routine("Post Input")
                .Kind(postInputRoutine)
                .Run((Iter it) => { Raylib.PollInputEvents(); });

            world.Routine<InputDirection2D>("Player Input Movement")
                .Kind(inputRoutine)
                .With<PlayerControlled>()
                .Each((ref InputDirection2D dir) =>
                {
                    dir.X = 0.0f;
                    dir.Y = 0.0f;

                    if (Raylib.IsKeyDown(KeyboardKey.W))
                    {
                        dir.Y = -1.0f;
                    }
                    else if (Raylib.IsKeyDown(KeyboardKey.S))
                    {
                        dir.Y = 1.0f;
                    }

                    if (Raylib.IsKeyDown(KeyboardKey.A))
                    {
                        dir.X = -1.0f;
                    }
                    else if (Raylib.IsKeyDown(KeyboardKey.D))
                    {
                        dir.X = 1.0f;
                    }

                    float velLength = MathF.Sqrt(MathF.Pow(dir.X, 2) + MathF.Pow(dir.Y, 2));
                    if (velLength == 0.0f) return;
                    dir.X *= (1.0f / velLength);
                    dir.Y *= (1.0f / velLength);
                });


            world.Routine<InputDirection2D, Position2D>()
                .With<ComputerControlled>()
                .Kind(postInputRoutine)
                .Each((Iter it, int i, ref InputDirection2D dir, ref Position2D pos) => {

                    Position2D p = pos;
                    Position2D target = new Position2D(float.MaxValue, float.MaxValue);

                    using Query q = world.QueryBuilder<Position2D>().With<PlayerControlled>().Build();
                    
                    q.Each((ref Position2D t) =>
                    {
                        if (Utils.DistanceFromTo(p, t) < Utils.DistanceFromTo(p, target)) target = t;
                    });

                    float x = target.X - pos.X;
                    float y = target.Y - pos.Y;
                    float l = MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
                    dir.X = x / l;
                    dir.Y = y / l;
                });

        }
    }
}
