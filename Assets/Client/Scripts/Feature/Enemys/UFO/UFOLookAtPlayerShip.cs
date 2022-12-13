using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class UFOLookAtPlayerShip : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<UFOComponent, SpaceShip, View, Movable>> _UFOShipsFilter = default;
        readonly EcsFilterInject<Inc<PlayerComponent, SpaceShip, View, Movable>> _playerShipFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<SpaceShip> _spaceShipPool = default;

        public void Run (IEcsSystems systems)
        {
            var UFOShipsEntityCount = _UFOShipsFilter.Value.GetEntitiesCount();

            if (UFOShipsEntityCount < 1)
            {
                return;
            }

            var playerShipEntityCount = _playerShipFilter.Value.GetEntitiesCount();

            if (playerShipEntityCount < 1)
            {
                Debug.LogWarning("PlayerShip wasnt found!");
                return;
            }
            else
            {
                Debug.LogWarning("Was found more what one playerShip.");
            }

            ref var playerShipView = ref _viewPool.Value.Get(_playerShipFilter.Value.GetRawEntities()[0]);

            var rawUFOShipEntities = _UFOShipsFilter.Value.GetRawEntities();

            for (int i = 0; i < UFOShipsEntityCount; i++)
            {
                ref var UFOView = ref _viewPool.Value.Get(rawUFOShipEntities[i]);
                ref var UFOSpaceShip = ref _spaceShipPool.Value.Get(rawUFOShipEntities[i]);

                Vector3 direction = playerShipView.Transform.position - UFOView.Transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);

                UFOView.Transform.rotation = Quaternion.Slerp(UFOView.Transform.rotation, rotation, UFOSpaceShip.RotateAngle * Time.deltaTime);
            }
        }
    }
}