using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Helper;
using System.Numerics;

namespace raylib_flecs_csharp.Systems.Combat
{
    public class CombatSystems : AbstractSystemCollection
    {
        public CombatSystems(World world) : base(world) { }

        protected override void InitSystems()
        {
            world.Observer("OnCollision take Damage System")
                .With<CollidedWith>(Ecs.Wildcard).Or().With<TriggeredWith>(Ecs.Wildcard)
                .With<Damage>()
                .Event(Ecs.OnAdd)
                .Each((Iter it, int i) =>
                {
                    Entity self = it.Entity(0);
                    Entity other = it.Pair(0).Second();

                    if (self.Get<Team>() == other.Get<Team>()) return;

                    float dmg = self.Get<Damage>().Value;                    
                    other.Set<TakeDamage>(new(dmg));
                });

            world.Observer<Health, TakeDamage>("Take Damage System")
                .Without<TemporaryImmunity>()
                .Event(Ecs.OnSet)
                .Each((Entity e, ref Health health, ref TakeDamage takeDamage) =>
                {
                    health.Value -= takeDamage.Value;
                    Console.WriteLine($"Took {takeDamage.Value} damage, current hp is {health.Value}");

                    e.Remove<TakeDamage>();
                    e.Add<DamageTaken>();

                    if (health.Value <= 0)
                        e.Add<ToBeDeleted>();
                });

            world.Observer<DamageTaken>("Add a temporary Immunity to the player after they take damage")
                .With<PlayerControlled>()
                .Event(Ecs.OnAdd)
                .Each((Entity e, ref DamageTaken damageTaken) =>
                {
                    Console.WriteLine("Temporary Immunity Set");
                    e.Set<TemporaryImmunity>(new(0.1f));
                    e.Remove<DamageTaken>();
                });

            world.System<TemporaryImmunity>("Check Immunity and remove when expired ")
                .Kind(Ecs.OnUpdate)
                .Each((Iter it, int i, ref TemporaryImmunity temporaryImmunity) =>
                {
                        temporaryImmunity.Value -= it.DeltaTime();
                        if (temporaryImmunity.Value <= 0.0f)
                            it.Entity(i).Remove<TemporaryImmunity>();
                });

            world.System<Health, Position2D>("Health bar")
                .With<PlayerControlled>()
                .Kind<Rendering.RenderSystems.Render>()
                .Each((ref Health health, ref Position2D pos) =>
                {
                    float percentHealth = health.Value / health.MaxValue;
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, 48, 5, Color.Black);
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, (int)(48.0f * percentHealth), 5, Color.Red);
                });

            world.System<Position2D, Team>("Start Attack")
                .With<PlayerControlled>()
                .Interval(1f)
                .Each((Entity e, ref Position2D pos, ref Team team) =>
                {

                    Position2D selfPos = pos;
                    Position2D target = new Position2D(0, 0);
                    float minDist = float.MaxValue;

                    using var q = world.QueryBuilder<Position2D>().With<ComputerControlled>().Build();

                    var inst = world.Entity()
                    .IsA(world.Lookup("Dagger Attack"))
                    .Set<Position2D>(new(pos.X, pos.Y))
                    .Set<Team>(team)
                    .Set<CollisionFilter>(Helper.Physics.PlayerSpawnedCollisionFilter);

                    q.Each((ref Position2D pos) =>
                    {
                        float dist = Utils.DistanceFromTo(selfPos, pos);
                        if (dist < minDist)
                        {
                            target = pos;
                            minDist = dist;
                        }
                    });

                    if (target.X == 0 && target.Y == 0) return;

                    Vector2 dir = Utils.GetDirectionVector(pos, target);
                    float rotation = Utils.GetVectorAngle(dir);

                    inst.Set<Rotation>(new(Utils.RadToDeg(rotation) + 90))
                        .Set<InputDirection2D>(new(dir.X, dir.Y));

                });
        }
    }
}
