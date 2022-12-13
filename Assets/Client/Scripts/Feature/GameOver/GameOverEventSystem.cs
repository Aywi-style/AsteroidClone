using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class GameOverEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<GameOverEvent>> _eventFilter = default;
        readonly EcsFilterInject<Inc<InterfaceComponent>> _interfaceFilter = default;

        readonly EcsPoolInject<GameOverEvent> _eventPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _eventEntity = GameState.NULL_ENTITY;

        public void Run(IEcsSystems systems)
        {
            var entityCount = _eventFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawEventEntities = _eventFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                _eventEntity = rawEventEntities[i];

                DisablePlayableSystems();

                ChangeInterfacePaneles();

                DeleteEvent();
            }
        }

        private void DisablePlayableSystems()
        {
            _gameState.Value.PlayControleAndMovableSystems = false;
            _gameState.Value.PlayFightSystems = false;
            _gameState.Value.PlaySpawnSystems = false;
        }

        private void ChangeInterfacePaneles()
        {
            var entityCount = _interfaceFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                Debug.LogWarning("Interface entity wasnt found!");
                return;
            }

            var rawInterfaceEntities = _interfaceFilter.Value.GetRawEntities();

            ref var interfaceComponent = ref _interfacePool.Value.Get(rawInterfaceEntities[0]);

            interfaceComponent.PlayableCanvasMB.Disable();
            interfaceComponent.GameOverCanvasMB.Enable(_gameState.Value.GetScore());
        }

        private void DeleteEvent()
        {
            _eventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
        }
    }
}