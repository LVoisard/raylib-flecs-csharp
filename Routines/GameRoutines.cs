using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Routines.Physics;
using raylib_flecs_csharp.Routines.PlayerInput;
using raylib_flecs_csharp.Routines.Rendering;
using System.Numerics;

namespace raylib_flecs_csharp.Routines
{
    public class GameRoutines : AbstractRoutineCollection
    {
        private PlayerInputRoutines _playerInputRoutines;
        private RenderRoutines _renderRoutines;
        private PhysicsRoutines _physicsRoutines;
        public GameRoutines(World world) : base(world) {
            _playerInputRoutines = new PlayerInputRoutines(world);
            _physicsRoutines = new PhysicsRoutines(world);
            _renderRoutines = new RenderRoutines(world);
        }

        protected override void InitRoutines()
        {
            world.Routine<Position2D, InputDirection2D, Speed>("Update Positions")
                .Each((Iter it, int i, ref Position2D pos, ref InputDirection2D dir, ref Speed sp) =>
                {                    
                    pos.X += dir.X * it.DeltaTime() * sp.Value;
                    pos.Y += dir.Y * it.DeltaTime() * sp.Value;
                });

            world.Routine("Spawner")
                .Interval(0.5f)
                .Run((Iter it) =>
                {
                    world.Entity()
                    .IsA(world.Lookup("Enemy (Basic)"))
                    .Set(new Position2D(Raylib.GetRandomValue(0, 1920), Raylib.GetRandomValue(0, 1080)));
                });

        }
    }
}
