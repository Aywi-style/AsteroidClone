namespace Client
{
    struct LaserWeapon
    {
        public bool IsReady;

        public int MaxChargeValue;
        public int CurrentChargeValue;

        public float MaxChargeCooldown;
        public float CurrentChargeCooldown;

        public float MaxFireCooldown;
        public float CurrentFireCooldown;

        public float EffectLifeDuration;
    }
}