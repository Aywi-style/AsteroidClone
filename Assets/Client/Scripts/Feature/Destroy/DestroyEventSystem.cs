using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DestroyEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<DestroyEvent>> _eventFilter = default;

        readonly EcsPoolInject<DestroyEvent> _eventPool = default;
        readonly EcsPoolInject<GameOverEvent> _gameOverEventPool = default;
        readonly EcsPoolInject<AsteroidsSpawnEvent> _asteroidsSpawnEventPool = default;
        readonly EcsPoolInject<FindEnemySpawnPoint> _findEnemySpawnPointPool = default;

        readonly EcsPoolInject<View> _viewPool = default;

        readonly EcsPoolInject<PlayerComponent> _playerPool = default;
        readonly EcsPoolInject<AsteroidComponent> _asteroidPool = default;
        readonly EcsPoolInject<UFOComponent> _UFOPool = default;
        readonly EcsPoolInject<BulletComponent> _bulletPool = default;

        readonly EcsPoolInject<Destroyed> _destroyedPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;
        private int _destroyedEntity = GameState.NULL_ENTITY;

        private int _asteroidsMinSpawnValue = 3;
        private int _asteroidsMaxSpawnValue = 5;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _eventFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawEventEntities = _eventFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                _eventEntity = rawEventEntities[i];

                ref var destroyEvent = ref _eventPool.Value.Get(_eventEntity);
                _destroyedEntity = destroyEvent.ExposedEntity;

                if (EntityIsPlayer())
                {
                    InvokeGameOver();
                }

                if (EntityIsAsteroid())
                {
                    SpawnNewAsteroids();
                    DestroyAsteroid();
                }

                if (EntityIsUFO())
                {
                    DestroyUFO();
                }

                if (EntityIsBullet())
                {
                    DestroyBullet();
                }

                if (HaventDestroyedComponent())
                {
                    AddDestroyedComponent();
                }
                else
                {
                    Debug.LogError("Destroyed entity has come!");
                }

                DeleteEvent();
            }
        }

        private bool EntityIsPlayer()
        {
            return _playerPool.Value.Has(_destroyedEntity);
        }

        private bool EntityIsAsteroid()
        {
            return _asteroidPool.Value.Has(_destroyedEntity);
        }

        private bool EntityIsUFO()
        {
            return _UFOPool.Value.Has(_destroyedEntity);
        }

        private bool EntityIsBullet()
        {
            return _bulletPool.Value.Has(_destroyedEntity);
        }

        private void SpawnNewAsteroids()
        {
            ref var asteroid = ref _asteroidPool.Value.Get(_destroyedEntity);

            if (asteroid.AsteroidType == AsteroidType.Small)
            {
                return;
            }

            ref var destroyedAsteroidView = ref _viewPool.Value.Get(_destroyedEntity);

            for (int i = 0; i < Random.Range(_asteroidsMinSpawnValue, _asteroidsMaxSpawnValue + 1); i++)
            {
                var newAsteroidEntity = _world.Value.NewEntity();
                
                ref var asteroidsSpawnEvent = ref _asteroidsSpawnEventPool.Value.Add(newAsteroidEntity);
                asteroidsSpawnEvent.AsteroidType = asteroid.AsteroidType + 1;

                ref var findEnemySpawnPoint = ref _findEnemySpawnPointPool.Value.Add(newAsteroidEntity);
                findEnemySpawnPoint.SpawnPoint = destroyedAsteroidView.Transform.position;
                findEnemySpawnPoint.IsFinded = true;
            }

            
        }

        private void DestroyAsteroid()
        {
            ref var asteroid = ref _asteroidPool.Value.Get(_destroyedEntity);

            ref var view = ref _viewPool.Value.Get(_destroyedEntity);
            view.GameObject.SetActive(false);

            _gameState.Value.AllEnabledPools.Asteroids[(int)asteroid.AsteroidType].ReturnToPool(view.GameObject);
        }

        private void DestroyUFO()
        {
            ref var view = ref _viewPool.Value.Get(_destroyedEntity);
            view.GameObject.SetActive(false);

            _gameState.Value.AllEnabledPools.UFOs.ReturnToPool(view.GameObject);
        }

        private void DestroyBullet()
        {
            ref var view = ref _viewPool.Value.Get(_destroyedEntity);
            view.GameObject.SetActive(false);

            _gameState.Value.AllEnabledPools.Bullets.ReturnToPool(view.GameObject);
        }

        private void InvokeGameOver()
        {
            _gameOverEventPool.Value.Add(_world.Value.NewEntity());
        }

        private bool HaventDestroyedComponent()
        {
            return !_destroyedPool.Value.Has(_destroyedEntity);
        }

        private void AddDestroyedComponent()
        {
            _destroyedPool.Value.Add(_destroyedEntity);
        }

        private void DeleteEvent()
        {
            _eventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
            _destroyedEntity = GameState.NULL_ENTITY;
        }
    }
}