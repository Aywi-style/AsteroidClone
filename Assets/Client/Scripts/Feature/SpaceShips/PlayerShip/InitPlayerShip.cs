using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitPlayerShip : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<PlayerComponent> _playerPool = default;
        readonly EcsPoolInject<SpaceShip> _spaceShipPool = default;
        readonly EcsPoolInject<Teleportable> _teleportablePool = default;
        readonly EcsPoolInject<LaserWeapon> _laserWeaponPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _playerShipEntity = GameState.NULL_ENTITY;

        public void Init (IEcsSystems systems)
        {
            _playerShipEntity = _world.Value.NewEntity();

            InitViewAndEntity();
            InitMovable();
            InitPlayerComponent();
            InitSpaceShip();
            InitTeleportable();
            InitLaserWeapon();
            RefreshInterfaceLaserInfo();
        }

        private void InitViewAndEntity()
        {
            var playerShipObject = GameObject.Instantiate(_gameState.Value.PlayerShipConfig.PlayerShipPrefab);

            ref var view = ref _viewPool.Value.Add(_playerShipEntity);
            view.GameObject = playerShipObject;
            view.Transform = playerShipObject.transform;

            if (view.GameObject.TryGetComponent(out SphereCollider sphereCollider))
            {
                view.SphereCollider = sphereCollider;
            }
            else
            {
                Debug.LogWarning("PlayerShip havent SphereCollider!");
            }

            if (!view.GameObject.TryGetComponent(out PlayerShipMB playerShipMB))
            {
                playerShipMB = view.GameObject.AddComponent<PlayerShipMB>();

                Debug.LogWarning("PlayerShip havent PlayerShipMB and it was added!");
            }

            playerShipMB.Init(_playerShipEntity);
        }

        private void InitMovable()
        {
            ref var movable = ref _movablePool.Value.Add(_playerShipEntity);
            movable.ForceVector = Vector3.forward * _gameState.Value.PlayerShipConfig.StartSpeed;
        }

        private void InitPlayerComponent()
        {
            ref var playerComponent = ref _playerPool.Value.Add(_playerShipEntity);

            ref var view = ref _viewPool.Value.Get(_playerShipEntity);

            if (!view.GameObject.TryGetComponent(out PlayerShipMB playerShipMB))
            {
                Debug.LogWarning("PlayerShip havent PlayerShipMB and shipEngines wasnt found!");
            }

            playerComponent.EnginesTrailRenderer = playerShipMB.EnginesTrailRenderer;
        }

        private void InitSpaceShip()
        {
            ref var spaceShip = ref _spaceShipPool.Value.Add(_playerShipEntity);
            spaceShip.Speed = _gameState.Value.PlayerShipConfig.StartSpeed;
            spaceShip.AccelerationValue = _gameState.Value.PlayerShipConfig.AccelerationValue;
            spaceShip.DecelerationPercent = _gameState.Value.PlayerShipConfig.DecelerationPercent;
            spaceShip.RotateAngle = _gameState.Value.PlayerShipConfig.RotateAngle;
        }

        private void InitTeleportable()
        {
             _teleportablePool.Value.Add(_playerShipEntity);
        }

        private void InitLaserWeapon()
        {
            ref var laserWeapon = ref _laserWeaponPool.Value.Add(_playerShipEntity);
            laserWeapon.IsReady = true;

            laserWeapon.MaxChargeValue = _gameState.Value.PlayerShipConfig.LasersMaxChargeValue;
            laserWeapon.CurrentChargeValue = laserWeapon.MaxChargeValue;

            laserWeapon.MaxChargeCooldown = _gameState.Value.PlayerShipConfig.LasersTimeToAddCharge;
            laserWeapon.CurrentChargeCooldown = laserWeapon.MaxChargeCooldown;

            laserWeapon.MaxFireCooldown = _gameState.Value.PlayerShipConfig.LaserMaxCoolDown;
            laserWeapon.CurrentFireCooldown = 0;

            laserWeapon.EffectLifeDuration = _gameState.Value.PlayerShipConfig.LaserEffectLifeDuration;
        }

        private void RefreshInterfaceLaserInfo()
        {
            ref var laserWeapon = ref _laserWeaponPool.Value.Get(_playerShipEntity);

            ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
            interfaceComponent.PlayableCanvasMB.RefreshLaserCooldown(laserWeapon.CurrentFireCooldown, laserWeapon.MaxFireCooldown);
            interfaceComponent.PlayableCanvasMB.RefreshLaserChargeValue(laserWeapon.CurrentChargeValue);
        }
    }
}