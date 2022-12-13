using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class BulletCollisionHandlerSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsFilterInject<Inc<BulletComponent, View>, Exc<Destroyed>> _bulletFilter = default;

        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;

        private bool _isFirstWork = true;

        private int _maxCollidersCount = 3;
        private int _enemysCount = 0;
        private Collider[] _enemiesColliders;

        private int _bulletWorkLayerMask = LayerMask.GetMask(nameof(GameLayers.Asteroid), nameof(GameLayers.UFO));

        public void Run (IEcsSystems systems)
        {
            if (_isFirstWork)
            {
                _enemiesColliders = new Collider[_maxCollidersCount];

                _isFirstWork = false;
            }

            var bulletsEntitiesCount = _bulletFilter.Value.GetEntitiesCount();

            if (bulletsEntitiesCount < 1)
            {
                return;
            }

            var rawBulletsEntities = _bulletFilter.Value.GetRawEntities();

            for (int i = 0; i < bulletsEntitiesCount; i++)
            {

                SaveEnemysInCollider(rawBulletsEntities[i]);

                if (_enemysCount < 1)
                {
                    continue;
                }

                DestroyObjects();

                InvokeDestroyEvent(rawBulletsEntities[i]);
            }
        }

        private void SaveEnemysInCollider(int bulletEntity)
        {
            ref var bulletView = ref _viewPool.Value.Get(bulletEntity);

            _enemysCount = Physics.OverlapSphereNonAlloc(bulletView.Transform.position, bulletView.SphereCollider.radius, _enemiesColliders, _bulletWorkLayerMask);
        }

        private void DestroyObjects()
        {
            var enemyEntity = GameState.NULL_ENTITY;

            for (int j = 0; j < _enemysCount; j++)
            {
                if (_enemiesColliders[j].TryGetComponent(out EcsInfoMB ecsInfoMB))
                {
                    enemyEntity = ecsInfoMB.GetEntity();

                    InvokeDestroyEvent(enemyEntity);
                }
                else
                {
                    Debug.LogWarning($"Enemy {_enemiesColliders[j].name} havent EcsInfoMB!");
                }
            }
        }

        private void InvokeDestroyEvent(int enemyEntity)
        {
            _destroyEventPool.Value.Add(_world.Value.NewEntity()).Invoke(enemyEntity);
        }
    }
}