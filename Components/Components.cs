using Raylib_cs;

namespace raylib_flecs_csharp.Components
{
    public record struct Position2D (float X, float Y);

    public record struct Velocity2D (float X, float Y);

    public record struct InputDirection2D  (float X, float Y);

    public record struct Speed (float Value);

    public record struct Scale (float Value);

    public record struct Rotation (float Value);
    public record struct CollisionRadius (float Value);
    public record struct Team (int Value);


    // Tag
    public record struct TakeDamage();
    public record struct PlayerControlled();
    public record struct ComputerControlled();
}
