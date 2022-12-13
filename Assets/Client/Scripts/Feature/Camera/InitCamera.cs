using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitCamera : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private int _mainCameraEntity = GameState.NULL_ENTITY;

        public void Init (IEcsSystems systems)
        {
            var mainCameraMB = GameObject.FindObjectsOfType<MainCameraMB>();

            if (mainCameraMB.Length > 1)
            {
                Debug.LogWarning("Was found more what one mainCameraMB.");
            }
            else if (mainCameraMB.Length < 1)
            {
                Debug.LogError("Not found mainCameraMB!");
                return;
            }

            _mainCameraEntity = _world.Value.NewEntity();

            InitMainCamera(ref mainCameraMB[0]);
        }

        private void InitMainCamera(ref MainCameraMB mainCameraMB)
        {
            ref var cameraComponent = ref _cameraPool.Value.Add(_mainCameraEntity);

            if (mainCameraMB.TryGetComponent(out Camera camera))
            {
                cameraComponent.MainCamera = camera;
            }
            else
            {
                Debug.LogError("Object with MainCameraMB havent CameraComponent!");
            }
        }
    }
}