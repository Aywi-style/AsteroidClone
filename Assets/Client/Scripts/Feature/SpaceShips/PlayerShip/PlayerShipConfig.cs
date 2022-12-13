using UnityEngine;

[CreateAssetMenu(fileName = "PlayerShipConfig", menuName = "Configs/PlayerShipConfig", order = 0)]
public class PlayerShipConfig : ScriptableObject
{
    [field: SerializeField]
    public GameObject PlayerShipPrefab { get; private set; }

    [field: SerializeField]
    public float StartSpeed { get; private set; }

    [field: SerializeField]
    public float AccelerationValue { get; private set; }

    [field: SerializeField]
    public float DecelerationPercent { get; private set; }

    [field: SerializeField]
    public float RotateAngle { get; private set; }

    [field: Header("Weapons")]
    [field: SerializeField]
    public GameObject BulletPrefab { get; private set; }

    [field: SerializeField]
    public float BulletLifeDuration { get; private set; }

    [field: SerializeField]
    public float BulletSpeed { get; private set; }


    [field: Space]
    [field: SerializeField]
    public GameObject LaserEffectPrefab { get; private set; }

    [field: SerializeField]
    public int LaserEffectLifeDuration { get; private set; }

    [field: Space]
    [field: SerializeField]
    public int LasersMaxChargeValue { get; private set; }

    [field: SerializeField]
    public int LasersTimeToAddCharge { get; private set; }

    [field: Space]
    [field: SerializeField]
    public float LaserMaxCoolDown { get; private set; }
}
