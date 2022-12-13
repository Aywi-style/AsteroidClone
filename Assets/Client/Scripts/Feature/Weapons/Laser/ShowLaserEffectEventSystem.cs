using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class ShowLaserEffectEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<LaserEffectComponent>> _laserEffectsFilter = default;

        readonly EcsPoolInject<LaserEffectComponent> _laserEffectsPool = default;

        private int _laserEffectEntity = GameState.NULL_ENTITY;

        public void Run(IEcsSystems systems)
        {
            var laserEffectsEntitiesCount = _laserEffectsFilter.Value.GetEntitiesCount();

            if (laserEffectsEntitiesCount < 1)
            {
                return;
            }

            var rawlaserEffectsEntities = _laserEffectsFilter.Value.GetRawEntities();

            for (int i = 0; i < laserEffectsEntitiesCount; i++)
            {
                _laserEffectEntity = rawlaserEffectsEntities[i];

                ref var laserEffects = ref _laserEffectsPool.Value.Get(_laserEffectEntity);

                if (laserEffects.CurrentLifeDuration == laserEffects.MaxLifeDuration)
                {
                    laserEffects.GameObject.SetActive(true);
                    laserEffects.GameObject.transform.position = laserEffects.FinishPoint;
                }

                laserEffects.CurrentLifeDuration--;

                if (laserEffects.CurrentLifeDuration > 0)
                {
                    continue;
                }

                DisableMissilesEffect(ref laserEffects);

                _laserEffectsPool.Value.Del(_laserEffectEntity);
            }
        }

        private void DisableMissilesEffect(ref LaserEffectComponent laserEffectComponent)
        {
            laserEffectComponent.GameObject.SetActive(false);
            _gameState.Value.AllEnabledPools.LaserEffects.ReturnToPool(laserEffectComponent.GameObject);
        }
    }
}