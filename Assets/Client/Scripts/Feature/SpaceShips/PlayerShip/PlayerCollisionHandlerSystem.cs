using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PlayerCollisionHandlerSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsFilterInject<Inc<PlayerComponent, View>> _playerFilter = default;

        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;

        private bool _isFirstWork = true;

        private int _maxCollidersCount = 1;
        private int _enemysCount = 0;
        private Collider[] _enemiesColliders;

        private int _playerWorkLayerMask = LayerMask.GetMask(nameof(GameLayers.Asteroid), nameof(GameLayers.UFO));

        public void Run(IEcsSystems systems)
        {
            if (_isFirstWork)
            {
                _enemiesColliders = new Collider[_maxCollidersCount];

                _isFirstWork = false;
            }

            var playerEntityCount = _playerFilter.Value.GetEntitiesCount();

            if (playerEntityCount < 1)
            {
                Debug.LogWarning("PlayerShip wasnt found!");
                return;
            }

            var rawPlayerEntities = _playerFilter.Value.GetRawEntities();

            SaveEnemysInCollider(rawPlayerEntities[0]);

            if (_enemysCount < 1)
            {
                return;
            }

            InvokeDestroyEvent(rawPlayerEntities[0]);
        }

        private void SaveEnemysInCollider(int playerEntity)
        {
            ref var view = ref _viewPool.Value.Get(playerEntity);

            _enemysCount = Physics.OverlapSphereNonAlloc(view.Transform.position, view.SphereCollider.radius, _enemiesColliders, _playerWorkLayerMask);
        }

        private void InvokeDestroyEvent(int playerEntity)
        {
            _destroyEventPool.Value.Add(_world.Value.NewEntity()).Invoke(playerEntity);
        }
    }
}