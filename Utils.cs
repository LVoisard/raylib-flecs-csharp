using raylib_flecs_csharp.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace raylib_flecs_csharp
{
    public static class Utils
    {
        public static float DistanceFromTo(Position2D a, Position2D b) { 
            float x = b.X - a.X;
            float y = b.Y - a.Y;
            return MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
        }

        public static Vector2 GetDirectionVector(Position2D a, Position2D b)
        { 
            float x= b.X - a.X;
            float y = b.Y - a.Y;
            return new Vector2(x, y);
        }
    }
}
