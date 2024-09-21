using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using System.Numerics;

namespace raylib_flecs_csharp.Routines.Rendering
{
    public class RenderRoutines : AbstractRoutineCollection
    {
        private Entity renderRoutine;
        private Entity preRender;
        private Entity render;
        private Entity postRender;
        public RenderRoutines(World world) : base(world) {}

        protected override void InitRoutinePipeline()
        {
            renderRoutine = world.Entity("RenderSystem")
                .Add(Ecs.Phase)
                .DependsOn(Ecs.PostUpdate)
                .ChildOf(Ecs.PostUpdate);

            preRender = world.Entity("PreRenderPhase")
                .Add(Ecs.Phase)
                .DependsOn(renderRoutine)
                .ChildOf(renderRoutine);            

            render = world.Entity("RenderPhase")
                .Add(Ecs.Phase)
                .DependsOn(renderRoutine)
                .ChildOf(renderRoutine);

            postRender = world.Entity("PostRenderPhase")
                .Add(Ecs.Phase)
                .DependsOn(renderRoutine)
                .ChildOf(renderRoutine);
        }

        protected override void InitRoutines()
        {
            world.Routine("PreRenderRoutine")
                .Kind(preRender)
                .Run((Iter it) => { 
                    Raylib.BeginDrawing(); 
                    Raylib.ClearBackground(Color.RayWhite); 
                });

            world.Routine("PostRenderRoutine")
                .Kind(postRender)
                .Run((Iter it) => { 
                    Raylib.EndDrawing(); 
                });

            world.Routine<Texture2D, Position2D, Scale>("Draw Objects (Position & Scale)")
                .Kind(render)
                .Without<Rotation>()
                .Without<Color>()
                .Each((ref Texture2D texture, ref Position2D p, ref Scale s) =>
                {
                    Raylib.DrawTextureEx(texture, new Vector2(p.X, p.Y), 0, s.Value, Color.White);
                });

            world.Routine<Texture2D, Position2D>("Draw Objects (Position Only)")
                .Kind(render)
                .Without<Scale>()
                .Without<Rotation>()
                .Without<Color>()
                .Each((ref Texture2D texture, ref Position2D p) =>
                {
                    Raylib.DrawTextureEx(texture, new Vector2(p.X, p.Y), 0, 1, Color.White);
                });

            world.Routine("Show FPS")
                .Kind(render)
                .Run((Iter it) => {
                    Raylib.DrawFPS(20, 20);
                });
        }

        public record RenderStart();
        public record Render();
        public record RenderEnd();
    }
}
