using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EnemySpawnByTimer : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<AsteroidsSpawnEvent> _asteroidsSpawnEventPool = default;
        readonly EcsPoolInject<UFOsSpawnEvent> _UFOsSpawnEventPool = default;

        readonly EcsPoolInject<FindEnemySpawnPoint> _findEnemySpawnPointPool = default;

        private int _spawnedEntity = GameState.NULL_ENTITY;

        private float _asteroidsCurrentCooldown;
        private float _asteroidsMaxCooldown;

        private float _UFOsCurrentCooldown;
        private float _UFOsMaxCooldown;

        private bool _isFirstWork = true;

        public void Run (IEcsSystems systems)
        {
            if (_isFirstWork)
            {
                _asteroidsMaxCooldown = _gameState.Value.AsteroidsConfig.SpawnCooldown;
                _UFOsMaxCooldown = _gameState.Value.UFOsConfig.SpawnCooldown;

                _isFirstWork = false;
            }

            _asteroidsCurrentCooldown += Time.deltaTime;
            _UFOsCurrentCooldown += Time.deltaTime;

            if (_asteroidsCurrentCooldown >= _asteroidsMaxCooldown)
            {
                _asteroidsCurrentCooldown = 0;

                SpawnEntity();
                AddFindEnemySpawnPointComponent();
                SetAsteroidSpawnSpecificity();
            }

            if (_UFOsCurrentCooldown >= _UFOsMaxCooldown)
            {
                _UFOsCurrentCooldown = 0;

                SpawnEntity();
                AddFindEnemySpawnPointComponent();
                _UFOsSpawnEventPool.Value.Add(_spawnedEntity);
            }
        }

        private void SpawnEntity()
        {
            _spawnedEntity = _world.Value.NewEntity();
        }

        private void AddFindEnemySpawnPointComponent()
        {
            _findEnemySpawnPointPool.Value.Add(_spawnedEntity);
        }

        private void SetAsteroidSpawnSpecificity()
        {
            ref var asteroidsSpawnEvent = ref _asteroidsSpawnEventPool.Value.Add(_spawnedEntity);
            asteroidsSpawnEvent.AsteroidType = (AsteroidType)Random.Range(0, _gameState.Value.AsteroidsConfig.Asteroids.Length);
        }
    }
}