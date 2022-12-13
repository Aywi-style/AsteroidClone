using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public sealed class EcsStartup : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerShipConfig PlayerShipConfig { get; private set; }

        [field: SerializeField]
        public AsteroidsConfig AsteroidsConfig { get; private set; }

        [field: SerializeField]
        public UFOsConfig UFOsConfig { get; private set; }

        private EcsWorld _world;
        private GameState _gameState;
        private EcsSystems _initSystems, _spawnSystems, _controleAndMovableSystems, _fightSystems, _gameOverSystems;

        private void Start()
        {
            _world = new EcsWorld();

            GameState.Clear();
            _gameState = GameState.Initialize(this);

            _initSystems = new EcsSystems(_world, _gameState);
            _spawnSystems = new EcsSystems(_world, _gameState);
            _controleAndMovableSystems = new EcsSystems(_world, _gameState);
            _fightSystems = new EcsSystems(_world, _gameState);
            _gameOverSystems = new EcsSystems(_world, _gameState);

            _initSystems
                .Add(new InitCamera())
                .Add(new InitInterface())
                .Add(new InitPlayerShip())
                .Add(new InitPools())
                ;

            _spawnSystems
                .Add(new EnemySpawnByTimer())
                .Add(new FindEnemySpawnPointSystem())

                .Add(new AsteroidsSpawnEventSystem())
                .Add(new UFOsSpawnEventSystem())
                ;

            _controleAndMovableSystems
                .Add(new TeleportSystem())

                .Add(new PlayerShipController())

                .Add(new UFOLookAtPlayerShip())
                .Add(new SpaceShipsForceChangingSystem())
                .Add(new MovingSystem())
                ;

            _fightSystems
                .Add(new ShowLaserEffectEventSystem())
                .Add(new LaserSpawnEventSystem())
                .Add(new LaserCooldownSystem())
                .Add(new AddLaserChargeByTimer())

                .Add(new BulletSpawnEventSystem())
                .Add(new BulletCollisionHandlerSystem())
                .Add(new BulletLifeDurationTracking())

                .Add(new PlayerCollisionHandlerSystem())

                .Add(new ScoreGivableHandlerSystem())

                .Add(new DestroyEventSystem())
                ;

            _gameOverSystems
                .Add(new GameOverEventSystem())
                ;
#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                ;
#endif

            InjectAllSystems(_initSystems, _spawnSystems, _controleAndMovableSystems, _fightSystems, _gameOverSystems);
            InitAllSystems(_initSystems, _spawnSystems, _controleAndMovableSystems, _fightSystems, _gameOverSystems);
        }

        private void Update()
        {
            _initSystems?.Run();

            if (_gameState.PlaySpawnSystems)
            {
                _spawnSystems?.Run();
            }

            if (_gameState.PlayControleAndMovableSystems)
            {
                _controleAndMovableSystems?.Run();
            }

            if (_gameState.PlayFightSystems)
            {
                _fightSystems?.Run();
            }

            _gameOverSystems?.Run();
        }

        private void OnDestroy()
        {
            OnDestroyAllSystems(_initSystems, _gameOverSystems);

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }

        private void InjectAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Inject();
            }
        }

        private void InitAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Init();
            }
        }

        private void OnDestroyAllSystems(params EcsSystems[] systems)
        {
            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] != null)
                {
                    systems[i].Destroy();
                    systems[i] = null;
                }
            }
        }
    }
}
