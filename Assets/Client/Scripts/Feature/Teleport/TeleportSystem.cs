using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TeleportSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;
        readonly EcsFilterInject<Inc<View, Teleportable>> _teleportableFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<PlayerComponent> _playerPool = default;

        private int _teleportableEntity = GameState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _cameraFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }
            else if (entityCount > 1)
            {
                Debug.LogWarning("Was found more what one camera!");
            }

            var rawCameraEntities = _cameraFilter.Value.GetRawEntities();

            ref var cameraComponent = ref _cameraPool.Value.Get(rawCameraEntities[0]);

            // [0] = Left, [1] = Right, [2] = Down, [3] = Up
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraComponent.MainCamera);

            foreach (var teleportableEntity in _teleportableFilter.Value)
            {
                _teleportableEntity = teleportableEntity;

                ref var view = ref _viewPool.Value.Get(_teleportableEntity);

                Vector3 point = cameraComponent.MainCamera.WorldToViewportPoint(view.Transform.position);

                // Left side
                if (point.x > 1)
                {
                    Ray ray = new Ray(view.Transform.position, -Vector3.right);
                    planes[0].Raycast(ray, out float distance);
                    Vector3 worldPosition = ray.GetPoint(distance);

                    view.Transform.position = worldPosition;
                    IfIsPlayerDoClearTrails();
                }
                // Right side
                else if (point.x < 0)
                {
                    Ray ray = new Ray(view.Transform.position, Vector3.right);
                    planes[1].Raycast(ray, out float distance);
                    Vector3 worldPosition = ray.GetPoint(distance);

                    view.Transform.position = worldPosition;
                    IfIsPlayerDoClearTrails();
                }

                // Down side
                if (point.y > 1)
                {
                    Ray ray = new Ray(view.Transform.position, -Vector3.forward);
                    planes[2].Raycast(ray, out float distance);
                    Vector3 worldPosition = ray.GetPoint(distance);

                    view.Transform.position = worldPosition;
                    IfIsPlayerDoClearTrails();
                }
                // Up side
                else if (point.y < 0)
                {
                    Ray ray = new Ray(view.Transform.position, Vector3.forward);
                    planes[3].Raycast(ray, out float distance);
                    Vector3 worldPosition = ray.GetPoint(distance);

                    view.Transform.position = worldPosition;
                    IfIsPlayerDoClearTrails();
                }
            }
        }

        private void IfIsPlayerDoClearTrails()
        {
            if (!_playerPool.Value.Has(_teleportableEntity))
            {
                return;
            }

            ref var player = ref _playerPool.Value.Get(_teleportableEntity);

            for (int i = 0; i < player.EnginesTrailRenderer.Length; i++)
            {
                player.EnginesTrailRenderer[i].Clear();
            }
        }
    }
}