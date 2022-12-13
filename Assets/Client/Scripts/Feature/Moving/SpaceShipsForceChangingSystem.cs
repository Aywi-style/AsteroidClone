using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SpaceShipsForceChangingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable, SpaceShip, View>> _movableSpaceShipsFilter = default;

        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<SpaceShip> _spaceShipPool = default;
        readonly EcsPoolInject<View> _viewPool = default;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _movableSpaceShipsFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawMovableSpaceShipsEntities = _movableSpaceShipsFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                ref var movable = ref _movablePool.Value.Get(rawMovableSpaceShipsEntities[i]);
                ref var spaceShip = ref _spaceShipPool.Value.Get(rawMovableSpaceShipsEntities[i]);
                ref var view = ref _viewPool.Value.Get(rawMovableSpaceShipsEntities[i]);

                movable.ForceVector *= 1 - (spaceShip.DecelerationPercent * Time.deltaTime);

                movable.ForceVector += view.Transform.forward * spaceShip.Speed * Time.deltaTime;
            }
        }
    }
}