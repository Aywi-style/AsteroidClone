using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class LaserCooldownSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<LaserWeapon>> _laserWeaponFilter = default;

        readonly EcsPoolInject<LaserWeapon> _laserWeaponPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Run (IEcsSystems systems)
        {
            var entityCount = _laserWeaponFilter.Value.GetEntitiesCount();

            if (entityCount < 1)
            {
                return;
            }

            var rawLasersEntities = _laserWeaponFilter.Value.GetRawEntities();

            for (int i = 0; i < entityCount; i++)
            {
                var laserEntity = rawLasersEntities[i];

                ref var laserWeapon = ref _laserWeaponPool.Value.Get(laserEntity);

                if (laserWeapon.IsReady)
                {
                    continue;
                }

                laserWeapon.CurrentFireCooldown -= Time.deltaTime;

                if (laserWeapon.CurrentFireCooldown <= 0)
                {
                    laserWeapon.CurrentFireCooldown = 0;
                    laserWeapon.IsReady = true;
                }

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
                interfaceComponent.PlayableCanvasMB.RefreshLaserCooldown(laserWeapon.CurrentFireCooldown, laserWeapon.MaxFireCooldown);
            }
        }
    }
}