﻿using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using System.Numerics;

namespace raylib_flecs_csharp.Systems.Rendering
{
    public class RenderSystems : AbstractSystemCollection
    {
        public record struct RenderStart;
        public record struct Render;
        public record struct RenderEnd;

        public record struct On60FPSOrLowerAcheived;

        private Entity preRender;
        private Entity render;
        private Entity postRender;
        public RenderSystems(World world) : base(world) {}

        protected override void InitSystemPipeline()
        {
            world.Component<RenderStart>().Entity.Add(Ecs.Phase).DependsOn(Ecs.PreStore).ChildOf(Ecs.PreStore);
            world.Component<Render>().Entity.Add(Ecs.Phase).DependsOn(Ecs.OnStore).ChildOf(Ecs.OnStore);
            world.Component<RenderEnd>().Entity.Add(Ecs.Phase).DependsOn<Render>().ChildOf(Ecs.OnStore);
        }

        protected override void InitSystems()
        {
            world.System("PreRenderRoutine")
                .Kind<RenderStart>()
                .Run((Iter it) => { 
                    Raylib.BeginDrawing(); 
                    Raylib.ClearBackground(Color.RayWhite); 
                });

            world.System("PostRenderRoutine")
                .Kind<RenderEnd>()
                .Run((Iter it) => { 
                    Raylib.EndDrawing(); 
                });

            world.System<Texture2D, Position2D, Scale>("Draw Objects (Position & Scale)")
                .Kind<Render>()
                .Without<Rotation>()
                .Without<Color>()
                .Each((ref Texture2D texture, ref Position2D p, ref Scale s) =>
                {
                    Raylib.DrawTextureEx(texture, new Vector2(p.X, p.Y), 0, s.Value, Color.White);
                });

            world.System<Texture2D, Position2D>("Draw Objects (Position Only)")
                .Kind<Render>()
                .Without<Scale>()
                .Without<Rotation>()
                .Without<Color>()
                .Each((ref Texture2D texture, ref Position2D p) =>
                {
                    Raylib.DrawTextureEx(texture, new Vector2(p.X, p.Y), 0, 1, Color.White);
                });

            world.System<Texture2D, Position2D, Rotation, Scale>("Draw Objects (Position Rotation & Scale)")
                .Kind<Render>()
                .Without<Color>()
                .Each((ref Texture2D texture, ref Position2D p, ref Rotation rot, ref Scale scale) =>
                {
                    Raylib.DrawTextureEx(texture, new Vector2(p.X, p.Y), rot.Value, scale.Value, Color.White);
                });

            //world.System("World Partitionning Debug")
            //    .Kind<RenderStart>()
            //    .Run((Iter it) => {
            //        int width = 1920;
            //        int height = 1080;
            //        int size = 64;
            //        int horizontalCells = (int)float.Ceiling(width / size);
            //        int verticalCells = (int)float.Ceiling(height / size);
            //        for (int y = 0; y < verticalCells; y++) {
            //            for (int x = 0; x < horizontalCells; x++) {
            //                Raylib.DrawRectangleLines(x * size, y * size, size, size, Color.Orange); 
            //            } 
            //        }

            //    });

            world.System("Player Contained In Debug")
                .With<ContainedIn>(Ecs.Wildcard)
                .With<PlayerControlled>()
                .Kind<RenderStart>()
                .Each((Iter it, int i) =>
                {
                    Position2D pos = it.Pair(0).Second().Get<Position2D>();
                    Raylib.DrawRectangle((int)pos.X * 64, (int)pos.Y * 64, 64, 64, Color.Green);
                });

            world.System("Show FPS")
                .Kind<Render>()
                .Run((Iter it) => {
                    Raylib.DrawText($"{Raylib.GetFPS()} FPS", 20, 20, 20, Color.DarkGreen);
                });

            world.System("Show Monster Number")
                .Kind<Render>()
                .Run((Iter it) => {
                    using Query<ComputerControlled> q = world.Query<ComputerControlled>();
                    Raylib.DrawText($"{q.Count()} Enemies", 20, 40, 20, Color.DarkGreen);
                });

            world.System("Show Entity Number")
                .Kind<Render>()
                .Run((Iter it) => {
                    int c = 0;
                    world.Children((Entity child) => { c++; });
                    Raylib.DrawText($"{c} Entities", 20, 60, 20, Color.DarkGreen);
                });
        }
    }
}
