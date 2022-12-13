using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MovingSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<View, Movable>, Exc<Destroyed>> _movableFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        readonly EcsPoolInject<PlayerComponent> _playerPool = default;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _movableFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawMovableEntities = _movableFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                var movableEntity = rawMovableEntities[i];

                ref var view = ref _viewPool.Value.Get(movableEntity);
                ref var movable = ref _movablePool.Value.Get(movableEntity);

                view.Transform.position += (movable.ForceVector * Time.deltaTime);

                if (!_playerPool.Value.Has(movableEntity))
                {
                    continue;
                }

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                interfaceComponent.PlayableCanvasMB.RefreshShipSpeed(movable.ForceVector);
                interfaceComponent.PlayableCanvasMB.RefreshShipCoordinates(view.Transform.position);
            }
        }
    }
}