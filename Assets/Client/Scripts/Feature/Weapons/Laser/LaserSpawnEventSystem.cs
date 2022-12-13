using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class LaserSpawnEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<LaserSpawnEvent>> _laserSpawnEventFilter = default;

        readonly EcsPoolInject<LaserSpawnEvent> _laserSpawnEventPool = default;
        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<LaserWeapon> _laserWeaponPool = default;
        readonly EcsPoolInject<LaserEffectComponent> _laserEffectPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _laserEventEntity = GameState.NULL_ENTITY;
        private int _laserOwnerEntity = GameState.NULL_ENTITY;

        private bool _isFirstWork = true;

        private RaycastHit[] _enemiesRaycastHits;
        private int _maxRaycastHitsCount = 50;
        private int _maxRaycastHitsDistance = 50;
        private int _enemysCount = 0;

        private int _bulletWorkLayerMask = LayerMask.GetMask(nameof(GameLayers.Asteroid), nameof(GameLayers.UFO));

        public void Run(IEcsSystems systems)
        {
            if (_isFirstWork)
            {
                _enemiesRaycastHits = new RaycastHit[_maxRaycastHitsCount];

                _isFirstWork = false;
            }

            var lasersEntitiesCount = _laserSpawnEventFilter.Value.GetEntitiesCount();

            if (lasersEntitiesCount < 1)
            {
                return;
            }

            var rawlasersEntities = _laserSpawnEventFilter.Value.GetRawEntities();

            for (int i = 0; i < lasersEntitiesCount; i++)
            {
                _laserEventEntity = rawlasersEntities[i];

                ref var laserSpawnEvent = ref _laserSpawnEventPool.Value.Get(_laserEventEntity);

                _laserOwnerEntity = laserSpawnEvent.OwnerEntity;

                ChangeLaserWeaponStatus();

                RefreshInterfaceChargeValue();

                ShowLaserEffect(ref laserSpawnEvent);

                SaveEnemysInCollider(ref laserSpawnEvent);

                if (_enemysCount < 1)
                {
                    DeleteEvent();
                    continue;
                }

                DestroyObjects();

                DeleteEvent();
            }
        }

        private void ChangeLaserWeaponStatus()
        {
            ref var laserWeapon = ref _laserWeaponPool.Value.Get(_laserOwnerEntity);

            laserWeapon.CurrentChargeValue--;
            laserWeapon.IsReady = false;
            laserWeapon.CurrentFireCooldown = laserWeapon.MaxFireCooldown;
        }

        private void RefreshInterfaceChargeValue()
        {
            ref var laserWeapon = ref _laserWeaponPool.Value.Get(_laserOwnerEntity);

            ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
            interfaceComponent.PlayableCanvasMB.RefreshLaserChargeValue(laserWeapon.CurrentChargeValue);
        }

        private void ShowLaserEffect(ref LaserSpawnEvent laserSpawnEvent)
        {
            ref var laserEffect = ref _laserEffectPool.Value.Add(_world.Value.NewEntity());

            laserEffect.GameObject = _gameState.Value.AllEnabledPools.LaserEffects.GetFromPool();
            laserEffect.GameObject.transform.position = laserSpawnEvent.SpawnPoint;

            if (laserEffect.GameObject.TryGetComponent(out TrailRenderer trailRenderer))
            {
                trailRenderer.Clear();
            }

            laserEffect.FinishPoint = laserSpawnEvent.SpawnPoint + (laserSpawnEvent.Direction * _maxRaycastHitsDistance);
            laserEffect.MaxLifeDuration = _gameState.Value.PlayerShipConfig.LaserEffectLifeDuration;
            laserEffect.CurrentLifeDuration = laserEffect.MaxLifeDuration;
        }

        private void SaveEnemysInCollider(ref LaserSpawnEvent laserSpawnEvent)
        {
            _enemysCount = Physics.RaycastNonAlloc(laserSpawnEvent.SpawnPoint, laserSpawnEvent.Direction, _enemiesRaycastHits, _maxRaycastHitsDistance, _bulletWorkLayerMask);
        }

        private void DestroyObjects()
        {
            var enemyEntity = GameState.NULL_ENTITY;

            for (int j = 0; j < _enemysCount; j++)
            {
                if (_enemiesRaycastHits[j].collider.TryGetComponent(out EcsInfoMB ecsInfoMB))
                {
                    enemyEntity = ecsInfoMB.GetEntity();

                    InvokeDestroyEvent(enemyEntity);
                }
                else
                {
                    Debug.LogWarning($"Enemy {_enemiesRaycastHits[j].collider.name} havent EcsInfoMB!");
                }
            }
        }

        private void InvokeDestroyEvent(int enemyEntity)
        {
            _destroyEventPool.Value.Add(_world.Value.NewEntity()).Invoke(enemyEntity);
        }

        private void DeleteEvent()
        {
            _laserSpawnEventPool.Value.Del(_laserEventEntity);

            _laserEventEntity = GameState.NULL_ENTITY;
            _enemysCount = 0;
        }
    }
}