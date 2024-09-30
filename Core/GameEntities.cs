using Flecs.NET.Core;
using Raylib_cs;
using raylib_flecs_csharp.Components;
using raylib_flecs_csharp.Helper;

namespace raylib_flecs_csharp.Core
{
    public class GameEntities
    {
        private World world;
        public GameEntities(World world) 
        {
            this.world = world;
            InitGameEntities();
        }

        private void InitGame()
        { 
            InitGameEntities();
        }

        
        private void InitGameEntities()
        {
            // prefab use a refence to this
            // world.Component<Texture2D>().Entity.Add(Ecs.OnInstantiate, Ecs.Inherit);
            
            // by default, collisions are set to true

            var player = world.Entity("Player")
                .Set(new Position2D(960, 540))
                .Set(new InputDirection2D(0, 0))
                .Set(Raylib.LoadTexture("./Resources/character.png"))
                .Set(new Speed(200.0f))
                .Set(new Scale(4.0f))
                .Set(new CollisionRadius(32.0f))
                .Set(new Team(0))
                .Set(new Health(100, 100))
                .Set(new Damage(5))
                .Set<CollisionFilter>(Physics.PlayerCollisionFilter)
                .Add<Immovable>()
                .Add<PlayerControlled>();

            var enemyPrefab = world.Prefab("Enemy (Basic)")
                .Set(new Position2D(0,0))
                .Set(new InputDirection2D(0,0))
                .Set(Raylib.LoadTexture("./Resources/ghost.png"))
                .Set(new Speed(50.0f))
                .Set(new Scale(4.0f))
                .Set(new CollisionRadius(32.0f))
                .Set(new Team(1))
                .Set(new Health(5, 5))
                .Set(new Damage(1))
                .Set<CollisionFilter>(Physics.EnemyCollisionFilter)
                .Add<ComputerControlled>();

            var attack = world.Prefab("Attack")
                .Set<Team>(new(0))
                .Set<Damage>(new(5));

            var daggerThrow = world.Prefab("Dagger Attack").IsA(attack)
                .Set(Raylib.LoadTexture("./Resources/dagger.png"))
                .Set<Rotation>(new(0))
                .Set<Scale>(new(2))
                .Set<Speed>(new(500))
                .Set<Components.Range>(new(1000))
                .Set<CollisionRadius>(new(32.0f))
                .Add<Trigger>()
                .Set<Position2D>(new(0, 0))
                .Set<DieAfterSeconds>(new(5f))
                .Add<DestroyOnTrigger>();

            var worldPartitionContainer = world.Entity("World Partition Container");
            var worldPartition = world.Prefab("World Partition")
                .Add<Position2D>();

        }


    }
}
