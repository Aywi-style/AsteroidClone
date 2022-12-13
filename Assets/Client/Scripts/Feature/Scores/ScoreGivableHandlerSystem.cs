using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ScoreGivableHandlerSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Destroyed, ScoreGivable>> _destroyedScoreGivableFilter = default;

        readonly EcsPoolInject<ScoreGivable> _scoreGivablePool = default;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _destroyedScoreGivableFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawDestroyedScoreGivableEntities = _destroyedScoreGivableFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                var scorableEntity = rawDestroyedScoreGivableEntities[i];

                ref var scoreGivable = ref _scoreGivablePool.Value.Get(scorableEntity);

                _gameState.Value.AddScore(scoreGivable.Value);

                _scoreGivablePool.Value.Del(scorableEntity);
            }
        }
    }
}