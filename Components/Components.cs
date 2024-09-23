using Flecs.NET.Core;
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
    public record struct CollisionRecord (Entity other);

    public record struct Health (float Value, float MaxValue);
    public record struct Damage (float Value);
    public record struct TakeDamage(float Value);
    public record struct TemporaryImmunity(float Value);

    public record struct Team (int Value);


    public record struct Uses();
    public record Skill();
    public record SwordAttack(Texture2D texture, float speed, Position2D pos) : Skill;

    // Tag
    public record struct Immovable();
    public record struct PlayerControlled();
    public record struct ComputerControlled();
}
