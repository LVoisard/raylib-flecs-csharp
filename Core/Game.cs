using Flecs.NET.Core;
using Flecs.NET.Bindings;
using Raylib_cs;
using raylib_flecs_csharp.Systems;

namespace raylib_flecs_csharp.Core
{
    public class Game
    {
        private World world;
        private GameEntities gameEntities;
        private GameSystems routines;

        public Game()
        {
            Raylib.InitWindow(1920, 1080, "It's Time to Play the Game!");

            world = World.Create();
            gameEntities = new GameEntities(world);
            routines = new GameSystems(world);

            world.Import<Ecs.Stats>();
            world.Set<flecs.EcsRest>(new());
            //Ecs.Log.SetLevel(1);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                world.Progress();
            }

            Raylib.CloseWindow();
        }
    }
}
