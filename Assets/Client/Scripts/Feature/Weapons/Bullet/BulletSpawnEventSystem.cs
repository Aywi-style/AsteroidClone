using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class BulletSpawnEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<BulletSpawnEvent>> _eventFilter = default;

        readonly EcsPoolInject<BulletSpawnEvent> _eventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<BulletComponent> _bulletPool = default;
        readonly EcsPoolInject<Teleportable> _teleportablePool = default;

        private int _eventEntity = GameState.NULL_ENTITY;

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

                ref var bulletSpawnEvent = ref _eventPool.Value.Get(rawEventEntities[i]);

                var newBulletEntity = _world.Value.NewEntity();

                var bulletObject = _gameState.Value.AllEnabledPools.Bullets.GetFromPool();
                bulletObject.transform.position = bulletSpawnEvent.SpawnPoint;

                ref var view = ref _viewPool.Value.Add(newBulletEntity);
                view.GameObject = bulletObject;
                view.Transform = view.GameObject.transform;

                if (view.GameObject.TryGetComponent(out SphereCollider sphereCollider))
                {
                    view.SphereCollider = sphereCollider;
                }
                else
                {
                    Debug.LogWarning("BulletPrefab havent SphereCollider!");
                }

                ref var movable = ref _movablePool.Value.Add(newBulletEntity);
                movable.ForceVector = bulletSpawnEvent.Direction * bulletSpawnEvent.Speed;

                ref var bulletComponent = ref _bulletPool.Value.Add(newBulletEntity);
                bulletComponent.CurrentLifeDuration = bulletSpawnEvent.MaxLifeDuration;

                _teleportablePool.Value.Add(newBulletEntity);

                DeleteEvent();
            }
        }

        private void DeleteEvent()
        {
            _eventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
        }
    }
}