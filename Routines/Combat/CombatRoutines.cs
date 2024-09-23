using Flecs.NET.Bindings;
using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;

namespace raylib_flecs_csharp.Routines.Combat
{
    public class CombatRoutines : AbstractRoutineCollection
    {
        public CombatRoutines(World world) : base(world) { }

        protected override void InitRoutines()
        {
            world.Routine<CollisionRecord>("Trigger take Damage System")
                .With<Health>()
                .Each((Entity e, ref CollisionRecord rec) =>
                {
                    if (!e.IsAlive() || !rec.other.IsAlive()) return;
                    if (e.Get<Team>() == rec.other.Get<Team>()) return;
                    if (!rec.other.Has<Damage>()) return;

                    if (e.Has<TakeDamage>()) return;
                    e.Set<TakeDamage>(new(rec.other.Get<Damage>().Value));
                });


            world.Routine<Health, TakeDamage>("Take Damage System")
                .Without<TemporaryImmunity>()
                .Each((Entity e, ref Health health, ref TakeDamage takeDamage) =>
                {
                    health.Value -= takeDamage.Value;
                    Console.WriteLine($"Took {takeDamage.Value} damage, current hp is {health.Value}");
                    e.Remove<TakeDamage>();

                    if (health.Value <= 0)
                        e.Destruct();

                    e.Set<TemporaryImmunity>(new(0.1f));
                });

            world.Routine<TemporaryImmunity>("Check Immunity and remove when expired ")
                .Each((Iter it, int i, ref TemporaryImmunity temporaryImmunity) =>
                {
                    temporaryImmunity.Value -= it.DeltaTime();
                    if (temporaryImmunity.Value <= 0.0f)
                        it.Entity(i).Remove<TemporaryImmunity>();
                });

            world.Routine<Health, Position2D>("Health bar")
                .With<PlayerControlled>()
                .Each((ref Health health, ref Position2D pos) =>
                {
                    float percentHealth = health.Value / health.MaxValue;
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, 48, 5, Color.Black);
                    Raylib.DrawRectangle((int)(pos.X - 8f), (int)pos.Y - 12, (int)(48.0f * percentHealth), 5, Color.Red);
                });

            world.Routine()
                .With<PlayerControlled>()
                .Interval(1f)
                .Each(e => {
                    e.Each<Uses>((Entity second) => {
                        Console.WriteLine(second.Name());
                    });
                });
        }
    }
}
