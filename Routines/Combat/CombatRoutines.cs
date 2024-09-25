using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using System.Numerics;

namespace raylib_flecs_csharp.Routines.Combat
{
    public class CombatRoutines : AbstractRoutineCollection
    {
        public CombatRoutines(World world) : base(world) { }

        protected override void InitRoutines()
        {
            world.Routine<CollisionRecord, Damage>("Trigger take Damage System")
                .Kind(Ecs.PostUpdate)
                .Each((Entity e, ref CollisionRecord rec, ref Damage damage) =>
                {
                    if (e.Get<Team>() == rec.other.Get<Team>()) return;

                    float dmg = damage.Value;
                    if (e.Has<TakeDamage>()) {
                        dmg += e.Get<TakeDamage>().Value;
                    }
                    rec.other.Set<TakeDamage>(new(dmg));

                });


            world.Routine<Health, TakeDamage>("Take Damage System")
                .Kind(Ecs.OnUpdate)
                .Without<TemporaryImmunity>()
                .Each((Entity e, ref Health health, ref TakeDamage takeDamage) =>
                {
                    health.Value -= takeDamage.Value;
                    Console.WriteLine($"Took {takeDamage.Value} damage, current hp is {health.Value}");
                    e.Remove<TakeDamage>();

                    if (health.Value <= 0)
                        e.Add<ToBeDeleted>();

                    e.Set<TemporaryImmunity>(new(0.1f));
                });

            world.Routine<TemporaryImmunity>("Check Immunity and remove when expired ")
                .Kind(Ecs.OnUpdate)
                .Each((Iter it, int i, ref TemporaryImmunity temporaryImmunity) =>
                {
                    temporaryImmunity.Value -= it.DeltaTime();
                    if (temporaryImmunity.Value <= 0.0f)
                        it.Entity(i).Remove<TemporaryImmunity>();
                });

            world.Routine<Health, Position2D>("Health bar")
                .Kind(Ecs.OnStore)
                .With<PlayerControlled>()
                .Each((ref Health health, ref Position2D pos) =>
                {
                    float percentHealth = health.Value / health.MaxValue;
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, 48, 5, Color.Black);
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, (int)(48.0f * percentHealth), 5, Color.Red);
                });

            world.Routine<Position2D, Team>("Start Attack")
                .With<PlayerControlled>()
                .Interval(1f)
                .Each((Entity e, ref Position2D pos, ref Team team) => {

                    Position2D selfPos = pos;
                    Position2D target = new Position2D(0,0);
                    float minDist = float.MaxValue;

                    using var q = world.QueryBuilder<Position2D>().With<ComputerControlled>().Build();

                    var inst = world.Entity()
                    .IsA(world.Lookup("Dagger Attack"))
                    .Set<Position2D>(new(pos.X, pos.Y))
                    .Set<Team>(team)
                    .Set<CollisionFilter>(Physics.Physics.PlayerSpawnedCollisionFilter);

                    q.Each((ref Position2D pos) => {
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
                        .Set<InputDirection2D>(new (dir.X, dir.Y));

                });
        }
    }
}
