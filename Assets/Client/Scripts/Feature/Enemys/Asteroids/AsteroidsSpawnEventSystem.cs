using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class AsteroidsSpawnEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<AsteroidsSpawnEvent, FindEnemySpawnPoint>> _asteroidsSpawnEventFilter = default;

        readonly EcsPoolInject<AsteroidsSpawnEvent> _asteroidsSpawnEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<AsteroidComponent> _asteroidPool = default;
        readonly EcsPoolInject<Teleportable> _teleportablePool = default;
        readonly EcsPoolInject<ScoreGivable> _scoreGivablePool = default;

        readonly EcsPoolInject<FindEnemySpawnPoint> _findEnemySpawnPointPool = default;

        private int _asteroidEntity = GameState.NULL_ENTITY;

        public void Run(IEcsSystems systems)
        {
            var eventEntityCount = _asteroidsSpawnEventFilter.Value.GetEntitiesCount();

            if (eventEntityCount < 1)
            {
                return;
            }

            var rawEventEntities = _asteroidsSpawnEventFilter.Value.GetRawEntities();

            for (int i = 0; i < eventEntityCount; i++)
            {
                _asteroidEntity = rawEventEntities[i];

                ref var asteroidsSpawnEvent = ref _asteroidsSpawnEventPool.Value.Get(_asteroidEntity);

                InitViewAndSpawn(ref asteroidsSpawnEvent);
                InitMovable(ref asteroidsSpawnEvent);
                InitAsteroidComponent(ref asteroidsSpawnEvent);
                InitTeleportable();
                InitScoreGivable(ref asteroidsSpawnEvent);

                DeleteEvent();
            }
        }

        private void InitViewAndSpawn(ref AsteroidsSpawnEvent asteroidsSpawnEvent)
        {
            var asteroidObject = _gameState.Value.AllEnabledPools.Asteroids[(int)asteroidsSpawnEvent.AsteroidType].GetFromPool();

            ref var view = ref _viewPool.Value.Add(_asteroidEntity);
            view.GameObject = asteroidObject;
            view.Transform = asteroidObject.transform;
            view.Transform.Rotate(Vector3.up, Random.Range(0f, 360f));

            if (!view.GameObject.TryGetComponent(out AsteroidMB asteroidMB))
            {
                asteroidMB = view.GameObject.AddComponent<AsteroidMB>();

                Debug.LogWarning("UFOShip havent asteroidMB and it was added!");
            }

            asteroidMB.Init(_asteroidEntity);

            ref var findEnemySpawnPoint = ref _findEnemySpawnPointPool.Value.Get(_asteroidEntity);
            view.Transform.position = findEnemySpawnPoint.SpawnPoint;

            _findEnemySpawnPointPool.Value.Del(_asteroidEntity);
        }

        private void InitMovable(ref AsteroidsSpawnEvent asteroidsSpawnEvent)
        {
            ref var movable = ref _movablePool.Value.Add(_asteroidEntity);
            ref var view = ref _viewPool.Value.Get(_asteroidEntity);

            movable.ForceVector = view.Transform.forward * _gameState.Value.AsteroidsConfig.Asteroids[(int)asteroidsSpawnEvent.AsteroidType].StartSpeed;
        }

        private void InitAsteroidComponent(ref AsteroidsSpawnEvent asteroidsSpawnEvent)
        {
            ref var asteroid = ref _asteroidPool.Value.Add(_asteroidEntity);
            asteroid.AsteroidType = asteroidsSpawnEvent.AsteroidType;
        }

        private void InitTeleportable()
        {
            _teleportablePool.Value.Add(_asteroidEntity);
        }

        private void InitScoreGivable(ref AsteroidsSpawnEvent asteroidsSpawnEvent)
        {
            ref var scoreGivable = ref _scoreGivablePool.Value.Add(_asteroidEntity);
            scoreGivable.Value = _gameState.Value.AsteroidsConfig.Asteroids[(int)asteroidsSpawnEvent.AsteroidType].ScoresCount;
        }

        private void DeleteEvent()
        {
            _asteroidsSpawnEventPool.Value.Del(_asteroidEntity);

            _asteroidEntity = GameState.NULL_ENTITY;
        }
    }
}