using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Routines;

namespace raylib_flecs_csharp.Core
{
    public class GameEntities
    {
        private World world = null;

        public GameEntities(World world) 
        {
            this.world = world;
            InitGameEntities();
        }

        private void InitGame()
        { 
            InitGameEntities();
        }

        private void InitGameEntities()
        {
            var player = world.Entity("Player")
                .Set(new Position2D(0,0))
                .Set(new InputDirection2D(0,0))
                .Set(Raylib.LoadTexture("./Resources/character.png"))
                .Set(new Speed (200.0f))
                .Set(new Scale (2.0f))
                .Set(new CollisionRadius(16.0f))
                .Set(new Team(0))
                .Add<PlayerControlled>();

            var enemyPrefab = world.Prefab("Enemy (Basic)")
                .Set(new Position2D(0,0))
                .Set(new InputDirection2D(0,0))
                .Set(Raylib.LoadTexture("./Resources/ghost.png"))
                .Set(new Speed(100.0f))
                .Set(new Scale(2.0f))
                .Set(new CollisionRadius(16.0f))
                .Set(new Team(1))
                .Add<ComputerControlled>();
        }

    }
}
