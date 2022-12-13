using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitInterface : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _interfaceEntity = GameState.NULL_ENTITY;

        public void Init (IEcsSystems systems)
        {
            _interfaceEntity = _world.Value.NewEntity();

            _gameState.Value.InterfaceEntity = _interfaceEntity;

            ref var interfaceComponent = ref _interfacePool.Value.Add(_interfaceEntity);

            InitPlayableCanvas(ref interfaceComponent);
            InitGameOverCanvas(ref interfaceComponent);
        }

        private void InitPlayableCanvas(ref InterfaceComponent interfaceComponent)
        {
            var playableCanvasesMB = GameObject.FindObjectsOfType<PlayableCanvasMB>();

            if (playableCanvasesMB == null)
            {
                Debug.LogError("PlayableCanvasMB wasnt found!");
                return;
            }
            else if (playableCanvasesMB.Length > 1)
            {
                Debug.LogWarning("Was found more what one PlayableCanvasMB.");
            }

            interfaceComponent.PlayableCanvasMB = playableCanvasesMB[0];
        }

        private void InitGameOverCanvas(ref InterfaceComponent interfaceComponent)
        {
            var gameOverCanvasMB = GameObject.FindObjectsOfType<GameOverCanvasMB>();

            if (gameOverCanvasMB == null)
            {
                Debug.LogError("GameOverCanvasMB wasnt found!");
                return;
            }
            else if (gameOverCanvasMB.Length > 1)
            {
                Debug.LogWarning("Was found more what one GameOverCanvasMB.");
            }

            interfaceComponent.GameOverCanvasMB = gameOverCanvasMB[0];
            interfaceComponent.GameOverCanvasMB.Disable();
        }
    }
}