using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitPools : IEcsInitSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        private int _basedBulletsCount = 20;
        private int _basedLaserEffectsCount = 1;
        private int _basedLargeAsteroidsCount = 10;
        private int _basedSmallAsteroidsCount = 10;
        private int _basedUFOsCount = 5;

        public void Init(IEcsSystems systems)
        {
            _gameState.Value.AllEnabledPools = new AllPools();
            _gameState.Value.AllEnabledPools.Asteroids = new Pool[_gameState.Value.AsteroidsConfig.Asteroids.Length];

            var spawnPoint = new Vector3(0, 0, -50f);

            _gameState.Value.AllEnabledPools.Bullets = new Pool(_gameState.Value.PlayerShipConfig.BulletPrefab, spawnPoint, _basedBulletsCount, parentName: "Bullets:");

            _gameState.Value.AllEnabledPools.LaserEffects = new Pool(_gameState.Value.PlayerShipConfig.LaserEffectPrefab, spawnPoint, _basedLaserEffectsCount, parentName: "LaserEffects:");

            _gameState.Value.AllEnabledPools.Asteroids[0] = new Pool(_gameState.Value.AsteroidsConfig.Asteroids[0].Prefab, spawnPoint, _basedLargeAsteroidsCount, parentName: "LargeAsteroids:");

            _gameState.Value.AllEnabledPools.Asteroids[1] = new Pool(_gameState.Value.AsteroidsConfig.Asteroids[1].Prefab, spawnPoint, _basedSmallAsteroidsCount, parentName: "SmallAsteroids:");

            _gameState.Value.AllEnabledPools.UFOs = new Pool(_gameState.Value.UFOsConfig.Prefab, spawnPoint, _basedUFOsCount, parentName: "UFOs:");
        }
    }
}