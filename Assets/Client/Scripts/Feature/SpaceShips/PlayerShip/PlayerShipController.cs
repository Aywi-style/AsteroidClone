using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    sealed class PlayerShipController : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<PlayerComponent, SpaceShip, View, Movable>> _playerShipFilter = default;

        readonly EcsPoolInject<BulletSpawnEvent> _bulletSpawnEventPool = default;
        readonly EcsPoolInject<LaserSpawnEvent> _laserSpawnEventPool = default;

        readonly EcsPoolInject<PlayerComponent> _playerShipComponentPool = default;
        readonly EcsPoolInject<SpaceShip> _spaceShipPool = default;
        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<LaserWeapon> _laserWeaponPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfaceWeaponPool = default;

        private float _currentRotateAngle;

        public void Run (IEcsSystems systems)
        {
            if (NotInputKeys())
            {
                return;
            }

            var entityCount = _playerShipFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                Debug.LogWarning("PlayerShip wasnt found!");
                return;
            }

            var rawPlayerShipEntities = _playerShipFilter.Value.GetRawEntities();

            var playerEntity = rawPlayerShipEntities[0];

            ref var playerShipComponent = ref _playerShipComponentPool.Value.Get(playerEntity);
            ref var spaceShip = ref _spaceShipPool.Value.Get(playerEntity);
            ref var view = ref _viewPool.Value.Get(playerEntity);
            ref var movable = ref _movablePool.Value.Get(playerEntity);
            ref var laserWeapon = ref _laserWeaponPool.Value.Get(playerEntity);

            ref var interfaceComponent = ref _interfaceWeaponPool.Value.Get(_gameState.Value.InterfaceEntity);

            _currentRotateAngle = 0;

            if (Input.GetKeyUp(KeyCode.W))
            {
                spaceShip.Speed = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                spaceShip.Speed = spaceShip.AccelerationValue;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _currentRotateAngle = -spaceShip.RotateAngle * Time.deltaTime;

                view.Transform.Rotate(0, _currentRotateAngle, 0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                _currentRotateAngle = spaceShip.RotateAngle * Time.deltaTime;

                view.Transform.Rotate(0, _currentRotateAngle, 0);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _bulletSpawnEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_gameState.Value.PlayerShipConfig.BulletLifeDuration, view.Transform.position, view.Transform.forward, _gameState.Value.PlayerShipConfig.BulletSpeed);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (laserWeapon.IsReady && laserWeapon.CurrentChargeValue > 0)
                {
                    _laserSpawnEventPool.Value.Add(_world.Value.NewEntity()).Invoke(playerEntity, view.Transform.position, view.Transform.forward);
                }
            }

            RefreshInterfaceRotateAngle(ref interfaceComponent);
        }

        private bool NotInputKeys()
        {
            return !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift));
        }

        private void RefreshInterfaceRotateAngle(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.PlayableCanvasMB.RefreshShipRotateAngle(Mathf.CeilToInt(_currentRotateAngle));
        }
    }
}