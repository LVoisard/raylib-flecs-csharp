﻿using Flecs.NET.Core;

namespace raylib_flecs_csharp.Components
{
    public record struct Position2D (float X, float Y);

    public record struct Velocity2D (float X, float Y);

    public record struct InputDirection2D  (float X, float Y);

    public record struct Speed (float Value);

    public record struct Scale (float Value);

    public record struct Rotation (float Value);
    public record struct CollisionRadius (float Value);
    public record struct Trigger;
    public record struct CollidedWith;
    public record struct TriggeredWith;
    public record struct DestroyOnCollision;
    public record struct DestroyOnTrigger;
    public record struct DieAfterSeconds(float Value);

    public record struct ContainedIn;
    public record struct Health (float Value, float MaxValue);
    public record struct Damage (float Value);
    public record struct Range (float Value);
    public record struct TakeDamage(float Value);
    public record struct DamageTaken;
    public record struct TemporaryImmunity(float Value);

    public record struct Team (int Value);

    // Tag
    public record struct Immovable;
    public record struct PlayerControlled;
    public record struct ComputerControlled;
    public record struct ToBeDeleted;
    public record struct Projectile;


    public record struct Tag(string name);
}
