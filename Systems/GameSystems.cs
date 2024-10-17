using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Systems.Combat;
using raylib_flecs_csharp.Systems.Physics;
using raylib_flecs_csharp.Systems.PlayerInput;
using raylib_flecs_csharp.Systems.Rendering;

namespace raylib_flecs_csharp.Systems
{
    public class GameSystems : AbstractSystemCollection
    {
        private PlayerInputSystems _playerInputRoutines;
        private CombatSystems _combatSystems;
        private PhysicsSystems _physicsSystems;
        private RenderSystems _renderSystems;
        public GameSystems(World world) : base(world) {
            _playerInputRoutines = new PlayerInputSystems(world);
            _combatSystems = new CombatSystems(world);
            _physicsSystems = new PhysicsSystems(world);
            _renderSystems = new RenderSystems(world);
        }

        protected override void InitSystems()
        {

            world.System("Spawner")
                .Kind(Ecs.OnUpdate)
                .Interval(0.05f)
                .Run((Iter it) =>
                {
                    world.Entity()
                    .IsA(world.Lookup("Enemy (Basic)"))
                    .Set(new Position2D(Raylib.GetRandomValue(0, 1920), Raylib.GetRandomValue(0, 1080)));
                });

            world.System("Delete Entity")
                .With<ToBeDeleted>()
                .Kind(Ecs.PreUpdate)
                .Each(e => {
                    e.Destruct();    
                });



            world.System<DieAfterSeconds>("Check DieAfterSeconds and delete when expired ")
                .Kind(Ecs.OnUpdate)
                .Each((Iter it, int i, ref DieAfterSeconds dieTimer) =>
                {
                    dieTimer.Value -= it.DeltaTime();
                    if (dieTimer.Value <= 0.0f)
                        it.Entity(i).Destruct();
                });
        }
    }
}
