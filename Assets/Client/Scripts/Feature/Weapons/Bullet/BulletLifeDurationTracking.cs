using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class BulletLifeDurationTracking : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsFilterInject<Inc<BulletComponent>, Exc<Destroyed>> _bulletsFilter = default;

        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<BulletComponent> _bulletPool = default;

        private int _bulletEntity = GameState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _bulletsFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawBulletsEntities = _bulletsFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                _bulletEntity = rawBulletsEntities[i];

                ref var bulletComponent = ref _bulletPool.Value.Get(_bulletEntity);

                if (bulletComponent.CurrentLifeDuration <= 0)
                {
                    InvokeDestroyEvent();

                    continue;
                }

                bulletComponent.CurrentLifeDuration -= Time.deltaTime;
            }
        }

        private void InvokeDestroyEvent()
        {
            _destroyEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_bulletEntity);
        }
    }
}