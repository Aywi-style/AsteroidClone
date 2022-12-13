using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class AddLaserChargeByTimer : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<LaserWeapon>> _laserWeaponFilter = default;

        readonly EcsPoolInject<LaserWeapon> _laserWeaponPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Run(IEcsSystems systems)
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

                if (laserWeapon.CurrentChargeValue >= laserWeapon.MaxChargeValue)
                {
                    continue;
                }

                laserWeapon.CurrentChargeCooldown -= Time.deltaTime;

                if (laserWeapon.CurrentChargeCooldown > 0)
                {
                    continue;
                }

                laserWeapon.CurrentChargeValue++;

                laserWeapon.CurrentChargeCooldown = laserWeapon.MaxChargeCooldown;

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
                interfaceComponent.PlayableCanvasMB.RefreshLaserChargeValue(laserWeapon.CurrentChargeValue);
            }
        }
    }
}