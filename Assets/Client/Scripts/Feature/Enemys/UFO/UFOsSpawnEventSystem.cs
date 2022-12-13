using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class UFOsSpawnEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<UFOsSpawnEvent, FindEnemySpawnPoint>> _UFOsSpawnEventFilter = default;

        readonly EcsPoolInject<UFOsSpawnEvent> _UFOsSpawnEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<UFOComponent> _UFOPool = default;
        readonly EcsPoolInject<SpaceShip> _spaceShipPool = default;
        readonly EcsPoolInject<Teleportable> _teleportablePool = default;
        readonly EcsPoolInject<ScoreGivable> _scoreGivablePool = default;

        readonly EcsPoolInject<FindEnemySpawnPoint> _findEnemySpawnPointPool = default;

        private int _UFOShipEntity = GameState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            var eventEntityCount = _UFOsSpawnEventFilter.Value.GetEntitiesCount();

            if (eventEntityCount < 1)
            {
                return;
            }

            var rawEventEntities = _UFOsSpawnEventFilter.Value.GetRawEntities();

            for (int i = 0; i < eventEntityCount; i++)
            {
                _UFOShipEntity = rawEventEntities[i];

                InitViewAndSpawn();
                InitMovable();
                InitUFOComponent();
                InitSpaceShip();
                InitTeleportable();
                InitScoreGivable();

                DeleteEvent();
            }
        }

        private void InitViewAndSpawn()
        {
            var UFOShipObject = _gameState.Value.AllEnabledPools.UFOs.GetFromPool();

            ref var view = ref _viewPool.Value.Add(_UFOShipEntity);
            view.GameObject = UFOShipObject;
            view.Transform = UFOShipObject.transform;

            if (!view.GameObject.TryGetComponent(out UFOShipMB UFOShipMB))
            {
                UFOShipMB = view.GameObject.AddComponent<UFOShipMB>();

                Debug.LogWarning("UFOShip havent UFOShipMB and it was added!");
            }

            UFOShipMB.Init(_UFOShipEntity);

            ref var findEnemySpawnPoint = ref _findEnemySpawnPointPool.Value.Get(_UFOShipEntity);
            view.Transform.position = findEnemySpawnPoint.SpawnPoint;

            _findEnemySpawnPointPool.Value.Del(_UFOShipEntity);
        }

        private void InitMovable()
        {
            ref var movable = ref _movablePool.Value.Add(_UFOShipEntity);

            movable.ForceVector = Vector3.zero;
        }

        private void InitUFOComponent()
        {
            _UFOPool.Value.Add(_UFOShipEntity);
        }

        private void InitSpaceShip()
        {
            ref var spaceShip = ref _spaceShipPool.Value.Add(_UFOShipEntity);
            spaceShip.Speed = _gameState.Value.UFOsConfig.StartSpeed;
            spaceShip.AccelerationValue = _gameState.Value.UFOsConfig.AccelerationValue;
            spaceShip.DecelerationPercent = _gameState.Value.UFOsConfig.DecelerationPercent;
            spaceShip.RotateAngle = _gameState.Value.UFOsConfig.RotateAngle;
        }

        private void InitTeleportable()
        {
            _teleportablePool.Value.Add(_UFOShipEntity);
        }

        private void InitScoreGivable()
        {
            ref var scoreGivable = ref _scoreGivablePool.Value.Add(_UFOShipEntity);
            scoreGivable.Value = _gameState.Value.UFOsConfig.ScoresCount;
        }

        private void DeleteEvent()
        {
            _UFOsSpawnEventPool.Value.Del(_UFOShipEntity);

            _UFOShipEntity = GameState.NULL_ENTITY;
        }
    }
}