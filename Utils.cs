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
            var dir = new Vector2(x, y);
            return dir / dir.Length();
        }

        public static float GetVectorAngle(Vector2 dir) 
        { 
            float a = MathF.Atan2(dir.Y, dir.X);

            return a;
        }

        public static float DegToRad(float deg)
        {
            return deg * MathF.PI / 180.0f;
        }

        public static float RadToDeg(float rad)
        {
            return rad * 180.0f / MathF.PI;
        }
    }
}
