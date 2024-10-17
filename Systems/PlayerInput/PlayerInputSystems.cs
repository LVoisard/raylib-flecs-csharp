using raylib_flecs_csharp.Components;
using Raylib_cs;
using raylib_flecs_csharp.Helper;
using Flecs.NET.Core;
using static Flecs.NET.Core.Ecs;

namespace raylib_flecs_csharp.Systems.PlayerInput
{
    public class PlayerInputSystems : AbstractSystemCollection
    {
        private Entity inputRoutine;
        private Entity postInputRoutine;

        public PlayerInputSystems(World world) : base(world) { 
        
        }

        protected override void InitSystemPipeline()
        {
            inputRoutine = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnLoad)
                .ChildOf(Ecs.OnLoad);

            postInputRoutine = world.Entity()
                .Add(Ecs.Phase)
                .DependsOn(Ecs.OnLoad)
                .ChildOf(Ecs.OnLoad);

        }

        protected override void InitSystems()
        {
            world.System("Post Input")
                .Kind(postInputRoutine)
                .Run((Iter it) => { Raylib.PollInputEvents(); });

            world.System<InputDirection2D>("Player Input Movement")
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



            world.System<InputDirection2D, Position2D>("Enemy Follow Player")
                .With<ComputerControlled>()                
                .Kind(postInputRoutine)
                .Each((Iter it, int i, ref InputDirection2D dir, ref Position2D pos) => {
                    Entity target = world.Lookup("Player");
                    if (target == 0) { dir.X = 0; dir.Y = 0; return; }
                    float x = target.Get<Position2D>().X - pos.X;
                    float y = target.Get<Position2D>().Y - pos.Y;
                    float l = MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
                    dir.X = x / l;
                    dir.Y = y / l;
                });
        }
    }
}
