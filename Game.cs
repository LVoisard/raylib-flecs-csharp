using Flecs.NET.Core;
using Flecs.NET.Bindings;
using Raylib_cs;
using raylib_flecs_csharp.Core;
using raylib_flecs_csharp.Routines;

namespace raylib_flecs_csharp
{
    public class Game
    {
        private World world;
        private GameEntities gameEntities;
        private GameRoutines routines;

        public Game() 
        {
            Raylib.InitWindow(1920, 1080, "It's Time to Play the Game!");

            world = World.Create();
            gameEntities = new GameEntities(world);
            routines = new GameRoutines(world);

            world.Import<Ecs.Stats>();
            world.Set<flecs.EcsRest>(new());
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                world.Progress(Raylib.GetFrameTime());
            }

            Raylib.CloseWindow();
        }
    }
}
