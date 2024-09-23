using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Routines.Combat;
using raylib_flecs_csharp.Routines.Physics;
using raylib_flecs_csharp.Routines.PlayerInput;
using raylib_flecs_csharp.Routines.Rendering;
using System.Numerics;

namespace raylib_flecs_csharp.Routines
{
    public class GameRoutines : AbstractRoutineCollection
    {
        private PlayerInputRoutines _playerInputRoutines;
        private CombatRoutines _combatRoutines;
        private PhysicsRoutines _physicsRoutines;
        private RenderRoutines _renderRoutines;
        public GameRoutines(World world) : base(world) {
            _playerInputRoutines = new PlayerInputRoutines(world);
            _combatRoutines = new CombatRoutines(world);
            _physicsRoutines = new PhysicsRoutines(world);
            _renderRoutines = new RenderRoutines(world);
        }

        protected override void InitRoutines()
        {

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
