using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class FindEnemySpawnPointSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;
        readonly EcsFilterInject<Inc<FindEnemySpawnPoint>> _findEnemySpawnPointFilter = default;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<FindEnemySpawnPoint> _findEnemySpawnPointPool = default;

        private float _ousideLeft = -0.5f;
        private float _ousideRight = 1.5f;

        private int _downSide = 0;
        private int _topSideInt = 2;
        private float _topSideFloat = 1f;

        public void Run(IEcsSystems systems)
        {
            var findSpawnEntityCount = _findEnemySpawnPointFilter.Value.GetEntitiesCount();

            if (findSpawnEntityCount < 1)
            {
                return;
            }

            var cameraEntityCount = _cameraFilter.Value.GetEntitiesCount();

            if (cameraEntityCount < 1)
            {
                return;
            }
            else if (cameraEntityCount > 1)
            {
                Debug.LogWarning("Was found more what one camera!");
            }

            var rawCameraEntities = _cameraFilter.Value.GetRawEntities();
            ref var cameraComponent = ref _cameraPool.Value.Get(rawCameraEntities[0]);

            var rawFindSpawnEntities = _findEnemySpawnPointFilter.Value.GetRawEntities();

            for (int i = 0; i < findSpawnEntityCount; i++)
            {
                ref var findEnemySpawnPoint = ref _findEnemySpawnPointPool.Value.Get(rawFindSpawnEntities[i]);

                if (findEnemySpawnPoint.IsFinded)
                {
                    continue;
                }

                findEnemySpawnPoint.SpawnPoint = GetRandomPointOutScreen(in cameraComponent);
                findEnemySpawnPoint.IsFinded = true;
            }
        }

        private Vector3 GetRandomPointOutScreen(in CameraComponent cameraComponent)
        {
            float xCoordinate = Random.Range(_ousideLeft, _ousideRight);

            float yCoordinate;

            if (xCoordinate > 1 || 0 > xCoordinate)
            {
                xCoordinate = Mathf.Clamp01(xCoordinate);
                yCoordinate = Random.Range(_downSide, _topSideFloat);
            }
            else
            {
                int yCoordinateInt = Random.Range(_downSide, _topSideInt);
                yCoordinate = yCoordinateInt;
            }

            var screenSpawnPoint = new Vector3(xCoordinate, yCoordinate, 0);
            Vector3 spawnPoint = cameraComponent.MainCamera.ViewportToWorldPoint(screenSpawnPoint);
            spawnPoint.y = 0;

            return spawnPoint;
        }
    }
}